namespace GroupDocs.Total.MVC.Products.Search.Dto.Info
{
    public class ExistingFolderInfo
    {
        public string folderName { get; set; }

        public int documentCount { get; set; }

        public ExistingDocumentInfo[] documentList { get; set; }
    }
}
