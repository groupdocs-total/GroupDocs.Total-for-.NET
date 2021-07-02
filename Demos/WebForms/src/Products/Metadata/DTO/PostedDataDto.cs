using GroupDocs.Total.WebForms.Products.Common.Entity.Web;
using System.Collections.Generic;

namespace GroupDocs.Total.WebForms.Products.Metadata.DTO
{
    public class PostedDataDto : PostedDataEntity
    {
        /// <summary>
        /// Collection of the document properties with their data.
        /// </summary>
        public IEnumerable<PostedMetadataPackageDto> packages { get; set; }
    }
}