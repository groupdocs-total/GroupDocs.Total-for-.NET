using System;

namespace GroupDocs.Total.MVC.Products.Search.Domain
{
    internal class Logger : ILogger
    {
        public void LogError(Exception exception, string message)
        {
            var time = DateTime.Now.ToString("s");
            var text = time + ": " + message + ": " + exception.ToString();
            Console.WriteLine(text);
        }

        public void LogError(string message)
        {
            var time = DateTime.Now.ToString("s");
            var text = time + ": " + message;
            Console.WriteLine(text);
        }

        public void LogInformation(string message)
        {
            var time = DateTime.Now.ToString("s");
            var text = time + ": " + message;
            Console.WriteLine(text);
        }
    }
}
