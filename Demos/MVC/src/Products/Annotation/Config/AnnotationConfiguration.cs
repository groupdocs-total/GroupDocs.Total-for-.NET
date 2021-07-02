using GroupDocs.Total.MVC.Products.Common.Config;
using GroupDocs.Total.MVC.Products.Common.Util.Parser;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace GroupDocs.Total.MVC.Products.Annotation.Config
{
    /// <summary>
    /// AnnotationConfiguration
    /// </summary>
    public class AnnotationConfiguration : CommonConfiguration
    {
        [JsonProperty]
        private readonly string filesDirectory = "DocumentSamples/Annotation";

        [JsonProperty]
        private readonly string defaultDocument = "";

        [JsonProperty]
        private readonly int preloadPageCount;

        [JsonProperty]
        private readonly bool textAnnotation = true;

        [JsonProperty]
        private readonly bool areaAnnotation = true;

        [JsonProperty]
        private readonly bool pointAnnotation = true;

        [JsonProperty]
        private readonly bool textStrikeoutAnnotation = true;

        [JsonProperty]
        private readonly bool polylineAnnotation = true;

        [JsonProperty]
        private readonly bool textFieldAnnotation = true;

        [JsonProperty]
        private readonly bool watermarkAnnotation = true;

        [JsonProperty]
        private readonly bool textReplacementAnnotation = true;

        [JsonProperty]
        private readonly bool arrowAnnotation = true;

        [JsonProperty]
        private readonly bool textRedactionAnnotation = true;

        [JsonProperty]
        private readonly bool resourcesRedactionAnnotation = true;

        [JsonProperty]
        private readonly bool textUnderlineAnnotation = true;

        [JsonProperty]
        private readonly bool distanceAnnotation = true;

        [JsonProperty]
        private readonly bool downloadOriginal = true;

        [JsonProperty]
        private readonly bool downloadAnnotated = true;

        [JsonProperty]
        private readonly bool zoom = true;

        /// <summary>
        /// Get Annotation configuration section from the Web.config
        /// </summary>
        public AnnotationConfiguration()
        {
            YamlParser parser = new YamlParser();
            dynamic configuration = parser.GetConfiguration("annotation");
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

            defaultDocument = valuesGetter.GetStringPropertyValue("defaultDocument", defaultDocument).Replace(@"\", "/");
            textAnnotation = valuesGetter.GetBooleanPropertyValue("textAnnotation", textAnnotation);
            areaAnnotation = valuesGetter.GetBooleanPropertyValue("areaAnnotation", areaAnnotation);
            pointAnnotation = valuesGetter.GetBooleanPropertyValue("pointAnnotation", pointAnnotation);
            textStrikeoutAnnotation = valuesGetter.GetBooleanPropertyValue("textStrikeoutAnnotation", textStrikeoutAnnotation);
            polylineAnnotation = valuesGetter.GetBooleanPropertyValue("polylineAnnotation", polylineAnnotation);
            textFieldAnnotation = valuesGetter.GetBooleanPropertyValue("textFieldAnnotation", textFieldAnnotation);
            watermarkAnnotation = valuesGetter.GetBooleanPropertyValue("watermarkAnnotation", watermarkAnnotation);
            textReplacementAnnotation = valuesGetter.GetBooleanPropertyValue("textReplacementAnnotation", textReplacementAnnotation);
            arrowAnnotation = valuesGetter.GetBooleanPropertyValue("arrowAnnotation", arrowAnnotation);
            textRedactionAnnotation = valuesGetter.GetBooleanPropertyValue("textRedactionAnnotation", textRedactionAnnotation);
            resourcesRedactionAnnotation = valuesGetter.GetBooleanPropertyValue("resourcesRedactionAnnotation", resourcesRedactionAnnotation);
            textUnderlineAnnotation = valuesGetter.GetBooleanPropertyValue("textUnderlineAnnotation", textUnderlineAnnotation);
            distanceAnnotation = valuesGetter.GetBooleanPropertyValue("distanceAnnotation", distanceAnnotation);
            downloadOriginal = valuesGetter.GetBooleanPropertyValue("downloadOriginal", downloadOriginal);
            downloadAnnotated = valuesGetter.GetBooleanPropertyValue("downloadAnnotated", downloadAnnotated);
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

        public string GetFilesDirectory()
        {
            return filesDirectory;
        }

        public int GetPreloadPageCount()
        {
            return preloadPageCount;
        }
    }
}