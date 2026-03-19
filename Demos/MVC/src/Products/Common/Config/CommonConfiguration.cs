using Newtonsoft.Json;

namespace GroupDocs.Total.MVC.Products.Common.Config
{
    /// <summary>
    /// CommonConfiguration.
    /// </summary>
    public class CommonConfiguration
    {
        [JsonProperty]
        public bool pageSelector { get; set; }

        [JsonProperty]
        public bool download { get; set; }

        [JsonProperty]
        public bool upload { get; set; }

        [JsonProperty]
        public bool print { get; set; }

        [JsonProperty]
        public bool browse { get; set; }

        [JsonProperty]
        public bool rewrite { get; set; }

        [JsonProperty]
        public bool enableRightClick { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonConfiguration"/> class.
        /// </summary>
        public CommonConfiguration()
        {
            ConfigurationValuesGetter valuesGetter = new ConfigurationValuesGetter("common");
            this.pageSelector = valuesGetter.GetBooleanPropertyValue("pageSelector", true);
            this.download = valuesGetter.GetBooleanPropertyValue("download", true);
            this.upload = valuesGetter.GetBooleanPropertyValue("upload", true);
            this.print = valuesGetter.GetBooleanPropertyValue("print", true);
            this.browse = valuesGetter.GetBooleanPropertyValue("browse", true);
            this.rewrite = valuesGetter.GetBooleanPropertyValue("rewrite", true);
            this.enableRightClick = valuesGetter.GetBooleanPropertyValue("enableRightClick", true);
        }
    }
}
