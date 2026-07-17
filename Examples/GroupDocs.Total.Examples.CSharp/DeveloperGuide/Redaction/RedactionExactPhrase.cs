namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Redaction;

using System.IO;
using System;
using GroupDocs.Redaction.Options;
using GroupDocs.Redaction.Redactions;
using GroupDocs.Redaction;

public static class RedactionExactPhrase
{
    public static void Run()
    {
        using (Redactor redactor = new Redactor("contract-v1.docx"))
        {
            RedactorChangeLog result = redactor.Apply(
                new ExactPhraseRedaction("Project Northlight", new ReplacementOptions("[REDACTED]")));

            Console.WriteLine("Status: " + result.Status);

            if (result.Status != RedactionStatus.Failed)
            {
                // Save() has no path overload: SaveOptions writes back over the source.
                // Write to a stream to keep the original intact.
                using (FileStream output = File.Create("redaction-exact-phrase.docx"))
                {
                    redactor.Save(output, new RasterizationOptions { Enabled = false });
                }
            }
        }
    }
}
