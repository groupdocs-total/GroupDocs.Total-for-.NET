namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Signature;

using GroupDocs.Signature.Domain;
using GroupDocs.Signature.Options;
using GroupDocs.Signature;

public static class SignatureSignWithText
{
    public static void Run()
    {
        using (Signature signature = new Signature("contract.pdf"))
        {
            TextSignOptions signOptions = new TextSignOptions("John Smith")
            {
                Left = 100,
                Top = 100,
                Width = 200,
                Height = 40
            };

            signature.Sign("signature-sign-with-text.pdf", signOptions);
        }
    }
}
