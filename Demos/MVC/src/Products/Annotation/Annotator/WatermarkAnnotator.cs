using GroupDocs.Annotation.Models;
using GroupDocs.Annotation.Models.AnnotationModels;
using GroupDocs.Annotation.Options;
using GroupDocs.Total.MVC.Products.Annotation.Entity.Web;
using System;

namespace GroupDocs.Total.MVC.Products.Annotation.Annotator
{
    public class WatermarkAnnotator : BaseAnnotator
    {
        private WatermarkAnnotation watermarkAnnotation;

        public WatermarkAnnotator(AnnotationDataEntity annotationData, PageInfo pageInfo)
            : base(annotationData, pageInfo)
        {
            watermarkAnnotation = new WatermarkAnnotation
            {
                Box = GetBox(),
                FontFamily = !string.IsNullOrEmpty(annotationData.font) ? annotationData.font : "Arial",
                FontColor = annotationData.fontColor,
                FontSize = annotationData.fontSize == 0 ? 12 : annotationData.fontSize,
                Text = annotationData.text
            };
        }

        public override AnnotationBase AnnotateWord()
        {
            watermarkAnnotation = InitAnnotationBase(watermarkAnnotation) as WatermarkAnnotation;
            return watermarkAnnotation;
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
            return AnnotateWord();
        }

        public override AnnotationBase AnnotateDiagram()
        {
            throw new NotSupportedException(string.Format(Message, annotationData.type));
        }

        protected override AnnotationType GetType()
        {
            return AnnotationType.Watermark;
        }
    }
}