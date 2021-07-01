using GroupDocs.Total.MVC.Products.Common.Entity.Web;

namespace GroupDocs.Total.MVC.Products.Annotation.Entity.Request
{
    public class TextCoordinatesRequest : PostedDataEntity
    {
        public string guid { get; set; }
        public string password { get; set; }
        public int pageNumber { get; set; }
    }
}