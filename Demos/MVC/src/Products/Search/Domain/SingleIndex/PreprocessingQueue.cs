using GroupDocs.Total.MVC.Products.Search.Dto.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroupDocs.Total.MVC.Products.Search.Domain.SingleIndex
{
    internal class PreprocessingQueue
    {
        private const int MaxThreadCount = 10;

        private readonly ILogger _logger;
        private readonly Action<IndexTask> _indexTaskPreprocessedCallback;

        private readonly object _syncRoot = new object();
        private readonly LinkedList<IndexTask> _queue = new LinkedList<IndexTask>();

        private readonly HashSet<IndexTask> _activeTasks = new HashSet<IndexTask>();

        public PreprocessingQueue(
            ILogger logger,
            Action<IndexTask> indexTaskPreprocessedCallback)
        {
            _logger = logger;
            _indexTaskPreprocessedCallback = indexTaskPreprocessedCallback;
        }

        public PreprocessingQueueInfo GetTasks()
        {
            lock (_syncRoot)
            {
                var info = new PreprocessingQueueInfo();
                info.activeTasks = _activeTasks
                    .Select(it => it.ToString())
                    .ToArray();
                info.queuedTasks = _queue
                    .Select(it => it.ToString())
                    .ToArray();
                return info;
            }
        }

        public void Enqueue(IndexTask indexTask)
        {
            lock (_syncRoot)
            {
                if (!indexTask.AllowMultipleTasksFromUser &&
                    HasIndexTask(indexTask))
                {
                    throw new DemoException("Only one index task can be initiated at a time.");
                }

                if (_activeTasks.Count < MaxThreadCount &&
                    _activeTasks.All(it => it.UserId != indexTask.UserId))
                {
                    Run(indexTask);
                }
                else
                {
                    _queue.AddLast(indexTask);
                }
            }
        }

        private bool HasIndexTask(IndexTask indexTask)
        {
            return
                _queue.Any(it => it.UserId == indexTask.UserId) ||
                _activeTasks.Any(it => it.UserId == indexTask.UserId);
        }

        private void Run(IndexTask indexTask)
        {
            _activeTasks.Add(indexTask);
            Task.Factory.StartNew(
                () =>
                {
                    try
                    {
                        indexTask.BeforePreprocess();
                        indexTask.Preprocess();
                        indexTask.AfterPreprocess();
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception, "Error of preprocessing index task: " + GetType().Name);
                    }

                    OnCompleted(indexTask);

                    _indexTaskPreprocessedCallback(indexTask);
                },
                TaskCreationOptions.LongRunning);
        }

        private void OnCompleted(IndexTask indexTask)
        {
            lock (_syncRoot)
            {
                _activeTasks.Remove(indexTask);

                if (_activeTasks.Count < MaxThreadCount)
                {
                    var currentNode = _queue.First;
                    while (currentNode != null)
                    {
                        var userId = currentNode.Value.UserId;
                        if (_activeTasks.All(it => it.UserId != userId))
                        {
                            _queue.Remove(currentNode);
                            Run(currentNode.Value);
                            break;
                        }

                        currentNode = currentNode.Next;
                    }
                }
            }
        }
    }
}
