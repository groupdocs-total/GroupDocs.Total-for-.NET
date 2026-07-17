namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Annotation;

using System.Collections.Generic;
using GroupDocs.Annotation.Models.AnnotationModels;
using GroupDocs.Annotation.Models;
using GroupDocs.Annotation.Options;
using GroupDocs.Annotation;

public static class AnnotationHighlight
{
    public static void Run()
    {
        using (Annotator annotator = new Annotator("contract.pdf"))
        {
            HighlightAnnotation highlight = new HighlightAnnotation
            {
                // Corner points of the region to highlight
                Points = new List<Point>
                {
                    new Point(80, 730),
                    new Point(240, 730),
                    new Point(80, 710),
                    new Point(240, 710)
                },
                BackgroundColor = 65535,
                Message = "Check this clause against the contract",
                PageNumber = 0,
                Opacity = 0.5
            };

            annotator.Add(highlight);
            annotator.Save("annotation-highlight.pdf");
        }
    }
}
