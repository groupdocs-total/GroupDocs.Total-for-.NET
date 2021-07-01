using GroupDocs.Annotation.Models;
using GroupDocs.Annotation.Models.AnnotationModels;
using GroupDocs.Annotation.Options;
using GroupDocs.Total.MVC.Products.Annotation.Entity.Web;
using System.Globalization;

namespace GroupDocs.Total.MVC.Products.Annotation.Annotator
{
    public class DistanceAnnotator : BaseAnnotator
    {
        private DistanceAnnotation distanceAnnotation;

        public DistanceAnnotator(AnnotationDataEntity annotationData, PageInfo pageInfo)
            : base(annotationData, pageInfo)
        {
            distanceAnnotation = new DistanceAnnotation
            {
                Box = GetBox()
            };
        }

        public override AnnotationBase AnnotateWord()
        {
            distanceAnnotation = InitAnnotationBase(distanceAnnotation) as DistanceAnnotation;
            return distanceAnnotation;
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

        protected new AnnotationBase InitAnnotationBase(AnnotationBase annotationBase)
        {
            distanceAnnotation = base.InitAnnotationBase(annotationBase) as DistanceAnnotation;
            // add replies
            string text = annotationData.text ?? "";
            CommentsEntity[]
            comments = annotationData.comments;
            if (comments != null && comments.Length != 0)
            {
                Reply reply = distanceAnnotation.Replies[0];
                if (reply != null)
                {
                    reply.Comment = string.Format("{0} {1}", text, reply.Comment);
                }
            }

            return distanceAnnotation;
        }

        protected override AnnotationType GetType()
        {
            return AnnotationType.Distance;
        }

        protected override Rectangle GetBox()
        {
            string svgPath = annotationData.svgPath;
            string startPoint = svgPath.Replace("[a-zA-Z]+", "").Split(' ')[0];
            string endPoint = svgPath.Replace("[a-zA-Z]+", "").Split(' ')[1];
            string[] start = startPoint.Split(',');
            float startX = float.Parse(start.Length > 0 ? start[0].Replace("M", "").Replace(",", ".") : "0", CultureInfo.InvariantCulture);
            float startY = float.Parse(start.Length > 0 ? start[1].Replace("M", "").Replace(",", ".") : "0", CultureInfo.InvariantCulture);
            string[] end = endPoint.Split(',');
            float endX = float.Parse(end.Length > 0 ? end[0].Replace("L", "").Replace(",", ".") : "0", CultureInfo.InvariantCulture) - startX;
            float endY = float.Parse(end.Length > 1 ? end[1].Replace("L", "").Replace(",", ".") : "0", CultureInfo.InvariantCulture) - startY;
            return new Rectangle(startX, startY, endX, endY);
        }
    }
}