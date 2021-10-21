namespace GroupDocs.Total.MVC.Products.Search.Domain.ViewerCache
{
    internal class PageInfo
    {
        private readonly int _number;
        private readonly string _name;
        private bool _isCached;

        public PageInfo(int number, string name, bool isCached)
        {
            _number = number;
            _name = name;
            _isCached = isCached;
        }

        public int PageNumber => _number;

        public string Name => _name;

        public bool IsCached
        {
            get => _isCached;
            set => _isCached = value;
        }
    }
}
