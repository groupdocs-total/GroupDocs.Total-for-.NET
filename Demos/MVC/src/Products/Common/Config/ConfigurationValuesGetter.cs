using System;
using System.Configuration;

namespace GroupDocs.Total.MVC.Products.Common.Config
{
    public class ConfigurationValuesGetter
    {
        private readonly string prefix;

        public ConfigurationValuesGetter(string sectionName)
        {
            this.prefix = sectionName + ":";
        }

        public string GetStringPropertyValue(string propertyName)
        {
            var value = ConfigurationManager.AppSettings[prefix + propertyName];
            return !string.IsNullOrEmpty(value) ? value : null;
        }

        public string GetStringPropertyValue(string propertyName, string defaultValue)
        {
            var value = ConfigurationManager.AppSettings[prefix + propertyName];
            return !string.IsNullOrEmpty(value) ? value : defaultValue;
        }

        public int GetIntegerPropertyValue(string propertyName, int defaultValue)
        {
            var value = ConfigurationManager.AppSettings[prefix + propertyName];
            return !string.IsNullOrEmpty(value) ? Convert.ToInt32(value) : defaultValue;
        }

        public int GetIntegerPropertyValue(string propertyName, int defaultValue, string innerPropertyName)
        {
            var key = !string.IsNullOrEmpty(innerPropertyName)
                ? prefix + propertyName + ":" + innerPropertyName
                : prefix + propertyName;
            var value = ConfigurationManager.AppSettings[key];
            return !string.IsNullOrEmpty(value) ? Convert.ToInt32(value) : defaultValue;
        }

        public bool GetBooleanPropertyValue(string propertyName, bool defaultValue)
        {
            var value = ConfigurationManager.AppSettings[prefix + propertyName];
            return !string.IsNullOrEmpty(value) ? Convert.ToBoolean(value) : defaultValue;
        }
    }
}
