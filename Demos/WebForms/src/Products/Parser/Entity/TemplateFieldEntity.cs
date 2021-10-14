using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroupDocs.Total.WebForms.Products.Parser.Entity
{
    public class TemplateFieldEntity
    {
        public string fieldType { get; set; }

        public string id { get; set; }

        public int pageNumber { get; set; }

        public string name { get; set; }

        public double x { get; set; }

        public double y { get; set; }

        public double width { get; set; }

        public double height { get; set; }

        public TemplateFieldTableSeparator[] columns { get; set; }
    }

    public class TemplateFieldTableSeparator
    {
        public string name { get; set; }

        public double value { get; set; }
    }
}