using GroupDocs.Total.MVC.Products.Common.Entity.Web;
using System.Collections.Generic;

namespace GroupDocs.Total.MVC.Products.Annotation.Entity.Web
{
    public class AnnotatedDocumentEntity : PageDescriptionEntity
    {
        public string guid;
        public List<PageDataDescriptionEntity> pages = new List<PageDataDescriptionEntity>();
        public string[] supportedAnnotations;
    }
}