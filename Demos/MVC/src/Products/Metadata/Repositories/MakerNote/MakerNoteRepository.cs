
using GroupDocs.Metadata.Common;
using GroupDocs.Metadata.Formats.Image;
using GroupDocs.Total.MVC.Products.Metadata.Model;
using System.Collections.Generic;

namespace GroupDocs.Total.MVC.Products.Metadata.Repositories.MakerNote
{
    public class MakerNoteRepository : MetadataPackageRepository
    {
        public MakerNoteRepository(MetadataPackage branchPackage) : base(branchPackage)
        {
        }

        protected override IEnumerable<MetadataPackage> GetPackages()
        {
            yield return BranchPackage;
        }

        public override IEnumerable<Property> GetProperties()
        {
            foreach (var package in GetPackages())
            {
                foreach (var property in package)
                {
                    TiffTag tag = property as TiffTag;
                    if (tag != null)
                    {
                        yield return new Property(((int)tag.TagID).ToString(), (PropertyType)property.Value.Type, property.Value.RawValue);
                    }
                }
            }
        }
    }
}