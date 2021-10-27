namespace GroupDocs.Total.MVC.Products.Search.Dto.Response
{
    public class LicenseRestrictionResponse
    {
        public bool isRestricted { get; set; }

        public string message { get; set; }

        public static LicenseRestrictionResponse CreateNonRestricted()
        {
            var response = new LicenseRestrictionResponse()
            {
                isRestricted = false,
            };
            return response;
        }

        public static LicenseRestrictionResponse CreateRestricted(string message)
        {
            var response = new LicenseRestrictionResponse()
            {
                isRestricted = true,
                message = message,
            };
            return response;
        }
    }
}
