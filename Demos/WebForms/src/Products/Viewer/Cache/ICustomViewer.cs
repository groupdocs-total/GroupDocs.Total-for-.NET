﻿using System.IO;

namespace GroupDocs.Total.WebForms.Products.Viewer.Cache
{
    interface ICustomViewer
    {
        GroupDocs.Viewer.Viewer GetViewer();
        Stream GetPdf();
        void CreateCache();
    }
}