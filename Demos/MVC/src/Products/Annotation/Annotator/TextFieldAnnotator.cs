using GroupDocs.Annotation.Models;
using GroupDocs.Annotation.Models.AnnotationModels;
using GroupDocs.Annotation.Options;
using GroupDocs.Total.MVC.Products.Annotation.Entity.Web;

namespace GroupDocs.Total.MVC.Products.Annotation.Annotator
{
    public class TextFieldAnnotator : BaseAnnotator
    {
        private TextFieldAnnotation textFieldAnnotation;

        public TextFieldAnnotator(AnnotationDataEntity annotationData, PageInfo pageInfo)
            : base(annotationData, pageInfo)
        {
            textFieldAnnotation = new TextFieldAnnotation
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
            textFieldAnnotation = InitAnnotationBase(textFieldAnnotation) as TextFieldAnnotation;
            return textFieldAnnotation;
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
            return AnnotationType.TextField;
        }
    }
}