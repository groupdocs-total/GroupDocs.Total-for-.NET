using Microsoft.Extensions.Logging;
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

        public IDisposable BeginScope<TState>(TState state)
        {
            return _logger.BeginScope(state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _logger.IsEnabled(logLevel);
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            var time = DateTime.Now.ToString("s");
            var message = formatter(state, exception);
            _logs.AppendLine(time + ": " + message);
            _logger.Log(logLevel, eventId, state, exception, formatter);
        }
    }
}
