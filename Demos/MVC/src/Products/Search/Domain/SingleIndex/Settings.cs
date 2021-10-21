using Microsoft.Extensions.Configuration;
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

        public Settings(IConfiguration configuration)
        {
            MaxUploadedFiles = configuration.GetValue<int>("AppSettings:MaxUploadedFiles");
            MaxIndexedFiles = configuration.GetValue<int>("AppSettings:MaxIndexedFiles");
            MaxFileLength = configuration.GetValue<long>("AppSettings:MaxFileLength");
            StoragePath = configuration["AppSettings:StoragePath"];
            IndexDirectoryName = configuration["AppSettings:IndexDirectoryName"];
            UploadedDirectoryName = configuration["AppSettings:UploadedDirectoryName"];
            StatusesDirectoryName = configuration["AppSettings:StatusesDirectoryName"];
            ViewerCacheDirectoryName = configuration["AppSettings:ViewerCacheDirectoryName"];
            TempIndexDirectoryName = configuration["AppSettings:TempIndexDirectoryName"];
            MinFolderLifetime = new TimeSpan(configuration.GetValue<int>("AppSettings:MinFolderLifetime"), 0, 0);
            CleanupPeriod = new TimeSpan(configuration.GetValue<int>("AppSettings:CleanupPeriod"), 0, 0);
            UrlBase = configuration["ApiResourceBaseUrls:UrlBase"];
            AdminId = configuration["AdminSettings:AdminId"];
            LogFilePath = configuration["Logging:FilePath"];
        }
    }
}
