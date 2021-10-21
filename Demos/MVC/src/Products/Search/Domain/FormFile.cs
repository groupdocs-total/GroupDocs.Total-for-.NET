using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace GroupDocs.Total.MVC.Products.Search.Domain
{
    public class FormFile : IFormFile
    {
        private readonly HttpPostedFile _postedFile;

        public FormFile(HttpPostedFile postedFile)
        {
            _postedFile = postedFile;
        }

        public string FileName => _postedFile.FileName;

        public long Length => _postedFile.ContentLength;

        public Task CopyToAsync(Stream target, CancellationToken cancellationToken)
        {
            return _postedFile.InputStream.CopyToAsync(target);
        }
    }
}
