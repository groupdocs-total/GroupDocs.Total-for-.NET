using GroupDocs.Annotation.Models;
using GroupDocs.Annotation.Models.AnnotationModels;
using GroupDocs.Annotation.Options;
using GroupDocs.Total.MVC.Products.Annotation.Entity.Web;
using System.Globalization;

namespace GroupDocs.Total.MVC.Products.Annotation.Annotator
{
    public class ArrowAnnotator : BaseAnnotator
    {
        private bool withGuid = false;
        private ArrowAnnotation arrowAnnotation;

        public ArrowAnnotator(AnnotationDataEntity annotationData, PageInfo pageInfo)
            : base(annotationData, pageInfo)
        {
            this.arrowAnnotation = new ArrowAnnotation
            {
                Box = GetBox()
            };
        }

        public override AnnotationBase AnnotateWord()
        {
            withGuid = false;
            arrowAnnotation = InitAnnotationBase(arrowAnnotation) as ArrowAnnotation;
            return arrowAnnotation;
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
            withGuid = true;
            arrowAnnotation = InitAnnotationBase(arrowAnnotation) as ArrowAnnotation;
            return arrowAnnotation;
        }

        public override AnnotationBase AnnotateImage()
        {
            return AnnotateWord();
        }

        public override AnnotationBase AnnotateDiagram()
        {
            return AnnotateWord();
        }

        protected override Reply GetAnnotationReplyInfo(CommentsEntity comment)
        {
            Reply annotationReplyInfo = base.GetAnnotationReplyInfo(comment);
            if (withGuid)
            {
                annotationReplyInfo.ParentReply = new Reply { Id = annotationData.id };
            }
            return annotationReplyInfo;
        }

        protected override AnnotationType GetType()
        {
            return AnnotationType.Arrow;
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