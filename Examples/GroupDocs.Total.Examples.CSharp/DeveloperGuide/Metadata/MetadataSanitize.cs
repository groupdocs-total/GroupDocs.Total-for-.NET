namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Metadata;

using System;
using GroupDocs.Metadata.Common;
using GroupDocs.Metadata;

public static class MetadataSanitize
{
    public static void Run()
    {
        using (Metadata metadata = new Metadata("contract.docx"))
        {
            int removed = metadata.Sanitize();
            Console.WriteLine("Properties removed: " + removed);

            metadata.Save("metadata-sanitize.docx");
        }
    }
}
