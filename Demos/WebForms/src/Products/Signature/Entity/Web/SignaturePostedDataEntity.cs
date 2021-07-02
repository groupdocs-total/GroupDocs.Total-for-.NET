using GroupDocs.Total.WebForms.Products.Common.Entity.Web;
using GroupDocs.Total.WebForms.Products.Signature.Entity.Xml;
using System.Collections.Generic;

namespace GroupDocs.Total.WebForms.Products.Signature.Entity.Web
{
    /// <summary>
    /// SignaturePostedDataEntity
    /// </summary>
    public class SignaturePostedDataEntity : PostedDataEntity
    {
        public string signatureType { get; set; }
        public SignatureDataEntity[] signaturesData { get; set; }
        public string image { get; set; }
        public StampXmlEntity[] stampData { get; set; }
        public string documentType { get; set; }
    }
}