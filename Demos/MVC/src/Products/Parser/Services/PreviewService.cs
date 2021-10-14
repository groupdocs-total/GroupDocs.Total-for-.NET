using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using GroupDocs.Parser.Options;
using GroupDocs.Total.MVC.Products.Common.Entity.Web;
using GroupDocs.Total.MVC.Products.Parser.Config;

namespace GroupDocs.Total.MVC.Products.Parser.Services
{
    public class PreviewService
    {
        private readonly ParserConfiguration parserConfiguration;
        private readonly FileService fileService;
        private const int SpreadsheetDPI = 96;

        public PreviewService(ParserConfiguration parserConfiguration, FileService fileService)
        {
            this.parserConfiguration = parserConfiguration;
            this.fileService = fileService;
        }

        public LoadDocumentEntity LoadDocument(PostedDataEntity entity)
        {
            using (var parser = CreateParser(entity))
            {
                var documentInfo = parser.GetDocumentInfo();
                var documentEntity = default(LoadDocumentEntity);

                if (documentInfo.FileType.Format == FileFormat.Spreadsheet)
                {
                    documentEntity = new LoadDocumentEntity(); // Spreadsheet isn't supported in this version of Web App
                }
                else
                {
                    documentEntity = GeneratePreview(parser, documentInfo);
                }

                return documentEntity;
            }
        }

        private static LoadDocumentEntity GeneratePreview(GroupDocs.Parser.Parser parser, IDocumentInfo documentInfo)
        {
            var documentEntity = new LoadDocumentEntity();

            var previewOptions = new PreviewOptions(
                new CreatePageStream(pageNumber => new MemoryStream()),
                new ReleasePageStream((pageNumber, stream) =>
                {
                    var i = pageNumber - 1;
                    var bytes = (stream as MemoryStream).ToArray();
                    stream.Dispose();

                    var page = new PageDescriptionEntity
                    {
                        width = documentInfo.Pages[i].Width,
                        height = documentInfo.Pages[i].Height,
                        number = pageNumber,
                    };

                    page.SetData(Convert.ToBase64String(bytes));
                    documentEntity.SetPages(page);
                }));

            parser.GeneratePreview(previewOptions);
            return documentEntity;
        }

        private GroupDocs.Parser.Parser CreateParser(PostedDataEntity entity)
        {
            return new GroupDocs.Parser.Parser(fileService.GetSourceFileStream(entity.guid));
        }
    }
}