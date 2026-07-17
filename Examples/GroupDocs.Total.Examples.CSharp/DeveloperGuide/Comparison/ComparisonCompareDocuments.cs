namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Comparison;

using GroupDocs.Comparison.Options;
using GroupDocs.Comparison;

public static class ComparisonCompareDocuments
{
    public static void Run()
    {
        using (Comparer comparer = new Comparer("contract-v1.docx"))
        {
            comparer.Add("contract-v2.docx");

            comparer.Compare("comparison-compare-documents.docx");
        }
    }
}
