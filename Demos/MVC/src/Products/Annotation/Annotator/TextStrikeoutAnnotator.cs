using GroupDocs.Annotation.Models;
using GroupDocs.Annotation.Models.AnnotationModels;
using GroupDocs.Annotation.Options;
using GroupDocs.Total.MVC.Products.Annotation.Entity.Web;
using System;

namespace GroupDocs.Total.MVC.Products.Annotation.Annotator
{
    public class TexStrikeoutAnnotator : AbstractTextAnnotator
    {
        private StrikeoutAnnotation strikeoutAnnotation;

        public TexStrikeoutAnnotator(AnnotationDataEntity annotationData, PageInfo pageInfo)
            : base(annotationData, pageInfo)
        {
            strikeoutAnnotation = new StrikeoutAnnotation
            {
                Points = GetPoints(annotationData, pageInfo)
            };
        }

        public override AnnotationBase AnnotateWord()
        {
            strikeoutAnnotation = InitAnnotationBase(strikeoutAnnotation) as StrikeoutAnnotation;
            return strikeoutAnnotation;
        }

        public override AnnotationBase AnnotatePdf()
        {
            strikeoutAnnotation = InitAnnotationBase(strikeoutAnnotation) as StrikeoutAnnotation;
            this.strikeoutAnnotation.FontColor = 0;
            return strikeoutAnnotation;
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
            strikeoutAnnotation = InitAnnotationBase(strikeoutAnnotation) as StrikeoutAnnotation;
            strikeoutAnnotation.Points = GetPointsForImages(annotationData, pageInfo);
            return strikeoutAnnotation;
        }

        public override AnnotationBase AnnotateDiagram()
        {
            throw new NotSupportedException(string.Format(Message, annotationData.type));
        }

        protected override AnnotationType GetType()
        {
            return AnnotationType.TextStrikeout;
        }
    }
}