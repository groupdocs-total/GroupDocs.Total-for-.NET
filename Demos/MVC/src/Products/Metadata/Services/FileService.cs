using GroupDocs.Total.MVC.Products.Common.Entity.Web;
using GroupDocs.Total.MVC.Products.Common.Resources;
using GroupDocs.Total.MVC.Products.Metadata.Config;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Web;

namespace GroupDocs.Total.MVC.Products.Metadata.Services
{
    public class FileService
    {
        private readonly MetadataConfiguration metadataConfiguration;

        public FileService(MetadataConfiguration metadataConfiguration)
        {
            this.metadataConfiguration = metadataConfiguration;
        }

        public IEnumerable<FileDescriptionEntity> LoadFileTree()
        {
            List<FileDescriptionEntity> fileList = new List<FileDescriptionEntity>();
            if (!string.IsNullOrEmpty(metadataConfiguration.GetFilesDirectory()))
            {
                var currentPath = metadataConfiguration.GetFilesDirectory();
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
            string tempFilePath = metadataConfiguration.GetTempFilePath();
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
                    string filePath = metadataConfiguration.GetInputFilePath(fileName);
                    RemoveFile(filePath);
                    RemoveFile(metadataConfiguration.GetOutputFilePath(fileName));
                    File.Move(tempFilePath, filePath);
                }
                else
                {
                    File.Move(tempFilePath, Resources.GetFreeFileName(metadataConfiguration.GetFilesDirectory(), fileName));
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
            string filePath = metadataConfiguration.GetInputFilePath(relativePath);
            return File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public Stream GetFileStream(string relativePath)
        {
            return GetFileStream(relativePath, true);
        }

        public Stream GetFileStream(string relativePath, bool readOnly)
        {
            var inputPath = metadataConfiguration.GetInputFilePath(relativePath);
            var outputPath = metadataConfiguration.GetOutputFilePath(relativePath);
            if (File.Exists(outputPath))
            {
                return File.Open(outputPath, FileMode.Open, FileAccess.ReadWrite);
            }
            if (!readOnly)
            {
                using (var sourceStream = File.Open(inputPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var destStream = File.Open(outputPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    sourceStream.CopyTo(destStream);
                    destStream.Position = 0;
                    return destStream;
                }
            }
            return File.Open(inputPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        private void RemoveFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                int maxReties = metadataConfiguration.GetFileOperationRetryCount();
                int timeout = metadataConfiguration.GetFileOperationTimeout();
                for (int i = 1; i <= maxReties; i++)
                {
                    try
                    {
                        File.Delete(filePath);
                        break;
                    }
                    catch (IOException)
                    {
                        if (i < maxReties)
                        {
                            Thread.Sleep(timeout);
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
        }
    }
}