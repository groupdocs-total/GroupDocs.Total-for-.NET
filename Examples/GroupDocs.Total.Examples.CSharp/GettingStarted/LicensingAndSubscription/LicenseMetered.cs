namespace GroupDocs.Total.Examples.CSharp.GettingStarted.LicensingAndSubscription;

using System;

public static class LicenseMetered
{
    public static void Run()
    {
        string publicKey = ""; // Your public license key
        string privateKey = ""; // Your private license key

        // Set metered keys for GroupDocs.Viewer
        GroupDocs.Viewer.Metered meteredViewer = new GroupDocs.Viewer.Metered();
        meteredViewer.SetMeteredKey(publicKey, privateKey);

        // Get consumption metrics
        decimal amountConsumed = GroupDocs.Viewer.Metered.GetConsumptionQuantity();
        decimal creditsConsumed = GroupDocs.Viewer.Metered.GetConsumptionCredit();

        // Set metered keys for GroupDocs.Conversion
        GroupDocs.Conversion.Metered meteredConversion = new GroupDocs.Conversion.Metered();
        meteredConversion.SetMeteredKey(publicKey, privateKey);
    }
}
