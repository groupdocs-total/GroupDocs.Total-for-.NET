namespace GroupDocs.Total.MVC.Products.Search.Dto.Response
{
    public class SummarySearchResult
    {
        public int totalOccurences { get; set; }

        public int totalFiles { get; set; }

        public int indexedFiles { get; set; }

        public SearchDocumentResult[] foundFiles { get; set; }

        public string searchDuration { get; set; }
    }
}
