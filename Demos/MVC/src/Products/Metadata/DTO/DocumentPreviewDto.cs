

using GroupDocs.Total.MVC.Products.Common.Entity.Web;
using Newtonsoft.Json;

namespace GroupDocs.Total.MVC.Products.Metadata.DTO
{
    public class DocumentPreviewDto : LoadDocumentEntity
    {
        [JsonProperty]
        private bool timeLimitExceeded;

        public void SetTimeLimitExceeded(bool value)
        {
            timeLimitExceeded = value;
        }
    }
}