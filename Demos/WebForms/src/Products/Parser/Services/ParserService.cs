using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GroupDocs.Parser.Data;
using GroupDocs.Parser.Templates;
using GroupDocs.Total.WebForms.Products.Common.Entity.Web;
using GroupDocs.Total.WebForms.Products.Parser.Config;
using GroupDocs.Total.WebForms.Products.Parser.Entity;

namespace GroupDocs.Total.WebForms.Products.Parser.Services
{
    public class ParserService
    {
        private readonly ParserConfiguration parserConfiguration;
        private readonly FileService fileService;

        public ParserService(ParserConfiguration parserConfiguration, FileService fileService)
        {
            this.parserConfiguration = parserConfiguration;
            this.fileService = fileService;
        }

        public FileDataEntity Parse(ParsePostedDataEntity postedData)
        {
            using (var parser = CreateParser(postedData))
            {
                var template = new Template(postedData.fields
                   .Select(f => CreateTemplateField(f))
                   .Where(i => i != null));

                var data = parser.ParseByTemplate(template)
                        .Select(i => new TemplateFieldValueEntity { name = i.Name, value = GetFieldValue(i) })
                        .ToArray();

                var entity = new FileDataEntity
                {
                    data = data
                };

                return entity;
            }
        }

        private GroupDocs.Parser.Parser CreateParser(PostedDataEntity entity)
        {
            return new GroupDocs.Parser.Parser(fileService.GetSourceFileStream(entity.guid));
        }

        private static object GetFieldValue(FieldData fieldData)
        {
            if (fieldData.PageArea is PageTextArea)
            {
                return (fieldData.PageArea as PageTextArea).Text;
            }
            else if (fieldData.PageArea is PageTableArea)
            {
                var table = fieldData.PageArea as PageTableArea;
                var rows = new List<string[]>();
                for (var i = 0; i < table.RowCount; i++)
                {
                    var row = new string[table.ColumnCount];
                    for (var j = 0; j < table.ColumnCount; j++)
                    {
                        row[j] = table[i, j]?.Text;
                    }

                    rows.Add(row);
                }

                return rows.ToArray();
            }
            else
            {
                return null;
            }
        }

        private static TemplateItem CreateTemplateField(TemplateFieldEntity f)
        {
            switch (f.fieldType)
            {
                case "FIXED":
                    return new TemplateField(
                        new TemplateFixedPosition(new Rectangle(new Point(f.x, f.y), new Size(f.width, f.height))),
                        f.name,
                        f.pageNumber == 0 ? null : (int?)f.pageNumber - 1);
                case "TABLE":
                    var rect = new Rectangle(new Point(f.x, f.y), new Size(f.width, f.height));
                    var columns = new List<double>();
                    columns.Add(rect.Left);
                    columns.AddRange(f.columns.Select(i => i.value + rect.Left));
                    columns.Add(rect.Right);

                    var parameters = new TemplateTableParameters(rect, columns);

                    return new TemplateTable(parameters, f.name, f.pageNumber == 0 ? null : (int?)f.pageNumber - 1);

                default:
                    return null;
            }
        }
    }
}