namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Parser;

using System.IO;
using System;
using GroupDocs.Parser.Options;
using GroupDocs.Parser;

public static class ParserExtractText
{
    public static void Run()
    {
        using (Parser parser = new Parser("contract.pdf"))
        {
            using (TextReader reader = parser.GetText())
            {
                if (reader == null)
                {
                    Console.WriteLine("Text extraction is not supported for this format.");
                }
                else
                {
                    Console.WriteLine(reader.ReadToEnd());
                }
            }
        }
    }
}
