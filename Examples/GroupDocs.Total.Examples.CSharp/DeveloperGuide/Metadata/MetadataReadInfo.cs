namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Metadata;

using System.IO;
using System;
using GroupDocs.Metadata.Common;
using GroupDocs.Metadata;

public static class MetadataReadInfo
{
    public static void Run()
    {
        using (Metadata metadata = new Metadata("contract.docx"))
        {
            IDocumentInfo info = metadata.GetDocumentInfo();

            Console.WriteLine("File format: " + info.FileType.FileFormat);
            Console.WriteLine("Extension: " + info.FileType.Extension);
            Console.WriteLine("MIME type: " + info.FileType.MimeType);
            Console.WriteLine("Pages: " + info.PageCount);
            Console.WriteLine("Size: " + info.Size + " bytes");
            Console.WriteLine("Encrypted: " + info.IsEncrypted);
        }
    }
}
