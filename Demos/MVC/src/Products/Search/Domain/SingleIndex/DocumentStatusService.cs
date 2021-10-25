using GroupDocs.Total.MVC.Products.Search.Domain.ViewerCache;
using GroupDocs.Total.MVC.Products.Search.Dto.Info;
using GroupDocs.Total.MVC.Products.Search.Dto.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GroupDocs.Total.MVC.Products.Search.Domain.SingleIndex
{
    internal class DocumentStatusService
    {
        private const int MaxWriteAttempts = 3;

        private readonly object _syncRoot = new object();
        private readonly Dictionary<DocumentKey, DocumentStatus> _statuses = new Dictionary<DocumentKey, DocumentStatus>();
        private readonly ILogger _logger;
        private readonly Settings _settings;

        public DocumentStatusService(
            ILogger logger,
            Settings settings)
        {
            _logger = logger;
            _settings = settings;
            LoadStatuses();
        }

        public void SetStatus(string userId, string fileName, DocumentStatus status)
        {
            var key = new DocumentKey(userId, fileName);
            lock (_syncRoot)
            {
                if (status == DocumentStatus.NotIndexed)
                {
                    _statuses.Remove(key);
                }
                else
                {
                    DocumentStatus currentStatus;
                    if (_statuses.TryGetValue(key, out currentStatus) &&
                        (currentStatus == DocumentStatus.Skipped ||
                        currentStatus == DocumentStatus.ProcessedWithError))
                    {
                    }
                    else
                    {
                        _statuses[key] = status;
                    }
                }
                WriteStatus(userId, fileName, status);
            }
        }

        public GetStatusResponse GetStatus(string userId)
        {
            lock (_syncRoot)
            {
                var statuses = _statuses
                    .Where(pair => pair.Key.UserId == userId)
                    .Select(pair => pair.Value);
                int indexing = 0;
                int pending = 0;
                int indexed = 0;
                foreach (var status in statuses)
                {
                    switch (status)
                    {
                        case DocumentStatus.Indexing:
                        case DocumentStatus.Merging:
                            indexing++;
                            break;
                        case DocumentStatus.Pending:
                        case DocumentStatus.Removing:
                        case DocumentStatus.NotIndexed:
                            pending++;
                            break;
                        case DocumentStatus.SuccessfullyProcessed:
                        case DocumentStatus.Skipped:
                        case DocumentStatus.ProcessedWithError:
                            indexed++;
                            break;
                    }
                }
                var response = new GetStatusResponse();
                response.Indexing = indexing;
                response.Pending = pending;
                response.Indexed = indexed;
                return response;
            }
        }

        public DocumentStatus GetStatus(string userId, string fileName)
        {
            var key = new DocumentKey(userId, fileName);
            lock (_syncRoot)
            {
                DocumentStatus status;
                if (_statuses.TryGetValue(key, out status))
                {
                    return status;
                }
            }

            return DocumentStatus.Unknown;
        }

        public DocumentStatus[] GetStatuses(string userId, string[] fileNames)
        {
            var result = new DocumentStatus[fileNames.Length];
            for (int i = 0; i < fileNames.Length; i++)
            {
                var fileName = fileNames[i];
                result[i] = GetStatus(userId, fileName);
            }
            return result;
        }

        public ExistingDocumentInfo[] GetStatuses()
        {
            lock (_syncRoot)
            {
                var array = _statuses
                .Select(pair => new ExistingDocumentInfo()
                {
                    FolderName = pair.Key.UserId,
                    FileName = pair.Key.FileName,
                    Length = GetFileLength(pair.Key.UserId, pair.Key.FileName),
                    Status = pair.Value.ToString(),
                })
                .ToArray();
                return array;
            }
        }

        public DocumentWithStatus[] GetDocuments(string userId)
        {
            lock (_syncRoot)
            {
                var result = _statuses
                    .Where(pair => pair.Key.UserId == userId)
                    .Select(pair => new DocumentWithStatus(pair.Key.FileName, pair.Value))
                    .ToArray();
                return result;
            }
        }

        public void Reload()
        {
            lock (_syncRoot)
            {
                _statuses.Clear();
                LoadStatuses();
            }
        }

        private long GetFileLength(string folderName, string fileName)
        {
            try
            {
                string temp1, temp2;
                var filePath = UserFileInfo.GetSourceFilePath(_settings, folderName, fileName, out temp1, out temp2);
                var fileInfo = new FileInfo(filePath);
                return fileInfo.Length;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private void WriteStatus(string userId, string fileName, DocumentStatus status)
        {
            var statusesDirectoryPath = Path.Combine(_settings.StoragePath, userId, _settings.StatusesDirectoryName);
            var statusFilePath = Path.Combine(statusesDirectoryPath, fileName);
            Exception exception;
            int writeAttemptCounter = 0;
            do
            {
                try
                {
                    if (status == DocumentStatus.NotIndexed)
                    {
                        Directory.CreateDirectory(statusesDirectoryPath);
                        File.Delete(statusFilePath);
                    }
                    else
                    {
                        Directory.CreateDirectory(statusesDirectoryPath);
                        File.WriteAllText(statusFilePath, status.ToString());
                    }
                    exception = null;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error writing document status.");
                    exception = ex;
                }
                writeAttemptCounter++;
            }
            while (exception != null && writeAttemptCounter < MaxWriteAttempts);
        }

        private void LoadStatuses()
        {
            try
            {
                Directory.CreateDirectory(_settings.StoragePath);
                var folders = Directory.GetDirectories(_settings.StoragePath, "*", SearchOption.TopDirectoryOnly);
                for (int i = 0; i < folders.Length; i++)
                {
                    var fullPath = folders[i];
                    UserFolder userFolder;
                    if (UserFolder.TryCreate(fullPath, out userFolder))
                    {
                        var statusesDirectory = Path.Combine(userFolder.FullPath, _settings.StatusesDirectoryName);
                        Directory.CreateDirectory(statusesDirectory);
                        var statusFiles = Directory.GetFiles(statusesDirectory, "*.*", SearchOption.TopDirectoryOnly);
                        for (int j = 0; j < statusFiles.Length; j++)
                        {
                            var statusFile = statusFiles[j];
                            try
                            {
                                var statusText = File.ReadAllText(statusFile);
                                var status = (DocumentStatus)Enum.Parse(typeof(DocumentStatus), statusText);
                                var fileName = Path.GetFileName(statusFile);
                                var key = new DocumentKey(userFolder.Name, fileName);
                                _statuses.Add(key, status);
                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e, "Error loading document status.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading document statuses.");
            }
        }
    }
}
