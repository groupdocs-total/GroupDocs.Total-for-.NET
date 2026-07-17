namespace GroupDocs.Total.Examples.CSharp.GettingStarted.LicensingAndSubscription;

public static class LicenseFromFile
{
    public static void Run()
    {
        string licensePath = "GroupDocs.Total.lic";

        // Set license for all products
        GroupDocs.Total.License.SetLicense(licensePath);

        // Or set license for specific products
        GroupDocs.Viewer.License licenseViewer = new GroupDocs.Viewer.License();
        licenseViewer.SetLicense(licensePath);

        GroupDocs.Conversion.License licenseConversion = new GroupDocs.Conversion.License();
        licenseConversion.SetLicense(licensePath);
    }
}
