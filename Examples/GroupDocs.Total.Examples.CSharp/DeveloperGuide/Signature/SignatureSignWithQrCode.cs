namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Signature;

using GroupDocs.Signature.Domain;
using GroupDocs.Signature.Options;
using GroupDocs.Signature;

public static class SignatureSignWithQrCode
{
    public static void Run()
    {
        using (Signature signature = new Signature("contract.pdf"))
        {
            QrCodeSignOptions signOptions = new QrCodeSignOptions("Approved by John Smith")
            {
                EncodeType = QrCodeTypes.QR,
                Left = 100,
                Top = 100,
                Width = 120,
                Height = 120
            };

            signature.Sign("signature-sign-with-qr-code.pdf", signOptions);
        }
    }
}
