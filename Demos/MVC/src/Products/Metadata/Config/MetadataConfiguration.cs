using GroupDocs.Total.MVC.Products.Common.Config;
using GroupDocs.Total.MVC.Products.Common.Util.Parser;
using Newtonsoft.Json;
using System;
using System.IO;

namespace GroupDocs.Total.MVC.Products.Metadata.Config
{
    /// <summary>
    /// MetadataConfiguration
    /// </summary>
    public class MetadataConfiguration : CommonConfiguration
    {
        private string filesDirectory = "DocumentSamples/Metadata";

        private readonly string outputDirectory = "DocumentSamples/Metadata/Output";

        private readonly string tempDirectory = "DocumentSamples/Metadata/Temp";

        private readonly int fileOperationTimeout;

        private readonly int fileOperationRetryCount;

        private readonly int previewTimeLimit;

        [JsonProperty]
        private string defaultDocument = "";

        [JsonProperty]
        private int preloadPageCount;

        [JsonProperty]
        private bool htmlMode = true;

        [JsonProperty]
        private bool cache = true;

        /// <summary>
        /// Constructor
        /// </summary>
        public MetadataConfiguration()
        {
            YamlParser parser = new YamlParser();
            dynamic configuration = parser.GetConfiguration("metadata");
            ConfigurationValuesGetter valuesGetter = new ConfigurationValuesGetter(configuration);

            // get Metadata configuration section from the web.config
            filesDirectory = valuesGetter.GetStringPropertyValue("filesDirectory", filesDirectory);
            filesDirectory = InitDirectory(filesDirectory);
            
            outputDirectory = valuesGetter.GetStringPropertyValue("outputDirectory", outputDirectory);
            outputDirectory = InitDirectory(outputDirectory);

            tempDirectory = valuesGetter.GetStringPropertyValue("tempDirectory", tempDirectory);
            tempDirectory = InitDirectory(tempDirectory);

            defaultDocument = valuesGetter.GetStringPropertyValue("defaultDocument", defaultDocument);
            preloadPageCount = valuesGetter.GetIntegerPropertyValue("preloadPageCount", preloadPageCount);
            previewTimeLimit = valuesGetter.GetIntegerPropertyValue("previewTimeLimit", previewTimeLimit);
            htmlMode = valuesGetter.GetBooleanPropertyValue("htmlMode", htmlMode);
            cache = valuesGetter.GetBooleanPropertyValue("cache", cache);
            browse = valuesGetter.GetBooleanPropertyValue("browse", browse);
            upload = valuesGetter.GetBooleanPropertyValue("upload", upload);
            fileOperationTimeout = valuesGetter.GetIntegerPropertyValue("fileOperationTimeout", fileOperationTimeout);
            fileOperationRetryCount = valuesGetter.GetIntegerPropertyValue("fileOperationRetryCount", fileOperationRetryCount);
        }

        public void SetFilesDirectory(string filesDirectory)
        {
            this.filesDirectory = filesDirectory;
        }

        public string GetFilesDirectory()
        {
            return filesDirectory;
        }

        public void SetDefaultDocument(string defaultDocument)
        {
            this.defaultDocument = defaultDocument;
        }

        public string GetDefaultDocument()
        {
            return defaultDocument;
        }

        public void SetPreloadPageCount(int preloadPageCount)
        {
            this.preloadPageCount = preloadPageCount;
        }

        public int GetPreloadPageCount()
        {
            return preloadPageCount;
        }

        public int GetPreviewTimeLimit()
        {
            return previewTimeLimit;
        }

        public int GetFileOperationTimeout()
        {
            return fileOperationTimeout;
        }

        public int GetFileOperationRetryCount()
        {
            return fileOperationRetryCount;
        }

        public void SetIsHtmlMode(bool isHtmlMode)
        {
            htmlMode = isHtmlMode;
        }

        public bool GetIsHtmlMode()
        {
            return htmlMode;
        }

        public void SetCache(bool Cache)
        {
            cache = Cache;
        }

        public bool GetCache()
        {
            return cache;
        }

        public string GetTempFilePath()
        {
            return Path.Combine(tempDirectory, Guid.NewGuid().ToString());
        }

        public string GetInputFilePath(string relativePath)
        {
            return GetAbsolutePath(filesDirectory, relativePath);
        }

        public string GetOutputFilePath(string relativePath)
        {
            return GetAbsolutePath(outputDirectory, relativePath);
        }

        private string GetAbsolutePath(string baseDirectory, string relativePath)
        {
            var absolutePath = Path.GetFullPath(Path.Combine(baseDirectory, relativePath));
            if (!absolutePath.StartsWith(baseDirectory))
            {
                throw new ArgumentException("Couldn't find the specified file path", nameof(relativePath));
            }

            return absolutePath;
        }

        private string InitDirectory(string path)
        {
            string absolutePath = path;
            if (!Path.IsPathRooted(path))
            {
                absolutePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path));
            }
            if (!Directory.Exists(absolutePath))
            {
                Directory.CreateDirectory(absolutePath);
            }

            return absolutePath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
    }
}