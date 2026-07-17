namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Viewer;

using GroupDocs.Viewer.Options;
using GroupDocs.Viewer.Results;
using GroupDocs.Viewer;

public static class ViewerRenderToHtml
{
    public static void Run()
    {
        using (Viewer viewer = new Viewer("contract.docx"))
        {
            // {0} is replaced with the page number, so page 1 becomes page_1.html
            HtmlViewOptions viewOptions = HtmlViewOptions.ForEmbeddedResources(
                "viewer-render-to-html/page_{0}.html");

            viewer.View(viewOptions);
        }
    }
}
