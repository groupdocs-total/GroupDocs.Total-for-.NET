
using GroupDocs.Total.MVC.Products.Annotation.Config;

namespace GroupDocs.Total.MVC.Products.Annotation.Util.Directory
{
    public class DirectoryUtils
    {
        public FilesDirectoryUtils FilesDirectory;       

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="annotationConfiguration">AnnotationConfiguration</param>
        public DirectoryUtils(AnnotationConfiguration annotationConfiguration)
        {
            FilesDirectory = new FilesDirectoryUtils(annotationConfiguration);            
        }
    }
}