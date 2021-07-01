using GroupDocs.Total.MVC.Products.Common.Util.Parser;
using System;
using System.Collections.Specialized;
using System.Configuration;

namespace GroupDocs.Total.MVC.Products.Common.Config
{
    /// <summary>
    /// Server configuration.
    /// </summary>
    public class ServerConfiguration : ConfigurationSection
    {
        private readonly NameValueCollection serverConfiguration = (NameValueCollection)System.Configuration.ConfigurationManager.GetSection("serverConfiguration");

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerConfiguration"/> class.
        /// Get server configuration section of the web.config.
        /// </summary>
        public ServerConfiguration()
        {
            YamlParser parser = new YamlParser();
            dynamic configuration = parser.GetConfiguration("server");
            ConfigurationValuesGetter valuesGetter = new ConfigurationValuesGetter(configuration);
            int defaultPort = Convert.ToInt32(this.serverConfiguration["httpPort"]);
            this.HttpPort = valuesGetter.GetIntegerPropertyValue("connector", defaultPort, "port");
            this.HostAddress = valuesGetter.GetStringPropertyValue("hostAddress", this.serverConfiguration["hostAddress"]);
        }

        public int HttpPort { get; set; }

        public string HostAddress { get; set; }
    }
}