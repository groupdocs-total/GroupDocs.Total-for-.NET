using GroupDocs.Total.MVC.Products.Common.Resources;
using GroupDocs.Total.MVC.Products.Search.Domain;
using GroupDocs.Total.MVC.Products.Search.Dto;
using GroupDocs.Total.MVC.Products.Search.Dto.Request;
using GroupDocs.Total.MVC.Products.Search.Dto.Response;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
                var response = new HttpResponseMessage();
                response.Content = new StringContent(result);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                return response;
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
                string fileName;
                var stream = _searchService.GetSourceFile(request, out fileName);
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(stream);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = fileName;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Download source file error.");
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        [HttpPost]
        [Route("search/downloadExtractedText")]
        public HttpResponseMessage DownloadExtractedText(HighlightRequest request)
        {
            try
            {
                string fileName;
                var stream = _searchService.GetExtractedText(request, out fileName);
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(stream);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = fileName;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Download extracted text error.");
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        [HttpPost]
        [Route("search/prepareDocument")]
        public HttpResponseMessage PrepareDocument(PrepareDocumentRequest request)
        {
            try
            {
                var response = _searchService.PrepareDocument(request);
                return this.Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Prepare document error.");
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        [HttpPost]
        [Route("search/getDocumentPage")]
        public HttpResponseMessage GetDocumentPage(GetDocumentPageRequest request)
        {
            try
            {
                var response = _searchService.GetDocumentPage(request);
                return this.Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get document page error.");
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        [HttpGet]
        [Route("search/resources/{containerName}/{resourceName}")]
        public HttpResponseMessage GetResource(string containerName, string resourceName)
        {
            try
            {
                string contentType;
                var stream = _searchService.GetResource(containerName, resourceName, out contentType);
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(stream);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get resource error.");
                return this.Request.CreateResponse(HttpStatusCode.NotFound, new Resources().GenerateException(ex));
            }
        }

        [HttpGet]
        [Route("search/getReport")]
        public HttpResponseMessage GetReport(string id)
        {
            var stream = _searchService.GetAppInfo(id);
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(stream);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = "Report.txt";
            return response;
        }

        [HttpPost]
        [Route("search/requestReindex")]
        public HttpResponseMessage RequestReindex(SearchBaseRequest request)
        {
            try
            {
                _searchService.RequestReindex(request);
                return this.Request.CreateResponse(HttpStatusCode.OK, LicenseRestrictionResponse.CreateNonRestricted());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Request re-index error.");
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        [HttpPost]
        [Route("search/removeFromIndex")]
        public HttpResponseMessage RemoveFromIndex(Dto.PostedDataEntity postedData)
        {
            try
            {
                _searchService.RemoveFileFromIndex(postedData);
                return this.Request.CreateResponse(HttpStatusCode.OK, LicenseRestrictionResponse.CreateNonRestricted());
            }
            catch (DemoException ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.OK, LicenseRestrictionResponse.CreateRestricted(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Remove from index error.");
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        [HttpPost]
        [Route("search/getFileStatus")]
        public HttpResponseMessage GetFileStatus(FileStatusGetRequest request)
        {
            try
            {
                var result = _searchService.GetFileStatus(request);
                return this.Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get file status error.");
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        [HttpPost]
        [Route("search/getStatus")]
        public HttpResponseMessage GetStatus(SearchBaseRequest request)
        {
            try
            {
                var response = _searchService.GetStatus(request);
                return this.Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get indexing status error.");
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        [HttpPost]
        [Route("search/getIndexProperties")]
        public HttpResponseMessage GetIndexProperties(SearchBaseRequest request)
        {
            try
            {
                var response = _searchService.GetIndexProperties(request);
                return this.Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get index properties error.");
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        [HttpPost]
        [Route("search/getInfo")]
        public HttpResponseMessage GetInfo(SearchBaseRequest request)
        {
            try
            {
                var response = _searchService.GetInfo(request);
                return this.Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get info error.");
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        [HttpPost]
        [Route("search/getAlphabetDictionary")]
        public HttpResponseMessage GetAlphabetDictionary(SearchBaseRequest request)
        {
            try
            {
                var response = _searchService.GetAlphabetDictionary(request);
                return this.Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get alphabet dictionary error.");
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        [HttpPost]
        [Route("search/setAlphabetDictionary")]
        public HttpResponseMessage SetAlphabetDictionary(AlphabetUpdateRequest request)
        {
            try
            {
                _searchService.SetAlphabetDictionary(request);
                return this.Request.CreateResponse(HttpStatusCode.OK, LicenseRestrictionResponse.CreateNonRestricted());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Set alphabet dictionary error.");
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        [HttpPost]
        [Route("search/getStopWordDictionary")]
        public HttpResponseMessage GetStopWordDictionary(SearchBaseRequest request)
        {
            try
            {
                var response = _searchService.GetStopWordDictionary(request);
                return this.Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get stop word dictionary error.");
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        [HttpPost]
        [Route("search/setStopWordDictionary")]
        public HttpResponseMessage SetStopWordDictionary(StopWordsUpdateRequest request)
        {
            try
            {
                _searchService.SetStopWordDictionary(request);
                return this.Request.CreateResponse(HttpStatusCode.OK, LicenseRestrictionResponse.CreateNonRestricted());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Set stop word dictionary error.");
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        [HttpPost]
        [Route("search/getSynonymDictionary")]
        public HttpResponseMessage GetSynonymDictionary(SearchBaseRequest request)
        {
            try
            {
                var response = _searchService.GetSynonymGroups(request);
                return this.Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get synonym dictionary error.");
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        [HttpPost]
        [Route("search/setSynonymDictionary")]
        public HttpResponseMessage SetSynonymDictionary(SynonymsUpdateRequest request)
        {
            try
            {
                _searchService.SetSynonymGroups(request);
                return this.Request.CreateResponse(HttpStatusCode.OK, LicenseRestrictionResponse.CreateNonRestricted());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Set synonym dictionary error.");
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        [HttpPost]
        [Route("search/getHomophoneDictionary")]
        public HttpResponseMessage GetHomophoneDictionary(SearchBaseRequest request)
        {
            try
            {
                var response = _searchService.GetHomophoneGroups(request);
                return this.Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get homophone dictionary error.");
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        [HttpPost]
        [Route("search/setHomophoneDictionary")]
        public HttpResponseMessage SetHomophoneDictionary(HomophonesUpdateRequest request)
        {
            try
            {
                _searchService.SetHomophoneGroups(request);
                return this.Request.CreateResponse(HttpStatusCode.OK, LicenseRestrictionResponse.CreateNonRestricted());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Set homophone dictionary error.");
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        [HttpPost]
        [Route("search/getSpellingCorrectorDictionary")]
        public HttpResponseMessage GetSpellingCorrectorDictionary(SearchBaseRequest request)
        {
            try
            {
                var response = _searchService.GetSpellingCorrectorWords(request);
                return this.Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get spelling corrector dictionary error.");
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        [HttpPost]
        [Route("search/setSpellingCorrectorDictionary")]
        public HttpResponseMessage SetSpellingCorrectorDictionary(SpellingCorrectorUpdateRequest request)
        {
            try
            {
                _searchService.SetSpellingCorrectorWords(request);
                return this.Request.CreateResponse(HttpStatusCode.OK, LicenseRestrictionResponse.CreateNonRestricted());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Set spelling corrector dictionary error.");
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        [HttpPost]
        [Route("search/getCharacterReplacementDictionary")]
        public HttpResponseMessage GetCharacterReplacementDictionary(SearchBaseRequest request)
        {
            try
            {
                var response = _searchService.GetCharacterReplacements(request);
                return this.Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get character replacement dictionary error.");
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        [HttpPost]
        [Route("search/setCharacterReplacementDictionary")]
        public HttpResponseMessage SetCharacterReplacementDictionary(CharacterReplacementsUpdateRequest request)
        {
            try
            {
                _searchService.SetCharacterReplacements(request);
                return this.Request.CreateResponse(HttpStatusCode.OK, LicenseRestrictionResponse.CreateNonRestricted());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Set character replacement dictionary error.");
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }
    }
}
