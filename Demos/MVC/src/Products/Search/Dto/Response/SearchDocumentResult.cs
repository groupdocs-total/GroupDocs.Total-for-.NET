namespace GroupDocs.Total.MVC.Products.Search.Dto.Response
{
    public class SearchDocumentResult
    {
        public string guid { get; set; }

        public string name { get; set; }

        public long size { get; set; }

        public int occurrences { get; set; }

        public string[] foundPhrases { get; set; }

        public string[] terms { get; set; }

        public string[][] termSequences { get; set; }

        public string documentId { get; set; }

        public bool isCaseSensitive { get; set; }

        public string formatFamily { get; set; }
    }
}
