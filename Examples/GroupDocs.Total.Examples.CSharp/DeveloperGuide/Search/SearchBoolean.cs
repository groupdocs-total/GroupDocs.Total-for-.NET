namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Search;

using System.IO;
using System;
using GroupDocs.Search.Options;
using GroupDocs.Search.Results;
using GroupDocs.Search;
using Index = GroupDocs.Search.Index;

public static class SearchBoolean
{
    public static void Run()
    {
        string indexFolder = "search-boolean/index";
        string documentsFolder = "archive";

        Index index = new Index(indexFolder);
        index.Add(documentsFolder);

        SearchResult result = index.Search("throughput AND NOT forecast");

        Console.WriteLine("Documents found: " + result.DocumentCount);

        for (int i = 0; i < result.DocumentCount; i++)
        {
            FoundDocument document = result.GetFoundDocument(i);
            Console.WriteLine("  " + Path.GetFileName(document.DocumentInfo.FilePath));
        }
    }
}
