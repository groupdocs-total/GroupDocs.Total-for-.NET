using GroupDocs.Search.Results;
using System;

namespace GroupDocs.Total.MVC.Products.Search.Domain.SingleIndex
{
    internal static class DocumentSerializer
    {
        public static string Serialize(FoundDocument document)
        {
            var bytes = document.Serialize();
            var base64String = Convert.ToBase64String(bytes);
            return base64String;
        }

        public static FoundDocument Deserialize(string base64String)
        {
            var bytes = Convert.FromBase64String(base64String);
            var document = FoundDocument.Deserialize(bytes);
            return document;
        }
    }
}
