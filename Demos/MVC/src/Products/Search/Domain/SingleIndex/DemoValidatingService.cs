using System.Globalization;
using System.IO;

namespace GroupDocs.Total.MVC.Products.Search.Domain.SingleIndex
{
    internal class DemoValidatingService
    {
        private readonly Settings _settings;

        public DemoValidatingService(Settings settings)
        {
            _settings = settings;
        }

        public void CheckFileLength(long fileLength)
        {
            if (fileLength > _settings.MaxFileLength)
            {
                var megabytes = (_settings.MaxFileLength / 1024.0 / 1024.0).ToString("F0", CultureInfo.InvariantCulture);
                throw new DemoException($"In demo mode, files up to {megabytes} MB in size can be uploaded.");
            }
        }

        public void CheckUploadedFiles(string uploadedDirectory)
        {
            var files = Directory.GetFiles(uploadedDirectory, "*.*", SearchOption.TopDirectoryOnly);
            if (files.Length >= _settings.MaxUploadedFiles)
            {
                throw new DemoException($"In demo mode, only {_settings.MaxUploadedFiles} files can be uploaded.");
            }
        }
    }
}
