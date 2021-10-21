using System;
using System.Threading;

namespace GroupDocs.Total.MVC.Products.Search.Domain.SingleIndex
{
    internal class CleanupSchedulerService : IDisposable
    {
        private readonly TaskQueueService _taskQueueService;

        private readonly Timer _timer;

        public CleanupSchedulerService(
            Settings settings,
            TaskQueueService taskQueueService)
        {
            _taskQueueService = taskQueueService;

            _timer = new Timer(TimerTick, null, new TimeSpan(0, 0, 1), settings.CleanupPeriod);
        }

        public void Dispose()
        {
            _timer.Dispose();
        }

        private void TimerTick(object state)
        {
            //_taskQueueService.EnqueueCleanupTask();
        }
    }
}
