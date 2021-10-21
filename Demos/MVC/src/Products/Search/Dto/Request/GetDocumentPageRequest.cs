namespace GroupDocs.Total.MVC.Products.Search.Dto.Request
{
    public class GetDocumentPageRequest : SearchBaseRequest
    {
        public string FileName { get; set; }

        public int PageNumber { get; set; }

        public string[] Terms { get; set; }

        public string[][] TermSequences { get; set; }

        public bool CaseSensitive { get; set; }
    }
}
