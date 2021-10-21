using System.Threading;

namespace GroupDocs.Total.MVC.Products.Search.Domain.ViewerCache
{
    internal class PageGate
    {
        public int PageNumber { get; }
        public AutoResetEvent Gate { get; }

        public PageGate(int pageNumber, AutoResetEvent gate)
        {
            PageNumber = pageNumber;
            Gate = gate;
        }
    }
}
