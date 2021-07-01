using GroupDocs.Annotation.Models;
using GroupDocs.Annotation.Models.AnnotationModels;
using GroupDocs.Annotation.Options;
using GroupDocs.Total.MVC.Products.Annotation.Entity.Web;
using System;

namespace GroupDocs.Total.MVC.Products.Annotation.Annotator
{
    public class TextRedactionAnnotator : TextHighlightAnnotator
    {
        private TextRedactionAnnotation textRedactionAnnotation;

        public TextRedactionAnnotator(AnnotationDataEntity annotationData, PageInfo pageInfo)
            : base(annotationData, pageInfo)
        {
            textRedactionAnnotation = new TextRedactionAnnotation
            {
                Points = GetPoints(annotationData, pageInfo)
            };
        }

        public override AnnotationBase AnnotateCells()
        {
            return AnnotatePdf();
        }

        public override AnnotationBase AnnotateSlides()
        {
            return AnnotatePdf();
        }

        public override AnnotationBase AnnotateImage()
        {
            textRedactionAnnotation = InitAnnotationBase(textRedactionAnnotation) as TextRedactionAnnotation;
            textRedactionAnnotation.Points = GetPointsForImages(annotationData, pageInfo);
            return textRedactionAnnotation;
        }

        public override AnnotationBase AnnotateDiagram()
        {
            throw new NotSupportedException(string.Format(Message, annotationData.type));
        }

        public override AnnotationBase AnnotatePdf()
        {
            textRedactionAnnotation = InitAnnotationBase(textRedactionAnnotation) as TextRedactionAnnotation;
            return textRedactionAnnotation;
        }

        protected override AnnotationType GetType()
        {
            return AnnotationType.TextRedaction;
        }
    }
}