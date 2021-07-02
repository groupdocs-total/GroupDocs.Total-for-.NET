using System;
using System.IO;
using System.Reflection;

namespace GroupDocs.Total.MVC.AppDomainGenerator
{
    /// <summary>
    /// Generates app domains and sets app licenses.
    /// </summary>
    public class DomainGenerator
    {
        private readonly Products.Common.Config.GlobalConfiguration globalConfiguration;
        private readonly Type currentType;

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainGenerator"/> class.
        /// </summary>
        /// <param name="assemblyName">Assembly name.</param>
        /// <param name="className">Class name.</param>
        public DomainGenerator(string assemblyName, string className)
        {
            this.globalConfiguration = new Products.Common.Config.GlobalConfiguration();

            // Get assembly path
            string assemblyPath = this.GetAssemblyPath(assemblyName);

            // Initiate GroupDocs license class
            this.currentType = this.CreateDomain(assemblyName + "Domain", assemblyPath, className);
        }

        /// <summary>
        /// Set GroupDocs.Viewer license.
        /// </summary>
        /// <param name="type">Type.</param>
        public void SetViewerLicense()
        {
            // Initiate License class
            var obj = (GroupDocs.Viewer.License)Activator.CreateInstance(this.currentType);

            // Set license
            this.SetLicense(obj);
        }

        /// <summary>
        /// Set GroupDocs.Signature license.
        /// </summary>
        /// <param name="type">Type.</param>
        public void SetSignatureLicense()
        {
            // Initiate license class
            var obj = (GroupDocs.Signature.License)Activator.CreateInstance(this.currentType);

            // Set license
            this.SetLicense(obj);
        }

        /// <summary>
        /// Set GroupDocs.Annotation license.
        /// </summary>
        /// <param name="type">Type.</param>
        public void SetAnnotationLicense()
        {
            // Initiate license class
            var obj = (GroupDocs.Annotation.License)Activator.CreateInstance(this.currentType);

            // Set license
            this.SetLicense(obj);
        }

        /// <summary>
        /// Set GroupDocs.Comparison license.
        /// </summary>
        /// <param name="type">Type.</param>
        public void SetComparisonLicense()
        {
            // Initiate license class
            var obj = (GroupDocs.Comparison.License)Activator.CreateInstance(this.currentType);

            // Set license
            this.SetLicense(obj);
        }

        /// <summary>
        /// Set GroupDocs.Conversion license.
        /// </summary>
        /// <param name="type">Type.</param>
        public void SetConversionLicense()
        {
            // Initiate license class
            var obj = (GroupDocs.Conversion.License)Activator.CreateInstance(this.currentType);

            // Set license
            this.SetLicense(obj);
        }

        /// <summary>
        /// Set GroupDocs.Editor license.
        /// </summary>
        /// <param name="type">Type.</param>
        public void SetEditorLicense()
        {
            // Initiate license class
            var obj = (GroupDocs.Editor.License)Activator.CreateInstance(this.currentType);

            // Set license
            this.SetLicense(obj);
        }

        /// <summary>
        /// Set GroupDocs.Metadata license.
        /// </summary>
        /// <param name="type">Type.</param>
        public void SetMetadataLicense()
        {
            // Initiate license class
            var obj = (GroupDocs.Metadata.License)Activator.CreateInstance(this.currentType);

            // Set license
            this.SetLicense(obj);
        }

        /// <summary>
        /// Set GroupDocs.Search license.
        /// </summary>
        /// <param name="type">Type.</param>
        public void SetSearchLicense()
        {
            // Initiate license class
            var obj = (GroupDocs.Search.License)Activator.CreateInstance(this.currentType);

            // Set license
            this.SetLicense(obj);
        }

        /// <summary>
        /// Get assembly full path by its name.
        /// </summary>
        /// <param name="assemblyName">string.</param>
        /// <returns>Assebmly name.</returns>
        private string GetAssemblyPath(string assemblyName)
        {
            string path = string.Empty;

            // Get path of the executable
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string uriPath = Uri.UnescapeDataString(uri.Path);

            // Get path of the assembly
            path = Path.Combine(Path.GetDirectoryName(uriPath), assemblyName);
            return path;
        }

        /// <summary>
        /// Create AppDomain for the assembly.
        /// </summary>
        /// <param name="domainName">Domain name.</param>
        /// <param name="assemblyPath">Assembly path.</param>
        /// <param name="className">Class name.</param>
        /// <returns>Assembly type.</returns>
        private Type CreateDomain(string domainName, string assemblyPath, string className)
        {
            // Create domain
            AppDomain dom = AppDomain.CreateDomain(domainName);
            AssemblyName assemblyName = new AssemblyName { CodeBase = assemblyPath };

            // Load assembly into the domain
            Assembly assembly = dom.Load(assemblyName);

            // Initiate class from the loaded assembly
            Type type = assembly.GetType(className);
            return type;
        }

        private void SetLicense(dynamic obj) {
            if (!string.IsNullOrEmpty(this.globalConfiguration.GetApplicationConfiguration().GetLicensePath()))
            {
                obj.SetLicense(this.globalConfiguration.GetApplicationConfiguration().GetLicensePath());
            }
        }
    }
}