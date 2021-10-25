using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GroupDocs.Total.MVC.Products.Search.Domain
{
    public interface IFormFile
    {
        string FileName { get; }

        long Length { get; }

        Task CopyToAsync(Stream target, CancellationToken cancellationToken);
    }
}
