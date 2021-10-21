using GroupDocs.Apps.Common.Contracts;
using GroupDocs.Apps.Common.DTO;
using GroupDocs.Apps.Common.DTO.Request;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GroupDocs.Total.MVC.Products.Search.Domain.SingleIndex
{
    internal class StorageService
    {
        private readonly IStorageApiConnect _storageApiConnect;

        public StorageService(IStorageApiConnect storageApiConnect)
        {
            _storageApiConnect = storageApiConnect;
        }

        public async Task<bool> FileExistsAsync(string folderName, string fileName)
        {
            var list = await GetFileListAsync(folderName);
            return list.Contains(fileName);
        }

        public bool FileExists(string folderName, string fileName)
        {
            var task = FileExistsAsync(folderName, fileName);
            task.Wait();
            return task.Result;
        }

        public async Task<string[]> GetFileListAsync(string folderName)
        {
            var request = new GetAllFilesRequest(folderName, StorageTypes.Source, false);
            var response = await _storageApiConnect.GetAllFiles(request);
            var fileList = response.Files
                .Select(file => file.FileName)
                .ToArray();
            return fileList;
        }

        public string[] GetFileList(string folderName)
        {
            var task = GetFileListAsync(folderName);
            task.Wait();
            return task.Result;
        }

        public async Task UploadFileAsync(string folderName, string fileName, Stream stream)
        {
            var files = new FileContent[]
            {
                new FileContent(fileName, stream),
            };
            var request = new FileUploadRequest(files, folderName, StorageTypes.Source);
            await _storageApiConnect.Upload(request);
        }

        public void UploadFile(string folderName, string fileName, Stream stream)
        {
            var task = UploadFileAsync(folderName, fileName, stream);
            task.Wait();
        }

        public async Task<Stream> DownloadFileAsync(string folderName, string fileName)
        {
            var request = new FileDownloadRequest(fileName, folderName, StorageTypes.Source);
            var response = await _storageApiConnect.DownloadFile(request);
            return response;
        }

        public Stream DownloadFile(string folderName, string fileName)
        {
            var task = DownloadFileAsync(folderName, fileName);
            task.Wait();
            return task.Result;
        }

        public async Task DeleteFileAsync(string folderName, string fileName)
        {
            var request = new FileDeleteRequest(fileName, folderName, StorageTypes.Source);
            await _storageApiConnect.Delete(request);
        }

        public void DeleteFile(string folderName, string fileName)
        {
            var task = DeleteFileAsync(folderName, fileName);
            task.Wait();
        }
    }
}
