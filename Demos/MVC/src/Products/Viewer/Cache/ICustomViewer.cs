namespace GroupDocs.Total.MVC.Products.Viewer.Cache
{
    interface ICustomViewer
    {
        GroupDocs.Viewer.Viewer GetViewer();

        void CreateCache();
    }
}