
namespace GroupDocs.Total.MVC.Products.Annotation.Entity.Web
{
    public class AnnotationDataEntity
    {
        public int id {get; set;}
        public int pageNumber {get; set;}
        public int fontColor { get; set; }
        public float fontSize {get; set;}
        public float left {get; set;}
        public float top {get; set;}
        public float width { get; set;}
        public float height {get; set;}
        public string svgPath {get; set;}
        public string type {get; set;}
        public string documentType {get; set;}
        public string text {get; set;}
        public string font {get; set;}        
        public bool imported { get; set; }
        public CommentsEntity[] comments {get; set;}
    }
}