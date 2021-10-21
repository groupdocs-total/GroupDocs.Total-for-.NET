namespace GroupDocs.Total.MVC.Products.Search.Dto.Request
{
    public class HighlightTermsRequest : SearchBaseRequest
    {
        public string Html { get; set; }

        public string[] Terms { get; set; }

        public bool CaseSensitive { get; set; }
    }
}
