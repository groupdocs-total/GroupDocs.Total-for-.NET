using System;

namespace GroupDocs.Total.MVC.Products.Search.Dto
{
    public class ExceptionEntity
    {
        public string message { get; set; }

        public Exception exception { get; set; }

        public static ExceptionEntity Create(Exception ex)
        {
            var result = new ExceptionEntity()
            {
                message = ex.Message,
                exception = ex,
            };
            return result;
        }
    }
}
