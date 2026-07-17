namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Editor;

using System;
using GroupDocs.Editor.Formats;
using GroupDocs.Editor.Options;
using GroupDocs.Editor;

public static class EditorHtmlToDocument
{
    public static void Run()
    {
        using (Editor editor = new Editor("contract.docx"))
        {
            using (EditableDocument original = editor.Edit(new WordProcessingEditOptions()))
            {
                // Stand in for the user's edit in a web editor
                string editedHtml = original.GetBodyContent()
                    .Replace("Consulting Services Agreement", "Consulting Services Agreement (revised)");

                using (EditableDocument edited = EditableDocument.FromMarkup(editedHtml, null))
                {
                    editor.Save(edited, "editor-html-to-document.docx",
                        new WordProcessingSaveOptions(WordProcessingFormats.Docx));
                }
            }
        }

        Console.WriteLine("Saved editor-html-to-document.docx");
    }
}
