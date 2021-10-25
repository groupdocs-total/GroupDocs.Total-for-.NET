using GroupDocs.Search.Options;
using System.IO;
using Tesseract;

namespace GroupDocs.Total.MVC.Products.Search.Domain.Ocr
{
    internal class TesseractOcrConnector : IOcrConnector
    {
        public TesseractOcrConnector()
        {
        }

        public string Recognize(OcrContext context)
        {
            var buffer = new byte[context.ImageStream.Length];
            context.ImageStream.Read(buffer, 0, buffer.Length);

            var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            path = Path.Combine(path, "tessdata");
            path = path.Replace("file:\\", "");

            using (var engine = new TesseractEngine(path, "eng", EngineMode.Default))
            using (Pix img = Pix.LoadFromMemory(buffer))
            using (Page recognizedPage = engine.Process(img))
            {
                string recognizedText = recognizedPage.GetText();
                return recognizedText;
            }
        }
    }
}
