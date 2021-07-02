using System;

namespace GroupDocs.Total.MVC.Products.Common.Config
{
    public class ConfigurationValuesGetter
    {
        private readonly dynamic configuration;

        public ConfigurationValuesGetter(dynamic configuration)
        {
            this.configuration = configuration;
        }

        public string GetStringPropertyValue(string propertyName)
        {
            return (this.configuration != null && this.configuration[propertyName] != null && !string.IsNullOrEmpty(this.configuration[propertyName].ToString())) ?
                this.configuration[propertyName].ToString() :
                null;
        }

        public string GetStringPropertyValue(string propertyName, string defaultValue)
        {
            return (this.configuration != null && this.configuration[propertyName] != null && !string.IsNullOrEmpty(this.configuration[propertyName].ToString())) ?
                this.configuration[propertyName].ToString() :
                defaultValue;
        }

        public int GetIntegerPropertyValue(string propertyName, int defaultValue)
        {
            int value;
            value = (this.configuration != null && this.configuration[propertyName] != null && !string.IsNullOrEmpty(this.configuration[propertyName].ToString())) ?
                Convert.ToInt32(this.configuration[propertyName]) :
                defaultValue;
            return value;
        }

        public int GetIntegerPropertyValue(string propertyName, int defaultValue, string innerPropertyName)
        {
            int value;
            if (!string.IsNullOrEmpty(innerPropertyName))
            {
                value = (this.configuration != null && this.configuration[propertyName] != null && !string.IsNullOrEmpty(this.configuration[propertyName][innerPropertyName].ToString())) ?
                    Convert.ToInt32(this.configuration[propertyName][innerPropertyName]) :
                    defaultValue;
            }
            else
            {
                value = (this.configuration != null && this.configuration[propertyName] != null && !string.IsNullOrEmpty(this.configuration[propertyName].ToString())) ?
                    Convert.ToInt32(this.configuration[propertyName]) :
                    defaultValue;
            }
            return value;
        }

        public bool GetBooleanPropertyValue(string propertyName, bool defaultValue)
        {
            return (this.configuration != null && this.configuration[propertyName] != null && !string.IsNullOrEmpty(this.configuration[propertyName].ToString())) ? Convert.ToBoolean(this.configuration[propertyName]) : defaultValue;
        }
    }
}