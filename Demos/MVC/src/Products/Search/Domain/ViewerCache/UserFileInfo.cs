using GroupDocs.Total.MVC.Products.Search.Domain.SingleIndex;
using System;
using System.IO;

namespace GroupDocs.Total.MVC.Products.Search.Domain.ViewerCache
{
    internal class UserFileInfo
    {
        private readonly Guid _userId;
        private readonly string _userFolderName;
        private readonly string _fileName;
        private readonly string _urlBase;

        private readonly string _userDirectoryPath;
        private readonly string _uploadedDirectoryPath;
        private readonly string _sourceFilePath;
        private readonly string _viewerCacheDirectoryPath;
        private readonly string _fileFolderName;
        private readonly string _fileCacheFolderPath;

        public UserFileInfo(
            Settings settings,
            Guid userId,
            string fileName)
        {
            _userId = userId;
            _userFolderName = userId.ToString();
            _fileName = fileName;
            _urlBase = settings.UrlBase;

            _sourceFilePath = GetSourceFilePath(
                settings,
                _userFolderName,
                fileName,
                out _userDirectoryPath,
                out _uploadedDirectoryPath);
            _viewerCacheDirectoryPath = Path.Combine(_userDirectoryPath, settings.ViewerCacheDirectoryName);
            _fileFolderName = fileName.Replace(".", "_");
            _fileCacheFolderPath = Path.Combine(_viewerCacheDirectoryPath, _fileFolderName);
        }

        public Guid UserId => _userId;
        public string UserFolderName => _userFolderName;
        public string FileName => _fileName;
        public string UrlBase => _urlBase;

        public string UserDirectoryPath => _userDirectoryPath;
        public string UploadedDirectoryPath => _uploadedDirectoryPath;
        public string SourceFilePath => _sourceFilePath;
        public string ViewerCacheDirectoryPath => _viewerCacheDirectoryPath;
        public string FileFolderName => _fileFolderName;
        public string FileCacheFolderPath => _fileCacheFolderPath;

        public static string GetSourceFilePath(
            Settings settings,
            string directoryName,
            string fileName,
            out string userDirectoryPath,
            out string uploadedDirectoryPath)
        {
            userDirectoryPath = Path.Combine(settings.StoragePath, directoryName);
            uploadedDirectoryPath = Path.Combine(userDirectoryPath, settings.UploadedDirectoryName);
            var sourceFilePath = Path.Combine(uploadedDirectoryPath, fileName);
            return sourceFilePath;
        }

        public string GetHtmlPageFilePath(int pageNumber)
        {
            string pageFileName = $"p{pageNumber}.html";
            string pageFilePath = Path.Combine(_fileCacheFolderPath, pageFileName);
            return pageFilePath;
        }

        public string GetHtmlPageResourceFilePath(int pageNumber, string resourceFileName)
        {
            string resFileName = $"p{pageNumber}_{resourceFileName}";
            string resFilePath = Path.Combine(_fileCacheFolderPath, resFileName);
            return resFilePath;
        }

        public string GetHtmlPageResourceUrl(int pageNumber, string resourceFileName)
        {
            var resourceName = $"p{pageNumber}_{resourceFileName}";
            var containerName = ResourcePathConverter.GetContainerName(_userId, _fileFolderName, resourceName);
            var resourceUrl = _urlBase + "v1/resources/" + containerName + "/" + resourceName;
            return resourceUrl;
        }

        public string GetResourceFilePath(string resourceName)
        {
            string resourceFilePath = Path.Combine(_fileCacheFolderPath, resourceName);
            return resourceFilePath;
        }
    }
}
