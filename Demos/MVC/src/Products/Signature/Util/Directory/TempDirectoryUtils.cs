using GroupDocs.Total.MVC.Products.Common.Util.Directory;
using GroupDocs.Total.MVC.Products.Signature.Config;
using System;

namespace GroupDocs.Total.MVC.Products.Signature.Util.Directory
{
    /// <summary>
    /// OutputDirectoryUtils
    /// </summary>
    public class TempDirectoryUtils : IDirectoryUtils
    {
        internal readonly string OUTPUT_FOLDER = "/SignedTemp";
        private SignatureConfiguration signatureConfiguration;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="signatureConfiguration">SignatureConfiguration</param>
        public TempDirectoryUtils(SignatureConfiguration signatureConfiguration)
        {
            this.signatureConfiguration = signatureConfiguration;

            // create output directories
            if (string.IsNullOrEmpty(signatureConfiguration.GetTempFilesDirectory()))
            {
                signatureConfiguration.SetTempFilesDirectory(signatureConfiguration.GetFilesDirectory() + OUTPUT_FOLDER);
            }
        }

        /// <summary>
        /// Get path
        /// </summary>
        /// <returns>string</returns>
        public string GetPath()
        {
            return signatureConfiguration.GetTempFilesDirectory();
        }
    }
}