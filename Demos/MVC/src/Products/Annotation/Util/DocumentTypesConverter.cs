using GroupDocs.Annotation.Models;

namespace GroupDocs.Total.MVC.Products.Annotation.Util
{
    public class DocumentTypesConverter
    {
        /// <summary>
        /// Convert document type from string into int
        /// </summary>
        /// <param name="documentType">string</param>
        /// <returns>int</returns>
        public static DocumentType GetDocumentType(string documentType)
        {
            switch (documentType)
            {
                case "Portable Document Format":
                case "PDF":
                    return DocumentType.Pdf;
                case "Microsoft Word":
                case "WORDS":
                case "Microsoft Word Open XML format (.docx)":
                    return DocumentType.Words;
                case "Microsoft PowerPoint":
                case "SLIDES":
                    return DocumentType.Slides;
                case "image":
                    return DocumentType.Images;
                case "Microsoft Excel":
                case "CELLS":
                    return DocumentType.Cells;
                case "AutoCAD Drawing File Format":
                case "diagram":
                    return DocumentType.Diagram;
                default:
                    return DocumentType.Undefined;
            }
        }
    }
}