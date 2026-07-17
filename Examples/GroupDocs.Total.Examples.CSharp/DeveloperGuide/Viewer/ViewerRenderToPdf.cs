namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Viewer;

using GroupDocs.Viewer.Options;
using GroupDocs.Viewer.Results;
using GroupDocs.Viewer;

public static class ViewerRenderToPdf
{
    public static void Run()
    {
        using (Viewer viewer = new Viewer("contract.docx"))
        {
            PdfViewOptions viewOptions = new PdfViewOptions("viewer-render-to-pdf.pdf");

            viewer.View(viewOptions);
        }
    }
}
