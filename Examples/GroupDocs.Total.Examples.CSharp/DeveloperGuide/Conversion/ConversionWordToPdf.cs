namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Conversion;

using System;
using GroupDocs.Conversion.Options.Convert;
using GroupDocs.Conversion.Options.Load;
using GroupDocs.Conversion;

public static class ConversionWordToPdf
{
    public static void Run()
    {
        using (Converter converter = new Converter("contract.docx"))
        {
            PdfConvertOptions convertOptions = new PdfConvertOptions();

            converter.Convert("conversion-word-to-pdf.pdf", convertOptions);
        }
    }
}
