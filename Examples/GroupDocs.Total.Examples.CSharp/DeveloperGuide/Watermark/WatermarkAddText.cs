namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Watermark;

using GroupDocs.Watermark.Common;
using GroupDocs.Watermark.Options;
using GroupDocs.Watermark.Watermarks;
using GroupDocs.Watermark;

public static class WatermarkAddText
{
    public static void Run()
    {
        using (Watermarker watermarker = new Watermarker("contract.docx"))
        {
            TextWatermark watermark = new TextWatermark(
                "CONFIDENTIAL", new Font("Arial", 42, FontStyle.Bold));

            watermark.ForegroundColor = Color.Red;
            watermark.Opacity = 0.4;
            watermark.RotateAngle = -45;
            watermark.HorizontalAlignment = HorizontalAlignment.Center;
            watermark.VerticalAlignment = VerticalAlignment.Center;

            watermarker.Add(watermark);
            watermarker.Save("watermark-add-text.docx");
        }
    }
}
