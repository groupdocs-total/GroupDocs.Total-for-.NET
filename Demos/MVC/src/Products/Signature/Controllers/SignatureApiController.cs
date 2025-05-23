﻿using GroupDocs.Signature;
using GroupDocs.Signature.Domain;
using GroupDocs.Signature.Options;
using GroupDocs.Total.MVC.Products.Common.Entity.Web;
using GroupDocs.Total.MVC.Products.Signature.Config;
using GroupDocs.Total.MVC.Products.Signature.Entity.Directory;
using GroupDocs.Total.MVC.Products.Signature.Entity.Web;
using GroupDocs.Total.MVC.Products.Signature.Entity.Xml;
using GroupDocs.Total.MVC.Products.Signature.Loader;
using GroupDocs.Total.MVC.Products.Signature.Signer;
using GroupDocs.Total.MVC.Products.Signature.Util.Directory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Results;
using System.Xml;
using System.Xml.Serialization;

namespace GroupDocs.Total.MVC.Products.Signature.Controllers
{
    /// <summary>
    /// SignatureApiController
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SignatureApiController : ApiController
    {
        private static readonly Common.Config.GlobalConfiguration GlobalConfiguration = new Common.Config.GlobalConfiguration();
        private readonly List<string> SupportedImageFormats = new List<string> { ".bmp", ".jpeg", ".jpg", ".tiff", ".tif", ".png" };
        private readonly DirectoryUtils DirectoryUtils = new DirectoryUtils(GlobalConfiguration.GetSignatureConfiguration());

        /// <summary>
        /// Load Signature configuration
        /// </summary>
        /// <returns>Signature configuration</returns>
        [HttpGet]
        [Route("signature/loadConfig")]
        public SignatureConfiguration LoadConfig()
        {
            return GlobalConfiguration.GetSignatureConfiguration();
        }

        /// <summary>
        /// Get all files and directories from storage
        /// </summary>
        /// <param name="postedData">SignaturePostedDataEntity</param>
        /// <returns>List of files and directories</returns>
        [HttpPost]
        [Route("signature/loadFileTree")]
        public HttpResponseMessage LoadFileTree(SignaturePostedDataEntity postedData)
        {
            // get request body
            string relDirPath = postedData.path;
            string signatureType = "";
            if (!string.IsNullOrEmpty(postedData.signatureType))
            {
                signatureType = postedData.signatureType;
            }
            // get file list from storage path
            try
            {
                string rootDirectory;
                switch (signatureType)
                {
                    case "digital":
                        rootDirectory = DirectoryUtils.DataDirectory.CertificateDirectory.Path;
                        break;
                    case "image":
                        rootDirectory = DirectoryUtils.DataDirectory.UploadedImageDirectory.Path;
                        break;
                    case "hand":
                        rootDirectory = DirectoryUtils.DataDirectory.ImageDirectory.Path;
                        break;
                    case "stamp":
                        rootDirectory = DirectoryUtils.DataDirectory.StampDirectory.Path;
                        break;
                    case "text":
                        rootDirectory = DirectoryUtils.DataDirectory.TextDirectory.Path;
                        break;
                    case "qrCode":
                        rootDirectory = DirectoryUtils.DataDirectory.QrCodeDirectory.Path;
                        break;
                    case "barCode":
                        rootDirectory = DirectoryUtils.DataDirectory.BarcodeDirectory.Path;
                        break;
                    default:
                        rootDirectory = DirectoryUtils.FilesDirectory.GetPath();
                        break;
                }
                // get all the files from a directory
                if (string.IsNullOrEmpty(relDirPath))
                {
                    relDirPath = rootDirectory;
                }
                else
                {
                    relDirPath = Path.Combine(rootDirectory, relDirPath);
                }
                SignatureLoader signatureLoader = new SignatureLoader(relDirPath, GlobalConfiguration, DirectoryUtils);
                List<SignatureFileDescriptionEntity> fileList;
                switch (signatureType)
                {
                    case "digital":
                        fileList = signatureLoader.LoadFiles();
                        break;
                    case "image":
                    case "hand":
                        fileList = signatureLoader.LoadImageSignatures();
                        break;
                    case "text":
                        fileList = signatureLoader.LoadTextSignatures(DataDirectoryEntity.DATA_XML_FOLDER);
                        break;
                    case "qrCode":
                    case "barCode":
                    case "stamp":
                        fileList = signatureLoader.LoadStampSignatures(DataDirectoryEntity.DATA_PREVIEW_FOLDER, DataDirectoryEntity.DATA_XML_FOLDER, signatureType);
                        break;
                    default:
                        fileList = signatureLoader.LoadFiles();
                        break;
                }
                return Request.CreateResponse(HttpStatusCode.OK, fileList);
            }
            catch (System.Exception ex)
            {
                // set exception message
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Common.Resources.Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Load document description
        /// </summary>
        /// <param name="postedData">SignaturePostedDataEntity</param>
        /// <returns>All info about the document</returns>
        [HttpPost]
        [Route("signature/loadDocumentDescription")]
        public HttpResponseMessage LoadDocumentDescription(SignaturePostedDataEntity postedData)
        {
            string password = "";
            try
            {
                // get/set parameters
                string documentGuid = postedData.guid;
                password = postedData.password;
                LoadDocumentEntity loadDocumentEntity = new LoadDocumentEntity();
                using (FileStream fileStream = File.Open(documentGuid, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    // get document info container
                    using (GroupDocs.Signature.Signature signature = new GroupDocs.Signature.Signature(fileStream, GetLoadOptions(password)))
                    {
                        IDocumentInfo documentInfo = signature.GetDocumentInfo();
                        List<SignatureLoadedPageEntity> pagesDescription = new List<SignatureLoadedPageEntity>();
                        for (int i = 0; i < documentInfo.PageCount; i++)
                        {
                            // initiate custom Document description object
                            SignatureLoadedPageEntity description = new SignatureLoadedPageEntity
                            {
                                // set current page size
                                height = documentInfo.Pages[i].Height,
                                width = documentInfo.Pages[i].Width,
                                number = i + 1
                            };
                            if (GlobalConfiguration.GetSignatureConfiguration().GetPreloadPageCount() == 0)
                            {
                                byte[] pageBytes = RenderPageToMemoryStream(signature, i).ToArray();
                                string encodedImage = Convert.ToBase64String(pageBytes);
                                pageBytes = null;
                                description.SetData(encodedImage);
                            }
                            pagesDescription.Add(description);
                        }
                        loadDocumentEntity.SetGuid(documentGuid);
                        foreach (SignatureLoadedPageEntity pageDescription in pagesDescription)
                        {
                            loadDocumentEntity.SetPages(pageDescription);
                        }
                    }
                }
                // return document description
                return Request.CreateResponse(HttpStatusCode.OK, loadDocumentEntity);
            }
            catch (PasswordRequiredException ex)
            {
                // set exception message
                return Request.CreateResponse(HttpStatusCode.Forbidden, new Common.Resources.Resources().GenerateException(ex, password));
            }
            catch (System.Exception ex)
            {
                // set exception message
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Common.Resources.Resources().GenerateException(ex, password));
            }
        }

        // TODO: Rewrite this
        static MemoryStream _pagePreviewStream = null;
        static MemoryStream RenderPageToMemoryStream(GroupDocs.Signature.Signature signature, int pageNumberToRender)
        {
            _pagePreviewStream = null;
            GroupDocs.Signature.Options.PreviewOptions previewOptions = 
                new GroupDocs.Signature.Options.PreviewOptions(CreatePageStream, ReleasePageStream)
            {
                PreviewFormat = PreviewOptions.PreviewFormats.PNG,
                PageNumbers = new[] { pageNumberToRender }
            };
            signature.GeneratePreview(previewOptions);

            return _pagePreviewStream;
        }

        private static Stream CreatePageStream(PreviewPageData pageData)
        {
            _pagePreviewStream = new MemoryStream();
            return _pagePreviewStream;
        }

        private static void ReleasePageStream(PreviewPageData pageData, Stream pageStream)
        {
            pageStream.Dispose();
        }

        /// <summary>
        /// Load document page
        /// </summary>
        /// <param name="postedData">SignaturePostedDataEntity</param>
        /// <returns>Document page image encoded in Base64</returns>
        [HttpPost]
        [Route("signature/loadDocumentPage")]
        public HttpResponseMessage LoadDocumentPage(SignaturePostedDataEntity postedData)
        {
            string password = "";
            try
            {
                // get/set parameters
                string documentGuid = postedData.guid;
                int pageNumber = postedData.page;
                password = postedData.password;
                SignatureLoadedPageEntity loadedPage = new SignatureLoadedPageEntity();

                using (FileStream fileStream = File.Open(documentGuid, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    using (GroupDocs.Signature.Signature signature = new GroupDocs.Signature.Signature(fileStream, GetLoadOptions(password)))
                    {
                        // get page image
                        byte[] bytes = RenderPageToMemoryStream(signature, pageNumber - 1).ToArray();
                        // encode ByteArray into string
                        string encodedImage = Convert.ToBase64String(bytes);
                        loadedPage.SetData(encodedImage);

                        IDocumentInfo documentInfo = signature.GetDocumentInfo();
                        // set current page info for result
                        loadedPage.height = documentInfo.Pages[pageNumber - 1].Height;
                        loadedPage.width = documentInfo.Pages[pageNumber - 1].Width;
                        loadedPage.number = pageNumber;
                    }
                }

                // return loaded page object
                return Request.CreateResponse(HttpStatusCode.OK, loadedPage);
            }
            catch (PasswordRequiredException ex)
            {
                // set exception message
                return Request.CreateResponse(HttpStatusCode.Forbidden, new Common.Resources.Resources().GenerateException(ex, password));
            }
            catch (System.Exception ex)
            {
                // set exception message
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Common.Resources.Resources().GenerateException(ex, password));
            }
        }

        /// <summary>
        /// Download document
        /// </summary>
        /// <param name="path">string</param>
        /// <returns></returns>
        [HttpGet]
        [Route("signature/downloadDocument")]
        public HttpResponseMessage DownloadDocument(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                // check if file exists
                if (File.Exists(path))
                {
                    // prepare response message
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    // add file into the response
                    var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    response.Content = new StreamContent(fileStream);
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = Path.GetFileName(path)
                    };
                    return response;
                }
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        [HttpPost]
        [Route("signature/downloadSigned")]
        public HttpResponseMessage DownloadSigned(SignaturePostedDataEntity signDocumentRequest)
        {
            SignatureDataEntity[] signaturesData = signDocumentRequest.signaturesData;
            if (signaturesData == null || signaturesData.Count() == 0)
            {
                // set exception message
                return Request.CreateResponse(HttpStatusCode.OK, new Common.Resources.Resources().GenerateException(new ArgumentNullException("Signature data is empty")));
            }

            // get document path
            string documentGuid = signDocumentRequest.guid;
            string fileName = Path.GetFileName(documentGuid);
            try
            {
                Stream inputStream = SignByStream(signDocumentRequest);
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StreamContent(inputStream)
                };
                // add file into the response
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = Path.GetFileName(fileName)
                };
                return response;
            }
            catch (System.Exception ex)
            {
                // set exception message
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Common.Resources.Resources().GenerateException(ex));
            }
        }

