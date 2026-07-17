namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Annotation;

using GroupDocs.Annotation.Models.AnnotationModels;
using GroupDocs.Annotation.Models;
using GroupDocs.Annotation.Options;
using GroupDocs.Annotation;

public static class AnnotationAddArea
{
    public static void Run()
    {
        using (Annotator annotator = new Annotator("contract.pdf"))
        {
            AreaAnnotation area = new AreaAnnotation
            {
                Box = new Rectangle(100, 100, 200, 100),
                BackgroundColor = 65535,
                Message = "Please confirm these figures",
                PageNumber = 0,
                Opacity = 0.7
            };

            annotator.Add(area);
            annotator.Save("annotation-add-area.pdf");
        }
    }
}
