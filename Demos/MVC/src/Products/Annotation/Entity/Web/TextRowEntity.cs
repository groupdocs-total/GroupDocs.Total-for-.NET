using System.Collections.Generic;

namespace GroupDocs.Total.MVC.Products.Annotation.Entity.Web
{
    public class TextRowEntity
    {
        public List<double> textCoordinates { get; set; }
        public double lineTop { get; set; }
        public double lineHeight { get; set; }
    }
}