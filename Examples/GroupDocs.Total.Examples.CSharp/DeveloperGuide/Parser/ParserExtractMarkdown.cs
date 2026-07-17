namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Parser;

using System.IO;
using System;
using GroupDocs.Parser.Options;
using GroupDocs.Parser;

public static class ParserExtractMarkdown
{
    public static void Run()
    {
        using (Parser parser = new Parser("contract.docx"))
        {
            using (TextReader reader = parser.GetFormattedText(
                new FormattedTextOptions(FormattedTextMode.Markdown)))
            {
                if (reader == null)
                {
                    Console.WriteLine("Formatted extraction is not supported for this format.");
                }
                else
                {
                    File.WriteAllText("parser-extract-markdown.md", reader.ReadToEnd());
                }
            }
        }
    }
}
