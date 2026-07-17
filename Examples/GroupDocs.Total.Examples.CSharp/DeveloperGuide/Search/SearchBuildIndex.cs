namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Search;

using System.IO;
using System;
using GroupDocs.Search.Options;
using GroupDocs.Search.Results;
using GroupDocs.Search;
using Index = GroupDocs.Search.Index;

public static class SearchBuildIndex
{
    public static void Run()
    {
        // The index is a folder the library owns; keep it out of your documents folder
        string indexFolder = "search-build-index/index";
        string documentsFolder = "archive";

        Index index = new Index(indexFolder);

        // Index every supported document in the folder
        index.Add(documentsFolder);

        SearchResult result = index.Search("throughput");

        Console.WriteLine("Documents found: " + result.DocumentCount);
        Console.WriteLine("Total occurrences: " + result.OccurrenceCount);

        for (int i = 0; i < result.DocumentCount; i++)
        {
            FoundDocument document = result.GetFoundDocument(i);
            // FilePath is absolute; the file name is the useful part here
            Console.WriteLine("  " + Path.GetFileName(document.DocumentInfo.FilePath)
                              + " (" + document.OccurrenceCount + " occurrences)");
        }
    }
}
