using System;

namespace GroupDocs.Total.MVC.Products.Search.Domain
{
    public class DemoException : Exception
    {
        public DemoException()
            : base()
        {
        }

        public DemoException(string message)
            : base(message)
        {
        }

        public DemoException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
