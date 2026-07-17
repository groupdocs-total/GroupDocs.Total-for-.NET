namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Parser;

using System.IO;
using System;
using GroupDocs.Parser.Options;
using GroupDocs.Parser;

public static class ParserDocumentInfo
{
    public static void Run()
    {
        using (Parser parser = new Parser("contract.pdf"))
        {
            IDocumentInfo info = parser.GetDocumentInfo();

            Console.WriteLine("File type: " + info.FileType);
            Console.WriteLine("Pages: " + info.PageCount);
            Console.WriteLine("Size: " + info.Size + " bytes");
        }
    }
}
