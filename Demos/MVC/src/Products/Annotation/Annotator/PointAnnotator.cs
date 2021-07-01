using GroupDocs.Annotation.Models;
using GroupDocs.Annotation.Models.AnnotationModels;
using GroupDocs.Annotation.Options;
using GroupDocs.Total.MVC.Products.Annotation.Entity.Web;

namespace GroupDocs.Total.MVC.Products.Annotation.Annotator
{
    public class PointAnnotator : BaseAnnotator
    {
        private PointAnnotation pointAnnotation;

        public PointAnnotator(AnnotationDataEntity annotationData, PageInfo pageInfo)
            : base(annotationData, pageInfo)
        {
            pointAnnotation = new PointAnnotation
            {
                Box = GetBox()
            };
        }

        public override AnnotationBase AnnotateWord()
        {
            pointAnnotation = base.InitAnnotationBase(pointAnnotation) as PointAnnotation;
            return pointAnnotation;
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
            return AnnotateWord();
        }

        protected override AnnotationType GetType()
        {
            return AnnotationType.Point;
        }
    }
}