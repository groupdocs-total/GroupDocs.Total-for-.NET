namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Conversion;

using System;
using GroupDocs.Conversion.Options.Convert;
using GroupDocs.Conversion.Options.Load;
using GroupDocs.Conversion;

public static class ConversionPdfToWord
{
    public static void Run()
    {
        using (Converter converter = new Converter("contract.pdf"))
        {
            WordProcessingConvertOptions convertOptions = new WordProcessingConvertOptions();

            converter.Convert("conversion-pdf-to-word.docx", convertOptions);
        }
    }
}
