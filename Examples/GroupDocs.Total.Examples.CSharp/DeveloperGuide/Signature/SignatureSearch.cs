namespace GroupDocs.Total.Examples.CSharp.DeveloperGuide.Signature;

using System.Collections.Generic;
using System;
using GroupDocs.Signature.Domain;
using GroupDocs.Signature.Options;
using GroupDocs.Signature;

public static class SignatureSearch
{
    public static void Run()
    {
        // Sign first, so there is something to find
        using (Signature signature = new Signature("contract.pdf"))
        {
            QrCodeSignOptions signOptions = new QrCodeSignOptions("Approved by John Smith")
            {
                Left = 100,
                Top = 100
            };
            signature.Sign("signature-search.pdf", signOptions);
        }

        using (Signature signature = new Signature("signature-search.pdf"))
        {
            QrCodeSearchOptions searchOptions = new QrCodeSearchOptions();
            List<QrCodeSignature> signatures = signature.Search<QrCodeSignature>(searchOptions);

            Console.WriteLine("Signatures found: " + signatures.Count);

            foreach (QrCodeSignature qrSignature in signatures)
            {
                Console.WriteLine("Type: " + qrSignature.EncodeType.TypeName
                                  + ", text: " + qrSignature.Text
                                  + ", page: " + qrSignature.PageNumber);
            }
        }
    }
}
