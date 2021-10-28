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
            var list = GetFileList(folderName);
            bool exists = list
                .Select(path => Path.GetFileName(path))
                .Contains(fileName);
            return exists;
        }

        public async Task<string[]> GetFileListAsync(string folderName)
        {
            return await Task.Factory.StartNew(() => GetFileList(folderName));
        }

        public string[] GetFileList(string folderName)
        {
            var folderPath = Path.Combine(_settings.DedicatedStoragePath, folderName);

            Directory.CreateDirectory(folderPath);

            var files = Directory.GetFiles(folderPath);
            return files;
        }

        public async Task UploadFileAsync(string folderName, string fileName, Stream stream)
        {
            await Task.Factory.StartNew(() => UploadFile(folderName, fileName, stream));
        }

        public void UploadFile(string folderName, string fileName, Stream stream)
        {
            var folderPath = Path.Combine(_settings.DedicatedStoragePath, folderName);

            Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);
            using (var fs = File.Create(filePath))
            {
                stream.CopyTo(fs);
            }
        }

        public async Task<Stream> DownloadFileAsync(string folderName, string fileName)
        {
            return await Task.Factory.StartNew(() => DownloadFile(folderName, fileName));
        }

        public Stream DownloadFile(string folderName, string fileName)
        {
            var folderPath = Path.Combine(_settings.DedicatedStoragePath, folderName);

            Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);
            return File.OpenRead(filePath);
        }

        public async Task DeleteFileAsync(string folderName, string fileName)
        {
            await Task.Factory.StartNew(() => DeleteFile(folderName, fileName));
        }

        public void DeleteFile(string folderName, string fileName)
        {
            var folderPath = Path.Combine(_settings.DedicatedStoragePath, folderName);
            var filePath = Path.Combine(folderPath, fileName);
            File.Delete(filePath);
        }
    }
}
