namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Metadata;

using System.Collections.Generic;
using System;
using GroupDocs.Metadata.Common;
using GroupDocs.Metadata.Tagging;
using GroupDocs.Metadata;

public static class MetadataFindProperties
{
    public static void Run()
    {
        using (Metadata metadata = new Metadata("contract.docx"))
        {
            // Every property tagged as identifying a person
            IEnumerable<MetadataProperty> properties = metadata.FindProperties(
                p => p.Tags.Contains(Tags.Person.Creator));

            foreach (MetadataProperty property in properties)
            {
                Console.WriteLine(property.Name + " = " + property.Value);
            }
        }
    }
}
