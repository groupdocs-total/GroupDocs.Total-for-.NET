using GroupDocs.Total.MVC.Products.Search.Dto.Info;
using GroupDocs.Search;
using GroupDocs.Search.Common;
using GroupDocs.Search.Events;
using GroupDocs.Search.Options;
using System;
using System.IO;
using Index = GroupDocs.Search.Index;

namespace GroupDocs.Total.MVC.Products.Search.Domain.SingleIndex
{
    internal class IndexFactoryService
    {
        private readonly ILogger _logger;
        private readonly Settings _settings;
        private readonly DictionaryStorageService _dictionaryStorageService;
        private readonly object _syncRoot = new object();
        private Index _index;
        private int _allocations;
        private bool _isExclusiveIndexAccessRequested;

        public event Action ExclusiveIndexAccess;

        public IndexFactoryService(
            ILogger logger,
            Settings settings,
            DictionaryStorageService dictionaryStorageService)
        {
            _logger = logger;
            _settings = settings;
            _dictionaryStorageService = dictionaryStorageService;
        }

        public IndexStatusInfo GetIndexInfo()
        {
            Index index = null;
            try
            {
                index = Allocate();
                var ii = index.IndexInfo;
                var info = new IndexStatusInfo();
                info.indexStatus = ii.IndexStatus.ToString();
                info.version = ii.Version;
                info.time = DateTime.Now.ToString("s");
                return info;
            }
            finally
            {
                if (index != null)
                {
                    Release(index);
                }
            }
        }

        public Index Allocate()
        {
            return Allocate(false);
        }

        public Index Allocate(bool recreate)
        {
            lock (_syncRoot)
            {
                if (recreate && _index != null)
                {
                    throw new InvalidOperationException("The index cannot be deleted because it is in use.");
                }

                if (_allocations == 0)
                {
                    if (_index != null)
                    {
                        throw new InvalidOperationException("The index is not null.");
                    }

                    _index = CreateOrOpenIndex(recreate);
                }
                else
                {
                    if (_index == null)
                    {
                        throw new InvalidOperationException("The index is null.");
                    }
                }
                _allocations++;

                return _index;
            }
        }

        public void Release(Index index)
        {
            if (index == null)
            {
                throw new ArgumentNullException(nameof(index));
            }

            lock (_syncRoot)
            {
                if (_allocations <= 0 ||
                    _index == null)
                {
                    throw new InvalidOperationException("Index has already been released.");
                }
                if (_index != index)
                {
                    throw new InvalidOperationException("Index instance is invalid.");
                }

                _allocations--;

                if (_allocations == 0)
                {
                    _index.Events.ErrorOccurred -= OnErrorOccurred;
                    _index.Dispose();
                    _index = null;

                    HandleExclusiveAccess();
                }
            }
        }

        public void RequestExclusiveIndexAccess()
        {
            _isExclusiveIndexAccessRequested = true;

            lock (_syncRoot)
            {
                if (_allocations == 0)
                {
                    HandleExclusiveAccess();
                }
            }
        }

        public Index CreateTempIndex(string indexDirectory, bool recreate)
        {
            // Creates an index on disk and populates it with dictionaries from the storage
            var settings = CreateIndexSettings(indexDirectory, false);
            var index = new Index(indexDirectory, settings, recreate);
            LoadDictionaries(index);
            return index;
        }

        public Index CreateTempIndexInMemory()
        {
            // Creates index in memory with default dictionaries
            var settings = CreateIndexSettings(null, true);
            var index = new Index(settings);
            return index;
        }

        private void HandleExclusiveAccess()
        {
            if (_isExclusiveIndexAccessRequested)
            {
                _isExclusiveIndexAccessRequested = false;

                ExclusiveIndexAccess?.Invoke();
            }
        }

        private Index CreateOrOpenIndex(bool recreate)
        {
            var indexDirectory = Path.Combine(_settings.StoragePath, _settings.IndexDirectoryName);
            var settings = CreateIndexSettings(indexDirectory, false);
            Index index = new Index(indexDirectory, settings, recreate);
            index.Events.ErrorOccurred += OnErrorOccurred;
            LoadDictionaries(index);
            return index;
        }

        private void LoadDictionaries(Index index)
        {
            var dictionaries = index.Dictionaries;
            _dictionaryStorageService.Load(_settings.AlphabetFileName, dictionaries.Alphabet, d => d.Clear());
            _dictionaryStorageService.Load(_settings.StopWordDictionaryFileName, dictionaries.StopWordDictionary, d => d.Clear());
            _dictionaryStorageService.Load(_settings.SynonymDictionaryFileName, dictionaries.SynonymDictionary, d => d.Clear());
            _dictionaryStorageService.Load(_settings.HomophoneDictionaryFileName, dictionaries.HomophoneDictionary, d => d.Clear());
            _dictionaryStorageService.Load(_settings.SpellingCorrectorDictionaryFileName, dictionaries.SpellingCorrector, d => d.Clear());
            _dictionaryStorageService.Load(_settings.CharacterReplacementDictionaryFileName, dictionaries.CharacterReplacements, d => d.Clear());
        }

        private IndexSettings CreateIndexSettings(string indexDirectory, bool inMemory)
        {
            var settings = new IndexSettings()
            {
                UseStopWords = false,
                UseRawTextExtraction = false,
                AutoDetectEncoding = true,
                TextStorageSettings = new TextStorageSettings(Compression.High),
            };
            if (!inMemory)
            {
                var indexLogFilePath = Path.Combine(indexDirectory, _settings.LogFileName);
                settings.Logger = new FileLogger(indexLogFilePath, 10);
            }
            return settings;
        }

        private void OnErrorOccurred(object sender, IndexErrorEventArgs e)
        {
            _logger.LogError(e.Message);
        }
    }
}
