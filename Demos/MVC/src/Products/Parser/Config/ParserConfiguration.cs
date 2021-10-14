using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using GroupDocs.Total.MVC.Products.Common.Config;
using GroupDocs.Total.MVC.Products.Common.Util.Parser;

namespace GroupDocs.Total.MVC.Products.Parser.Config
{
    public class ParserConfiguration
    {
        private string filesDirectory = "DocumentSamples/Parser";
        private readonly string outputDirectory = "DocumentSamples/Parser/Output";
        private readonly string tempDirectory = "DocumentSamples/Parser/Temp";

        public ParserConfiguration()
        {
            YamlParser parser = new YamlParser();
            dynamic configuration = parser.GetConfiguration("parser");
            ConfigurationValuesGetter valuesGetter = new ConfigurationValuesGetter(configuration);

            // get Metadata configuration section from the web.config
            filesDirectory = valuesGetter.GetStringPropertyValue("filesDirectory", filesDirectory);
            filesDirectory = InitDirectory(filesDirectory);

            outputDirectory = valuesGetter.GetStringPropertyValue("outputDirectory", outputDirectory);
            outputDirectory = InitDirectory(outputDirectory);
            
            tempDirectory = valuesGetter.GetStringPropertyValue("tempDirectory", tempDirectory);
            tempDirectory = InitDirectory(tempDirectory);
        }

        public void SetFilesDirectory(string filesDirectory)
        {
            this.filesDirectory = filesDirectory;
        }

        public string GetFilesDirectory()
        {
            return filesDirectory;
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

        private static string GetAbsolutePath(string baseDirectory, string relativePath)
        {
            var absolutePath = Path.GetFullPath(Path.Combine(baseDirectory, relativePath));
            if (!absolutePath.StartsWith(baseDirectory))
            {
                throw new ArgumentException("Couldn't find the specified file path", nameof(relativePath));
            }

            return absolutePath;
        }

        private static string InitDirectory(string path)
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