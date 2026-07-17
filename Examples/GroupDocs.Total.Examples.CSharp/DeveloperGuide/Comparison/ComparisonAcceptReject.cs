namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Comparison;

using System;
using GroupDocs.Comparison.Options;
using GroupDocs.Comparison.Result;
using GroupDocs.Comparison;

public static class ComparisonAcceptReject
{
    public static void Run()
    {
        using (Comparer comparer = new Comparer("contract-v1.docx"))
        {
            comparer.Add("contract-v2.docx");
            comparer.Compare();

            ChangeInfo[] changes = comparer.GetChanges();

            // Accept everything except the first change, which we reject
            foreach (ChangeInfo change in changes)
            {
                change.ComparisonAction = ComparisonAction.Accept;
            }
            if (changes.Length > 0)
            {
                changes[0].ComparisonAction = ComparisonAction.Reject;
                Console.WriteLine("Rejected: '" + changes[0].TargetText + "'");
            }

            comparer.ApplyChanges("comparison-accept-reject.docx", new ApplyChangeOptions
            {
                Changes = changes
            });
        }
    }
}
