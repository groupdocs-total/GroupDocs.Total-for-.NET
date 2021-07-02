using GroupDocs.Annotation.Models;
using GroupDocs.Annotation.Models.AnnotationModels;
using GroupDocs.Annotation.Options;
using GroupDocs.Total.MVC.Products.Annotation.Entity.Web;
using System;

namespace GroupDocs.Total.MVC.Products.Annotation.Annotator
{
    public class TextUnderlineAnnotator : AbstractTextAnnotator
    {
        private UnderlineAnnotation underlineAnnotation;

        public TextUnderlineAnnotator(AnnotationDataEntity annotationData, PageInfo pageInfo)
            : base(annotationData, pageInfo)
        {
            underlineAnnotation = new UnderlineAnnotation
            {
                Points = GetPoints(annotationData, pageInfo)
            };
        }

        public override AnnotationBase AnnotateWord()
        {
            underlineAnnotation = InitAnnotationBase(underlineAnnotation) as UnderlineAnnotation;
            underlineAnnotation.FontColor = 1201033;
            return underlineAnnotation;
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
            underlineAnnotation = InitAnnotationBase(underlineAnnotation) as UnderlineAnnotation;
            underlineAnnotation.FontColor = 0;
            return underlineAnnotation;
        }

        public override AnnotationBase AnnotateImage()
        {
            underlineAnnotation = InitAnnotationBase(underlineAnnotation) as UnderlineAnnotation;
            underlineAnnotation.FontColor = 1201033;
            underlineAnnotation.Points = GetPointsForImages(annotationData, pageInfo);
            return underlineAnnotation;
        }

        public override AnnotationBase AnnotateDiagram()
        {
            throw new NotSupportedException(string.Format(Message, annotationData.type));
        }

        protected override AnnotationType GetType()
        {
            return AnnotationType.TextUnderline;
        }
    }
}