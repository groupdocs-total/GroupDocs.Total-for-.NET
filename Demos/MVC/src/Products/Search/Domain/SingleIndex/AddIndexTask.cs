using GroupDocs.Total.MVC.Products.Search.Domain.Ocr;
using GroupDocs.Search.Common;
using GroupDocs.Search.Events;
using GroupDocs.Search.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Index = GroupDocs.Search.Index;

namespace GroupDocs.Total.MVC.Products.Search.Domain.SingleIndex
{
    internal class AddIndexTask : IndexTask
    {
        private readonly StorageService _storageService;
        private readonly DocumentDescriptor[] _files;
        private readonly string[] _attributes;
        private readonly string _tempIndexDirectoryPath;
        private readonly bool _recognizeTextInImages;
        private readonly OcrImageCounter _ocrImageCounter;
        private Index _tempIndex;

        public AddIndexTask(
            ILogger logger,
            IndexFactoryService indexFactoryService,
            DocumentStatusService documentStatusService,
            StorageService storageService,
            Settings settings,
            string userId,
            bool recognizeTextInImages,
            IEnumerable<string> fileNames)
            : base(logger, indexFactoryService, settings, documentStatusService, userId, fileNames)
        {
            var userDirectory = Path.Combine(Settings.StoragePath, UserId);
            var tempIndexDirectoryName = Settings.TempIndexDirectoryName + "_" + Guid.NewGuid().ToString("N");
            _tempIndexDirectoryPath = Path.Combine(userDirectory, tempIndexDirectoryName);
            var uploadedDirectory = Path.Combine(userDirectory, Settings.UploadedDirectoryName);
            _storageService = storageService;
            _files = fileNames
                .Select(fileName =>
                {
                    var filePath = Path.Combine(uploadedDirectory, fileName);
                    return new DocumentDescriptor(userId, fileName, filePath);
                })
                .ToArray();
            _attributes = new string[]
            {
                userId,
            };
            _recognizeTextInImages = recognizeTextInImages;

            _ocrImageCounter = new OcrImageCounter(Settings);
        }

        public override void Init()
        {
            SetStatus(DocumentStatus.Pending);
        }

        public override void BeforePreprocess()
        {
            SetStatus(DocumentStatus.Indexing);
        }

        public override void AfterPreprocess()
        {
            SetStatus(DocumentStatus.Pending);
        }

        public override void BeforeRun()
        {
            SetStatus(DocumentStatus.Merging);
        }

        public override void AfterRun()
        {
            SetStatus(DocumentStatus.SuccessfullyProcessed);
        }

        public override void Preprocess()
        {
            Logger.LogInformation("Start preprocessing: " + (_files.Length == 1 ? _files[0].DocumentKey : _files.Length.ToString()));

            foreach (var document in _files)
            {
                try
                {
                    if (!File.Exists(document.FilePath))
                    {
                        using (var inputStream = _storageService.DownloadFile(UserId, document.FileName))
                        using (var outputStream = File.Create(document.FilePath))
                        {
                            inputStream.CopyTo(outputStream);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error of copying files: " + GetType().Name);
                }
            }

            var index = IndexFactoryService.Allocate();
            try
            {
                var documents = GetDocumentsForProcessing(index);
                _tempIndex = IndexFactoryService.CreateTempIndex(_tempIndexDirectoryPath, true);

                _tempIndex.Events.ErrorOccurred += OnErrorOccurred;
                _tempIndex.Events.OperationProgressChanged += OnOperationProgressChanged;
                _tempIndex.Events.FileIndexing += OnFileIndexing;

                var options = new IndexingOptions();
                if (_recognizeTextInImages)
                {
                    options.OcrIndexingOptions.EnabledForEmbeddedImages = true;
                    options.OcrIndexingOptions.EnabledForSeparateImages = true;
                    options.OcrIndexingOptions.OcrConnector = new TesseractOcrConnector();
                }
                _tempIndex.Add(documents, options);

                _tempIndex.Events.ErrorOccurred -= OnErrorOccurred;
                _tempIndex.Events.OperationProgressChanged -= OnOperationProgressChanged;
                _tempIndex.Events.FileIndexing -= OnFileIndexing;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error of preprocessing index task: " + GetType().Name);
            }
            finally
            {
                IndexFactoryService.Release(index);
            }

            Logger.LogInformation("End preprocessing: " + (_files.Length == 1 ? _files[0].DocumentKey : _files.Length.ToString()));
        }

        protected override void RunProtected(Index index)
        {
            Logger.LogInformation("Start processing: " + (_files.Length == 1 ? _files[0].DocumentKey : _files.Length.ToString()));

            if (_tempIndex != null)
            {
                index.Merge(_tempIndex, new MergeOptions());
                _tempIndex.Dispose();
            }

            index.Optimize();

            Directory.Delete(_tempIndexDirectoryPath, true);

            Logger.LogInformation("End processing: " + (_files.Length == 1 ? _files[0].DocumentKey : _files.Length.ToString()));
        }

        private Document[] GetDocumentsForProcessing(Index index)
        {
            var indexedFiles = new HashSet<string>(index.GetIndexedDocuments()
                .Where(di => di.FilePath.StartsWith(UserId, StringComparison.Ordinal))
                .Select(di => di.FilePath));
            var alreadyAdded = _files
                .Where(d => indexedFiles.Contains(d.DocumentKey));
            int maxCount = Settings.MaxIndexedFiles;
            int countToAdd = indexedFiles.Count >= maxCount ? 0 : maxCount - indexedFiles.Count;
            var notAdded = _files
                .Where(d => !indexedFiles.Contains(d.DocumentKey))
                .Take(countToAdd);
            var toAdd = alreadyAdded.Concat(notAdded);

            var documents = toAdd
                .Select(d =>
                {
                    var documentLoader = new DocumentLoader(_attributes, d);
                    var document = Document.CreateLazy(DocumentSourceKind.Stream, documentLoader.DocumentKey, documentLoader);
                    return document;
                })
                .ToArray();
            return documents;
        }

        private void OnFileIndexing(object sender, FileIndexingEventArgs e)
        {
            _ocrImageCounter.Reset();
        }

        private void OnOperationProgressChanged(object sender, OperationProgressEventArgs e)
        {
            var documentKey = e.LastDocumentKey;
            int startIndex = UserId.Length + Settings.DocumentKeySeparator.Length;
            var fileName = documentKey.Substring(startIndex);
            DocumentStatus status;
            switch (e.LastDocumentStatus)
            {
                case GroupDocs.Search.Common.DocumentStatus.SuccessfullyProcessed:
                    status = DocumentStatus.SuccessfullyProcessed;
                    break;
                case GroupDocs.Search.Common.DocumentStatus.Skipped:
                    status = DocumentStatus.Skipped;
                    break;
                case GroupDocs.Search.Common.DocumentStatus.ProcessedWithError:
                    status = DocumentStatus.ProcessedWithError;
                    break;
                default:
                    throw new NotSupportedException("Document status is not supported: " + e.LastDocumentStatus);
            }
            DocumentStatusService.SetStatus(UserId, fileName, status);
        }

        private void OnErrorOccurred(object sender, IndexErrorEventArgs e)
        {
            Logger.LogError(e.Message);
        }
    }
}
