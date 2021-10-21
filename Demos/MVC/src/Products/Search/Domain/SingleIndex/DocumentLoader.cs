using GroupDocs.Search.Common;
using System;
using System.IO;

namespace GroupDocs.Total.MVC.Products.Search.Domain.SingleIndex
{
    internal class DocumentLoader : IDocumentLoader
    {
        private readonly string[] _attributes;
        private readonly DocumentDescriptor _descriptor;

        public DocumentLoader(string[] attributes, DocumentDescriptor descriptor)
        {
            _attributes = attributes;
            _descriptor = descriptor;
        }

        public string DocumentKey => _descriptor.DocumentKey;

        public void CloseDocument()
        {
        }

        public Document LoadDocument()
        {
            var extension = Path.GetExtension(_descriptor.FilePath);
            var buffer = File.ReadAllBytes(_descriptor.FilePath);
            var stream = new MemoryStream(buffer);
            var document = Document.CreateFromStream(_descriptor.DocumentKey, DateTime.Now, extension, stream);
            document.Attributes = _attributes;
            return document;
        }
    }
}
