using GroupDocs.Total.WebForms.Products.Signature.Config;

namespace GroupDocs.Total.WebForms.Products.Signature.Entity.Directory
{
    /// <summary>
    /// CertificateDataDirectoryEntity
    /// </summary>
    public class CertificateDataDirectoryEntity : DataDirectoryEntity
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="signatureConfiguration">SignatureConfiguration</param>
        public CertificateDataDirectoryEntity(SignatureConfiguration signatureConfiguration)
            : base(signatureConfiguration, "/Certificates")
        {
        }
    }
}