using Newtonsoft.Json;
using System;
using System.IO;
using YamlDotNet.Serialization;

namespace GroupDocs.Total.MVC.Products.Common.Util.Parser
{
    public class YamlParser
    {
        private static readonly string YamlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configuration.yml");
        private readonly dynamic configuationData;

        /// <summary>
        /// Initializes a new instance of the <see cref="YamlParser"/> class.
        /// </summary>
        public YamlParser()
        {
            if (File.Exists(YamlPath))
            {
                using (var reader = new StringReader(File.ReadAllText(YamlPath)))
                {
                    var deserializer = new DeserializerBuilder().Build();
                    var yamlObject = deserializer.Deserialize(reader);

                    var serializer = new SerializerBuilder()
                        .JsonCompatible()
                        .Build();

                    this.configuationData = serializer.Serialize(yamlObject);
                }
            }
        }

        public dynamic GetConfiguration(string configurationSectionName)
        {
            dynamic productConfiguration = null;
            if (this.configuationData != null)
            {
                productConfiguration = JsonConvert.DeserializeObject(this.configuationData)[configurationSectionName];
            }

            return productConfiguration;
        }
    }
}