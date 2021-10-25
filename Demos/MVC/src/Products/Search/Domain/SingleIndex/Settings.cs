using System;

namespace GroupDocs.Total.MVC.Products.Search.Domain.SingleIndex
{
    public class Settings
    {
        public const string DocumentKeySeparator = "/";

        public int MaxUploadedFiles { get; private set; }
        public int MaxIndexedFiles { get; private set; }
        public long MaxFileLength { get; private set; }
        public string StoragePath { get; private set; }
        public string IndexDirectoryName { get; private set; }
        public string UploadedDirectoryName { get; private set; }
        public string StatusesDirectoryName { get; private set; }
        public string ViewerCacheDirectoryName { get; private set; }
        public string TempIndexDirectoryName { get; private set; }
        public TimeSpan MinFolderLifetime { get; private set; }
        public TimeSpan CleanupPeriod { get; private set; }
        public string UrlBase { get; private set; }
        public string AdminId { get; private set; }
        public string LogFilePath { get; private set; }
        public TimeSpan OcrTimeLimit { get; } = new TimeSpan(0, 1, 0);
        public int MaxOcrImageCount { get; } = 5;
        public string LogFileName { get; } = "log.txt";

        public string AlphabetFileName { get; } = "alphabet.txt";
        public string StopWordDictionaryFileName { get; } = "stopwords.txt";
        public string SynonymDictionaryFileName { get; } = "synonyms.txt";
        public string HomophoneDictionaryFileName { get; } = "homophones.txt";
        public string SpellingCorrectorDictionaryFileName { get; } = "spellings.txt";

        public Settings()
        {
            MaxUploadedFiles = 50;
            MaxIndexedFiles = 50;
            MaxFileLength = 20971520;
            StoragePath = "C:/SearchApp/Storage/";
            IndexDirectoryName = "Index";
            UploadedDirectoryName = "Uploaded";
            StatusesDirectoryName = "Statuses";
            ViewerCacheDirectoryName = "ViewerCache";
            TempIndexDirectoryName = "TempIndex";
            MinFolderLifetime = new TimeSpan(24, 0, 0);
            CleanupPeriod = new TimeSpan(1, 0, 0);
            UrlBase = "https://localhost:44369/";
            AdminId = "a8fafbe1-b61b-4005-b47c-8fdfd31924f0";
            LogFilePath = "C:/SearchApp/Log.txt";
        }
    }
}
