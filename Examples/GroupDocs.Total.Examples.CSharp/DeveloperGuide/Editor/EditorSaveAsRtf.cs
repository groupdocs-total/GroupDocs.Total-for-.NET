namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Editor;

using System;
using GroupDocs.Editor.Formats;
using GroupDocs.Editor.Options;
using GroupDocs.Editor;

public static class EditorSaveAsRtf
{
    public static void Run()
    {
        using (Editor editor = new Editor("contract.docx"))
        {
            using (EditableDocument document = editor.Edit(new WordProcessingEditOptions()))
            {
                editor.Save(document, "editor-save-as-rtf.rtf",
                    new WordProcessingSaveOptions(WordProcessingFormats.Rtf));
            }
        }

        Console.WriteLine("Saved editor-save-as-rtf.rtf");
    }
}
