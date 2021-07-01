using GroupDocs.Annotation;
using GroupDocs.Annotation.Exceptions;
using GroupDocs.Annotation.Models;
using GroupDocs.Annotation.Models.AnnotationModels;
using GroupDocs.Annotation.Options;
using GroupDocs.Total.MVC.Products.Annotation.Annotator;
using GroupDocs.Total.MVC.Products.Annotation.Config;
using GroupDocs.Total.MVC.Products.Annotation.Entity.Web;
using GroupDocs.Total.MVC.Products.Annotation.Util;
using GroupDocs.Total.MVC.Products.Common.Entity.Web;
using GroupDocs.Total.MVC.Products.Common.Resources;
using GroupDocs.Total.MVC.Products.Common.Util.Comparator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace GroupDocs.Total.MVC.Products.Annotation.Controllers
{
    /// <summary>
    /// SignatureApiController
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AnnotationApiController : ApiController
    {
        private static Common.Config.GlobalConfiguration globalConfiguration = new Common.Config.GlobalConfiguration();
        private readonly List<string> SupportedImageFormats = new List<string> { ".bmp", ".jpeg", ".jpg", ".tiff", ".tif", ".png", ".dwg", ".dcm", ".dxf" };

        /// <summary>
        /// Load Annotation configuration
        /// </summary>
        /// <returns>Annotation configuration</returns>
        [HttpGet]
        [Route("annotation/loadConfig")]
        public AnnotationConfiguration LoadConfig()
        {
            return globalConfiguration.GetAnnotationConfiguration();
        }

        /// <summary>
        /// Get all files and directories from storage
        /// </summary>
        /// <param name="postedData">SignaturePostedDataEntity</param>
        /// <returns>List of files and directories</returns>
        [HttpPost]
        [Route("annotation/loadFileTree")]
        public HttpResponseMessage loadFileTree(PostedDataEntity postedData)
        {
            // get request body
            string relDirPath = postedData.path;
            // get file list from storage path
            try
            {
                // get all the files from a directory
                if (string.IsNullOrEmpty(relDirPath))
                {
                    relDirPath = globalConfiguration.GetAnnotationConfiguration().GetFilesDirectory();
                }
                else
                {
                    relDirPath = Path.Combine(globalConfiguration.GetAnnotationConfiguration().GetFilesDirectory(), relDirPath);
                }

                List<FileDescriptionEntity> fileList = new List<FileDescriptionEntity>();
                List<string> allFiles = new List<string>(Directory.GetFiles(relDirPath));
                allFiles.AddRange(Directory.GetDirectories(relDirPath));

                allFiles.Sort(new FileNameComparator());
                allFiles.Sort(new FileTypeComparator());

                foreach (string file in allFiles)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    // check if current file/folder is hidden
                    if (!(fileInfo.Attributes.HasFlag(FileAttributes.Hidden) ||
                        Path.GetFileName(file).Equals(Path.GetFileName(globalConfiguration.GetAnnotationConfiguration().GetFilesDirectory())) ||
                        Path.GetFileName(file).StartsWith(".")))
                    {
                        FileDescriptionEntity fileDescription = new FileDescriptionEntity();
                        fileDescription.guid = Path.GetFullPath(file);
                        fileDescription.name = Path.GetFileName(file);
                        // set is directory true/false
                        fileDescription.isDirectory = fileInfo.Attributes.HasFlag(FileAttributes.Directory);
                        // set file size
                        if (!fileDescription.isDirectory)
                        {
                            fileDescription.size = fileInfo.Length;
                        }
                        // add object to array list
                        fileList.Add(fileDescription);
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, fileList);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Load document description
        /// </summary>
        /// <param name="postedData">Post data</param>
        /// <returns>Document info object</returns>
        [HttpPost]
        [Route("annotation/loadDocumentDescription")]
        public HttpResponseMessage LoadDocumentDescription(AnnotationPostedDataEntity postedData)
        {
            string password = "";
            try
            {
                AnnotatedDocumentEntity loadDocumentEntity = LoadDocument(postedData, globalConfiguration.GetAnnotationConfiguration().GetPreloadPageCount() == 0);
                // return document description
                return Request.CreateResponse(HttpStatusCode.OK, loadDocumentEntity);
            }
            catch (Exception ex)
            {
                // set exception message
                // TODO: return InternalServerError for common Exception and Forbidden for PasswordProtectedException
                return Request.CreateResponse(HttpStatusCode.Forbidden, new Resources().GenerateException(ex, password));
            }
        }

        public AnnotatedDocumentEntity LoadDocument(AnnotationPostedDataEntity loadDocumentRequest, bool loadAllPages)
        {
            string password = loadDocumentRequest.password;
            AnnotatedDocumentEntity description = new AnnotatedDocumentEntity();
            string documentGuid = loadDocumentRequest.guid;

            using (GroupDocs.Annotation.Annotator annotator = new GroupDocs.Annotation.Annotator(documentGuid, GetLoadOptions(password)))
            {
                IDocumentInfo info = annotator.Document.GetDocumentInfo();
                AnnotationBase[] annotations = annotator.Get().ToArray();
                description.guid = loadDocumentRequest.guid;
                string documentType = getDocumentType(info);

                description.supportedAnnotations = new SupportedAnnotations().GetSupportedAnnotations(documentType);

                List<string> pagesContent = new List<string>();

                if (loadAllPages)
                {
                    pagesContent = GetAllPagesContent(annotator, info);
                }

                for (int i = 0; i < info.PagesInfo.Count; i++)
                {
                    PageDataDescriptionEntity page = new PageDataDescriptionEntity
                    {
                        number = i + 1,
                        height = info.PagesInfo[i].Height,
                        width = info.PagesInfo[i].Width,
                    };

                    if (annotations != null && annotations.Length > 0)
                    {
                        page.SetAnnotations(AnnotationMapper.MapForPage(annotations, i + 1, info.PagesInfo[i], documentType));
                    }

                    if (pagesContent.Count > 0)
                    {
                        page.SetData(pagesContent[i]);
                    }
                    description.pages.Add(page);
                }
            }

            description.guid = documentGuid;
            // return document description
            return description;
        }

        /// <summary>
        /// Get document page
        /// </summary>
        /// <param name="loadDocumentPageRequest"></param>
        /// <returns>Document page image</returns>
        [HttpPost]
        [Route("annotation/loadDocumentPage")]
        public HttpResponseMessage LoadDocumentPage(AnnotationPostedDataEntity loadDocumentPageRequest)
        {
            string password = "";
            try
            {
                // get/set parameters
                string documentGuid = loadDocumentPageRequest.guid;
                int pageNumber = loadDocumentPageRequest.page;
                password = loadDocumentPageRequest.password;
                PageDataDescriptionEntity loadedPage = new PageDataDescriptionEntity();

                // get page image
                byte[] bytes;

                using (GroupDocs.Annotation.Annotator annotator = new GroupDocs.Annotation.Annotator(documentGuid, GetLoadOptions(password)))
                {
                    using (var memoryStream = RenderPageToMemoryStream(annotator, pageNumber))
                    {
                        bytes = memoryStream.ToArray();
                    }

                    IDocumentInfo info = annotator.Document.GetDocumentInfo();
                    AnnotationBase[] annotations = annotator.Get().ToArray();
                    string documentType = getDocumentType(info);

                    if (annotations != null && annotations.Length > 0)
                    {
                        loadedPage.SetAnnotations(AnnotationMapper.MapForPage(annotations, pageNumber, info.PagesInfo[pageNumber - 1], documentType));
                    }

                    string encodedImage = Convert.ToBase64String(bytes);
                    loadedPage.SetData(encodedImage);

                    loadedPage.height = info.PagesInfo[pageNumber - 1].Height;
                    loadedPage.width = info.PagesInfo[pageNumber - 1].Width;
                    loadedPage.number = pageNumber;
                }

                // return loaded page object
                return Request.CreateResponse(HttpStatusCode.OK, loadedPage);
            }
            catch (Exception ex)
            {
                // set exception message
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex, password));
            }
        }

        private string getDocumentType(IDocumentInfo info)
        {
            string documentType = string.Empty;
            if (info.FileType != null)
            {
                documentType = SupportedImageFormats.Contains(info.FileType.Extension) ? "image" : info.FileType.ToString();
            }
            else
            {
                documentType = "Portable Document Format";
            }

            return documentType;
        }

        private static List<string> GetAllPagesContent(GroupDocs.Annotation.Annotator annotator, IDocumentInfo pages)
        {
            List<string> allPages = new List<string>();

            //get page HTML
            for (int i = 0; i < pages.PageCount; i++)
            {
                byte[] bytes;
                using (var memoryStream = RenderPageToMemoryStream(annotator, i + 1))
                {
                    bytes = memoryStream.ToArray();
                }

                string encodedImage = Convert.ToBase64String(bytes);
                allPages.Add(encodedImage);
            }

            return allPages;
        }

        static MemoryStream RenderPageToMemoryStream(GroupDocs.Annotation.Annotator annotator, int pageNumberToRender)
        {
            MemoryStream result = new MemoryStream();

            PreviewOptions previewOptions = new PreviewOptions(pageNumber => result)
            {
                PreviewFormat = PreviewFormats.PNG,
                PageNumbers = new[] { pageNumberToRender },
                RenderComments = false
            };

            annotator.Document.GeneratePreview(previewOptions);

            return result;
        }

        /// <summary>
        /// Upload document
        /// </summary>      
        /// <returns>Uploaded document object</returns>
        [HttpPost]
        [Route("annotation/uploadDocument")]
        public HttpResponseMessage UploadDocument()
        {
            try
            {
                string url = HttpContext.Current.Request.Form["url"];
                // get documents storage path
                string documentStoragePath = globalConfiguration.GetAnnotationConfiguration().GetFilesDirectory();
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
                UploadedDocumentEntity uploadedDocument = new UploadedDocumentEntity
                {
                    guid = fileSavePath
                };
                return Request.CreateResponse(HttpStatusCode.OK, uploadedDocument);
            }
            catch (Exception ex)
            {
                // set exception message
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Download document
        /// </summary>
        /// <param name="path">string</param>
        /// <param name="annotated">bool</param>
        /// <returns></returns>
        [HttpGet]
        [Route("annotation/downloadDocument")]
        public HttpResponseMessage DownloadDocument(string path)
        {
            // add file into the response
            if (File.Exists(path))
            {
                RemoveAnnotations(path, "");
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                var fileStream = new FileStream(path, FileMode.Open);
                response.Content = new StreamContent(fileStream);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = Path.GetFileName(path);
                return response;
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
        }

        /// <summary>
        /// Download document
        /// </summary>
        /// <param name="path">string</param>
        /// <param name="annotated">bool</param>
        /// <returns></returns>
        [HttpGet]
        [Route("annotation/downloadAnnotated")]
        public HttpResponseMessage DownloadAnnotated(string path)
        {
            // add file into the response
            if (File.Exists(path))
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                var fileStream = new FileStream(path, FileMode.Open);
                response.Content = new StreamContent(fileStream);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = Path.GetFileName(path);
                return response;
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
        }

        ///// <summary>
        ///// Annotate document
        ///// </summary>      
        ///// <returns>Annotated document info</returns>
        [HttpPost]
        [Route("annotation/annotate")]
        public HttpResponseMessage Annotate(AnnotationPostedDataEntity annotateDocumentRequest)
        {
            AnnotatedDocumentEntity annotatedDocument = new AnnotatedDocumentEntity();
            try
            {
                // get/set parameters
                string documentGuid = annotateDocumentRequest.guid;
                string password = annotateDocumentRequest.password;
                string documentType = SupportedImageFormats.Contains(Path.GetExtension(annotateDocumentRequest.guid).ToLowerInvariant()) ? "image" : annotateDocumentRequest.documentType;
                string tempPath = GetTempPath(documentGuid);

                AnnotationDataEntity[] annotationsData = annotateDocumentRequest.annotationsData;
                // initiate list of annotations to add
                List<AnnotationBase> annotations = new List<AnnotationBase>();

                using (GroupDocs.Annotation.Annotator annotator = new GroupDocs.Annotation.Annotator(documentGuid, GetLoadOptions(password)))
                {
                    IDocumentInfo info = annotator.Document.GetDocumentInfo();

                    for (int i = 0; i < annotationsData.Length; i++)
                    {
                        AnnotationDataEntity annotationData = annotationsData[i];
                        PageInfo pageInfo = info.PagesInfo[annotationsData[i].pageNumber - 1];
                        // add annotation, if current annotation type isn't supported by the current document type it will be ignored
                        try
                        {
                            BaseAnnotator baseAnnotator = AnnotatorFactory.createAnnotator(annotationData, pageInfo);
                            if (baseAnnotator.IsSupported(documentType))
                            {
                                annotations.Add(baseAnnotator.GetAnnotationBase(documentType));
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new AnnotatorException(ex.Message, ex);
                        }
                    }
                }

                // Add annotation to the document
                RemoveAnnotations(documentGuid, password);
                // check if annotations array contains at least one annotation to add
                if (annotations.Count != 0)
                {
                    using (GroupDocs.Annotation.Annotator annotator = new GroupDocs.Annotation.Annotator(documentGuid, GetLoadOptions(password)))
                    {
                        foreach (var annotation in annotations)
                        {
                            annotator.Add(annotation);
                        }

                        annotator.Save(tempPath);
                    }

                    if (File.Exists(documentGuid))
                    {
                        File.Delete(documentGuid);
                    }

                    File.Move(tempPath, documentGuid);
                }

                annotatedDocument = new AnnotatedDocumentEntity();
                annotatedDocument.guid = documentGuid;
                if (annotateDocumentRequest.print)
                {
                    annotatedDocument.pages = GetAnnotatedPagesForPrint(password, documentGuid);
                    File.Move(documentGuid, annotateDocumentRequest.guid);
                }
            }
            catch (Exception ex)
            {
                // set exception message
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Resources().GenerateException(ex));
            }

            return Request.CreateResponse(HttpStatusCode.OK, annotatedDocument);
        }

        private static List<PageDataDescriptionEntity> GetAnnotatedPagesForPrint(string password, string documentGuid)
        {
            AnnotatedDocumentEntity description = new AnnotatedDocumentEntity();
            try
            {
                using (FileStream outputStream = File.OpenRead(documentGuid))
                {
                    using (GroupDocs.Annotation.Annotator annotator = new GroupDocs.Annotation.Annotator(outputStream, GetLoadOptions(password)))
                    {
                        IDocumentInfo info = annotator.Document.GetDocumentInfo();
                        List<string> pagesContent = GetAllPagesContent(annotator, info);

                        for (int i = 0; i < info.PagesInfo.Count; i++)
                        {
                            PageDataDescriptionEntity page = new PageDataDescriptionEntity();

                            if (pagesContent.Count > 0)
                            {
                                page.SetData(pagesContent[i]);
                            }

                            description.pages.Add(page);
                        }
                    }
                }

                return description.pages;
            }
            catch (FileNotFoundException ex)
            {
                throw ex;
            }
        }

        public static void RemoveAnnotations(string documentGuid, string password)
        {
            string tempPath = GetTempPath(documentGuid);

            try
            {
                using (Stream inputStream = File.Open(documentGuid, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    using (GroupDocs.Annotation.Annotator annotator = new GroupDocs.Annotation.Annotator(inputStream, GetLoadOptions(password)))
                    {
                        annotator.Save(tempPath, new SaveOptions { AnnotationTypes = AnnotationType.None });
                    }
                }

                File.Delete(documentGuid);
                File.Move(tempPath, documentGuid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string GetTempPath(string documentGuid)
        {
            string tempFilename = Path.GetFileNameWithoutExtension(documentGuid) + "_tmp";
            string tempPath = Path.Combine(Path.GetDirectoryName(documentGuid), tempFilename + Path.GetExtension(documentGuid));
            return tempPath;
        }

        private static LoadOptions GetLoadOptions(string password)
        {
            LoadOptions loadOptions = new LoadOptions
            {
                Password = password,
            };

            return loadOptions;
        }
    }
}