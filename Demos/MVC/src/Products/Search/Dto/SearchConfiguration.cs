namespace GroupDocs.Total.MVC.Products.Search.Dto
{
    public class SearchConfiguration : CommonConfiguration
    {
        public string filesDirectory { get; set; } = string.Empty;

        public string indexDirectory { get; set; } = string.Empty;

        public string indexedFilesDirectory { get; set; } = string.Empty;
    }
}
