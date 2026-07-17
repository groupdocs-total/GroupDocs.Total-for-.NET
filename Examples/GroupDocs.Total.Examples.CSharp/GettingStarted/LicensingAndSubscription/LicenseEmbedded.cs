namespace GroupDocs.Total.Examples.CSharp.GettingStarted.LicensingAndSubscription;

public static class LicenseEmbedded
{
    public static void Run()
    {
        // Set license for all products
        GroupDocs.Total.License.SetLicense("GroupDocs.Total.lic");

        // Or set license for specific products
        GroupDocs.Viewer.License licenseViewer = new GroupDocs.Viewer.License();
        licenseViewer.SetLicense("GroupDocs.Total.lic");

        GroupDocs.Conversion.License licenseConversion = new GroupDocs.Conversion.License();
        licenseConversion.SetLicense("GroupDocs.Total.lic");
    }
}
