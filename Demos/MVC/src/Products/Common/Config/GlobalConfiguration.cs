using GroupDocs.Total.MVC.Products.Annotation.Config;
using GroupDocs.Total.MVC.Products.Comparison.Config;
using GroupDocs.Total.MVC.Products.Conversion.Config;
using GroupDocs.Total.MVC.Products.Editor.Config;
using GroupDocs.Total.MVC.Products.Metadata.Config;
using GroupDocs.Total.MVC.Products.Parser.Config;
using GroupDocs.Total.MVC.Products.Search.Config;
using GroupDocs.Total.MVC.Products.Signature.Config;
using GroupDocs.Total.MVC.Products.Viewer.Config;

namespace GroupDocs.Total.MVC.Products.Common.Config
{
    /// <summary>
    /// Global configuration.
    /// </summary>
    public class GlobalConfiguration
    {
        private readonly ServerConfiguration server;
        private readonly ApplicationConfiguration application;
        private readonly CommonConfiguration common;
        private readonly SignatureConfiguration signature;
        private readonly ViewerConfiguration viewer;
        private readonly AnnotationConfiguration annotation;
        private readonly ComparisonConfiguration comparison;
        private readonly ConversionConfiguration conversion;
        private readonly EditorConfiguration editor;
        private readonly MetadataConfiguration metadata;
        private readonly SearchConfiguration search;
        private readonly ParserConfiguration parser;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalConfiguration"/> class.
        /// Get all configurations.
        /// </summary>
        public GlobalConfiguration()
        {
            this.server = new ServerConfiguration();
            this.application = new ApplicationConfiguration();
            this.signature = new SignatureConfiguration();
            this.viewer = new ViewerConfiguration();
            this.common = new CommonConfiguration();
            this.annotation = new AnnotationConfiguration();
            this.comparison = new ComparisonConfiguration();
            this.conversion = new ConversionConfiguration();
            this.editor = new EditorConfiguration();
            this.metadata = new MetadataConfiguration();
            this.search = new SearchConfiguration();
            this.parser = new ParserConfiguration();
        }

        public EditorConfiguration GetEditorConfiguration()
        {
            return this.editor;
        }

        public ServerConfiguration GetServerConfiguration()
        {
            return this.server;
        }

        public ApplicationConfiguration GetApplicationConfiguration()
        {
            return this.application;
        }

        public CommonConfiguration GetCommonConfiguration()
        {
            return this.common;
        }

        public ViewerConfiguration GetViewerConfiguration()
        {
            return this.viewer;
        }

        public AnnotationConfiguration GetAnnotationConfiguration()
        {
            return this.annotation;
        }

        public SignatureConfiguration GetSignatureConfiguration()
        {
            return this.signature;
        }

        public ComparisonConfiguration GetComparisonConfiguration()
        {
            return this.comparison;
        }

        public ConversionConfiguration GetConversionConfiguration()
        {
            return this.conversion;
        }

        public MetadataConfiguration GetMetadataConfiguration()
        {
            return this.metadata;
        }

        public SearchConfiguration GetSearchConfiguration()
        {
            return this.search;
        }

        public ParserConfiguration GetParserConfiguration()
        {
            return this.parser;
        }
    }
}