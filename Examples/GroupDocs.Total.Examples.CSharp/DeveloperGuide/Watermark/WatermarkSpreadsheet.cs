namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Watermark;

using GroupDocs.Watermark.Options;
using GroupDocs.Watermark.Watermarks;
using GroupDocs.Watermark;

public static class WatermarkSpreadsheet
{
    public static void Run()
    {
        using (Watermarker watermarker = new Watermarker("rate-card.xlsx"))
        {
            TextWatermark watermark = new TextWatermark(
                "DRAFT", new Font("Calibri", 36, FontStyle.Bold));

            watermark.Opacity = 0.3;
            watermark.RotateAngle = -30;

            watermarker.Add(watermark);
            watermarker.Save("watermark-spreadsheet.xlsx");
        }
    }
}
