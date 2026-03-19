using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace GroupDocs.Total.WebForms.Products.Common.Config
{
    /// <summary>
    /// Application configuration.
    /// </summary>
    public class ApplicationConfiguration
    {
        private readonly string licensePath = "Licenses";

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationConfiguration"/> class.
        /// Get license path from the application configuration section of the web.config.
        /// </summary>
        public ApplicationConfiguration()
        {
            ConfigurationValuesGetter valuesGetter = new ConfigurationValuesGetter("application");
            string license = valuesGetter.GetStringPropertyValue("licensePath");
            if (string.IsNullOrEmpty(license))
            {
                string licensesDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.licensePath);
                if (Directory.Exists(licensesDir))
                {
                    string[] files = Directory.GetFiles(licensesDir, "*.lic");
                    if (files.Length > 0)
                    {
                        this.licensePath = files[0];
                        return;
                    }
                }

                Debug.WriteLine("No license file found, launched in trial mode");
                this.licensePath = string.Empty;
            }
            else
            {
                if (!IsFullPath(license))
                {
                    license = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, license);
                    if (!Directory.Exists(Path.GetDirectoryName(license)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(license));
                    }
                }

                this.licensePath = license;
                if (!File.Exists(this.licensePath))
                {
                    Debug.WriteLine("License file path is incorrect, launched in trial mode");
                    this.licensePath = string.Empty;
                }
            }
        }

        public string GetLicensePath()
        {
            return this.licensePath;
        }

        private static bool IsFullPath(string path)
        {
            return !string.IsNullOrWhiteSpace(path)
                && path.IndexOfAny(System.IO.Path.GetInvalidPathChars().ToArray()) == -1
                && Path.IsPathRooted(path)
                && !Path.GetPathRoot(path).Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal);
        }
    }
}
