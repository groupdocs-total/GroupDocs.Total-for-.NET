using GroupDocs.Total.MVC.Products.Common.Entity.Web;
using GroupDocs.Total.MVC.Products.Common.Resources;
using GroupDocs.Total.MVC.Products.Search.Domain;
using GroupDocs.Total.MVC.Products.Search.Dto;
using GroupDocs.Total.MVC.Products.Search.Dto.Request;
using GroupDocs.Total.MVC.Products.Search.Dto.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace GroupDocs.Total.MVC.Products.Search.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SearchApiController : ApiController
    {
        private readonly ILogger _logger;
        private readonly ISearchService _searchService;

        public SearchApiController(
            ILogger logger,
            ISearchService searchService)
        {
            _logger = logger;
            _searchService = searchService;
        }

        [HttpGet]
        [Route("search/loadConfig")]
        public SearchConfiguration LoadConfig()
        {
            var configuration = _searchService.GetConfiguration();
            return configuration;
        }

        [HttpPost]
        [Route("search/getUploadedFiles")]
        public async Task<IndexedFileDescriptionEntity[]> GetUploadedFiles(SearchBaseRequest request)
        {
            var indexedFiles = await _searchService.GetUploadedFiles(request);
            return indexedFiles;
        }

        [HttpPost]
        [Route("search/getIndexedFiles")]
        public IndexedFileDescriptionEntity[] GetIndexedFiles(SearchBaseRequest request)
        {
            var indexedFiles = _searchService.GetIndexedFiles(request);
            return indexedFiles;
        }

        [HttpPost]
        [Route("search/uploadDocument")]
        public async Task<HttpResponseMessage> UploadDocumentAsync()
        {
            try
            {
                string url = HttpContext.Current.Request.Form["url"];
                string folderName = HttpContext.Current.Request.Form["folderName"];
                string indexAfterUploadString = HttpContext.Current.Request.Form["indexAfterUpload"];
                string recognizeTextInImagesString = HttpContext.Current.Request.Form["recognizeTextInImages"];
                bool indexAfterUpload = string.Equals(indexAfterUploadString, "true", StringComparison.OrdinalIgnoreCase);
                bool recognizeTextInImages = string.Equals(recognizeTextInImagesString, "true", StringComparison.OrdinalIgnoreCase);
                var httpPostedFile = HttpContext.Current.Request.Files["file"];
                IFormFile file = new FormFile(httpPostedFile);
                var context = new UploadDocumentContext(file, url, folderName, indexAfterUpload, recognizeTextInImages);
                var uploadedDocument = await _searchService.UploadDocumentAsync(context);
                return this.Request.CreateResponse(HttpStatusCode.OK, uploadedDocument);
            }
            catch (DemoException ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.OK, LicenseRestrictionResponse.CreateRestricted(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Upload document error.");
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        [HttpPost]
        [Route("search/deleteFiles")]
        public HttpResponseMessage DeleteFiles(FilesDeleteRequest request)
        {
            try
            {
                _searchService.DeleteFiles(request);
                return this.Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete files error.");
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        [HttpPost]
        [Route("search/addFilesToIndex")]
        public async Task<HttpResponseMessage> AddFilesToIndexAsync(AddToIndexRequest request)
        {
            try
            {
                await _searchService.AddFilesToIndexAsync(request);
                return this.Request.CreateResponse(HttpStatusCode.OK, LicenseRestrictionResponse.CreateNonRestricted());
            }
            catch (DemoException ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.OK, LicenseRestrictionResponse.CreateRestricted(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Add files to index error.");
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        [HttpPost]
        [Route("search/downloadAndAddToIndex")]
        public async Task<HttpResponseMessage> DownloadAndAddToIndexAsync(AddToIndexRequest request)
        {
            try
            {
                await _searchService.DownloadAndAddToIndexAsync(request);
                return this.Request.CreateResponse(HttpStatusCode.OK, LicenseRestrictionResponse.CreateNonRestricted());
            }
            catch (DemoException ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.OK, LicenseRestrictionResponse.CreateRestricted(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Download and add to index error.");
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        [HttpPost]
        [Route("search/search")]
        public HttpResponseMessage Search(SearchPostedData postedData)
        {
            try
            {
                var result = _searchService.Search(postedData);
                return this.Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        [HttpPost]
        [Route("search/highlight")]
        public HttpResponseMessage Highlight(HighlightRequest request)
        {
            try
            {
                var result = _searchService.Highlight(request);
                return this.Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Highlight error.");
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        [HttpPost]
        [Route("search/downloadSourceFile")]
        public HttpResponseMessage DownloadSourceFile(HighlightRequest request)
        {
            try
            {
                var stream = _searchService.GetSourceFile(request, out string fileName);
                var contentType = "application/octet-stream";
                return File(stream, contentType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Download source file error.");
                return StatusCode(StatusCodes.Status500InternalServerError);
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