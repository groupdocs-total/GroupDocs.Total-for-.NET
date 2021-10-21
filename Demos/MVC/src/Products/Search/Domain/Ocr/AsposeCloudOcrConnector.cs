using Aspose.Ocr.Cloud.Sdk;
using Aspose.Ocr.Cloud.Sdk.Model;
using Aspose.Ocr.Cloud.Sdk.Model.Requests;
using GroupDocs.Total.MVC.Products.Search.Domain.SingleIndex;
using GroupDocs.Search.Options;
using System;
using System.Threading.Tasks;

namespace GroupDocs.Total.MVC.Products.Search.Domain.Ocr
{
    internal class AsposeCloudOcrConnector : IOcrConnector
    {
        private const string WatermarkMaxDuration = "In demo mode, using OCR, the image is processed no longer than {0} minutes.";
        private const string WatermarkMaxImageCount = "In demo mode, only the first 5 images in a document are processed using OCR.";

        private readonly OcrImageCounter _ocrImageCounter;
        private readonly TimeSpan _ocrTimeLimit;
        private readonly Configuration _configuration;

        public AsposeCloudOcrConnector(
            OcrImageCounter ocrImageCounter,
            TimeSpan ocrTimeLimit)
        {
            _ocrImageCounter = ocrImageCounter;
            _ocrTimeLimit = ocrTimeLimit;
            _configuration = new Configuration();
            _configuration.AppSid = "fdd0d2f7-5497-454c-9c89-0e7847f45d39";
            _configuration.AppKey = "0a9c56769a3da8c12c885f1c87ff340a";
        }

        public string Recognize(OcrContext context)
        {

            if (!_ocrImageCounter.Increase())
            {
                return WatermarkMaxImageCount;
            }

            OcrApi api = new OcrApi(_configuration);
            var request = new PostOcrFromUrlOrContentRequest(context.ImageStream);

            var task = Task.Factory.StartNew(() => {
                OCRResponse response = api.PostOcrFromUrlOrContent(request);
                return response.Text;
            });

            if (task.Wait(_ocrTimeLimit))
            {
                return task.Result;
            }
            else
            {
                var message = string.Format(WatermarkMaxDuration, _ocrTimeLimit.TotalMinutes.ToString("F2"));
                return message;
            }
        }
    }
}
