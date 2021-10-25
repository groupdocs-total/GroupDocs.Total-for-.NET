using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GroupDocs.Total.MVC.Products.Search.Domain.SingleIndex
{
    internal class StorageService
    {
        private readonly Settings _settings;

        public StorageService(Settings settings)
        {
            _settings = settings;
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
            return await Task.Factory.StartNew(() =>
            {
                var folderPath = Path.Combine(_settings.StoragePath, folderName);
                var files = Directory.GetFiles(folderPath);
                return files;
            });
        }

        public string[] GetFileList(string folderName)
        {
            var task = GetFileListAsync(folderName);
            task.Wait();
            return task.Result;
        }

        public async Task UploadFileAsync(string folderName, string fileName, Stream stream)
        {
            await Task.Factory.StartNew(() =>
            {
                var folderPath = Path.Combine(_settings.StoragePath, folderName);
                var filePath = Path.Combine(folderPath, fileName);
                using (var fs = File.Create(filePath))
                {
                    stream.CopyToAsync(fs);
                }
            });
        }

        public void UploadFile(string folderName, string fileName, Stream stream)
        {
            var task = UploadFileAsync(folderName, fileName, stream);
            task.Wait();
        }

        public async Task<Stream> DownloadFileAsync(string folderName, string fileName)
        {
            return await Task.Factory.StartNew(() =>
            {
                var folderPath = Path.Combine(_settings.StoragePath, folderName);
                var filePath = Path.Combine(folderPath, fileName);
                return File.OpenRead(filePath);
            });
        }

        public Stream DownloadFile(string folderName, string fileName)
        {
            var task = DownloadFileAsync(folderName, fileName);
            task.Wait();
            return task.Result;
        }

        public async Task DeleteFileAsync(string folderName, string fileName)
        {
            await Task.Factory.StartNew(() =>
            {
                var folderPath = Path.Combine(_settings.StoragePath, folderName);
                var filePath = Path.Combine(folderPath, fileName);
                File.Delete(filePath);
            });
        }

        public void DeleteFile(string folderName, string fileName)
        {
            var task = DeleteFileAsync(folderName, fileName);
            task.Wait();
        }
    }
}
