namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Viewer;

using System.IO;
using System;
using GroupDocs.Viewer.Options;
using GroupDocs.Viewer.Results;
using GroupDocs.Viewer;

public static class ViewerDocumentInfo
{
    public static void Run()
    {
        using (Viewer viewer = new Viewer("contract.docx"))
        {
            ViewInfoOptions viewInfoOptions = ViewInfoOptions.ForHtmlView();
            ViewInfo viewInfo = viewer.GetViewInfo(viewInfoOptions);

            Console.WriteLine("File type: " + viewInfo.FileType);
            Console.WriteLine("Pages: " + viewInfo.Pages.Count);

            foreach (Page page in viewInfo.Pages)
            {
                Console.WriteLine("Page " + page.Number + ": " + page.Width + "x" + page.Height);
            }
        }
    }
}
