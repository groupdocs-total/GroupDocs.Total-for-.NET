namespace GroupDocs.Total.MVC.Products.Search.Dto.Info
{
    public class SearchAppInfo
    {
        public IndexStatusInfo indexStatus { get; set; }

        public PreprocessingQueueInfo preprocessingQueue { get; set; }

        public TaskQueueInfo taskQueue { get; set; }

        public ExistingDocumentInfo[] documentList { get; set; }

        public ExistingFolderInfo[] folderList { get; set; }
    }
}
