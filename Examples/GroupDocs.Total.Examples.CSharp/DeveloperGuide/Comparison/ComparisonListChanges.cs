namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Comparison;

using System;
using GroupDocs.Comparison.Options;
using GroupDocs.Comparison.Result;
using GroupDocs.Comparison;

public static class ComparisonListChanges
{
    public static void Run()
    {
        using (Comparer comparer = new Comparer("contract-v1.docx"))
        {
            comparer.Add("contract-v2.docx");
            comparer.Compare();

            ChangeInfo[] changes = comparer.GetChanges();

            Console.WriteLine("Changes found: " + changes.Length);

            foreach (ChangeInfo change in changes)
            {
                Console.WriteLine(change.Id + ". " + change.Type
                                  + " | source: '" + change.SourceText + "'"
                                  + " | target: '" + change.TargetText + "'");
            }
        }
    }
}
