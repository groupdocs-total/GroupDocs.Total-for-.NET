using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GroupDocs.Total.MVC.Products.Common.Entity.Web;

namespace GroupDocs.Total.MVC.Products.Parser.Entity
{
    public class FileDataEntity : PostedDataEntity
    {
        public TemplateFieldValueEntity[] data { get; set; }
    }
}