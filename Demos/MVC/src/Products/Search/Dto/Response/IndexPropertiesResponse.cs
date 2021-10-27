namespace GroupDocs.Total.MVC.Products.Search.Dto.Response
{
    public class IndexPropertiesResponse
    {
        public string indexVersion { get; set; }

        public string indexType { get; set; }

        public bool useStopWords { get; set; }

        public bool useCharacterReplacements { get; set; }

        public bool autoDetectEncoding { get; set; }

        public bool useRawTextExtraction { get; set; }

        public string textStorageCompression { get; set; }
    }
}
