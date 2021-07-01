
using GroupDocs.Metadata.Common;
using GroupDocs.Metadata.Standards.Exif.MakerNote;
using System.Collections.Generic;

namespace GroupDocs.Total.MVC.Products.Metadata.Repositories.MakerNote
{
    public class CanonMakerNoteRepository : MakerNoteRepository
    {
        public CanonMakerNoteRepository(MetadataPackage branchPackage) : base(branchPackage)
        {
        }

        protected override IEnumerable<MetadataPackage> GetPackages()
        {
            var canonMakerNote = (CanonMakerNotePackage)BranchPackage;
            yield return canonMakerNote;
            yield return canonMakerNote.CameraSettings;
        }
    }
}