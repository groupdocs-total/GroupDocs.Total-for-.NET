namespace GroupDocs.Total.MVC.Products.Search.Dto.Request
{
    public class AddToIndexRequest : SearchBaseRequest
    {
        public PostedDataEntity[] Files { get; set; }

        public bool RecognizeTextInImages { get; set; }
    }
}
