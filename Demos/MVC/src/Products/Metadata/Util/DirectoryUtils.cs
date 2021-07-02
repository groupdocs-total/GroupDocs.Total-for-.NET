using System.IO;

namespace GroupDocs.Total.MVC.Products.Metadata.Util
{
    public static class DirectoryUtils
    {
        public static void MoveFile(string source, string destination)
        {
            if (File.Exists(destination))
            {
                File.Delete(destination);
            }

            File.Move(source, destination);
        }
    }
}