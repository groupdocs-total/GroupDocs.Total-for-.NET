# GroupDocs.Total for .NET

[![NuGet](https://img.shields.io/nuget/v/GroupDocs.Total)](https://www.nuget.org/packages/GroupDocs.Total/)
![Downloads](https://img.shields.io/nuget/dt/GroupDocs.Total?label=nuget%20downloads)

GroupDocs.Total for .NET is a comprehensive suite of document management APIs for .NET developers. It provides a wide range of functionalities to view, annotate, convert, compare, sign, assemble, and redact documents seamlessly within .NET applications.

## Features

- **Document Viewing**: Render and display over 50 document formats including PDF, Microsoft Office, and images.
- **Annotation**: Add, remove, and manage annotations in various document formats.
- **Conversion**: Convert documents from one format to another with high fidelity.
- **Comparison**: Compare documents to highlight changes and differences.
- **e-Signature**: Integrate electronic signature capabilities into your applications.
- **Assembly**: Automate the generation of documents by merging templates with data.
- **Redaction**: Redact sensitive information from documents securely.

## Supported Formats

GroupDocs.Total for .NET supports a wide range of document formats, including but not limited to:
- **PDF**: PDF, PDF/A
- **Microsoft Office**: Word, Excel, PowerPoint, Visio, OneNote
- **Images**: JPEG, PNG, BMP, TIFF, GIF
- **Others**: HTML, TXT, RTF, XML, EPUB, and many more.

## Getting Started

To get started with GroupDocs.Total for .NET, follow these steps:

1. **Install via NuGet**:
    ```sh
    Install-Package GroupDocs.Total
    ```

2. **Add Namespaces of used GroupDocs product**:
    ```csharp
    using GroupDocs.Conversion;
    using GroupDocs.Viewer;
    ```

3. **Example Usage**:
    ```csharp
    // Example code to load a document and convert it to PDF
    var converter = new GroupDocs.Conversion.Converter("sample.docx");
    var convertOptions = new GroupDocs.Conversion.Options.Convert.PdfConvertOptions();
    converter.Convert("output.pdf", convertOptions);
    ```

## Documentation

Comprehensive documentation for GroupDocs.Total for .NET is available at the [GroupDocs Documentation](https://docs.groupdocs.com/total/net/).

## Support

If you encounter any issues or have any questions, please reach out to us via the following channels:
- [GitHub Issues](https://github.com/groupdocs-total/GroupDocs.Total-for-.NET/issues)
- [Free Support Forum](https://forum.groupdocs.com/)
- [Paid Support Helpdesk](https://helpdesk.groupdocs.com/)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

We welcome contributions to the GroupDocs.Total for .NET repository. 

## About GroupDocs

GroupDocs is a leading provider of document management solutions for developers. Our APIs are designed to make document automation processes seamless and efficient. For more information, visit [our website](https://www.groupdocs.com/).
