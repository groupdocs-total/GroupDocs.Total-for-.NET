using GroupDocs.Total.MVC.Products.Search.Domain.ViewerCache;
using GroupDocs.Total.MVC.Products.Search.Dto;
using GroupDocs.Total.MVC.Products.Search.Dto.Info;
using GroupDocs.Total.MVC.Products.Search.Dto.Request;
using GroupDocs.Total.MVC.Products.Search.Dto.Response;
using GroupDocs.Search.Common;
using GroupDocs.Search.Dictionaries;
using GroupDocs.Search.Highlighters;
using GroupDocs.Search.Options;
using GroupDocs.Search.Results;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Index = GroupDocs.Search.Index;
using System.Web;

namespace GroupDocs.Total.MVC.Products.Search.Domain.SingleIndex
{
    internal class SingleIndexService : ISearchService
    {
        private readonly ILogger _logger;
        private readonly Settings _settings;
        private readonly DemoValidatingService _demoValidator;
        private readonly SearchConfiguration _searchConfiguration;
        private readonly StorageService _storageService;
        private readonly DictionaryStorageService _dictionaryStorageService;
        private readonly DocumentStatusService _documentStatusService;
        private readonly HtmlCacheService _htmlCacheService;

        private readonly IndexFactoryService _indexFactoryService;
        private readonly TaskQueueService _taskQueueService;
        private readonly ReindexingService _reindexingService;

        public SingleIndexService(
            ILogger logger,
            Settings settings,
            DemoValidatingService demoValidator,
            StorageService storageService,
            DictionaryStorageService dictionaryStorageService,
            DocumentStatusService documentStatusService,
            HtmlCacheService htmlCacheService,
            IndexFactoryService indexFactoryService,
            TaskQueueService taskQueueService,
            ReindexingService reindexingService)
        {
            _logger = logger;
            _settings = settings;
            _demoValidator = demoValidator;
            _searchConfiguration = new SearchConfiguration();
            _storageService = storageService;
            _dictionaryStorageService = dictionaryStorageService;
            _documentStatusService = documentStatusService;
            _htmlCacheService = htmlCacheService;
            _indexFactoryService = indexFactoryService;
            _taskQueueService = taskQueueService;
            _reindexingService = reindexingService;
        }

        public Task AddFilesToIndexAsync(AddToIndexRequest request)
        {
            var userFolderName = EnsureFolderName(request.FolderName);

            var fileNames = request.Files
                .Select(pde => pde.guid)
                .ToArray();
            _taskQueueService.EnqueueAddTask(userFolderName, fileNames, request.RecognizeTextInImages);
            return Task.FromResult(true);
        }

        public void DeleteFiles(FilesDeleteRequest request)
        {
            Guid userId;
            var userFolderName = EnsureFolderName(request.FolderName, out userId);

            foreach (var pde in request.Files)
            {
                var fileName = pde.guid;

                var documentCache = _htmlCacheService.GetCache(userId, fileName);

                _storageService.DeleteFile(userFolderName, fileName);

                if (File.Exists(documentCache.FileInfo.SourceFilePath))
                {
                    File.Delete(documentCache.FileInfo.SourceFilePath);
                }

                documentCache.DeleteHtmlCache();
            }

            var fileNames = request.Files
                .Select(pde => pde.guid)
                .ToArray();
            _taskQueueService.EnqueueDeleteTask(userFolderName, fileNames);
        }

