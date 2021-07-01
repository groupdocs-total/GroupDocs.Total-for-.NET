using GroupDocs.Total.MVC.Products.Signature.Config;

namespace GroupDocs.Total.MVC.Products.Signature.Entity.Directory
{
    /// <summary>
    /// StampDataDirectoryEntity
    /// </summary>
    public class StampDataDirectoryEntity : DataDirectoryEntity
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="signatureConfiguration">SignatureConfiguration</param>
        public StampDataDirectoryEntity(SignatureConfiguration signatureConfiguration)
            : base(signatureConfiguration, "/Stamps")
        {
        }
    }
}