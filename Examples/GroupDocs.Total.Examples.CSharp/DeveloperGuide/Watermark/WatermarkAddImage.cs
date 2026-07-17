namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Watermark;

using GroupDocs.Watermark.Common;
using GroupDocs.Watermark.Options;
using GroupDocs.Watermark.Watermarks;
using GroupDocs.Watermark;

public static class WatermarkAddImage
{
    public static void Run()
    {
        using (Watermarker watermarker = new Watermarker("contract.pdf"))
        {
            using (ImageWatermark watermark = new ImageWatermark("logo.png"))
            {
                watermark.Opacity = 0.5;
                watermark.HorizontalAlignment = HorizontalAlignment.Center;
                watermark.VerticalAlignment = VerticalAlignment.Center;

                watermarker.Add(watermark);
            }

            watermarker.Save("watermark-add-image.pdf");
        }
    }
}
