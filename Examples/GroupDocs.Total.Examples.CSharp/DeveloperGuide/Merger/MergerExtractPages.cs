namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Merger;

using System.IO;
using GroupDocs.Merger.Domain.Options;
using GroupDocs.Merger;

public static class MergerExtractPages
{
    public static void Run()
    {
        using (Merger merger = new Merger("contract.pdf"))
        {
            // Keep pages 1 to 3
            ExtractOptions extractOptions = new ExtractOptions(1, 3);

            merger.ExtractPages(extractOptions);
            merger.Save(Path.GetFullPath("merger-extract-pages.pdf"));
        }
    }
}
