using GroupDocs.Total.MVC.Products.Search.Domain.SingleIndex;
using System;
using System.Collections.Generic;

namespace GroupDocs.Total.MVC.Products.Search.Domain.ViewerCache
{
    internal class HtmlCacheService
    {
        private readonly Settings _settings;
        private readonly Dictionary<DocumentKey, DocumentCache> _dictionary = new Dictionary<DocumentKey, DocumentCache>();
        private readonly object _syncRoot = new object();

        public HtmlCacheService(
            Settings settings)
        {
            _settings = settings;
        }

        public DocumentCache GetCache(Guid userId, string fileName)
        {
            var key = new DocumentKey(userId.ToString(), fileName);
            lock (_syncRoot)
            {
                if (!_dictionary.TryGetValue(key, out DocumentCache documentCache))
                {
                    var userFileInfo = new UserFileInfo(_settings, userId, fileName);
                    documentCache = new DocumentCache(this, userFileInfo);
                    _dictionary.Add(key, documentCache);
                }
                return documentCache;
            }
        }

        public void Delete(Guid userId, string fileName)
        {
            var key = new DocumentKey(userId.ToString(), fileName);
            lock (_syncRoot)
            {
                _dictionary.Remove(key);
            }
        }
    }
}
