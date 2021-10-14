using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using GroupDocs.Total.MVC.Products.Common.Entity.Web;
using GroupDocs.Total.MVC.Products.Common.Resources;
using GroupDocs.Total.MVC.Products.Parser.Config;

namespace GroupDocs.Total.MVC.Products.Parser.Services
{
    public class FileService
    {
        private readonly ParserConfiguration parserConfiguration;

        public FileService(ParserConfiguration parserConfiguration)
        {
            this.parserConfiguration = parserConfiguration;
        }

        public IEnumerable<FileDescriptionEntity> LoadFileTree()
        {
            List<FileDescriptionEntity> fileList = new List<FileDescriptionEntity>();
            if (!string.IsNullOrEmpty(parserConfiguration.GetFilesDirectory()))
            {
                var currentPath = parserConfiguration.GetFilesDirectory();
                var inputDirectory = new DirectoryInfo(currentPath);
                var allFiles = inputDirectory.GetFiles();

                const int newFileTimeFrameMin = -10;
                var newFileBorder = DateTime.Now.AddMinutes(newFileTimeFrameMin);
                Array.Sort(allFiles, (x, y) =>
                {
                    if (x.LastAccessTime >= newFileBorder && y.LastAccessTime >= newFileBorder)
                    {
                        return DateTime.Compare(y.LastAccessTime, x.LastAccessTime);
                    }
                    if (x.LastAccessTime < newFileBorder && y.LastAccessTime < newFileBorder)
                    {
                        return string.Compare(x.Name, y.Name, CultureInfo.InvariantCulture, CompareOptions.None);
                    }

                    return x.LastAccessTime >= newFileBorder ? -1 : 1;
                });

                foreach (var file in allFiles)
                {
                    // check if current file/folder is hidden
                    if (!file.Attributes.HasFlag(FileAttributes.Hidden) &&
                        !file.Name.StartsWith(".") &&
                        !file.Attributes.HasFlag(FileAttributes.Directory))
                    {
                        // add object to array list
                        fileList.Add(new FileDescriptionEntity
                        {
                            guid = file.Name,
                            name = file.Name,
                            size = file.Length,
                        });
                    }
                }
            }
            return fileList;
        }

        public UploadedDocumentEntity UploadDocument(HttpRequest request)
        {
            string url = request.Form["url"];
            // get documents storage path
            bool rewrite = bool.Parse(request.Form["rewrite"]);
            string fileName;
            string tempFilePath = parserConfiguration.GetTempFilePath();
            if (string.IsNullOrEmpty(url))
            {
                // Get the uploaded document from the Files collection
                var httpPostedFile = request.Files["file"];
                if (httpPostedFile == null || Path.IsPathRooted(httpPostedFile.FileName))
                {
                    throw new ArgumentException("Could not upload the file");
                }
                fileName = httpPostedFile.FileName;

                // Save the uploaded file to "UploadedFiles" folder
                httpPostedFile.SaveAs(tempFilePath);
            }
            else
            {
                using (WebClient client = new WebClient())
                {
                    // get file name from the URL
                    Uri uri = new Uri(url);
                    fileName = Path.GetFileName(uri.LocalPath);

                    // Download the Web resource and save it into the current filesystem folder.
                    client.DownloadFile(url, tempFilePath);
                }
            }

            try
            {
                if (rewrite)
                {
                    string filePath = parserConfiguration.GetInputFilePath(fileName);
                    RemoveFile(filePath);
                    RemoveFile(parserConfiguration.GetOutputFilePath(fileName));
                    File.Move(tempFilePath, filePath);
                }
                else
                {
                    File.Move(tempFilePath, Resources.GetFreeFileName(parserConfiguration.GetFilesDirectory(), fileName));
                }
            }
            finally
            {
                File.Delete(tempFilePath);
            }

            UploadedDocumentEntity uploadedDocument = new UploadedDocumentEntity();
            uploadedDocument.guid = Path.GetFileName(fileName);

            return uploadedDocument;
        }

        public Stream GetSourceFileStream(string relativePath)
        {
            string filePath = parserConfiguration.GetInputFilePath(relativePath);
            return File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        private void RemoveFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}