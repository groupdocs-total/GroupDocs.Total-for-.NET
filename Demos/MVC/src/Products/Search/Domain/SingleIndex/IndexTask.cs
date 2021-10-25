using System;
using System.Collections.Generic;
using System.Linq;
using Index = GroupDocs.Search.Index;

namespace GroupDocs.Total.MVC.Products.Search.Domain.SingleIndex
{
    internal abstract class IndexTask
    {
        private readonly IndexTaskLogger _logger;
        private readonly IndexFactoryService _indexFactoryService;
        private readonly Settings _settings;
        private readonly DocumentStatusService _documentStatusService;
        private readonly string _userId;
        private readonly string[] _fileNames;

        public IndexTask(
            ILogger logger,
            IndexFactoryService indexFactoryService,
            Settings settings,
            DocumentStatusService documentStatusService,
            string userId,
            IEnumerable<string> fileNames)
        {
            _logger = new IndexTaskLogger(logger);
            _indexFactoryService = indexFactoryService;
            _settings = settings;
            _documentStatusService = documentStatusService;
            _userId = userId;
            _fileNames = fileNames.ToArray();
        }

        public ILogger Logger => _logger;

        public string Logs => _logger.Logs;

        public IndexFactoryService IndexFactoryService => _indexFactoryService;

        public Settings Settings => _settings;

        public DocumentStatusService DocumentStatusService => _documentStatusService;

        public string UserId => _userId;

        public string[] FileNames => _fileNames;

        public bool AllowMultipleTasksFromUser { get; set; }

        public abstract void Preprocess();

        public void Run()
        {
            var index = IndexFactoryService.Allocate();
            try
            {
                RunProtected(index);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error of performing index task: " + GetType().Name);
            }
            finally
            {
                IndexFactoryService.Release(index);
            }
        }

        protected void SetStatus(DocumentStatus status)
        {
            foreach (var fileName in FileNames)
            {
                DocumentStatusService.SetStatus(UserId, fileName, status);
            }
        }

        public abstract void Init();

        public abstract void BeforePreprocess();

        public abstract void AfterPreprocess();

        public abstract void BeforeRun();

        public abstract void AfterRun();

        public override string ToString()
        {
            var logs = Logs;
            var name = GetType().Name;
            var message = string.IsNullOrWhiteSpace(logs) ? name : name + " (" + UserId + "): " + logs;
            return message;
        }

        protected abstract void RunProtected(Index index);
    }
}
