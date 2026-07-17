namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Viewer;

using GroupDocs.Viewer.Options;
using GroupDocs.Viewer.Results;
using GroupDocs.Viewer;

public static class ViewerRenderToPng
{
    public static void Run()
    {
        using (Viewer viewer = new Viewer("contract.docx"))
        {
            PngViewOptions viewOptions = new PngViewOptions(
                "viewer-render-to-png/page_{0}.png");

            // Render only the first two pages
            viewer.View(viewOptions, 1, 2);
        }
    }
}
