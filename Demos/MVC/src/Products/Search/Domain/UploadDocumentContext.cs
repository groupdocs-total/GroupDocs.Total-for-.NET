using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GroupDocs.Total.MVC.Products.Search.Domain
{
    public class UploadDocumentContext
    {
        private readonly IFormFile _file;

        public long FileLength => _file.Length;

        public string FileName => _file.FileName;

        public string Url { get; private set; }

        public string FolderName { get; private set; }

        public bool IndexAfterUpload { get; private set; }

        public bool RecognizeTextInImages { get; private set; }

        public UploadDocumentContext(
            IFormFile file,
            string url,
            string folderName,
            bool indexAfterUpload,
            bool recognizeTextInImages)
        {
            _file = file;
            Url = url;
            FolderName = folderName;
            IndexAfterUpload = indexAfterUpload;
            RecognizeTextInImages = recognizeTextInImages;
        }

        public async Task FileCopyToAsync(Stream target)
        {
            await _file.CopyToAsync(target, CancellationToken.None);
        }
    }
}