        private Stream SignByStream(SignaturePostedDataEntity postedData)
        {
            try
            {
                // get/set parameters
                string documentGuid = postedData.guid;
                string password = postedData.password;
                SignatureDataEntity[] signaturesData = postedData.signaturesData;
                List<SignOptions> signsCollection = new List<SignOptions>();
                // check if document type is image
                if (SupportedImageFormats.Contains(Path.GetExtension(documentGuid)))
                {
                    postedData.documentType = "image";
                }
                // set signature password if required
                for (int i = 0; i < signaturesData.Length; i++)
                {
                    switch (signaturesData[i].SignatureType)
                    {
                        case "image":
                        case "hand":
                            SignImage(postedData.documentType, signaturesData[i], signsCollection);
                            break;
                        case "stamp":
                            SignStamp(postedData.documentType, signaturesData[i], signsCollection);
                            break;
                        case "qrCode":
                        case "barCode":
                            SignOptical(postedData.documentType, signaturesData[i], signsCollection);
                            break;
                        case "text":
                            SignText(postedData.documentType, signaturesData[i], signsCollection);
                            break;
                        case "digital":
                            SignDigital(postedData.documentType, password, signaturesData[i], signsCollection);
                            break;
                        default:
                            SignImage(postedData.documentType, signaturesData[i], signsCollection);
                            break;
                    }
                }
                // return loaded page object
                Stream signedDocument = SignDocumentStream(documentGuid, password, signsCollection);
                // return loaded page object
                return signedDocument;
            }
            catch (System.Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

        /// <summary>
        /// Upload document
        /// </summary>
        /// <returns>Uploaded document object</returns>
        [HttpPost]
        [Route("signature/uploadDocument")]
        public HttpResponseMessage UploadDocument()
        {
            try
            {
                // get posted data
                string url = HttpContext.Current.Request.Form["url"];
                string signatureType = HttpContext.Current.Request.Form["signatureType"];
                bool rewrite = bool.Parse(HttpContext.Current.Request.Form["rewrite"]);
                // get path for where to save the file
                string fileSavePath = "";
                switch (signatureType)
                {
                    case "digital":
                        fileSavePath = DirectoryUtils.DataDirectory.CertificateDirectory.Path;
                        break;
                    case "image":
                        fileSavePath = DirectoryUtils.DataDirectory.UploadedImageDirectory.Path;
                        break;
                    default:
                        fileSavePath = DirectoryUtils.FilesDirectory.GetPath();
                        break;
                }
                // check if file selected or URL
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
                                fileSavePath = Path.Combine(fileSavePath, httpPostedFile.FileName);
                            }
                            else
                            {
                                fileSavePath = Common.Resources.Resources.GetFreeFileName(fileSavePath, httpPostedFile.FileName);
                            }

                            // Save the uploaded file to "UploadedFiles" folder
                            httpPostedFile.SaveAs(fileSavePath);
                            httpPostedFile.InputStream.Close();
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
                            fileSavePath = Path.Combine(fileSavePath, fileName);
                        }
                        else
                        {
                            fileSavePath = Common.Resources.Resources.GetFreeFileName(fileSavePath, fileName);
                        }
                        // Download the Web resource and save it into the current filesystem folder.
                        client.DownloadFile(url, fileSavePath);
                    }
                }
                // initiate uploaded file description class
                SignatureFileDescriptionEntity uploadedDocument = new SignatureFileDescriptionEntity
                {
                    guid = fileSavePath
                };
                MemoryStream ms = new MemoryStream();
                using (FileStream file = new FileStream(fileSavePath, FileMode.Open, FileAccess.ReadWrite))
                {
                    file.CopyTo(ms);
                    byte[] imageBytes = ms.ToArray();
                    // Convert byte[] to Base64 String
                    uploadedDocument.image = Convert.ToBase64String(imageBytes);
                }
                ms.Close();
                return Request.CreateResponse(HttpStatusCode.OK, uploadedDocument);
            }
            catch (System.Exception ex)
            {
                // set exception message
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Common.Resources.Resources().GenerateException(ex));
            }
        }


        /// <summary>
        /// Load selected signature image preview
        /// </summary>
        /// <param name="postedData">SignaturePostedDataEntity</param>
        /// <returns>Signature image preview in Base64</returns>
        [HttpPost]
        [Route("signature/loadSignatureImage")]
        public HttpResponseMessage LoadSignatureImage(SignaturePostedDataEntity postedData)
        {
            try
            {
                // get/set parameters
                string documentGuid = postedData.guid;
                SignatureLoadedPageEntity loadedPage = new SignatureLoadedPageEntity();
                MemoryStream ms = new MemoryStream();
                using (FileStream file = new FileStream(documentGuid, FileMode.Open, FileAccess.ReadWrite))
                {
                    file.CopyTo(ms);
                    byte[] imageBytes = ms.ToArray();
                    // Convert byte[] to Base64 String
                    loadedPage.SetData(Convert.ToBase64String(imageBytes));
                }
                ms.Close();
                if (postedData.signatureType.Equals("text"))
                {
                    // get xml data of the Text signature
                    string xmlFileName = Path.GetFileNameWithoutExtension(Path.GetFileName(documentGuid));
                    string xmlPath = DirectoryUtils.DataDirectory.TextDirectory.XmlPath;
                    // Load xml data
                    TextXmlEntity textData = LoadXmlData<TextXmlEntity>(xmlPath, xmlFileName);
                    loadedPage.props = textData;
                }
                // return loaded page object
                return Request.CreateResponse(HttpStatusCode.OK, loadedPage);
            }
            catch (System.Exception ex)
            {
                // set exception message
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Common.Resources.Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Save image signature
        /// </summary>
        /// <param name="postedData">SignaturePostedDataEntity</param>
        /// <returns>Image signature preview</returns>
        [HttpPost]
        [Route("signature/saveImage")]
        public HttpResponseMessage SaveImage(SignaturePostedDataEntity postedData)
        {
            try
            {
                // get/set parameters
                string encodedImage = postedData.image.Replace("data:image/png;base64,", "");
                FileDescriptionEntity savedImage = new FileDescriptionEntity();
                string[] files = Directory.GetFiles(DirectoryUtils.DataDirectory.ImageDirectory.Path);
                string imageName = "";
                if (files.Length == 0)
                {
                    imageName = string.Format("{0:000}.png", 1);
                }
                else
                {
                    for (int i = 0; i <= files.Length; i++)
                    {
                        string path = Path.Combine(DirectoryUtils.DataDirectory.ImageDirectory.Path, string.Format("{0:000}.png", i + 1));
                        if (files.Contains(path))
                        {
                            continue;
                        }
                        else
                        {
                            imageName = string.Format("{0:000}.png", i + 1);
                        }
                    }
                }
                string imagePath = Path.Combine(DirectoryUtils.DataDirectory.ImageDirectory.Path, imageName);
                if (File.Exists(imagePath))
                {
                    imageName = Path.GetFileName(Common.Resources.Resources.GetFreeFileName(DirectoryUtils.DataDirectory.ImageDirectory.Path, imageName));
                    imagePath = Path.Combine(DirectoryUtils.DataDirectory.ImageDirectory.Path, imageName);
                }
                File.WriteAllBytes(imagePath, Convert.FromBase64String(encodedImage));
                savedImage.guid = imagePath;
                // return loaded page object
                return Request.CreateResponse(HttpStatusCode.OK, savedImage);
            }
            catch (System.Exception ex)
            {
                // set exception message
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Common.Resources.Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Save stamp signature
        /// </summary>
        /// <param name="postedData">SignaturePostedDataEntity</param>
        /// <returns>Stamp signature preview image</returns>
        [HttpPost]
        [Route("signature/saveStamp")]
        public HttpResponseMessage SaveStamp(SignaturePostedDataEntity postedData)
        {
            string previewPath = DirectoryUtils.DataDirectory.StampDirectory.PreviewPath;
            string xmlPath = DirectoryUtils.DataDirectory.StampDirectory.XmlPath;
            try
            {
                // get/set parameters
                string encodedImage = postedData.image.Replace("data:image/png;base64,", "");
                StampXmlEntity[] stampData = postedData.stampData;

                string newFileName = "";
                FileDescriptionEntity savedImage = new FileDescriptionEntity();
                string filePath = "";
                string[] listOfFiles = Directory.GetFiles(previewPath);
                for (int i = 0; i <= listOfFiles.Length; i++)
                {
                    int number = i + 1;
                    newFileName = string.Format("{0:000}", number);
                    filePath = previewPath + "/" + newFileName + ".png";
                    if (File.Exists(filePath))
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                File.WriteAllBytes(filePath, Convert.FromBase64String(encodedImage));
                savedImage.guid = filePath;
                // stamp data to xml file saving
                SaveXmlData(xmlPath, newFileName, stampData);
                // return loaded page object
                return Request.CreateResponse(HttpStatusCode.OK, savedImage);
            }
            catch (System.Exception ex)
            {
                // set exception message
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Common.Resources.Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Save optical signature
        /// </summary>
        /// <param name="postedData">SignaturePostedDataEntity</param>
        /// <returns>Signature preview image</returns>
        [HttpPost]
        [Route("signature/saveOpticalCode")]
        public HttpResponseMessage SaveOpticalCode([FromBody]dynamic postedData)
        {
            try
            {
                OpticalXmlEntity opticalCodeData = JsonConvert.DeserializeObject<OpticalXmlEntity>(postedData.properties.ToString());
                string signatureType = postedData.signatureType;

                // initiate signature data wrapper with default values
                SignatureDataEntity signaturesData = new SignatureDataEntity
                {
                    ImageHeight = 200,
                    ImageWidth = 200,
                    Left = 0,
                    Top = 0
                };

                signaturesData.setHorizontalAlignment(HorizontalAlignment.Center);
                signaturesData.setVerticalAlignment(VerticalAlignment.Center);
                // initiate signer object
                string previewPath;
                string xmlPath;
                QrCodeSigner qrSigner;
                BarCodeSigner barCodeSigner;
                // initiate signature options collection
                var signOptionsCollection = new List<SignOptions>();

                // check optical signature type
                if (signatureType.Equals("qrCode"))
                {
                    qrSigner = new QrCodeSigner(opticalCodeData, signaturesData);
                    // get preview path
                    previewPath = DirectoryUtils.DataDirectory.QrCodeDirectory.PreviewPath;
                    // get xml file path
                    xmlPath = DirectoryUtils.DataDirectory.QrCodeDirectory.XmlPath;
                    // generate unique file names for preview image and xml file
                    signOptionsCollection.Add(qrSigner.SignImage());
                }
                else
                {
                    barCodeSigner = new BarCodeSigner(opticalCodeData, signaturesData);
                    // get preview path
                    previewPath = DirectoryUtils.DataDirectory.BarcodeDirectory.PreviewPath;
                    // get xml file path
                    xmlPath = DirectoryUtils.DataDirectory.BarcodeDirectory.XmlPath;
                    // generate unique file names for preview image and xml file
                    signOptionsCollection.Add(barCodeSigner.SignImage());
                }

                string[] listOfFiles = Directory.GetFiles(previewPath);
                string fileName = "";
                string filePath = "";
                if (!string.IsNullOrEmpty(opticalCodeData.imageGuid))
                {
                    filePath = opticalCodeData.imageGuid;
                    fileName = Path.GetFileNameWithoutExtension(opticalCodeData.imageGuid);
                }
                else
                {
                    for (int i = 0; i <= listOfFiles.Length; i++)
                    {
                        // set file name, for example 001
                        fileName = opticalCodeData.text;
                        filePath = Path.Combine(previewPath, fileName + ".png");
                        // check if file with such name already exists
                        if (File.Exists(filePath))
                        {
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                // generate empty image for future signing with Optical signature, such approach required to get QR-Code as image
                using (Bitmap bitMap = new Bitmap(200, 200))
                {
                    using (MemoryStream memory = new MemoryStream())
                    {
                        using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                        {
                            bitMap.Save(memory, ImageFormat.Png);
                            byte[] bytes = memory.ToArray();
                            fs.Write(bytes, 0, bytes.Length);
                        }
                    }
                }

                // Optical data to xml file saving
                SaveXmlData(xmlPath, fileName, opticalCodeData);

                // set signing save options
                SaveOptions saveOptions = new SaveOptions
                {
                    AddMissingExtenstion = true,
                    OverwriteExistingFiles = true
                };

                string tempFile = Path.Combine(previewPath, fileName + "signed.png");

                // sign generated image with Optical signature
                using (FileStream outputStream = File.Create(Path.Combine(tempFile)))
                {
                    using (GroupDocs.Signature.Signature signature = new GroupDocs.Signature.Signature(filePath))
                    {
                        signature.Sign(outputStream, signOptionsCollection, saveOptions);
                    }
                }

                File.Delete(filePath);
                File.Move(tempFile, filePath);

                // set data for response
                opticalCodeData.imageGuid = filePath;
                opticalCodeData.height = Convert.ToInt32(signaturesData.ImageHeight);
                opticalCodeData.width = Convert.ToInt32(signaturesData.ImageWidth);

                // get signature preview as Base64 string
                byte[] imageArray = File.ReadAllBytes(filePath);
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                opticalCodeData.encodedImage = base64ImageRepresentation;

                if (opticalCodeData.temp)
                {
                    File.Delete(filePath);
                    File.Delete(Path.Combine(xmlPath, fileName + ".xml"));
                }

                // return loaded page object
                return Request.CreateResponse(HttpStatusCode.OK, opticalCodeData);
            }
            catch (System.Exception ex)
            {
                // set exception message
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Common.Resources.Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Save text signature
        /// </summary>
        /// <param name="postedData">SignaturePostedDataEntity</param>
        /// <returns>Text signature preview image</returns>
        [HttpPost]
        [Route("signature/saveText")]
        public HttpResponseMessage SaveText([FromBody] dynamic postedData)
        {
            string xmlPath = DirectoryUtils.DataDirectory.TextDirectory.XmlPath;
            try
            {
                TextXmlEntity textData = JsonConvert.DeserializeObject<TextXmlEntity>(postedData.properties.ToString());
                string[] listOfFiles = Directory.GetFiles(xmlPath);
                string fileName = "";
                string filePath = "";
                if (File.Exists(textData.imageGuid))
                {
                    fileName = Path.GetFileNameWithoutExtension(textData.imageGuid);
                    filePath = textData.imageGuid;
                }
                else
                {
                    for (int i = 0; i <= listOfFiles.Length; i++)
                    {
                        int number = i + 1;
                        // set file name, for example 001
                        fileName = string.Format("{0:000}", number);
                        filePath = Path.Combine(xmlPath, fileName + ".xml");
                        // check if file with such name already exists
                        if (File.Exists(filePath))
                        {
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                // Save text data to an xml file
                SaveXmlData(xmlPath, fileName, textData);
                // set Text data for response
                textData.imageGuid = filePath;
                // return loaded page object               
                return Request.CreateResponse(HttpStatusCode.OK, textData);
            }
            catch (System.Exception ex)
            {
                // set exception message
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Common.Resources.Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Save text signature
        /// </summary>
        /// <param name="postedData">SignaturePostedDataEntity</param>
        /// <returns>Text signature preview image</returns>
        [HttpGet]
        [Route("signature/getFonts")]
        public HttpResponseMessage GetFonts()
        {
            try
            {
                List<string> fonts = new List<string>();

                foreach (FontFamily font in FontFamily.Families)
                {
                    fonts.Add(font.Name);
                }
                return Request.CreateResponse(HttpStatusCode.OK, fonts);
            }
            catch (System.Exception ex)
            {
                // set exception message
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Common.Resources.Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Delete signature
        /// </summary>
        /// <param name="postedData">SignaturePostedDataEntity</param>
        /// <returns>Text signature preview image</returns>
        [HttpPost]
        [Route("signature/deleteSignatureFile")]
        public HttpResponseMessage DeleteSignatureFile(DeleteSignatureFileRequest deleteSignatureFileRequest)
        {
            try
            {
                string signatureType = deleteSignatureFileRequest.getSignatureType();
                switch (signatureType)
                {
                    case "image":
                    case "digital":
                    case "hand":
                        if (File.Exists(deleteSignatureFileRequest.getGuid()))
                        {
                            File.Delete(deleteSignatureFileRequest.getGuid());
                        }
                        break;
                    default:
                        if (File.Exists(deleteSignatureFileRequest.getGuid()))
                        {
                            File.Delete(deleteSignatureFileRequest.getGuid());
                        }
                        string xmlFilePath = GetXmlFilePath(
                            signatureType,
                            Path.GetFileNameWithoutExtension(deleteSignatureFileRequest.getGuid()) + ".xml"
                        );
                        File.Delete(xmlFilePath);
                        break;
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (System.Exception ex)
            {
                // set exception message
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Common.Resources.Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Sign document with digital signature
        /// </summary>
        /// <param name="postedData">SignaturePostedDataEntity</param>
        /// <returns>Signed document info</returns>
        [HttpPost]
        [Route("signature/sign")]
        public HttpResponseMessage Sign(SignaturePostedDataEntity postedData)
        {
            try
            {
                // get/set parameters
                string documentGuid = postedData.guid;
                string password = postedData.password;
                SignatureDataEntity[] signaturesData = postedData.signaturesData;
                var signsCollection = new List<SignOptions>();
                // check if document type is image                
                if (SupportedImageFormats.Contains(Path.GetExtension(documentGuid)))
                {
                    postedData.documentType = "image";
                }
                // set signature password if required
                for (int i = 0; i < signaturesData.Length; i++)
                {
                    switch (signaturesData[i].SignatureType)
                    {
                        case "image":
                        case "hand":
                            SignImage(postedData.documentType, signaturesData[i], signsCollection);
                            break;
                        case "stamp":
                            SignStamp(postedData.documentType, signaturesData[i], signsCollection);
                            break;
                        case "qrCode":
                        case "barCode":
                            SignOptical(postedData.documentType, signaturesData[i], signsCollection);
                            break;
                        case "text":
                            SignText(postedData.documentType, signaturesData[i], signsCollection);
                            break;
                        case "digital":
                            SignDigital(postedData.documentType, password, signaturesData[i], signsCollection);
                            break;
                        default:
                            SignImage(postedData.documentType, signaturesData[i], signsCollection);
                            break;
                    }
                }
                // return loaded page object
                SignedDocumentEntity signedDocument = SignDocument(documentGuid, password, signsCollection);
                // return loaded page object
                return Request.CreateResponse(HttpStatusCode.OK, signedDocument);
            }
            catch (System.Exception ex)
            {
                // set exception message
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new Common.Resources.Resources().GenerateException(ex));
            }
        }

        /// <summary>
        /// Add digital signature
        /// </summary>
        /// <param name="documentType">string</param>
        /// <param name="password">string</param>
        /// <param name="signaturesData">SignatureDataEntity</param>
        /// <param name="signsCollection">SignatureOptionsCollection</param>
        private static void SignDigital(string documentType, string password, SignatureDataEntity signaturesData, List<SignOptions> signsCollection)
        {
            try
            {
                // initiate digital signer
                DigitalSigner signer = new DigitalSigner(signaturesData, password);
                AddSignOptions(documentType, signsCollection, signer);
            }
            catch (System.Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

        /// <summary>
        /// Add image signature
        /// </summary>
        /// <param name="documentType">string</param>
        /// <param name="signaturesData">SignatureDataEntity</param>
        /// <param name="signsCollection">SignatureOptionsCollection</param>
        private static void SignImage(string documentType, SignatureDataEntity signaturesData, List<SignOptions> signsCollection)
        {
            try
            {
                // initiate image signer object
                ImageSigner signer = new ImageSigner(signaturesData);
                // prepare signing options and sign document
                AddSignOptions(documentType, signsCollection, signer);
            }
            catch (System.Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

        /// <summary>
        /// Add stamp signature
        /// </summary>
        /// <param name="documentType">string</param>
        /// <param name="signaturesData">SignatureDataEntity</param>
        /// <param name="signsCollection">SignatureOptionsCollection</param> 
        private void SignStamp(string documentType, SignatureDataEntity signaturesData, List<SignOptions> signsCollection)
        {
            string xmlPath = DirectoryUtils.DataDirectory.StampDirectory.XmlPath;
            try
            {
                string xmlFileName = Path.GetFileNameWithoutExtension(Path.GetFileName(signaturesData.SignatureGuid));
                // Load xml data
                StampXmlEntity[] stampData = LoadXmlData<StampXmlEntity[]>(xmlPath, xmlFileName);
                // since stamp ine are added stating from the most outer line we need to reverse the stamp data array
                Array.Reverse(stampData);
                // initiate stamp signer
                StampSigner signer = new StampSigner(stampData, signaturesData);
                // prepare signing options and sign document
                AddSignOptions(documentType, signsCollection, signer);
            }
            catch (System.Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

        /// <summary>
        /// Add Optical signature
        /// </summary>
        /// <param name="documentType">string</param>
        /// <param name="signaturesData">SignatureDataEntity</param>
        /// <param name="signsCollection">SignatureOptionsCollection</param> 
        private void SignOptical(string documentType, SignatureDataEntity signaturesData, List<SignOptions> signsCollection)
        {
            try
            {
                string signatureType = signaturesData.SignatureType;
                // get xml files root path
                string xmlPath = (signatureType.Equals("qrCode")) ? DirectoryUtils.DataDirectory.QrCodeDirectory.XmlPath : DirectoryUtils.DataDirectory.BarcodeDirectory.XmlPath;
                // prepare signing options and sign document               
                // get xml data of the QR-Code
                string xmlFileName = Path.GetFileNameWithoutExtension(Path.GetFileName(signaturesData.SignatureGuid));
                // Load xml data
                OpticalXmlEntity opticalCodeData = LoadXmlData<OpticalXmlEntity>(xmlPath, xmlFileName);
                // initiate QRCode signer object
                BaseSigner signer = null;
                if (signatureType.Equals("qrCode"))
                {
                    signer = new QrCodeSigner(opticalCodeData, signaturesData);
                }
                else
                {
                    signer = new BarCodeSigner(opticalCodeData, signaturesData);
                }
                // prepare signing options and sign document
                AddSignOptions(documentType, signsCollection, signer);
            }
            catch (System.Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

        /// <summary>
        /// Add text signature
        /// </summary>
        /// <param name="documentType">string</param>
        /// <param name="signaturesData">SignatureDataEntity</param>
        /// <param name="signsCollection">SignatureOptionsCollection</param>
        private void SignText(string documentType, SignatureDataEntity signaturesData, List<SignOptions> signsCollection)
        {
            string xmlPath = DirectoryUtils.DataDirectory.TextDirectory.XmlPath;
            try
            {
                // get xml data of the Text signature
                string xmlFileName = Path.GetFileNameWithoutExtension(signaturesData.SignatureGuid);
                // Load xml data
                TextXmlEntity textData = LoadXmlData<TextXmlEntity>(xmlPath, xmlFileName);
                // initiate QRCode signer object
                TextSigner signer = new TextSigner(textData, signaturesData);
                // prepare signing options and sign document
                AddSignOptions(documentType, signsCollection, signer);
            }
            catch (System.Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

        /// <summary>
        /// Add signature options to the signatures collection
        /// </summary>
        /// <param name="documentType">string</param>
        /// <param name="signsCollection">SignatureOptionsCollection</param>
        /// <param name="signer">SignatureSigner</param>
        private static void AddSignOptions(string documentType, List<SignOptions> signsCollection, BaseSigner signer)
        {
            switch (documentType)
            {
                case "Portable Document Format":
                    signsCollection.Add(signer.SignPdf());
                    break;
                case "Microsoft Word":
                    signsCollection.Add(signer.SignWord());
                    break;
                case "Microsoft PowerPoint":
                    signsCollection.Add(signer.SignSlides());
                    break;
                case "image":
                    signsCollection.Add(signer.SignImage());
                    break;
                case "Microsoft Excel":
                    signsCollection.Add(signer.SignCells());
                    break;
            }
        }

        /// <summary>
        /// Sign document
        /// </summary>
        /// <param name="documentGuid">string</param>
        /// <param name="password">string</param>
        /// <param name="signsCollection">SignatureOptionsCollection</param>
        /// <returns></returns>
        private static SignedDocumentEntity SignDocument(string documentGuid, string password, List<SignOptions> signsCollection)
        {
            // set save options
            SaveOptions saveOptions = new SaveOptions
            {
                OverwriteExistingFiles = false
            };

            // sign document
            string tempFilename = Path.GetFileNameWithoutExtension(documentGuid) + "_tmp";
            string tempPath = Path.Combine(Path.GetDirectoryName(documentGuid), tempFilename + Path.GetExtension(documentGuid));
            SignedDocumentEntity signedDocument = new SignedDocumentEntity();
            using (GroupDocs.Signature.Signature signature = new GroupDocs.Signature.Signature(documentGuid, GetLoadOptions(password)))
            {
                signature.Sign(tempPath, signsCollection, saveOptions);
            }

            File.Delete(documentGuid);
            File.Move(tempPath, documentGuid);

            signedDocument.guid = documentGuid;

            return signedDocument;
        }

        /// <summary>
        /// Sign document
        /// </summary>
        /// <param name="documentGuid">string</param>
        /// <param name="password">string</param>
        /// <param name="signsCollection">SignatureOptionsCollection</param>
        /// <returns></returns>
        private static Stream SignDocumentStream(string documentGuid, string password, List<SignOptions> signsCollection)
        {
            // set save options
            SaveOptions saveOptions = new SaveOptions();

            FileStream fileStream = File.Open(documentGuid, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

            using (GroupDocs.Signature.Signature signature = new GroupDocs.Signature.Signature(fileStream, GetLoadOptions(password)))
            {
                signature.Sign(documentGuid, signsCollection, saveOptions);

                return fileStream;
            }
        }

        private string GetXmlFilePath(string signatureType, string fileName)
        {
            string path;
            switch (signatureType)
            {
                case "stamp":
                    path = Path.Combine(DirectoryUtils.DataDirectory.StampDirectory.XmlPath, fileName);
                    break;
                case "text":
                    path = Path.Combine(DirectoryUtils.DataDirectory.TextDirectory.XmlPath, fileName);
                    break;
                case "qrCode":
                    path = Path.Combine(DirectoryUtils.DataDirectory.QrCodeDirectory.XmlPath, fileName);
                    break;
                case "barCode":
                    path = Path.Combine(DirectoryUtils.DataDirectory.BarcodeDirectory.XmlPath, fileName);
                    break;
                default:
                    throw new ArgumentNullException("Signature type is not defined");
            }
            return path;
        }

        /// <summary>
        /// Load signature XML data from file
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="xmlPath">string</param>
        /// <param name="xmlFileName">string</param>
        /// <returns>Signature data object</returns>
        public static T LoadXmlData<T>(string xmlPath, string xmlFileName)
        {
            // initiate return object type
            T returnObject = default(T);
            if (string.IsNullOrEmpty(xmlFileName))
            {
                return default(T);
            }

            try
            {
                // get stream of the xml file
                using (StreamReader xmlStream = new StreamReader(Path.Combine(xmlPath, xmlFileName + ".xml")))
                {
                    // initiate serializer
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    // deserialize XML into the object
                    returnObject = (T)serializer.Deserialize(xmlStream);
                }
            }
            catch (System.Exception ex)
            {
                Console.Error.Write(ex.Message);
            }
            return returnObject;
        }

        /// <summary>
        /// Save signature data into the XML
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="xmlPath">string</param>
        /// <param name="xmlFileName">string</param>
        /// <param name="serializableObject">Object</param>
        private void SaveXmlData<T>(string xmlPath, string xmlFileName, T serializableObject)
        {
            if (serializableObject == null) { return; }

            try
            {
                // initiate xml
                XmlDocument xmlDocument = new XmlDocument();
                // initiate serializer
                XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());
                // save xml file
                using (MemoryStream stream = new MemoryStream())
                {
                    // serialize data into the xml
                    serializer.Serialize(stream, serializableObject);
                    stream.Position = 0;
                    xmlDocument.Load(stream);
                    xmlDocument.Save(Path.Combine(xmlPath, xmlFileName + ".xml"));
                    stream.Close();
                }
            }
            catch (System.Exception ex)
            {
                Console.Error.Write(ex.Message);
            }
        }

        private static LoadOptions GetLoadOptions(string password)
        {
            LoadOptions loadOptions = new LoadOptions
            {
                Password = password
            };

            return loadOptions;
        }
    }
}