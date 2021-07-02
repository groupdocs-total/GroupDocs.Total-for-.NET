using GroupDocs.Total.MVC.Products.Common.Util.Directory;
using GroupDocs.Total.MVC.Products.Signature.Config;

namespace GroupDocs.Total.MVC.Products.Signature.Util.Directory
{
    /// <summary>
    /// FilesDirectoryUtils
    /// </summary>
    public class FilesDirectoryUtils : IDirectoryUtils
    {
        private SignatureConfiguration signatureConfiguration;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="signatureConfiguration">SignatureConfiguration</param>
        public FilesDirectoryUtils(SignatureConfiguration signatureConfiguration)
        {
            this.signatureConfiguration = signatureConfiguration;            
        }

        /// <summary>
        /// Get path
        /// </summary>
        /// <returns>string</returns>
        public string GetPath()
        {
            return signatureConfiguration.GetFilesDirectory();
        }
    }
}