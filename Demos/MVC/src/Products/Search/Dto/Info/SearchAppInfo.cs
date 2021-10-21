namespace GroupDocs.Total.MVC.Products.Search.Dto.Info
{
    public class SearchAppInfo
    {
        public IndexStatusInfo IndexStatus { get; set; }

        public PreprocessingQueueInfo PreprocessingQueue { get; set; }

        public TaskQueueInfo TaskQueue { get; set; }

        public ExistingDocumentInfo[] DocumentList { get; set; }

        public ExistingFolderInfo[] FolderList { get; set; }
    }
}
