using System;
using System.IO;
using GroupDocs.Total.MVC.Products.Common.Config;
using GroupDocs.Total.MVC.Products.Search.Util.Directory;
using Newtonsoft.Json;

namespace GroupDocs.Total.MVC.Products.Search.Config
{
    /// <summary>
    /// SearchConfiguration.
    /// </summary>
    public class SearchConfiguration : CommonConfiguration
    {
        [JsonProperty]
        private string filesDirectory = "Files/Search";

        [JsonProperty]
        private string indexDirectory = "Files/Search/Index";

        [JsonProperty]
        private string indexedFilesDirectory = "Files/Search/Indexed";

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchConfiguration"/> class.
        /// Constructor.
        /// </summary>
        public SearchConfiguration()
        {
            // get Search configuration section from the web.config
            ConfigurationValuesGetter valuesGetter = new ConfigurationValuesGetter("search");

            this.filesDirectory = valuesGetter.GetStringPropertyValue("filesDirectory", this.filesDirectory);
            if (!DirectoryUtils.IsFullPath(this.filesDirectory))
            {
                this.filesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.filesDirectory);
                if (!Directory.Exists(this.filesDirectory))
                {
                    Directory.CreateDirectory(this.filesDirectory);
                }
            }

            this.indexDirectory = valuesGetter.GetStringPropertyValue("indexDirectory", this.indexDirectory);
            if (!DirectoryUtils.IsFullPath(this.indexDirectory))
            {
                this.indexDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.indexDirectory);
                if (!Directory.Exists(this.indexDirectory))
                {
                    Directory.CreateDirectory(this.indexDirectory);
                }
            }

            this.indexedFilesDirectory = valuesGetter.GetStringPropertyValue("indexedFilesDirectory", this.indexedFilesDirectory);
            if (!DirectoryUtils.IsFullPath(this.indexedFilesDirectory))
            {
                this.indexedFilesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.indexedFilesDirectory);
                if (!Directory.Exists(this.indexedFilesDirectory))
                {
                    Directory.CreateDirectory(this.indexedFilesDirectory);
                }
            }
        }

        public void SetFilesDirectory(string filesDirectory)
        {
            this.filesDirectory = filesDirectory;
        }

        public string GetFilesDirectory()
        {
            return this.filesDirectory;
        }

        public void SetIndexDirectory(string indexDirectory)
        {
            this.indexDirectory = indexDirectory;
        }

        public string GetIndexDirectory()
        {
            return this.indexDirectory;
        }

        public void SetIndexedFilesDirectory(string indexedFilesDirectory)
        {
            this.indexedFilesDirectory = indexedFilesDirectory;
        }

        public string GetIndexedFilesDirectory()
        {
            return this.indexedFilesDirectory;
        }
    }
}