        public async Task DownloadAndAddToIndexAsync(AddToIndexRequest request)
        {
            Guid userId;
            var userFolderName = EnsureFolderName(request.FolderName, out userId);

            foreach (var pde in request.Files)
            {
                var fileName = pde.guid;
                string password = string.IsNullOrEmpty(pde.password) ? null : pde.password;

                var documentCache = _htmlCacheService.GetCache(userId, fileName);
                var fileInfo = documentCache.FileInfo;

                Directory.CreateDirectory(fileInfo.UploadedDirectoryPath);
                _demoValidator.CheckUploadedFiles(fileInfo.UploadedDirectoryPath);

                using (var inputStream = await _storageService.DownloadFileAsync(userFolderName, fileName))
                using (var outputStream = File.Create(fileInfo.SourceFilePath))
                {
                    _demoValidator.CheckFileLength(inputStream.Length);

                    await inputStream.CopyToAsync(outputStream);
                }

                try
                {
                    var fileFolderPath = fileInfo.FileCacheFolderPath;
                    if (Directory.Exists(fileFolderPath))
                    {
                        Directory.Delete(fileFolderPath, true);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Download and add to index error: removing folder from viewer cache.");
                }
            }

            var fileNames = request.Files
                .Select(pde => pde.guid)
                .ToArray();
            _taskQueueService.EnqueueAddTask(userFolderName, fileNames, request.RecognizeTextInImages);
        }

        public SearchConfiguration GetConfiguration()
        {
            return _searchConfiguration;
        }

        public IndexedFileDescriptionEntity[] GetFileStatus(FileStatusGetRequest request)
        {
            if (request.Files == null || request.Files.Length == 0)
            {
                return new IndexedFileDescriptionEntity[0];
            }

            var userFolderName = EnsureFolderName(request.FolderName);

            var result = request.Files
                .Select(pde =>
                {
                    var fileName = pde.guid;
                    var status = _documentStatusService.GetStatus(userFolderName, fileName);
                    var indexingFile = new IndexedFileDescriptionEntity
                    {
                        documentStatus = status.ToString(),
                        guid = fileName,
                    };
                    return indexingFile;
                })
                .ToArray();
            return result;
        }

        public GetStatusResponse GetStatus(SearchBaseRequest request)
        {
            var userFolderName = EnsureFolderName(request.FolderName);

            var response = _documentStatusService.GetStatus(userFolderName);
            return response;
        }

        public IndexedFileDescriptionEntity[] GetIndexedFiles(SearchBaseRequest request)
        {
            var userFolderName = EnsureFolderName(request.FolderName);

            var userDirectory = Path.Combine(_settings.StoragePath, userFolderName);
            var uploadedDirectory = Path.Combine(userDirectory, _settings.UploadedDirectoryName);

            int fileNameStartIndex = userFolderName.Length + Settings.DocumentKeySeparator.Length;
            var documents = _documentStatusService.GetDocuments(userFolderName);
            var indexedFiles = documents
                .Select(document =>
                {
                    var filePath = Path.Combine(uploadedDirectory, document.FileName);
                    var fileInfo = new FileInfo(filePath);
                    long fileLength = fileInfo.Exists ? fileInfo.Length : 0L;
                    var descriptor = new IndexedFileDescriptionEntity()
                    {
                        guid = document.FileName,
                        name = document.FileName,
                        isDirectory = false,
                        size = fileLength,
                        documentStatus = document.Status.ToString(),
                    };
                    return descriptor;
                })
                .ToArray();
            return indexedFiles;
        }

        public IndexPropertiesResponse GetIndexProperties(SearchBaseRequest request)
        {
            var userFolderName = EnsureFolderName(request.FolderName); // Check for valid user ID

            var index = _indexFactoryService.Allocate();
            try
            {
                var indexProperties = new IndexPropertiesResponse();
                indexProperties.indexVersion = index.IndexInfo.Version;
                indexProperties.indexType = index.IndexSettings.IndexType.ToString();
                indexProperties.useStopWords = index.IndexSettings.UseStopWords;
                indexProperties.useCharacterReplacements = index.IndexSettings.UseCharacterReplacements;
                indexProperties.autoDetectEncoding = index.IndexSettings.AutoDetectEncoding;
                indexProperties.useRawTextExtraction = index.IndexSettings.UseRawTextExtraction;
                var tss = index.IndexSettings.TextStorageSettings;
                indexProperties.textStorageCompression = tss == null ? "No storage" : tss.Compression.ToString();

                return indexProperties;
            }
            finally
            {
                _indexFactoryService.Release(index);
            }
        }

        public SearchAppInfo GetInfo(SearchBaseRequest request)
        {
            var userFolderName = EnsureFolderName(request.FolderName);

            var response = new SearchAppInfo();

            if (userFolderName != _settings.AdminId)
            {
                return response;
            }

            response.IndexStatus = _indexFactoryService.GetIndexInfo();
            response.PreprocessingQueue = _taskQueueService.PreprocessingQueue.GetTasks();
            response.TaskQueue = _taskQueueService.GetTasks();
            response.DocumentList = _documentStatusService.GetStatuses();
            return response;
        }

        public async Task<IndexedFileDescriptionEntity[]> GetUploadedFiles(SearchBaseRequest request)
        {
            var userFolderName = EnsureFolderName(request.FolderName);

            var fileList = await _storageService.GetFileListAsync(userFolderName);

            var userDirectory = Path.Combine(_settings.StoragePath, userFolderName);
            var uploadedDirectoryPath = Path.Combine(userDirectory, _settings.UploadedDirectoryName);
            Directory.CreateDirectory(uploadedDirectoryPath);

            var uploadedFiles = new IndexedFileDescriptionEntity[fileList.Length];
            for (int i = 0; i < fileList.Length; i++)
            {
                var fileName = fileList[i];
                var sourceFilePath = await EnsureFileExistsAsync(userFolderName, fileName);

                var fileInfo = new FileInfo(sourceFilePath);
                var descriptor = new IndexedFileDescriptionEntity()
                {
                    guid = fileInfo.Name,
                    name = fileInfo.Name,
                    isDirectory = false,
                    size = fileInfo.Length,
                };
                uploadedFiles[i] = descriptor;
            }
            return uploadedFiles;
        }

        public string Highlight(HighlightRequest request)
        {
            var userFolderName = EnsureFolderName(request.FolderName);

            var index = _indexFactoryService.Allocate();
            try
            {
                var document = DocumentSerializer.Deserialize(request.FoundDocumentId);
                var outputAdapter = new StringOutputAdapter();
                var highlighter = new HtmlHighlighter(outputAdapter);
                var options = new HighlightOptions();
                options.GenerateHead = false;
                options.UseInlineStyles = false;
                index.Highlight(document, highlighter, options);
                var text = outputAdapter.GetResult();
                var result = text
                    .Replace(userFolderName + "/", "")
                    .Replace(userFolderName, "")
                    .Replace("\"><span class=\"highlighted-term\">", "\"><span class=\"counted-term highlighted-term\">");
                return result;
            }
            finally
            {
                _indexFactoryService.Release(index);
            }
        }

        public MemoryStream GetSourceFile(HighlightRequest request, out string fileName)
        {
            var userFolderName = EnsureFolderName(request.FolderName);

            var document = DocumentSerializer.Deserialize(request.FoundDocumentId);
            var documentInfo = document.DocumentInfo;
            var documentKey = documentInfo.FilePath;
            fileName = GetDocumentFileName(userFolderName, documentKey);

            var ensureFileExistsTask = EnsureFileExistsAsync(userFolderName, fileName);
            ensureFileExistsTask.Wait();
            var sourceFilePath = ensureFileExistsTask.Result;

            var stream = new MemoryStream();
            using (var fileStream = File.OpenRead(sourceFilePath))
            {
                fileStream.CopyTo(stream);
            }
            stream.Position = 0;
            return stream;
        }

        public MemoryStream GetExtractedText(HighlightRequest request, out string fileName)
        {
            var userFolderName = EnsureFolderName(request.FolderName);

            var index = _indexFactoryService.Allocate();
            try
            {
                var document = DocumentSerializer.Deserialize(request.FoundDocumentId);
                var documentInfo = document.DocumentInfo;
                var documentKey = documentInfo.FilePath;
                var localFileName = GetDocumentFileName(userFolderName, documentKey);

                var outputAdapter = new StringOutputAdapter();
                index.GetDocumentText(documentInfo, outputAdapter);
                var result = outputAdapter.GetResult()
                    .Replace(userFolderName + "/", "")
                    .Replace(userFolderName, "");

                var stream = new MemoryStream();
                using (var writer = new StreamWriter(stream, Encoding.UTF8, 4096, true))
                {
                    writer.Write(result);
                }
                stream.Position = 0;
                fileName = localFileName + ".html";
                return stream;
            }
            finally
            {
                _indexFactoryService.Release(index);
            }
        }

        public PrepareDocumentResponse PrepareDocument(PrepareDocumentRequest request)
        {
            Guid userId;
            var userFolderName = EnsureFolderName(request.FolderName, out userId);

            string fileName = request.FileName;
            string password = string.IsNullOrEmpty(request.Password) ? null : request.Password;

            var documentCache = _htmlCacheService.GetCache(userId, fileName);
            var ensureFileExistsTask = EnsureFileExistsAsync(userFolderName, fileName);
            ensureFileExistsTask.Wait();
            bool isPrepared = documentCache.PrepareDocument(password);

            var response = new PrepareDocumentResponse();
            response.fileName = fileName;
            response.isPrepared = isPrepared;
            return response;
        }

        public GetDocumentPageResponse GetDocumentPage(GetDocumentPageRequest request)
        {
            Guid userId;
            var userFolderName = EnsureFolderName(request.FolderName, out userId);

            string fileName = request.FileName;
            int pageNumber = request.PageNumber;

            var documentCache = _htmlCacheService.GetCache(userId, fileName);
            string pageName;
            int pageCount;
            var pageContent = documentCache.GetPageContent(pageNumber, out pageName, out pageCount);
            var data = HighlightTermsInHtml(pageContent, request.Terms, request.TermSequences, request.CaseSensitive);
            var response = new GetDocumentPageResponse
            {
                fileName = fileName,
                pageNumber = pageNumber,
                pageCount = pageCount,
                sheetName = pageName,
                data = data,
            };
            return response;
        }

        public Stream GetResource(string containerName, string resourceName, out string contentType)
        {
            if (string.IsNullOrEmpty(containerName))
            {
                throw new ArgumentNullException(nameof(containerName));
            }
            if (string.IsNullOrEmpty(resourceName))
            {
                throw new ArgumentNullException(nameof(resourceName));
            }

            string fileName;
            var userId = ResourcePathConverter.GetFolderName(containerName, resourceName, out fileName);

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var documentCache = _htmlCacheService.GetCache(userId, fileName);
            var fileInfo = documentCache.FileInfo;

            string resourceFilePath = fileInfo.GetResourceFilePath(resourceName);

            contentType = MimeMapping.GetMimeMapping(resourceFilePath);
            if (string.IsNullOrEmpty(contentType))
            {
                contentType = "application/octet-stream";
            }

            FileStream fileStream = new FileStream(resourceFilePath, FileMode.Open);
            return fileStream;
        }

        public void RemoveFileFromIndex(PostedDataEntity postedData)
        {
            var userFolderName = EnsureFolderName(postedData.FolderName);

            var fileNames = new string[] { postedData.guid };

            _taskQueueService.EnqueueDeleteTask(userFolderName, fileNames);
        }

        public SummarySearchResult Search(SearchPostedData request)
        {
            var userFolderName = EnsureFolderName(request.FolderName);

            var index = _indexFactoryService.Allocate();
            try
            {
                SearchOptions searchOptions = new SearchOptions();
                searchOptions.SearchDocumentFilter = SearchDocumentFilter.CreateAttribute(userFolderName);
                searchOptions.UseCaseSensitiveSearch = request.CaseSensitiveSearch;
                if (!request.CaseSensitiveSearch)
                {
                    searchOptions.FuzzySearch.Enabled = request.FuzzySearch;
                    searchOptions.FuzzySearch.FuzzyAlgorithm = new TableDiscreteFunction(request.FuzzySearchMistakeCount);
                    searchOptions.FuzzySearch.OnlyBestResults = request.FuzzySearchOnlyBestResults;
                    searchOptions.KeyboardLayoutCorrector.Enabled = request.KeyboardLayoutCorrection;
                    searchOptions.UseSynonymSearch = request.SynonymSearch;
                    searchOptions.UseHomophoneSearch = request.HomophoneSearch;
                    searchOptions.UseWordFormsSearch = request.WordFormsSearch;
                    searchOptions.SpellingCorrector.Enabled = request.SpellingCorrection;
                    searchOptions.SpellingCorrector.MaxMistakeCount = request.SpellingCorrectionMistakeCount;
                    searchOptions.SpellingCorrector.OnlyBestResults = request.SpellingCorrectionOnlyBestResults;
                }

                var searchQuery = request.Query.Trim();
                var alphabet = index.Dictionaries.Alphabet;
                var containedSeparators = new HashSet<char>();
                foreach (char c in searchQuery)
                {
                    var type = alphabet.GetCharacterType(c);
                    if (type == CharacterType.Separator)
                    {
                        containedSeparators.Add(c);
                    }
                }

                if (containedSeparators.Count > 0)
                {
                    foreach (char specialChar in containedSeparators)
                    {
                        searchQuery = searchQuery.Replace(specialChar, ' ');
                    }
                }

                switch (request.SearchType)
                {
                    case "SearchAll":
                        searchQuery = searchQuery.Replace(" ", " AND ");
                        break;
                    case "SearchAny":
                        searchQuery = searchQuery.Replace(" ", " OR ");
                        break;
                    default:
                        if (searchQuery.Contains(' '))
                        {
                            searchQuery = "\"" + searchQuery + "\"";
                        }
                        break;
                }

                var result = index.Search(searchQuery, searchOptions);

                SummarySearchResult summaryResult = new SummarySearchResult();
                List<SearchDocumentResult> foundFiles = new List<SearchDocumentResult>();

                HighlightOptions options = new HighlightOptions
                {
                    TermsBefore = 5,
                    TermsAfter = 5,
                    TermsTotal = 13,
                };

                for (int i = 0; i < result.DocumentCount; i++)
                {
                    SearchDocumentResult searchDocumentResult = new SearchDocumentResult();

                    FoundDocument document = result.GetFoundDocument(i);
                    var documentId = DocumentSerializer.Serialize(document);
                    HtmlFragmentHighlighter highlighter = new HtmlFragmentHighlighter();
                    index.Highlight(document, highlighter, options);
                    FragmentContainer[] fragmentContainers = highlighter.GetResult();

                    List<string> foundPhrases = new List<string>();
                    for (int j = 0; j < fragmentContainers.Length; j++)
                    {
                        FragmentContainer container = fragmentContainers[j];
                        if (!string.Equals(container.FieldName, "filename", StringComparison.OrdinalIgnoreCase))
                        {
                            string[] fragments = container.GetFragments();
                            if (fragments.Length > 0)
                            {
                                for (int k = 0; k < fragments.Length; k++)
                                {
                                    var handledFragment = fragments[k].Replace("<br>", string.Empty);
                                    foundPhrases.Add(handledFragment);
                                }
                            }
                        }
                    }

                    int occurrences = foundPhrases.Count == 0 ? 0 : document.OccurrenceCount;
                    var documentKey = document.DocumentInfo.FilePath;
                    var fileName = GetDocumentFileName(userFolderName, documentKey);

                    var ensureFileExistsTask = EnsureFileExistsAsync(userFolderName, fileName);
                    ensureFileExistsTask.Wait();
                    var sourceFilePath = ensureFileExistsTask.Result;

                    var fileInfo = new FileInfo(sourceFilePath);
                    var fullName = document.DocumentInfo.ToString()
                        .Replace(userFolderName + "/", "")
                        .Replace(userFolderName, "")
                        .Replace("\\", "/");
                    searchDocumentResult.guid = fileName;
                    searchDocumentResult.name = fullName;
                    searchDocumentResult.size = fileInfo.Length;
                    searchDocumentResult.occurrences = occurrences;
                    searchDocumentResult.foundPhrases = foundPhrases.ToArray();
                    searchDocumentResult.terms = document.Terms;
                    searchDocumentResult.termSequences = document.TermSequences;
                    searchDocumentResult.documentId = documentId;
                    searchDocumentResult.isCaseSensitive = request.CaseSensitiveSearch;
                    searchDocumentResult.formatFamily = document.DocumentInfo.FormatFamily.Name.ToLowerInvariant();

                    foundFiles.Add(searchDocumentResult);
                }

                summaryResult.foundFiles = foundFiles.ToArray();
                summaryResult.totalOccurences = result.OccurrenceCount;
                summaryResult.totalFiles = result.DocumentCount;
                string searchDurationString = result.SearchDuration.ToString(@"ss\.ff");
                summaryResult.searchDuration = searchDurationString.Equals("00.00") ? "< 1" : searchDurationString;
                summaryResult.indexedFiles = index.GetIndexedDocuments().Length;

                return summaryResult;
            }
            finally
            {
                _indexFactoryService.Release(index);
            }
        }

        public async Task<UploadedDocumentEntity> UploadDocumentAsync(UploadDocumentContext context)
        {
            Guid userId;
            var userFolderName = EnsureFolderName(context.FolderName, out userId);

            string fileName;
            if (string.IsNullOrEmpty(context.Url))
            {
                // Upload from disk
                _demoValidator.CheckFileLength(context.FileLength);

                using (var tempStream = new MemoryStream())
                {
                    await context.FileCopyToAsync(tempStream);
                    tempStream.Position = 0;
                    fileName = context.FileName;

                    var fileInfo = new UserFileInfo(_settings, userId, fileName);
                    await UploadFileAsync(fileInfo, tempStream);
                }
            }
            else
            {
                // Upload from URL
                using (WebClient client = new WebClient())
                {
                    Uri uri = new Uri(context.Url);
                    fileName = Path.GetFileName(uri.LocalPath);
                    var data = await client.DownloadDataTaskAsync(uri);
                    _demoValidator.CheckFileLength(data.Length);

                    using (var tempStream = new MemoryStream(data))
                    {
                        var fileInfo = new UserFileInfo(_settings, userId, fileName);
                        await UploadFileAsync(fileInfo, tempStream);
                    }
                }
            }

            if (context.IndexAfterUpload)
            {
                _taskQueueService.EnqueueAddTask(
                    userFolderName,
                    new string[] { fileName },
                    context.RecognizeTextInImages,
                    true);
            }

            UploadedDocumentEntity uploadedDocument = new UploadedDocumentEntity
            {
                guid = fileName,
                isRestricted = false,
            };
            return uploadedDocument;
        }

        public MemoryStream GetAppInfo(string id)
        {
            var index = _indexFactoryService.Allocate();
            try
            {
                var stream = new MemoryStream();
                if (id == _settings.AdminId)
                {
                    using (var writer = new StreamWriter(stream, Encoding.UTF8, 4096, true))
                    {
                        GetSearchAppLog(writer);
                        GetIndexLog(writer);
                        GetSystemInfo(writer);
                        GetIndexInfo(index, writer);
                        GetFileList(writer);
                        GetIndexedFileList(index, writer);
                        GetUploadedFileList(writer);
                    }
                    stream.Position = 0;
                }
                return stream;
            }
            finally
            {
                _indexFactoryService.Release(index);
            }
        }

        public void RequestReindex(SearchBaseRequest request)
        {
            var userFolderName = EnsureFolderName(request.FolderName);

            if (userFolderName == _settings.AdminId)
            {
                _reindexingService.Reindex();
            }
        }

        public AlphabetReadResponse GetAlphabetDictionary(SearchBaseRequest request)
        {
            var userFolderName = EnsureFolderName(request.FolderName);

            var response = new AlphabetReadResponse();

            if (userFolderName != _settings.AdminId)
            {
                return response;
            }

            using (var tempIndex = _indexFactoryService.CreateTempIndexInMemory())
            {
                var dictionary = tempIndex.Dictionaries.Alphabet;

                _dictionaryStorageService.Load(_settings.AlphabetFileName, dictionary, d => d.Clear());

                int count = dictionary.Count;

                response.characters = new AlphabetCharacter[count];
                int order = 0;
                for (int i = char.MinValue; i <= char.MaxValue; i++)
                {
                    var characterType = dictionary.GetCharacterType((char)i);
                    if (characterType != CharacterType.Separator)
                    {
                        response.characters[order] = new AlphabetCharacter()
                        {
                            Character = i,
                            Type = (int)characterType,
                        };
                        order++;
                    }
                }

                GC.KeepAlive(tempIndex);
                return response;
            }
        }

        public void SetAlphabetDictionary(AlphabetUpdateRequest request)
        {
            var userFolderName = EnsureFolderName(request.FolderName);

            if (userFolderName != _settings.AdminId)
            {
                return;
            }

            using (var tempIndex = _indexFactoryService.CreateTempIndexInMemory())
            {
                var dictionary = tempIndex.Dictionaries.Alphabet;

                var separator = Enumerable.Range(char.MinValue, char.MaxValue)
                    .Select(v => (char)v);
                {
                    int letterType = (int)CharacterType.Letter;
                    var letter = request.Characters
                        .Where(ac => ac.Type == letterType)
                        .Select(ac => (char)ac.Character)
                        .ToArray();
                    dictionary.SetRange(letter, CharacterType.Letter);
                    separator = separator.Except(letter);
                }
                {
                    int blendedType = (int)CharacterType.Blended;
                    var blended = request.Characters
                        .Where(ac => ac.Type == blendedType)
                        .Select(ac => (char)ac.Character)
                        .ToArray();
                    dictionary.SetRange(blended, CharacterType.Blended);
                    separator = separator.Except(blended);
                }
                {
                    int separateWordType = (int)CharacterType.SeparateWord;
                    var separateWord = request.Characters
                        .Where(ac => ac.Type == separateWordType)
                        .Select(ac => (char)ac.Character)
                        .ToArray();
                    dictionary.SetRange(separateWord, CharacterType.SeparateWord);
                    separator = separator.Except(separateWord);
                }
                dictionary.SetRange(separator.ToArray(), CharacterType.Separator);

                _dictionaryStorageService.Save(_settings.AlphabetFileName, dictionary);

                GC.KeepAlive(tempIndex);
            }
        }

        public StopWordsReadResponse GetStopWordDictionary(SearchBaseRequest request)
        {
            var userFolderName = EnsureFolderName(request.FolderName);

            var response = new StopWordsReadResponse();

            if (userFolderName != _settings.AdminId)
            {
                return response;
            }

            using (var tempIndex = _indexFactoryService.CreateTempIndexInMemory())
            {
                var dictionary = tempIndex.Dictionaries.StopWordDictionary;

                _dictionaryStorageService.Load(_settings.StopWordDictionaryFileName, dictionary, d => d.Clear());

                response.stopWords = dictionary.ToArray();

                GC.KeepAlive(tempIndex);
                return response;
            }
        }

        public void SetStopWordDictionary(StopWordsUpdateRequest request)
        {
            var userFolderName = EnsureFolderName(request.FolderName);

            if (userFolderName != _settings.AdminId)
            {
                return;
            }

            using (var tempIndex = _indexFactoryService.CreateTempIndexInMemory())
            {
                var dictionary = tempIndex.Dictionaries.StopWordDictionary;

                dictionary.Clear();
                dictionary.AddRange(request.StopWords);

                _dictionaryStorageService.Save(_settings.StopWordDictionaryFileName, dictionary);

                GC.KeepAlive(tempIndex);
            }
        }

        public SynonymsReadResponse GetSynonymGroups(SearchBaseRequest request)
        {
            var userFolderName = EnsureFolderName(request.FolderName);

            var response = new SynonymsReadResponse();

            if (userFolderName != _settings.AdminId)
            {
                return response;
            }

            using (var tempIndex = _indexFactoryService.CreateTempIndexInMemory())
            {
                var dictionary = tempIndex.Dictionaries.SynonymDictionary;

                _dictionaryStorageService.Load(_settings.SynonymDictionaryFileName, dictionary, d => d.Clear());

                response.synonymGroups = tempIndex.Dictionaries.SynonymDictionary.GetAllSynonymGroups();

                GC.KeepAlive(tempIndex);
                return response;
            }
        }

        public void SetSynonymGroups(SynonymsUpdateRequest request)
        {
            var userFolderName = EnsureFolderName(request.FolderName);

            if (userFolderName != _settings.AdminId)
            {
                return;
            }

            using (var tempIndex = _indexFactoryService.CreateTempIndexInMemory())
            {
                var dictionary = tempIndex.Dictionaries.SynonymDictionary;

                dictionary.Clear();
                dictionary.AddRange(request.SynonymGroups);

                _dictionaryStorageService.Save(_settings.SynonymDictionaryFileName, dictionary);

                GC.KeepAlive(tempIndex);
            }
        }

        public HomophonesReadResponse GetHomophoneGroups(SearchBaseRequest request)
        {
            var userFolderName = EnsureFolderName(request.FolderName);

            var response = new HomophonesReadResponse();

            if (userFolderName != _settings.AdminId)
            {
                return response;
            }

            using (var tempIndex = _indexFactoryService.CreateTempIndexInMemory())
            {
                var dictionary = tempIndex.Dictionaries.HomophoneDictionary;

                _dictionaryStorageService.Load(_settings.HomophoneDictionaryFileName, dictionary, d => d.Clear());

                response.homophoneGroups = tempIndex.Dictionaries.HomophoneDictionary.GetAllHomophoneGroups();

                GC.KeepAlive(tempIndex);
                return response;
            }
        }

        public void SetHomophoneGroups(HomophonesUpdateRequest request)
        {
            var userFolderName = EnsureFolderName(request.FolderName);

            if (userFolderName != _settings.AdminId)
            {
                return;
            }

            using (var tempIndex = _indexFactoryService.CreateTempIndexInMemory())
            {
                var dictionary = tempIndex.Dictionaries.HomophoneDictionary;

                dictionary.Clear();
                dictionary.AddRange(request.HomophoneGroups);

                _dictionaryStorageService.Save(_settings.HomophoneDictionaryFileName, dictionary);

                GC.KeepAlive(tempIndex);
            }
        }

        public SpellingCorrectorReadResponse GetSpellingCorrectorWords(SearchBaseRequest request)
        {
            var userFolderName = EnsureFolderName(request.FolderName);

            var response = new SpellingCorrectorReadResponse();

            if (userFolderName != _settings.AdminId)
            {
                return response;
            }

            using (var tempIndex = _indexFactoryService.CreateTempIndexInMemory())
            {
                var dictionary = tempIndex.Dictionaries.SpellingCorrector;

                _dictionaryStorageService.Load(_settings.SpellingCorrectorDictionaryFileName, dictionary, d => d.Clear());

                response.words = tempIndex.Dictionaries.SpellingCorrector.GetWords();

                GC.KeepAlive(tempIndex);
                return response;
            }
        }

        public void SetSpellingCorrectorWords(SpellingCorrectorUpdateRequest request)
        {
            var userFolderName = EnsureFolderName(request.FolderName);

            if (userFolderName != _settings.AdminId)
            {
                return;
            }

            using (var tempIndex = _indexFactoryService.CreateTempIndexInMemory())
            {
                var dictionary = tempIndex.Dictionaries.SpellingCorrector;

                dictionary.Clear();
                dictionary.AddRange(request.Words);

                _dictionaryStorageService.Save(_settings.SpellingCorrectorDictionaryFileName, dictionary);

                GC.KeepAlive(tempIndex);
            }
        }

        private async Task<string> EnsureFileExistsAsync(string userFolderName, string fileName)
        {
            string temp;
            string uploadedDirectoryPath;
            var sourceFilePath = UserFileInfo.GetSourceFilePath(
                _settings,
                userFolderName,
                fileName,
                out temp,
                out uploadedDirectoryPath);
            if (!File.Exists(sourceFilePath))
            {
                Directory.CreateDirectory(uploadedDirectoryPath);
                using (var inputStream = await _storageService.DownloadFileAsync(userFolderName, fileName))
                using (var outputStream = File.Create(sourceFilePath))
                {
                    await inputStream.CopyToAsync(outputStream);
                }
            }
            return sourceFilePath;
        }

        private static string EnsureFolderName(string folderName)
        {
            Guid temp;
            return EnsureFolderName(folderName, out temp);
        }

        private static string EnsureFolderName(string folderName, out Guid userId)
        {
            if (Guid.TryParse(folderName, out userId))
            {
                return folderName.ToLowerInvariant();
            }

            throw new DemoException("The user folder name is not provided in the request.");
        }

        private async Task UploadFileAsync(
            UserFileInfo userFileInfo,
            MemoryStream inputStream)
        {
            _demoValidator.CheckUploadedFiles(userFileInfo.UploadedDirectoryPath);

            // Copy user's file to local folder
            Directory.CreateDirectory(userFileInfo.UploadedDirectoryPath);
            inputStream.Position = 0;
            using (var outputStream = File.Create(userFileInfo.SourceFilePath))
            {
                await inputStream.CopyToAsync(outputStream);
            }

            // Copy user's file to Storage
            inputStream.Position = 0;
            await _storageService.UploadFileAsync(userFileInfo.UserFolderName, userFileInfo.FileName, inputStream);

            try
            {
                var fileFolderPath = userFileInfo.FileCacheFolderPath;
                if (Directory.Exists(fileFolderPath))
                {
                    Directory.Delete(fileFolderPath, true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Upload document error: removing folder from viewer cache.");
            }
        }

        private void GetFileList(StreamWriter writer)
        {
            var stringBuilder = new StringBuilder();
            try
            {
                var indexDirectory = Path.Combine(_settings.StoragePath, _settings.IndexDirectoryName);
                var files = Directory.GetFiles(indexDirectory, "*.*", SearchOption.AllDirectories);
                foreach (var filePath in files)
                {
                    var fileInfo = new System.IO.FileInfo(filePath);
                    var length = fileInfo.Length.ToString(CultureInfo.InvariantCulture).PadRight(10, ' ');
                    stringBuilder.AppendLine(length + filePath);
                }
            }
            catch (Exception ex)
            {
                stringBuilder.AppendLine(ex.ToString());
            }

            writer.WriteLine("------------------------------- Index files -------------------------------");
            writer.WriteLine(stringBuilder);
            writer.WriteLine();
        }

        private void GetUploadedFileList(StreamWriter writer)
        {
            var stringBuilder = new StringBuilder();
            try
            {
                var directories = Directory.GetDirectories(_settings.StoragePath, "*.*", SearchOption.TopDirectoryOnly);
                foreach (var directoryPath in directories)
                {
                    var directoryInfo = new DirectoryInfo(directoryPath);
                    var name = directoryInfo.Name;
                    Guid temp;
                    if (Guid.TryParse(name, out temp))
                    {
                        var path = Path.Combine(directoryInfo.FullName, _settings.UploadedDirectoryName);
                        stringBuilder.AppendLine(name);
                        if (Directory.Exists(path))
                        {
                            var files = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly);
                            foreach (var filePath in files)
                            {
                                var fileInfo = new System.IO.FileInfo(filePath);
                                var length = fileInfo.Length.ToString(CultureInfo.InvariantCulture).PadRight(10, ' ');
                                stringBuilder.AppendLine("    " + length + fileInfo.Name);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                stringBuilder.AppendLine(ex.ToString());
            }

            writer.WriteLine("------------------------------- Uploaded files -------------------------------");
            writer.WriteLine(stringBuilder);
            writer.WriteLine();
        }

        private void GetSystemInfo(StreamWriter writer)
        {
            var stringBuilder = new StringBuilder();
            try
            {
                stringBuilder.AppendLine("OSVersion: " + Environment.OSVersion);
                stringBuilder.AppendLine("MachineName: " + Environment.MachineName);
                stringBuilder.AppendLine("UserName: " + Environment.UserName);
                stringBuilder.AppendLine("UserDomainName: " + Environment.UserDomainName);
                stringBuilder.AppendLine("ProcessorCount: " + Environment.ProcessorCount);
                stringBuilder.AppendLine("CLR version: " + Environment.Version);
                stringBuilder.AppendLine("SystemPageSize (B): " + Environment.SystemPageSize);
                stringBuilder.AppendLine("Memory working set (MB): " + (Environment.WorkingSet / 1024 / 1024));
                stringBuilder.AppendLine("LogicalDrives: " + string.Join(", ", Environment.GetLogicalDrives()));
                stringBuilder.AppendLine("Ellapsed: " + TimeSpan.FromMilliseconds(Environment.TickCount).ToString("c"));
            }
            catch (Exception ex)
            {
                stringBuilder.AppendLine(ex.ToString());
            }

            writer.WriteLine("------------------------------- System info -------------------------------");
            writer.WriteLine(stringBuilder);
            writer.WriteLine();
        }

        private void GetIndexInfo(Index index, StreamWriter writer)
        {
            var stringBuilder = new StringBuilder();
            try
            {
                stringBuilder.AppendLine("IndexVersion: " + index.IndexInfo.Version);
                stringBuilder.AppendLine("IndexType: " + index.IndexSettings.IndexType.ToString());
                stringBuilder.AppendLine("UseStopWords: " + index.IndexSettings.UseStopWords);
                stringBuilder.AppendLine("UseCharacterReplacements: " + index.IndexSettings.UseCharacterReplacements);
                stringBuilder.AppendLine("AutoDetectEncoding: " + index.IndexSettings.AutoDetectEncoding);
                stringBuilder.AppendLine("UseRawTextExtraction: " + index.IndexSettings.UseRawTextExtraction);
                var tss = index.IndexSettings.TextStorageSettings;
                stringBuilder.AppendLine("TextStorageCompression: " + (tss == null ? "No storage" : tss.Compression.ToString()));
                stringBuilder.AppendLine("IndexStatus: " + index.IndexInfo.IndexStatus);
            }
            catch (Exception ex)
            {
                stringBuilder.AppendLine(ex.ToString());
            }

            writer.WriteLine("------------------------------- Index info -------------------------------");
            writer.WriteLine(stringBuilder);
            writer.WriteLine();
        }

        private void GetIndexedFileList(Index index, StreamWriter writer)
        {
            var stringBuilder = new StringBuilder();
            try
            {
                var indexedDocuments = index.GetIndexedDocuments();
                foreach (var di in indexedDocuments)
                {
                    stringBuilder.AppendLine((di.IndexedWithError ? "error " : "ok    ") + di.FilePath);
                }
            }
            catch (Exception ex)
            {
                stringBuilder.AppendLine(ex.ToString());
            }

            writer.WriteLine("------------------------------- Indexed files -------------------------------");
            writer.WriteLine(stringBuilder);
            writer.WriteLine();
        }

        private void GetIndexLog(StreamWriter writer)
        {
            string log;
            try
            {
                var indexDirectory = Path.Combine(_settings.StoragePath, _settings.IndexDirectoryName);
                var indexLogFilePath = Path.Combine(indexDirectory, _settings.LogFileName);
                log = File.ReadAllText(indexLogFilePath);
            }
            catch (Exception ex)
            {
                log = ex.ToString();
            }

            writer.WriteLine("------------------------------- Index log -------------------------------");
            writer.WriteLine(log);
            writer.WriteLine();
        }

        private void GetSearchAppLog(StreamWriter writer)
        {
            string log;
            try
            {
                var filePath = _settings.LogFilePath;
                log = File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                log = ex.ToString();
            }

            writer.WriteLine("------------------------------- Search app log -------------------------------");
            writer.WriteLine(log);
            writer.WriteLine();
        }

        private static string GetDocumentFileName(string userId, string documentKey)
        {
            int startIndex = userId.Length + Settings.DocumentKeySeparator.Length;
            var fileName = documentKey.Substring(startIndex);
            return fileName;
        }

        private string HighlightTermsInHtml(string data, string[] terms, string[][] phrases, bool caseSensitive)
        {
            if ((terms == null || terms.Length == 0) &&
                (phrases == null || phrases.Length == 0))
            {
                return data;
            }

            var index = _indexFactoryService.Allocate();
            try
            {
                var highlightedData = Highlighter.HtmlHighlighterNew.Handle(
                    data,
                    caseSensitive,
                    index.Dictionaries.Alphabet,
                    terms,
                    phrases);
                return highlightedData;
            }
            finally
            {
                _indexFactoryService.Release(index);
            }
        }
    }
}
