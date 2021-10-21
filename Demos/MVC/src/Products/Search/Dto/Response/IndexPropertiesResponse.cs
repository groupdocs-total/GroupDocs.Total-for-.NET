namespace GroupDocs.Total.MVC.Products.Search.Dto.Response
{
    public class IndexPropertiesResponse
    {
        public string IndexVersion { get; set; }

        public string IndexType { get; set; }

        public bool UseStopWords { get; set; }

        public bool UseCharacterReplacements { get; set; }

        public bool AutoDetectEncoding { get; set; }

        public bool UseRawTextExtraction { get; set; }

        public string TextStorageCompression { get; set; }
    }
}
