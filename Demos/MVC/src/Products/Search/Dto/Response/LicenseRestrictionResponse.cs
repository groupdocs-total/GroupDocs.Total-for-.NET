namespace GroupDocs.Total.MVC.Products.Search.Dto.Response
{
    public class LicenseRestrictionResponse
    {
        public bool IsRestricted { get; set; }

        public string Message { get; set; }

        public static LicenseRestrictionResponse CreateNonRestricted()
        {
            var response = new LicenseRestrictionResponse()
            {
                IsRestricted = false,
            };
            return response;
        }

        public static LicenseRestrictionResponse CreateRestricted(string message)
        {
            var response = new LicenseRestrictionResponse()
            {
                IsRestricted = true,
                Message = message,
            };
            return response;
        }
    }
}
