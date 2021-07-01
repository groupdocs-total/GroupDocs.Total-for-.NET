using GroupDocs.Total.MVC.Products.Common.Config;
using GroupDocs.Total.MVC.Products.Common.Util.Parser;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace GroupDocs.Total.MVC.Products.Signature.Config
{
    /// <summary>
    /// SignatureConfiguration
    /// </summary>
    public class SignatureConfiguration : CommonConfiguration
    {
        [JsonProperty]
        private string filesDirectory = "DocumentSamples/Signature";

        [JsonProperty]
        private readonly string defaultDocument = "";

        [JsonProperty]
        private string dataDirectory = "";

        [JsonProperty]
        private readonly int preloadPageCount;

        [JsonProperty]
        private readonly bool textSignature = true;

        [JsonProperty]
        private readonly bool imageSignature = true;

        [JsonProperty]
        private readonly bool digitalSignature = true;

        [JsonProperty]
        private readonly bool qrCodeSignature = true;

        [JsonProperty]
        private readonly bool barCodeSignature = true;

        [JsonProperty]
        private readonly bool stampSignature = true;

        [JsonProperty]
        private readonly bool handSignature = true;

        [JsonProperty]
        private readonly bool downloadOriginal = true;

        [JsonProperty]
        private readonly bool downloadSigned = true;

        [JsonProperty]
        private string tempFilesDirectory = "";

        [JsonProperty]
        private readonly bool zoom = true;
        
        /// <summary>
        /// Get Signature configuration section from the Web.config
        /// </summary>
        public SignatureConfiguration()
        {
            YamlParser parser = new YamlParser();
            dynamic configuration = parser.GetConfiguration("signature");
            ConfigurationValuesGetter valuesGetter = new ConfigurationValuesGetter(configuration);

            filesDirectory = valuesGetter.GetStringPropertyValue("filesDirectory", filesDirectory);
            if (!IsFullPath(filesDirectory))
            {
                filesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filesDirectory);
                if (!Directory.Exists(filesDirectory))
                {
                    Directory.CreateDirectory(filesDirectory);
                }
            }

            dataDirectory = valuesGetter.GetStringPropertyValue("dataDirectory", dataDirectory);
            defaultDocument = valuesGetter.GetStringPropertyValue("defaultDocument", defaultDocument);
            textSignature = valuesGetter.GetBooleanPropertyValue("textSignature", textSignature);
            imageSignature = valuesGetter.GetBooleanPropertyValue("imageSignature", imageSignature);
            digitalSignature = valuesGetter.GetBooleanPropertyValue("digitalSignature", digitalSignature);
            qrCodeSignature = valuesGetter.GetBooleanPropertyValue("qrCodeSignature", qrCodeSignature);
            barCodeSignature = valuesGetter.GetBooleanPropertyValue("barCodeSignature", barCodeSignature);
            stampSignature = valuesGetter.GetBooleanPropertyValue("stampSignature", stampSignature);
            handSignature = valuesGetter.GetBooleanPropertyValue("handSignature", handSignature);
            downloadOriginal = valuesGetter.GetBooleanPropertyValue("downloadOriginal", downloadOriginal);
            downloadSigned = valuesGetter.GetBooleanPropertyValue("downloadSigned", downloadSigned);
            preloadPageCount = valuesGetter.GetIntegerPropertyValue("preloadPageCount", preloadPageCount);
            zoom = valuesGetter.GetBooleanPropertyValue("zoom", zoom);
        }

        private static bool IsFullPath(string path)
        {
            return !string.IsNullOrWhiteSpace(path)
                && path.IndexOfAny(Path.GetInvalidPathChars().ToArray()) == -1
                && Path.IsPathRooted(path)
                && !Path.GetPathRoot(path).Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal);
        }

        public void SetFilesDirectory(string filesDirectory)
        {
            this.filesDirectory = filesDirectory;
        }

        public string GetFilesDirectory()
        {
            return filesDirectory;
        }

        public void SetDataDirectory(string dataDirectory)
        {
            this.dataDirectory = dataDirectory;
        }

        public string GetDataDirectory()
        {
            return dataDirectory;
        }

        public int GetPreloadPageCount()
        {
            return preloadPageCount;
        }

        public void SetTempFilesDirectory(string tempFilesDirectory)
        {
            this.tempFilesDirectory = tempFilesDirectory;
        }

        public string GetTempFilesDirectory()
        {
            return tempFilesDirectory;
        }
    }
}