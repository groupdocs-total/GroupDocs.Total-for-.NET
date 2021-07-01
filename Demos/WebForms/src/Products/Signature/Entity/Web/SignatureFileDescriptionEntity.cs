using GroupDocs.Total.WebForms.Products.Common.Entity.Web;

namespace GroupDocs.Total.WebForms.Products.Signature.Entity.Web
{
    /// <summary>
    /// SignatureFileDescriptionEntity
    /// </summary>
    public class SignatureFileDescriptionEntity : FileDescriptionEntity
    {
        public string image { get; set; }
        public string text { get; set; }
        public string fontColor { get; set; }
    }
}