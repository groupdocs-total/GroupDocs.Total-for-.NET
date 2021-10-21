namespace GroupDocs.Total.MVC.Products.Search.Dto
{
    public class KeyPasswordPair
    {
        public KeyPasswordPair(string key, string password)
        {
            Key = key;
            Password = password;
        }

        public string Key { get; private set; }

        public string Password { get; private set; }
    }
}
