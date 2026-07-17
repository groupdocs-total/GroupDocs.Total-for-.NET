namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Conversion;

using System;
using GroupDocs.Conversion.Contracts;
using GroupDocs.Conversion.Options.Convert;
using GroupDocs.Conversion.Options.Load;
using GroupDocs.Conversion;

public static class ConversionDocumentInfo
{
    public static void Run()
    {
        using (Converter converter = new Converter("contract.pdf"))
        {
            IDocumentInfo info = converter.GetDocumentInfo();

            Console.WriteLine("Format: " + info.Format);
            Console.WriteLine("Pages: " + info.PagesCount);
            Console.WriteLine("Size: " + info.Size + " bytes");
        }
    }
}
