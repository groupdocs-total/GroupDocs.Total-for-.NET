using System;
using System.IO;
using System.Linq;

namespace GroupDocs.Total.MVC.Products.Search.Domain.SingleIndex
{
    internal class UserFolder
    {
        private readonly string _fullPath;
        private readonly string _name;
        private readonly Guid _guid;

        private UserFolder(string fullPath, string name, Guid guid)
        {
            _fullPath = fullPath;
            _name = name;
            _guid = guid;
        }

        public string FullPath => _fullPath;

        public string Name => _name;

        public Guid Guid => _guid;

        public DocumentDescriptor[] GetDocuments(string uploadedDirectoryName)
        {
            var uploadedDirectoryPath = Path.Combine(_fullPath, uploadedDirectoryName);
            var filePaths = Directory.GetFiles(uploadedDirectoryPath, "*.*", SearchOption.TopDirectoryOnly);
            var descriptors = filePaths
                .Select(filePath =>
                {
                    var fileName = Path.GetFileName(filePath);
                    return new DocumentDescriptor(_name, fileName, filePath);
                })
                .ToArray();
            return descriptors;
        }

        public static bool TryCreate(string path, out UserFolder userFolder)
        {
            var name = Path.GetFileName(path);
            if (Guid.TryParse(name, out Guid guid))
            {
                userFolder = new UserFolder(path, name, guid);
                return true;
            }

            userFolder = null;
            return false;
        }
    }
}
