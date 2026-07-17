namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Redaction;

using System.IO;
using System;
using GroupDocs.Redaction.Options;
using GroupDocs.Redaction.Redactions;
using GroupDocs.Redaction;

public static class RedactionRegex
{
    public static void Run()
    {
        using (Redactor redactor = new Redactor("contract-v1.docx"))
        {
            // Any currency amount such as "120,000 USD"
            RedactorChangeLog result = redactor.Apply(
                new RegexRedaction(@"\d{1,3}(,\d{3})*\s?USD", new ReplacementOptions("[AMOUNT]")));

            Console.WriteLine("Status: " + result.Status);

            if (result.Status != RedactionStatus.Failed)
            {
                // Save() has no path overload: SaveOptions writes back over the source.
                // Write to a stream to keep the original intact.
                using (FileStream output = File.Create("redaction-regex.docx"))
                {
                    redactor.Save(output, new RasterizationOptions { Enabled = false });
                }
            }
        }
    }
}
