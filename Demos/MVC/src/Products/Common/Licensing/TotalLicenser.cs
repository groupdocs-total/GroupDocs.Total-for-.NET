namespace GroupDocs.Total.MVC.Products.Common.Licensing
{
    public static class GroupDocsTotalLicenser
    {
        public static void SetLicense()
        {
            Config.GlobalConfiguration globalConfiguration
                = new Config.GlobalConfiguration();

            string licensePath = globalConfiguration
                .GetApplicationConfiguration()
                .GetLicensePath();

            if (!string.IsNullOrEmpty(licensePath))
            {
                new GroupDocs.Viewer.License().SetLicense(licensePath);
                new GroupDocs.Signature.License().SetLicense(licensePath);
                new GroupDocs.Annotation.License().SetLicense(licensePath);
                new GroupDocs.Comparison.License().SetLicense(licensePath);
                new GroupDocs.Conversion.License().SetLicense(licensePath);
                new GroupDocs.Editor.License().SetLicense(licensePath);
                new GroupDocs.Metadata.License().SetLicense(licensePath);
                new GroupDocs.Search.License().SetLicense(licensePath);
                new Aspose.Html.License().SetLicense(licensePath);
            }
        }
    }
}