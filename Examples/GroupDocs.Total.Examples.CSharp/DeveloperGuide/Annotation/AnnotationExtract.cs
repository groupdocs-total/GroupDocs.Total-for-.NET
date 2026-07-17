namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Annotation;

using System.Collections.Generic;
using System;
using GroupDocs.Annotation.Models.AnnotationModels;
using GroupDocs.Annotation.Models;
using GroupDocs.Annotation.Options;
using GroupDocs.Annotation;

public static class AnnotationExtract
{
    public static void Run()
    {
        // Annotate first, so there is something to read back
        using (Annotator annotator = new Annotator("contract.pdf"))
        {
            annotator.Add(new AreaAnnotation
            {
                Box = new Rectangle(100, 100, 200, 100),
                Message = "Please confirm these figures",
                PageNumber = 0
            });
            annotator.Save("annotation-extract.pdf");
        }

        using (Annotator annotator = new Annotator("annotation-extract.pdf"))
        {
            List<AnnotationBase> annotations = annotator.Get();

            Console.WriteLine("Annotations found: " + annotations.Count);

            foreach (AnnotationBase annotation in annotations)
            {
                Console.WriteLine(annotation.GetType().Name
                                  + " on page " + annotation.PageNumber
                                  + ": " + annotation.Message);
            }
        }
    }
}
