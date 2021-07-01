using GroupDocs.Total.WebForms.Products.Signature.Config;

namespace GroupDocs.Total.WebForms.Products.Signature.Entity.Directory
{
    /// <summary>
    /// QrCodeDataDirectoryEntity
    /// </summary>
    public class QrCodeDataDirectoryEntity : DataDirectoryEntity
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="signatureConfiguration">SignatureConfiguration</param>
        public QrCodeDataDirectoryEntity(SignatureConfiguration signatureConfiguration)
            : base(signatureConfiguration, "/QrCodes")
        {
        }
    }
}