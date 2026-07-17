namespace GroupDocs.Total.Examples.CSharp.GettingStarted.LicensingAndSubscription;

using System.IO;

public static class LicenseFromStream
{
    public static void Run()
    {
        string licensePath = "GroupDocs.Total.lic";
        using (FileStream licenseStream = File.OpenRead(licensePath))
        {
            // Set license for all products
            GroupDocs.Total.License.SetLicense(licenseStream);

            // Or set license for specific products
            GroupDocs.Viewer.License licenseViewer = new GroupDocs.Viewer.License();
            licenseViewer.SetLicense(licenseStream);

            GroupDocs.Conversion.License licenseConversion = new GroupDocs.Conversion.License();
            licenseConversion.SetLicense(licenseStream);
        }
    }
}
