using GroupDocs.Total.MVC.Products.Search.Dto.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GroupDocs.Total.MVC.Products.Search.Domain.SingleIndex
{
    internal class TaskQueueService : IDisposable
    {
        private const int MaxLogCount = 100;

        private readonly PreprocessingQueue _preprocessingQueue;

        private readonly object _syncRoot = new object();
        private readonly Queue<IndexTask> _queue = new Queue<IndexTask>();
        private IndexTask _currentTask;

        private bool _alive = true;
        private readonly AutoResetEvent _gate = new AutoResetEvent(false);

        private readonly ILogger _logger;
        private readonly IndexFactoryService _indexFactoryService;
        private readonly Settings _settings;
        private readonly DocumentStatusService _documentStatusService;
        private readonly StorageService _storageService;
        private readonly Queue<string> _logs = new Queue<string>();

        public TaskQueueService(
            ILogger logger,
            IndexFactoryService indexFactoryService,
            Settings settings,
            DocumentStatusService documentStatusService,
            StorageService storageService)
        {
            _logger = logger;
            _indexFactoryService = indexFactoryService;
            _settings = settings;
            _documentStatusService = documentStatusService;
            _storageService = storageService;
            var thread = new Thread(TaskRunner);
            thread.IsBackground = true;
            thread.Start();

            _preprocessingQueue = new PreprocessingQueue(_logger, OnIndexTaskPreprocessed);
        }

        ~TaskQueueService()
        {
            DisposePrivate();
        }

        public void Dispose()
        {
            DisposePrivate();
            GC.SuppressFinalize(this);
        }

        private void DisposePrivate()
        {
            _alive = false;
            _gate.Set();
            _gate.Dispose();
        }

        public string[] Logs => _logs.ToArray();

        public PreprocessingQueue PreprocessingQueue => _preprocessingQueue;

        public void EnqueueAddTask(
            string userId,
            string[] fileNames,
            bool recognizeTextInImages,
            bool allowMultipleTasksFromUser = false)
        {
            var task = new AddIndexTask(
                _logger,
                _indexFactoryService,
                _documentStatusService,
                _storageService,
                _settings,
                userId,
                recognizeTextInImages,
                fileNames);
            task.AllowMultipleTasksFromUser = allowMultipleTasksFromUser;
            Enqueue(task);
        }

        public void EnqueueDeleteTask(string userId, string[] fileNames)
        {
            var task = new DeleteIndexTask(
                _logger,
                _indexFactoryService,
                _documentStatusService,
                _settings,
                userId,
                fileNames);
            Enqueue(task);
        }

        public void EnqueueCleanupTask()
        {
            var task = new CleanupIndexTask(
                _logger,
                _indexFactoryService,
                _documentStatusService,
                _settings);
            Enqueue(task);
        }

        public TaskQueueInfo GetTasks()
        {
            lock (_syncRoot)
            {
                var info = new TaskQueueInfo();
                info.tasks = _queue
                    .Select(it => it.ToString())
                    .ToArray();
                info.taskLogs = Logs;
                return info;
            }
        }

        private void Enqueue(IndexTask task)
        {
            task.Init();
            _preprocessingQueue.Enqueue(task);
        }

        private void OnIndexTaskPreprocessed(IndexTask task)
        {
            lock (_syncRoot)
            {
                _queue.Enqueue(task);
                _gate.Set();
            }
        }

        private void TaskRunner()
        {
            while (_gate.WaitOne() && _alive)
            {
                lock (_syncRoot)
                {
                    if (_queue.Count > 0)
                    {
                        _currentTask = _queue.Peek();
                    }
                }

                if (_currentTask != null)
                {
                    _currentTask.BeforeRun();
                    _currentTask.Run();
                    _currentTask.AfterRun();

                    if (!string.IsNullOrWhiteSpace(_currentTask.Logs))
                    {
                        _logs.Enqueue(_currentTask.ToString());
                    }
                    while (_logs.Count > MaxLogCount)
                    {
                        _logs.Dequeue();
                    }
                }

                lock (_syncRoot)
                {
                    if (_currentTask != null)
                    {
                        if (_queue.Count > 0)
                        {
                            _queue.Dequeue();
                        }
                        _currentTask = null;
                    }

                    if (_queue.Count > 0)
                    {
                        _gate.Set();
                    }
                }
            }
        }
    }
}
