namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Merger;

using System.IO;
using System;
using GroupDocs.Merger.Domain.Options;
using GroupDocs.Merger.Domain.Result;
using GroupDocs.Merger;

public static class MergerDocumentInfo
{
    public static void Run()
    {
        using (Merger merger = new Merger("contract.pdf"))
        {
            IDocumentInfo info = merger.GetDocumentInfo();

            Console.WriteLine("File type: " + info.Type.FileFormat);
            Console.WriteLine("Pages: " + info.PageCount);
            Console.WriteLine("Size: " + info.Size + " bytes");
        }
    }
}
