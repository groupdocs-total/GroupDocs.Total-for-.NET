using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GroupDocs.Total.MVC.Products.Search.Domain.ViewerCache
{
    internal class CacheGenerator : IDisposable
    {
        private class PageOrderInfo
        {
            public int PageNumber { get; }
            public bool IsCached { get; set; }

            public PageOrderInfo(int pageNumber, bool isCached)
            {
                PageNumber = pageNumber;
                IsCached = isCached;
            }
        }

        public void Dispose()
        {
            _htmlViewer?.Dispose();
        }

        private readonly UserFileInfo _fileInfo;
        private readonly string _password;

        private HtmlViewer _htmlViewer;

        private PageOrderInfo[] _orderArray;
        private int _orderIndex;
        private Dictionary<int, PageOrderInfo> _dictionary;
        private readonly Queue<PageOrderInfo> _requestedPages = new Queue<PageOrderInfo>();
        private readonly object _requestedPagesSync = new object();

        public event Action<PageInfo[]> ViewerCreated;
        public event Action<int> PageHandled;
        public event Action Finished;

        public CacheGenerator(
            UserFileInfo fileInfo,
            string password)
        {
            _fileInfo = fileInfo;
            _password = password;
        }

        public void Start()
        {
            var task = new Task(() =>
            {
                var htmlViewer = new HtmlViewer(_fileInfo, _password);
                var pageArray = htmlViewer.GetPages()
                    .Select(page => new PageInfo(page.Number, page.Name, false))
                    .ToArray();
                _orderArray = pageArray
                    .Select(pi => new PageOrderInfo(pi.PageNumber, pi.IsCached))
                    .ToArray();
                _dictionary = _orderArray
                    .ToDictionary(poi => poi.PageNumber, poi => poi);

                _htmlViewer = htmlViewer;

                ViewerCreated?.Invoke(pageArray);

                bool isActive = true;
                while (isActive)
                {
                    bool isAllDone = GetNextPage(out PageOrderInfo orderInfo);

                    if (isAllDone)
                    {
                        isActive = false;
                        break;
                    }
                    else
                    {
                        if (!orderInfo.IsCached)
                        {
                            if (!IsPageCached(orderInfo.PageNumber))
                            {
                                _htmlViewer.CreateCacheForPage(orderInfo.PageNumber);
                            }

                            orderInfo.IsCached = true;
                        }

                        PageHandled?.Invoke(orderInfo.PageNumber);
                    }
                }

                Finished?.Invoke();
            },
            TaskCreationOptions.LongRunning);
            task.Start();
        }

        public void RequestPage(int pageNumber)
        {
            lock (_requestedPagesSync)
            {
                if (_dictionary.TryGetValue(pageNumber, out PageOrderInfo orderInfo))
                {
                    _requestedPages.Enqueue(orderInfo);
                }
                else
                {
                    throw new InvalidOperationException("The page is not represented in the dictionary: " + pageNumber);
                }
            }
        }

        private bool GetNextPage(out PageOrderInfo info)
        {
            lock (_requestedPagesSync)
            {
                if (_requestedPages.Count > 0)
                {
                    var requestedPage = _requestedPages.Dequeue();
                    info = requestedPage;
                    return false;
                }
                else
                {
                    while (_orderIndex < _orderArray.Length)
                    {
                        var orderInfo = _orderArray[_orderIndex];
                        _orderIndex++;

                        if (!orderInfo.IsCached)
                        {
                            info = orderInfo;
                            return false;
                        }
                    }

                    info = null;
                    return true;
                }
            }
        }

        private bool IsPageCached(int pageNumber)
        {
            var htmlPageFilePath = _fileInfo.GetHtmlPageFilePath(pageNumber);
            return File.Exists(htmlPageFilePath);
        }
    }
}
