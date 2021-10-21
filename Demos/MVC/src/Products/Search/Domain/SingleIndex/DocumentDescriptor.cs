namespace GroupDocs.Total.MVC.Products.Search.Domain.SingleIndex
{
    internal class DocumentDescriptor
    {
        private readonly string _userId;
        private readonly string _fileName;
        private readonly string _documentKey;
        private readonly string _filePath;

        public DocumentDescriptor(string userId, string fileName, string filePath)
        {
            _userId = userId;
            _fileName = fileName;
            _documentKey = userId + Settings.DocumentKeySeparator + fileName;
            _filePath = filePath;
        }

        public string UserId => _userId;

        public string FileName => _fileName;

        public string DocumentKey => _documentKey;

        public string FilePath => _filePath;

        public override string ToString()
        {
            return _documentKey;
        }
    }
}
