namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Conversion;

using System;
using GroupDocs.Conversion.Options.Convert;
using GroupDocs.Conversion.Options.Load;
using GroupDocs.Conversion;

public static class ConversionSpecificPages
{
    public static void Run()
    {
        using (Converter converter = new Converter("contract.docx"))
        {
            PdfConvertOptions convertOptions = new PdfConvertOptions
            {
                PageNumber = 1,
                PagesCount = 2
            };

            converter.Convert("conversion-specific-pages.pdf", convertOptions);
        }
    }
}
