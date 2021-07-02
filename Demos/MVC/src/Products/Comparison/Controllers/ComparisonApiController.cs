using GroupDocs.Comparison.Common.Exceptions;
using GroupDocs.Total.MVC.Products.Common.Entity.Web;
using GroupDocs.Total.MVC.Products.Common.Resources;
using GroupDocs.Total.MVC.Products.Common.Util.LowercaseContractResolver;
using GroupDocs.Total.MVC.Products.Comparison.Config;
using GroupDocs.Total.MVC.Products.Comparison.Model.Request;
using GroupDocs.Total.MVC.Products.Comparison.Model.Response;
using GroupDocs.Total.MVC.Products.Comparison.Service;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace GroupDocs.Total.MVC.Products.Comparison.Controllers
{
    /// <summary>
    /// SignatureApiController
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ComparisonApiController : ApiController
    {
        private readonly IComparisonService comparisonService;
        private static readonly Common.Config.GlobalConfiguration globalConfiguration = new Common.Config.GlobalConfiguration();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="globalConfiguration">GlobalConfiguration</param>
        public ComparisonApiController()
        {
            comparisonService = new ComparisonServiceImpl(globalConfiguration);
        }

        /// <summary>
        /// Load Comparison configuration
        /// </summary>
        /// <returns>Comparison configuration</returns>
        [HttpGet]
        [Route("comparison/loadConfig")]
        public ComparisonConfiguration LoadConfig()
        {
            return globalConfiguration.GetComparisonConfiguration();
        }

        /// <summary>
        /// Get all files and directories from storage
        /// </summary>
        /// <param name="postedData">Post data</param>
        /// <returns>List of files and directories</returns>
        [HttpPost]
        [Route("comparison/loadFileTree")]
        public HttpResponseMessage loadFileTree(PostedDataEntity fileTreeRequest)
        {
            return Request.CreateResponse(HttpStatusCode.OK, comparisonService.LoadFiles(fileTreeRequest));
        }

        /// <summary>
        /// Download results
        /// </summary>
        /// <param name=""></param>
        [HttpGet]
        [Route("comparison/downloadDocument")]
        public HttpResponseMessage DownloadDocument(string guid)
        {
            string filePath = guid;
            if (!string.IsNullOrEmpty(filePath))
            {
                if (File.Exists(filePath))
                {
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    var fileStream = new FileStream(filePath, FileMode.Open);
                    response.Content = new StreamContent(fileStream);
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    response.Content.Headers.ContentDisposition.FileName = Path.GetFileName(filePath);
                    return response;
                }
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        /// <summary>
        /// Upload document
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        [Route("comparison/uploadDocument")]
        public HttpResponseMessage UploadDocument()
        {
            try
            {
                string url = HttpContext.Current.Request.Form["url"];
                // get documents storage path
                string documentStoragePath = globalConfiguration.GetComparisonConfiguration().GetFilesDirectory();
                bool rewrite = bool.Parse(HttpContext.Current.Request.Form["rewrite"]);
                string fileSavePath = "";
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
                UploadedDocumentEntity uploadedDocument = new UploadedDocumentEntity();
                uploadedDocument.guid = fileSavePath;
                return Request.CreateResponse(HttpStatusCode.OK, uploadedDocument);
            }
            catch (System.Exception ex)
            {
                // set exception message
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Compare files from local storage
        /// </summary>
        /// <param name="compareRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("comparison/compare")]
        public HttpResponseMessage Compare(CompareRequest compareRequest)
        {
            try
            {
                // check formats
                if (comparisonService.CheckFiles(compareRequest))
                {
                    // compare
                    CompareResultResponse result = comparisonService.Compare(compareRequest);
                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.ContractResolver = new LowercaseContractResolver();
                    string json = JsonConvert.SerializeObject(result, Formatting.Indented, settings);
                    var compareResult = JsonConvert.DeserializeObject(json);
                    return Request.CreateResponse(HttpStatusCode.OK, compareResult);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new Resources().GenerateException(new Exception("Document types are different")));
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Get result page
        /// </summary>
        /// <param name="loadResultPageRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("comparison/loadDocumentDescription")]
        public HttpResponseMessage LoadDocumentDescription(PostedDataEntity loadResultPageRequest)
        {
            try
            {
                LoadDocumentEntity document = ComparisonServiceImpl.LoadDocumentPages(loadResultPageRequest.guid,
                                                                                  loadResultPageRequest.password,
                                                                                  globalConfiguration.GetComparisonConfiguration().GetPreloadResultPageCount() == 0);
                return Request.CreateResponse(HttpStatusCode.OK, document);
            }
            catch (PasswordProtectedFileException ex)
            {
                // set exception message
                return Request.CreateResponse(HttpStatusCode.Forbidden, new Resources().GenerateException(ex, loadResultPageRequest.password));
            }
            catch (Exception ex)
            {
                // set exception message
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex, loadResultPageRequest.password));
            }
        }

        /// <summary>
        /// Get document page
        /// </summary>
        /// <param name="postedData">Post data</param>
        /// <returns>Document page object</returns>
        [HttpPost]
        [Route("comparison/loadDocumentPage")]
        public HttpResponseMessage LoadDocumentPage(PostedDataEntity postedData)
        {
            return Request.CreateResponse(HttpStatusCode.OK, comparisonService.LoadDocumentPage(postedData));
        }
    }
}