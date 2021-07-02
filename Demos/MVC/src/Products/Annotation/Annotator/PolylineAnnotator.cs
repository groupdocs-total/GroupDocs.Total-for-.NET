using GroupDocs.Annotation.Models;
using GroupDocs.Annotation.Models.AnnotationModels;
using GroupDocs.Annotation.Options;
using GroupDocs.Total.MVC.Products.Annotation.Entity.Web;

namespace GroupDocs.Total.MVC.Products.Annotation.Annotator
{
    public class PolylineAnnotator : BaseAnnotator
    {
        private PolylineAnnotation polylineAnnotation;

        public PolylineAnnotator(AnnotationDataEntity annotationData, PageInfo pageInfo)
            : base(annotationData, pageInfo)
        {
            this.polylineAnnotation = new PolylineAnnotation
            {
                Box = GetBox(),
                PenColor = 1201033,
                PenWidth = 2,
                SvgPath = annotationData.svgPath
            };
        }

        public override AnnotationBase AnnotateWord()
        {
            polylineAnnotation = InitAnnotationBase(polylineAnnotation) as PolylineAnnotation;
            return polylineAnnotation;
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
            polylineAnnotation = InitAnnotationBase(polylineAnnotation) as PolylineAnnotation;
            FillCreatorName(polylineAnnotation, annotationData);
            return polylineAnnotation;
        }

        /// <summary>
        /// Fill creator name field in annotation info
        /// </summary>
        /// <param name="polylineAnnotation">AnnotationBase</param>
        protected static void FillCreatorName(AnnotationBase polylineAnnotation, AnnotationDataEntity annotationData)
        {
            CommentsEntity[] comments = annotationData.comments;
            if (comments != null && comments.Length > 0 && comments[0] != null)
            {
                polylineAnnotation.User = new User
                {
                    Name = comments[0].userName
                };
            }
        }

        public override AnnotationBase AnnotateImage()
        {
            return AnnotateSlides();
        }

        public override AnnotationBase AnnotateDiagram()
        {
            return AnnotateSlides();
        }

        protected override AnnotationType GetType()
        {
            return AnnotationType.Polyline;
        }
    }
}