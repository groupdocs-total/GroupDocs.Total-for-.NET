using GroupDocs.Total.MVC.Products.Signature.Config;

namespace GroupDocs.Total.MVC.Products.Signature.Entity.Directory
{
    /// <summary>
    /// BarcodeDataDirectoryEntity
    /// </summary>
    public class BarcodeDataDirectoryEntity : DataDirectoryEntity
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="signatureConfiguration">SignatureConfiguration</param>
        public BarcodeDataDirectoryEntity(SignatureConfiguration signatureConfiguration)
            : base(signatureConfiguration, "/BarCodes")
        {
        }
    }
}