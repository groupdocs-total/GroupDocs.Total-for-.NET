namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Merger;

using System.IO;
using GroupDocs.Merger.Domain.Options;
using GroupDocs.Merger;

public static class MergerJoinDocuments
{
    public static void Run()
    {
        using (Merger merger = new Merger("contract.docx"))
        {
            merger.Join("statement-of-work.docx");

            merger.Save(Path.GetFullPath("merger-join-documents.docx"));
        }
    }
}
