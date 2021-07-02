using System;
using System.Collections.Generic;
using System.Linq;
using GroupDocs.Total.MVC.Products.Metadata.Context;
using GroupDocs.Total.MVC.Products.Metadata.DTO;
using GroupDocs.Total.MVC.Products.Metadata.Model;
using GroupDocs.Total.MVC.Products.Metadata.Util;

namespace GroupDocs.Total.MVC.Products.Metadata.Services
{
    public class MetadataService
    {
        private readonly HashSet<PropertyType> arrayTypes = new HashSet<PropertyType>
        {
            PropertyType.PropertyValueArray,
            PropertyType.StringArray,
            PropertyType.ByteArray,
            PropertyType.DoubleArray,
            PropertyType.IntegerArray,
            PropertyType.LongArray,
        };

        private readonly FileService fileService;

        public MetadataService(FileService fileService)
        {
            this.fileService = fileService;
        }

        public IEnumerable<ExtractedPackageDto> GetPackages(PostedDataDto postedData)
        {
            using (var inputStream = fileService.GetFileStream(postedData.guid))
            using (MetadataContext context = new MetadataContext(inputStream, postedData.password))
            {
                var packages = new List<ExtractedPackageDto>();
                foreach (var package in context.GetPackages())
                {
                    List<PropertyDto> properties = new List<PropertyDto>();
                    List<KnownPropertyDto> descriptors = new List<KnownPropertyDto>();

                    foreach (var property in package.Properties)
                    {
                        properties.Add(new PropertyDto
                        {
                            name = property.Name,
                            value = property.Value is Array ? ArrayUtil.AsString((Array)property.Value) : property.Value,
                            type = (int)property.Type,
                        });
                    }

                    foreach (var descriptor in package.Descriptors)
                    {
                        var accessLevel = descriptor.AccessLevel;
                        if (arrayTypes.Contains(descriptor.Type))
                        {
                            accessLevel &= AccessLevels.Remove;
                        }

                        descriptors.Add(new KnownPropertyDto
                        {
                            name = descriptor.Name,
                            type = (int)descriptor.Type,
                            accessLevel = (int)accessLevel
                        });
                    }

                    packages.Add(new ExtractedPackageDto
                    {
                        id = package.Id,
                        name = package.Name,
                        index = package.Index,
                        type = (int)package.Type,
                        properties = properties,
                        knownProperties = descriptors,
                    });

                }
                return packages;
            }
        }

        public byte[] ExportMetadata(PostedDataDto postedData)
        {
            using (var inputStream = fileService.GetFileStream(postedData.guid))
            using (MetadataContext context = new MetadataContext(inputStream, postedData.password))
            {
                return context.ExportProperties();
            }
        }

        public void SaveProperties(PostedDataDto postedData)
        {
            UpdateMetadata(postedData, (context) =>
            {
                foreach (var packageInfo in postedData.packages)
                {
                    context.UpdateProperties(packageInfo.id, packageInfo.properties.Select(p => new Property(p.name, (PropertyType)p.type, p.value)));
                }
            });
        }

        public void RemoveProperties(PostedDataDto postedData)
        {
            UpdateMetadata(postedData, (context) =>
            {
                foreach (var packageInfo in postedData.packages)
                {
                    context.RemoveProperties(packageInfo.id, packageInfo.properties.Select(p => p.name));
                }
            });
        }

        public void CleanMetadata(PostedDataDto postedData)
        {
            UpdateMetadata(postedData, (context) => context.Sanitize());
        }

        private void UpdateMetadata(PostedDataDto postedData, Action<MetadataContext> updateMethod)
        {
            using (var inputStream = fileService.GetFileStream(postedData.guid, false))
            using (MetadataContext context = new MetadataContext(inputStream, postedData.password))
            {
                updateMethod(context);
                context.Save();
            }
        }
    }
}