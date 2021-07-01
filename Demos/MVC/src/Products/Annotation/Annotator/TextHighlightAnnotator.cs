using GroupDocs.Annotation.Models;
using GroupDocs.Annotation.Models.AnnotationModels;
using GroupDocs.Annotation.Options;
using GroupDocs.Total.MVC.Products.Annotation.Entity.Web;
using System;

namespace GroupDocs.Total.MVC.Products.Annotation.Annotator
{
    public class TextHighlightAnnotator : AbstractTextAnnotator
    {
        private HighlightAnnotation highlightAnnotation;

        public TextHighlightAnnotator(AnnotationDataEntity annotationData, PageInfo pageInfo)
            : base(annotationData, pageInfo)
        {
            highlightAnnotation = new HighlightAnnotation
            {
                Points = GetPoints(annotationData, pageInfo)
            };
        }

        public override AnnotationBase AnnotateWord()
        {
            highlightAnnotation = InitAnnotationBase(highlightAnnotation) as HighlightAnnotation;
            return highlightAnnotation;
        }

        public override AnnotationBase AnnotatePdf()
        {
            return AnnotateWord();
        }

        public override AnnotationBase AnnotateCells()
        {
            return AnnotateWord();
        }

        public override AnnotationBase AnnotateSlides()
        {
            return AnnotateWord();
        }

        public override AnnotationBase AnnotateImage()
        {
            highlightAnnotation = InitAnnotationBase(highlightAnnotation) as HighlightAnnotation;
            highlightAnnotation.Points = GetPointsForImages(annotationData, pageInfo);
            return highlightAnnotation;
        }

        public override AnnotationBase AnnotateDiagram()
        {
            throw new NotSupportedException(string.Format(Message, annotationData.type));
        }

        protected override AnnotationType GetType()
        {
            return AnnotationType.TextHighlight;
        }
    }
}