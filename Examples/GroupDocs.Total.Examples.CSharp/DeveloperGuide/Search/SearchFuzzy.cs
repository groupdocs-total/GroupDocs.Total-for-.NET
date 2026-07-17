namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Search;

using System;
using GroupDocs.Search.Options;
using GroupDocs.Search.Results;
using GroupDocs.Search;
using Index = GroupDocs.Search.Index;

public static class SearchFuzzy
{
    public static void Run()
    {
        string indexFolder = "search-fuzzy/index";
        string documentsFolder = "archive";

        Index index = new Index(indexFolder);
        index.Add(documentsFolder);

        SearchOptions options = new SearchOptions();
        options.FuzzySearch.Enabled = true;
        options.FuzzySearch.FuzzyAlgorithm = new TableDiscreteFunction(3);

        // Matches "throughput" even when spelled "througput" or "throughtput"
        SearchResult result = index.Search("throughput", options);

        Console.WriteLine("Documents found: " + result.DocumentCount);
        Console.WriteLine("Total occurrences: " + result.OccurrenceCount);
    }
}
