using System;

namespace GroupDocs.Total.WebForms.Products.Common.Config
{
    /// <summary>
    /// Server configuration.
    /// </summary>
    public class ServerConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerConfiguration"/> class.
        /// Get server configuration section of the web.config.
        /// </summary>
        public ServerConfiguration()
        {
            ConfigurationValuesGetter valuesGetter = new ConfigurationValuesGetter("server");
            this.HttpPort = valuesGetter.GetIntegerPropertyValue("connector", 8080, "port");
            this.HostAddress = valuesGetter.GetStringPropertyValue("hostAddress", "localhost");
        }

        public int HttpPort { get; set; }

        public string HostAddress { get; set; }
    }
}
