using System.IO;

namespace GroupDocs.Total.MVC.Products.Viewer.Cache
{
    interface ICustomViewer
    {
        GroupDocs.Viewer.Viewer GetViewer();

        Stream GetPdf();

        void CreateCache();
    }
}