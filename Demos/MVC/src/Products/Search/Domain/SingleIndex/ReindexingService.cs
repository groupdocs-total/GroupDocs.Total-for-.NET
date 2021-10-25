using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Index = GroupDocs.Search.Index;

namespace GroupDocs.Total.MVC.Products.Search.Domain.SingleIndex
{
    internal class ReindexingService
    {
        private readonly ILogger _logger;
        private readonly Settings _settings;
        private readonly IndexFactoryService _indexFactoryService;
        private readonly TaskQueueService _taskQueueService;

        public ReindexingService(
            ILogger logger,
            Settings settings,
            IndexFactoryService indexFactoryService,
            TaskQueueService taskQueueService)
        {
            _logger = logger;
            _settings = settings;
            _indexFactoryService = indexFactoryService;
            _indexFactoryService.ExclusiveIndexAccess += OnExclusiveIndexAccess;
            _taskQueueService = taskQueueService;
        }

        public void Reindex()
        {
            _indexFactoryService.RequestExclusiveIndexAccess();
        }

        private void OnExclusiveIndexAccess()
        {
            DoReindex();
        }

        public void Reindex(DateTime limit)
        {
            var currentDate = DateTime.Now;
            if (currentDate > limit)
            {
                return;
            }

            DoReindex();
        }

        private void DoReindex()
        {
            try
            {
                DocumentKey[] documents = GetDocuments();

                AddDocumentsToIndex(documents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reindexing error.");
            }
        }

        private DocumentKey[] GetDocuments()
        {
            Index index = _indexFactoryService.Allocate();
            try
            {
                var indexedDocumentKeys = index.GetIndexedDocuments()
                .Select(di =>
                {
                    DocumentKey documentKey;
                    if (DocumentKey.TryCreateFromString(di.FilePath, out documentKey))
                    {
                        return documentKey;
                    }
                    else
                    {
                        return null;
                    }
                })
                .Where(dk => dk != null)
                .ToArray();

                var uploadedDocumentKeys = new List<DocumentKey>();
                Directory.CreateDirectory(_settings.StoragePath);
                var userDirectories = Directory.GetDirectories(_settings.StoragePath, "*", SearchOption.TopDirectoryOnly);
                for (int i = 0; i < userDirectories.Length; i++)
                {
                    var fullPath = userDirectories[i];
                    UserFolder userFolder;
                    if (UserFolder.TryCreate(fullPath, out userFolder))
                    {
                        var userId = userFolder.Guid.ToString();
                        var uploadedDirectory = Path.Combine(userFolder.FullPath, _settings.UploadedDirectoryName);
                        var files = Directory.GetFiles(uploadedDirectory);
                        for (int j = 0; j < files.Length; j++)
                        {
                            var filePath = files[j];
                            var fileName = Path.GetFileName(filePath);
                            var documentKey = new DocumentKey(userId, fileName);
                            uploadedDocumentKeys.Add(documentKey);
                        }
                    }
                }

                var result = indexedDocumentKeys
                        .Intersect(uploadedDocumentKeys)
                        .ToArray();
                return result;
            }
            finally
            {
                _indexFactoryService.Release(index);
            }
        }

        private void AddDocumentsToIndex(DocumentKey[] documents)
        {
            Index index = _indexFactoryService.Allocate(true);
            try
            {
                var groups = documents
                    .GroupBy(dk => dk.UserId);
                foreach (var group in groups)
                {
                    var userId = group.Key;
                    var fileNames = group
                        .Select(dk => dk.FileName)
                        .ToArray();
                    _taskQueueService.EnqueueAddTask(userId, fileNames, false);
                }
            }
            finally
            {
                _indexFactoryService.Release(index);
            }
        }
    }
}
