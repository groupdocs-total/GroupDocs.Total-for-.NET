using System;

namespace GroupDocs.Total.MVC.Products.Search.Domain
{
    public interface ILogger
    {
        void LogError(Exception exception, string message);

        void LogError(string message);

        void LogInformation(string message);
    }
}
