namespace GroupDocs.Total.MVC.Products.Search.Dto.Info
{
    public class ExistingFolderInfo
    {
        public string FolderName { get; set; }

        public int DocumentCount { get; set; }

        public ExistingDocumentInfo[] DocumentList { get; set; }
    }
}
