using GroupDocs.Search.Dictionaries;
using System;
using System.IO;

namespace GroupDocs.Total.MVC.Products.Search.Domain.SingleIndex
{
    internal class DictionaryStorageService
    {
        private readonly object _dictionaryFileSync = new object();
        private readonly Settings _settings;
        private readonly StorageService _storageService;

        public DictionaryStorageService(
            Settings settings,
            StorageService storageService)
        {
            _settings = settings;
            _storageService = storageService;
        }

        public void Load<T>(string dictionaryFileName, T dictionary, Action<T> clearAction)
            where T : DictionaryBase
        {
            lock (_dictionaryFileSync)
            {
                var dictionaryFilePath = Path.Combine(_settings.StoragePath, dictionaryFileName);
                var exists = DownloadDictionary(dictionaryFileName, dictionaryFilePath);
                if (exists)
                {
                    clearAction(dictionary);
                    dictionary.ImportDictionary(dictionaryFilePath);
                }
            }
        }

        public void Save(string dictionaryFileName, DictionaryBase dictionary)
        {
            lock (_dictionaryFileSync)
            {
                var dictionaryFilePath = Path.Combine(_settings.StoragePath, dictionaryFileName);
                dictionary.ExportDictionary(dictionaryFilePath);
                UploadDictionary(dictionaryFileName, dictionaryFilePath);
            }
        }

        private bool DownloadDictionary(string fileName, string localFilePath)
        {
            using (var fileStream = File.OpenWrite(localFilePath))
            {
                var folderName = _settings.AdminId;
                if (_storageService.FileExists(folderName, fileName))
                {
                    using (var stream = _storageService.DownloadFile(folderName, fileName))
                    {
                        stream.Position = 0;
                        stream.CopyTo(fileStream);
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private void UploadDictionary(string fileName, string localFilePath)
        {
            using (var fileStream = File.OpenRead(localFilePath))
            {
                var folderName = _settings.AdminId;
                _storageService.UploadFile(folderName, fileName, fileStream);
            }
        }
    }
}
