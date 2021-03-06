using GroupDocs.Total.WebForms.Products.Common.Entity.Web;
using System.Collections.Generic;
using System.IO;

namespace GroupDocs.Total.WebForms.Products.Comparison.Model.Request
{
    public class CompareRequest
    {
        /// <summary>
        /// Contains list of the documents paths
        /// </summary>
        public List<CompareFileDataRequest> guids { get; set; }
    }
}