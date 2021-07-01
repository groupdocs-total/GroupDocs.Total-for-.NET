using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using GroupDocs.Total.MVC.Products.Common.Entity.Web;
using GroupDocs.Total.MVC.Products.Common.Resources;
using GroupDocs.Total.MVC.Products.Common.Util.Comparator;
using GroupDocs.Total.MVC.Products.Search.Config;
using GroupDocs.Total.MVC.Products.Search.Entity.Web;
using GroupDocs.Total.MVC.Products.Search.Entity.Web.Request;
using GroupDocs.Total.MVC.Products.Search.Service;

namespace GroupDocs.Total.MVC.Products.Search.Controllers
{
    /// <summary>
    /// SearchApiController.
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SearchApiController : ApiController
    {
        private readonly Common.Config.GlobalConfiguration globalConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchApiController"/> class.
        /// Constructor.
        /// </summary>
        public SearchApiController()
        {
            this.globalConfiguration = new Common.Config.GlobalConfiguration();
        }

        /// <summary>
        /// Load Search configuration.
        /// </summary>
        /// <returns>Search configuration.</returns>
        [HttpGet]
        [Route("search/loadConfig")]
        public SearchConfiguration LoadConfig()
        {
            SearchService.InitIndex(this.globalConfiguration);

            return this.globalConfiguration.GetSearchConfiguration();
        }

        /// <summary>
        /// Gets all files and directories from storage.
        /// </summary>
        /// <param name="fileTreeRequest">Posted data with path.</param>
        /// <returns>List of files and directories.</returns>
        [HttpPost]
        [Route("search/loadFileTree")]
        public HttpResponseMessage LoadFileTree(PostedDataEntity fileTreeRequest)
        {
            try
            {
                List<IndexedFileDescriptionEntity> filesList = new List<IndexedFileDescriptionEntity>();

                string filesDirectory = string.IsNullOrEmpty(fileTreeRequest.path) ?
                                        this.globalConfiguration.GetSearchConfiguration().GetFilesDirectory() :
                                        fileTreeRequest.path;

                if (!string.IsNullOrEmpty(this.globalConfiguration.GetSearchConfiguration().GetFilesDirectory()))
                {
                    filesList = this.LoadFiles(filesDirectory);
                }

                return this.Request.CreateResponse(HttpStatusCode.OK, filesList);
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.OK, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Loads documents.
        /// </summary>
        /// <param name="filesDirectory">Files directory.</param>
        /// <returns>List of documents.</returns>
        public List<IndexedFileDescriptionEntity> LoadFiles(string filesDirectory)
        {
            List<string> allFiles = new List<string>(Directory.GetFiles(filesDirectory));
            allFiles.AddRange(Directory.GetDirectories(filesDirectory));
            List<IndexedFileDescriptionEntity> fileList = new List<IndexedFileDescriptionEntity>();

            allFiles.Sort(new FileNameComparator());
            allFiles.Sort(new FileDateComparator());

            foreach (string file in allFiles)
            {
                FileInfo fileInfo = new FileInfo(file);

                if (!(Path.GetFileName(file).StartsWith(".") ||
                      fileInfo.Attributes.HasFlag(FileAttributes.Hidden) ||
                      Path.GetFileName(file).Equals(Path.GetFileName(this.globalConfiguration.GetSearchConfiguration().GetFilesDirectory())) ||
                      Path.GetFileName(file).Equals(Path.GetFileName(this.globalConfiguration.GetSearchConfiguration().GetIndexDirectory())) ||
                      Path.GetFileName(file).Equals(Path.GetFileName(this.globalConfiguration.GetSearchConfiguration().GetIndexedFilesDirectory()))))
                {
                    IndexedFileDescriptionEntity fileDescription = new IndexedFileDescriptionEntity
                    {
                        guid = Path.GetFullPath(file),
                        name = Path.GetFileName(file),

                        // set is directory true/false
                        isDirectory = fileInfo.Attributes.HasFlag(FileAttributes.Directory),
                    };

                    // set file size
                    if (!fileDescription.isDirectory)
                    {
                        fileDescription.size = fileInfo.Length;
                    }

                    if (filesDirectory.Contains(this.globalConfiguration.GetSearchConfiguration().GetIndexedFilesDirectory()))
                    {
                        string value;
                        if (SearchService.FileIndexingStatusDict.TryGetValue(fileDescription.guid, out value))
                        {
                            fileDescription.documentStatus = value;
                        }
                        else
                        {
                            fileDescription.documentStatus = "Indexing";
                        }
                    }

                    fileList.Add(fileDescription);
                }
            }

            return fileList;
        }

        /// <summary>
        /// Uploads document.
        /// </summary>
        /// <returns>Uploaded document object.</returns>
        [HttpPost]
        [Route("search/uploadDocument")]
        public HttpResponseMessage UploadDocument()
        {
            try
            {
                string url = HttpContext.Current.Request.Form["url"];

                // get documents storage path
                string documentStoragePath = this.globalConfiguration.GetSearchConfiguration().GetFilesDirectory();
                bool rewrite = bool.Parse(HttpContext.Current.Request.Form["rewrite"]);
                string fileSavePath = string.Empty;
                if (string.IsNullOrEmpty(url))
                {
                    if (HttpContext.Current.Request.Files.AllKeys != null)
                    {
                        // Get the uploaded document from the Files collection
                        var httpPostedFile = HttpContext.Current.Request.Files["file"];
                        if (httpPostedFile != null)
                        {
                            if (rewrite)
                            {
                                // Get the complete file path
                                fileSavePath = Path.Combine(documentStoragePath, httpPostedFile.FileName);
                            }
                            else
                            {
                                fileSavePath = Resources.GetFreeFileName(documentStoragePath, httpPostedFile.FileName);
                            }

                            // Save the uploaded file to "UploadedFiles" folder
                            httpPostedFile.SaveAs(fileSavePath);
                        }
                    }
                }
                else
                {
                    using (WebClient client = new WebClient())
                    {
                        // get file name from the URL
                        Uri uri = new Uri(url);
                        string fileName = Path.GetFileName(uri.LocalPath);
                        if (rewrite)
                        {
                            // Get the complete file path
                            fileSavePath = Path.Combine(documentStoragePath, fileName);
                        }
                        else
                        {
                            fileSavePath = Resources.GetFreeFileName(documentStoragePath, fileName);
                        }

                        // Download the Web resource and save it into the current filesystem folder.
                        client.DownloadFile(url, fileSavePath);
                    }
                }

                UploadedDocumentEntity uploadedDocument = new UploadedDocumentEntity
                {
                    guid = fileSavePath,
                };

                return this.Request.CreateResponse(HttpStatusCode.OK, uploadedDocument);
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Adds files to index.
        /// </summary>
        /// <param name="postedData">Files array.</param>
        /// <returns>HttpResponseMessage.</returns>
        [HttpPost]
        [Route("search/addFilesToIndex")]
        public HttpResponseMessage AddFilesToIndex(PostedDataEntity[] postedData)
        {
            try
            {
                SearchService.AddFilesToIndex(postedData, this.globalConfiguration);
                return this.Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Performs search.
        /// </summary>
        /// <param name="postedData">Search query.</param>
        /// <returns>Search results.</returns>
        [HttpPost]
        [Route("search/search")]
        public HttpResponseMessage Search(SearchPostedData postedData)
        {
            try
            {
                var result = SearchService.Search(postedData, this.globalConfiguration);
                return this.Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Delete file.
        /// </summary>
        /// <param name="postedData">File info.</param>
        /// <returns>HttpResponseMessage.</returns>
        [HttpPost]
        [Route("search/removeFromIndex")]
        public HttpResponseMessage RemoveFromIndex(PostedDataEntity postedData)
        {
            try
            {
                SearchService.RemoveFileFromIndex(postedData.guid);

                return this.Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Gets documents status.
        /// </summary>
        /// <param name="postedData">Files array.</param>
        /// <returns>Indexed files list with current status.</returns>
        [HttpPost]
        [Route("search/getFileStatus")]
        public HttpResponseMessage GetFileStatus(PostedDataEntity[] postedData)
        {
            var indexingFilesList = new List<IndexedFileDescriptionEntity>();

            foreach (var file in postedData)
            {
                var indexingFile = new IndexedFileDescriptionEntity();

                string value;
                if (SearchService.FileIndexingStatusDict.TryGetValue(file.guid, out value))
                {
                    if (value.Equals("PasswordRequired"))
                    {
                        return this.Request.CreateResponse(HttpStatusCode.Forbidden, new Resources().GenerateException(new Exception("Password required.")));
                    }

                    indexingFile.documentStatus = value;
                }
                else
                {
                    indexingFile.documentStatus = "Indexing";
                }

                indexingFile.guid = file.guid;

                indexingFilesList.Add(indexingFile);
            }

            return this.Request.CreateResponse(HttpStatusCode.OK, indexingFilesList);
        }
    }
}