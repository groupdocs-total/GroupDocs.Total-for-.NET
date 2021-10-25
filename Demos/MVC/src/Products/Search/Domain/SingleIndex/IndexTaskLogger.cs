using System;
using System.Text;

namespace GroupDocs.Total.MVC.Products.Search.Domain.SingleIndex
{
    internal class IndexTaskLogger : ILogger
    {
        private readonly ILogger _logger;
        private readonly StringBuilder _logs = new StringBuilder();

        public IndexTaskLogger(ILogger logger)
        {
            _logger = logger;
        }

        public string Logs => _logs.ToString();

        public void LogError(Exception exception, string message)
        {
            var time = DateTime.Now.ToString("s");
            _logs.AppendLine(time + ": " + message + ": " + exception.ToString());

            _logger.LogError(exception, message);
        }

        public void LogError(string message)
        {
            var time = DateTime.Now.ToString("s");
            _logs.AppendLine(time + ": " + message);

            _logger.LogError(message);
        }

        public void LogInformation(string message)
        {
            var time = DateTime.Now.ToString("s");
            _logs.AppendLine(time + ": " + message);

            _logger.LogInformation(message);
        }
    }
}
