namespace GroupDocs.Total.MVC.Products.Search.Domain.SingleIndex
{
    internal class DocumentWithStatus
    {
        public DocumentWithStatus(string fileName, DocumentStatus status)
        {
            FileName = fileName;
            Status = status;
        }

        public string FileName { get; }

        public DocumentStatus Status { get; }
    }
}
