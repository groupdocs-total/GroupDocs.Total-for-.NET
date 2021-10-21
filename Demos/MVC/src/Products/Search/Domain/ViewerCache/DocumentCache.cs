using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace GroupDocs.Total.MVC.Products.Search.Domain.ViewerCache
{
    internal class DocumentCache
    {
        private enum CacheState
        {
            Created,
            Preparing,
            Generating,
            Completed,
        }

        private CacheState _state;
        private readonly HtmlCacheService _htmlCacheService;
        private readonly UserFileInfo _fileInfo;

        private readonly object _syncRoot = new object();
        private readonly List<PageGate> _gates = new List<PageGate>();

        private CacheGenerator _cacheGenerator;
        private Dictionary<int, PageInfo> _pages;

        public DocumentCache(
            HtmlCacheService htmlCacheService,
            UserFileInfo fileInfo)
        {
            _state = CacheState.Created;
            _htmlCacheService = htmlCacheService;
            _fileInfo = fileInfo;
        }

        public UserFileInfo FileInfo => _fileInfo;

        public bool PrepareDocument(string password)
        {
            lock (_syncRoot)
            {
                switch (_state)
                {
                    case CacheState.Created:
                        _cacheGenerator = new CacheGenerator(_fileInfo, password);
                        _cacheGenerator.ViewerCreated += OnCacheGeneratorViewerCreated;
                        _cacheGenerator.PageHandled += OnCacheGeneratorPageHandled;
                        _cacheGenerator.Finished += OnCacheGeneratorFinished;
                        _cacheGenerator.Start();
                        _state = CacheState.Preparing;
                        return false;

                    case CacheState.Preparing:
                        return false;

                    case CacheState.Generating:
                    case CacheState.Completed:
                        return true;

                    default:
                        throw StateIsNotSupported(_state);
                }
            }
        }

        public string GetPageContent(int pageNumber, out string pageName, out int pageCount)
        {
            AutoResetEvent gate = null;
            try
            {
                lock (_syncRoot)
                {
                    switch (_state)
                    {
                        case CacheState.Created:
                        case CacheState.Preparing:
                            throw new NotSupportedException();

                        case CacheState.Generating:
                            {
                                var pageInfo = _pages[pageNumber];
                                pageName = pageInfo.Name;
                                pageCount = _pages.Count;
                                if (!pageInfo.IsCached)
                                {
                                    gate = new AutoResetEvent(false);
                                    var pageGate = new PageGate(pageNumber, gate);
                                    _gates.Add(pageGate);
                                    _cacheGenerator.RequestPage(pageNumber);
                                    pageInfo.IsCached = true;
                                }
                            }
                            break;

                        case CacheState.Completed:
                            {
                                var pageInfo = _pages[pageNumber];
                                pageName = pageInfo.Name;
                                pageCount = _pages.Count;
                                if (!pageInfo.IsCached)
                                {
                                    throw new Exception("There is an incomplete page in the document: " + _fileInfo.FileName);
                                }
                            }
                            break;

                        default:
                            throw StateIsNotSupported(_state);
                    }
                }

                gate?.WaitOne();
            }
            finally
            {
                gate?.Dispose();
            }

            var htmlFilePath = FileInfo.GetHtmlPageFilePath(pageNumber);
            return File.ReadAllText(htmlFilePath);
        }

        public void DeleteHtmlCache()
        {
            lock (_syncRoot)
            {
                if (_cacheGenerator != null)
                {
                    try
                    {
                        DestroyCacheGenerator();
                    }
                    catch (Exception)
                    {
                    }
                }

                var fileCacheFolderPath = _fileInfo.FileCacheFolderPath;
                if (Directory.Exists(fileCacheFolderPath))
                {
                    Directory.Delete(fileCacheFolderPath, true);
                }
            }

            _htmlCacheService.Delete(_fileInfo.UserId, _fileInfo.FileName);
        }

        private void OnCacheGeneratorViewerCreated(PageInfo[] pages)
        {
            lock (_syncRoot)
            {
                _state = CacheState.Generating;
                _pages = pages
                    .ToDictionary(pi => pi.PageNumber, pi => pi);
            }
        }

        private void OnCacheGeneratorPageHandled(int pageNumber)
        {
            lock (_syncRoot)
            {
                _pages[pageNumber].IsCached = true;

                var array = _gates.ToArray();
                foreach (var gate in array)
                {
                    if (gate.PageNumber == pageNumber)
                    {
                        _gates.Remove(gate);
                        gate.Gate.Set();
                    }
                }
            }
        }

        private void OnCacheGeneratorFinished()
        {
            lock (_syncRoot)
            {
                _state = CacheState.Completed;
                DestroyCacheGenerator();
            }
        }

        private void DestroyCacheGenerator()
        {
            _cacheGenerator.ViewerCreated -= OnCacheGeneratorViewerCreated;
            _cacheGenerator.PageHandled -= OnCacheGeneratorPageHandled;
            _cacheGenerator.Finished -= OnCacheGeneratorFinished;
            _cacheGenerator.Dispose();
            _cacheGenerator = null;
        }

        private static NotSupportedException StateIsNotSupported(CacheState state)
        {
            return new NotSupportedException("The cache state is not supported: " + state);
        }
    }
}
