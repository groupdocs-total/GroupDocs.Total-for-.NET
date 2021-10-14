using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using GroupDocs.Parser.Exceptions;
using GroupDocs.Total.WebForms.Products.Common.Entity.Web;
using GroupDocs.Total.WebForms.Products.Common.Resources;
using GroupDocs.Total.WebForms.Products.Parser.Config;
using GroupDocs.Total.WebForms.Products.Parser.Entity;
using GroupDocs.Total.WebForms.Products.Parser.Services;

namespace GroupDocs.Total.WebForms.Products.Parser.Controllers
{
    /// <summary>
    /// ParserApiController
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ParserApiController : ApiController
    {
        private readonly Common.Config.GlobalConfiguration globalConfiguration;
        private readonly ParserService parserService;
        private readonly PreviewService previewService;
        private readonly FileService fileService;

        public ParserApiController()
        {
            globalConfiguration = new Common.Config.GlobalConfiguration();

            fileService = new FileService(globalConfiguration.GetParserConfiguration());
            previewService = new PreviewService(globalConfiguration.GetParserConfiguration(), fileService);
            parserService = new ParserService(globalConfiguration.GetParserConfiguration(), fileService);
        }

        /// <summary>
        /// Load Parser configuration
        /// </summary>
        /// <returns>Parser configuration</returns>
        [HttpGet]
        [Route("parser/loadConfig")]
        public ParserConfiguration LoadConfig()
        {
            return globalConfiguration.GetParserConfiguration();
        }

        /// <summary>
        /// Get all files and directories from storage
        /// </summary>
        /// <returns>List of files and directories</returns>
        [HttpPost]
        [Route("parser/loadFileTree")]
        public HttpResponseMessage LoadFileTree()
        {
            //if (!globalConfiguration.GetParserConfiguration().browse)
            //{
            //    return Request.CreateResponse(HttpStatusCode.NotFound);
            //}
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, fileService.LoadFileTree());
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Upload document
        /// </summary>
        /// <returns>Uploaded document object</returns>
        [HttpPost]
        [Route("parser/uploadDocument")]
        public HttpResponseMessage UploadDocument()
        {
            //if (!globalConfiguration.GetParserConfiguration().upload)
            //{
            //    return Request.CreateResponse(HttpStatusCode.NotFound);
            //}
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, fileService.UploadDocument(HttpContext.Current.Request));
            }
            catch (Exception ex)
            {
                // set exception message
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Load document description
        /// </summary>
        /// <param name="postedData">Post data</param>
        /// <returns>Document info object</returns>
        [HttpPost]
        [Route("parser/loadDocumentDescription")]
        public HttpResponseMessage LoadDocumentDescription(PostedDataEntity postedData)
        {
            try
            {
                // return document description
                return Request.CreateResponse(HttpStatusCode.OK, previewService.LoadDocument(postedData));
            }
            catch (InvalidPasswordException ex)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, new Resources().GenerateException(ex));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Parse document
        /// </summary>
        /// <param name="fileDataRequest">Post data</param>
        /// <returns>Extracted document data</returns>
        [HttpPost]
        [Route("parser/parse")]
        public HttpResponseMessage ParseByTemplate(ParsePostedDataEntity postedData)
        {
            try
            {
                // return document description
                return Request.CreateResponse(HttpStatusCode.OK, parserService.Parse(postedData));
            }
            catch (InvalidPasswordException ex)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, new Resources().GenerateException(ex));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

    }
}