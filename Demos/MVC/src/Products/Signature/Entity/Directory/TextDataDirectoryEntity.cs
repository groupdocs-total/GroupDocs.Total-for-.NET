using GroupDocs.Total.MVC.Products.Signature.Config;

namespace GroupDocs.Total.MVC.Products.Signature.Entity.Directory
{
    /// <summary>
    /// TextDataDirectoryEntity
    /// </summary>
    public class TextDataDirectoryEntity : DataDirectoryEntity
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="signatureConfiguration">SignatureConfiguration</param>
        public TextDataDirectoryEntity(SignatureConfiguration signatureConfiguration)
                : base(signatureConfiguration, "/Text")
        {
        }
    }
}