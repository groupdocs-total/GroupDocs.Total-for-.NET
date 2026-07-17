namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Editor;

using System.IO;
using System;
using GroupDocs.Editor.Formats;
using GroupDocs.Editor.Options;
using GroupDocs.Editor;

public static class EditorDocumentToHtml
{
    public static void Run()
    {
        using (Editor editor = new Editor("contract.docx"))
        {
            // Stage 1: parse the document into an editable form
            using (EditableDocument document = editor.Edit(new WordProcessingEditOptions()))
            {
                // Stage 2: the HTML your web editor would load
                string bodyHtml = document.GetBodyContent();

                File.WriteAllText("editor-document-to-html.html", document.GetEmbeddedHtml());

                Console.WriteLine("Body HTML length: " + bodyHtml.Length + " characters");
            }
        }
    }
}
