using GroupDocs.Total.MVC.Products.Common.Util.Comparator;
using GroupDocs.Total.MVC.Products.Signature.Entity.Web;
using GroupDocs.Total.MVC.Products.Signature.Entity.Xml;
using GroupDocs.Total.MVC.Products.Signature.Util.Directory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace GroupDocs.Total.MVC.Products.Signature.Loader
{
    /// <summary>
    /// SignatureLoader
    /// </summary>
    public class SignatureLoader
    {
        public string CurrentPath;
        public Common.Config.GlobalConfiguration globalConfiguration;
        private readonly DirectoryUtils DirectoryUtils;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">string</param>
        /// <param name="globalConfiguration">Common.Config.GlobalConfiguration</param>
        public SignatureLoader(string path, Common.Config.GlobalConfiguration globalConfiguration, DirectoryUtils directoryUtils)
        {
            CurrentPath = path;
            this.globalConfiguration = globalConfiguration;
            DirectoryUtils = directoryUtils;
        }

        /// <summary>
        /// Load image signatures
        /// </summary>
        /// <returns>List[SignatureFileDescriptionEntity]</returns>
        public List<SignatureFileDescriptionEntity> LoadImageSignatures()
        {
            string[] files = Directory.GetFiles(CurrentPath, "*.*", SearchOption.TopDirectoryOnly);
            List<string> allFiles = new List<string>(files);
            List<SignatureFileDescriptionEntity> fileList = new List<SignatureFileDescriptionEntity>();
            try
            {
                allFiles.Sort(new FileDateComparator());
                allFiles.Sort(new FileNameComparator());

                foreach (string file in allFiles)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    // check if current file/folder is hidden
                    if (fileInfo.Attributes.HasFlag(FileAttributes.Hidden) || 
                        file.Equals(globalConfiguration.GetSignatureConfiguration().GetDataDirectory()))
                    {
                        // ignore current file and skip to next one
                        continue;
                    }
                    else
                    {
                        SignatureFileDescriptionEntity fileDescription = new SignatureFileDescriptionEntity
                        {
                            guid = Path.GetFullPath(file),
                            name = Path.GetFileName(file),
                            // set is directory true/false
                            isDirectory = fileInfo.Attributes.HasFlag(FileAttributes.Directory),
                            // set file size
                            size = fileInfo.Length
                        };
                        // get image Base64 incoded string
                        byte[] imageArray = File.ReadAllBytes(file);
                        string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                        fileDescription.image = base64ImageRepresentation;
                        // add object to array list
                        fileList.Add(fileDescription);
                    }
                }
                return fileList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Load digital signatures or documents
        /// </summary>
        /// <returns>List[SignatureFileDescriptionEntity]</returns>
        public List<SignatureFileDescriptionEntity> LoadFiles()
        {
            List<string> allFiles = new List<string>(Directory.GetFiles(CurrentPath));
            allFiles.AddRange(Directory.GetDirectories(CurrentPath));
            List<SignatureFileDescriptionEntity> fileList = new List<SignatureFileDescriptionEntity>();
            string dataDirectory = globalConfiguration.GetSignatureConfiguration().GetDataDirectory();
            string outputDirectory = globalConfiguration.GetSignatureConfiguration().GetFilesDirectory() +
                DirectoryUtils.GetTempFolder().OUTPUT_FOLDER;

            try
            {
                allFiles.Sort(new FileNameComparator());
                allFiles.Sort(new FileDateComparator());

                foreach (string file in allFiles)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    // check if current file/folder is hidden
                    if (!fileInfo.Attributes.HasFlag(FileAttributes.Hidden) &&
                        !Path.GetFileName(file).StartsWith(".") &&
                        !Path.GetFileName(file).Equals(Path.GetFileName(dataDirectory), StringComparison.OrdinalIgnoreCase) &&
                        !Path.GetFileName(file).Equals(Path.GetFileName(outputDirectory), StringComparison.OrdinalIgnoreCase))
                    {
                        SignatureFileDescriptionEntity fileDescription = new SignatureFileDescriptionEntity
                        {
                            guid = Path.GetFullPath(file),
                            name = Path.GetFileName(file),
                            // set is directory true/false
                            isDirectory = fileInfo.Attributes.HasFlag(FileAttributes.Directory)
                        };
                        // set file size
                        if (!fileDescription.isDirectory)
                        {
                            fileDescription.size = fileInfo.Length;
                        }
                        // add object to array list
                        fileList.Add(fileDescription);
                    }
                }
                return fileList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Load stamps
        /// </summary>
        /// <param name="previewFolder">string</param>
        /// <param name="xmlFolder">string</param>
        /// <returns>List[SignatureFileDescriptionEntity]</returns>
        public List<SignatureFileDescriptionEntity> LoadStampSignatures(string previewFolder, string xmlFolder, string signatureType)
        {
            string imagesPath = CurrentPath + previewFolder;
            string xmlPath = CurrentPath + xmlFolder;
            string[] imageFiles = Directory.GetFiles(imagesPath, "*.png", SearchOption.TopDirectoryOnly);
            // get all files from the directory
            List<SignatureFileDescriptionEntity> fileList = new List<SignatureFileDescriptionEntity>();
            try
            {
                if (imageFiles != null && imageFiles.Length > 0)
                {
                    string[] xmlFiles = Directory.GetFiles(xmlPath);
                    List<string> filesList = new List<string>();
                    foreach (string imageFile in imageFiles)
                    {
                        foreach (string xmlFile in xmlFiles)
                        {
                            if (Path.GetFileNameWithoutExtension(xmlFile).Equals(Path.GetFileNameWithoutExtension(imageFile)))
                            {
                                filesList.Add(imageFile);
                            }
                        }
                    }
                    // sort list of files and folders
                    filesList.Sort(new FileDateComparator());
                    filesList.Sort(new FileNameComparator());
                    foreach (string file in filesList)
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        // check if current file/folder is hidden
                        if (fileInfo.Attributes.HasFlag(FileAttributes.Hidden) || 
                            file.Equals(globalConfiguration.GetSignatureConfiguration().GetDataDirectory()))
                        {
                            // ignore current file and skip to next one
                            continue;
                        }
                        else
                        {
                            SignatureFileDescriptionEntity fileDescription = new SignatureFileDescriptionEntity
                            {
                                guid = Path.GetFullPath(file),
                                name = Path.GetFileName(file),
                                // set is directory true/false
                                isDirectory = fileInfo.Attributes.HasFlag(FileAttributes.Directory),
                                // set file size
                                size = fileInfo.Length
                            };
                            // get image Base64 incoded string
                            byte[] imageArray = File.ReadAllBytes(file);
                            string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                            fileDescription.image = base64ImageRepresentation;
                            if ("qrCode".Equals(signatureType) || "barCode".Equals(signatureType))
                            {
                                // get stream of the xml file
                                StreamReader xmlStream = new StreamReader(Path.Combine(xmlPath, Path.GetFileNameWithoutExtension(file) + ".xml"));
                                // initiate serializer
                                XmlSerializer serializer = null;
                                serializer = new XmlSerializer(typeof(OpticalXmlEntity));
                                // deserialize XML into the object
                                OpticalXmlEntity xmlData = (OpticalXmlEntity)serializer.Deserialize(xmlStream);
                                fileDescription.text = xmlData.text;
                                xmlStream.Close();
                                xmlStream.Dispose();
                            }
                            // add object to array list
                            fileList.Add(fileDescription);
                        }
                    }
                }
                return fileList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SignatureFileDescriptionEntity> LoadTextSignatures(string xmlFolder)
        {
            try
            {
                string xmlPath = CurrentPath + xmlFolder;
                string[] xmlFiles = Directory.GetFiles(xmlPath);
                // get all files from the directory
                List<SignatureFileDescriptionEntity> fileList = new List<SignatureFileDescriptionEntity>();
                foreach (string xmlFile in xmlFiles)
                {
                    SignatureFileDescriptionEntity fileDescription = new SignatureFileDescriptionEntity
                    {
                        guid = xmlFile,
                        name = Path.GetFileName(xmlFile)
                    };
                    // get stream of the xml file
                    StreamReader xmlStream = new StreamReader(xmlFile);
                    // initiate serializer
                    XmlSerializer serializer = new XmlSerializer(typeof(TextXmlEntity));
                    // deserialize XML into the object
                    TextXmlEntity xmlData = (TextXmlEntity)serializer.Deserialize(xmlStream);
                    fileDescription.text = xmlData.text;
                    fileDescription.fontColor = xmlData.fontColor;
                    xmlStream.Close();
                    xmlStream.Dispose();
                    // add object to array list
                    fileList.Add(fileDescription);
                }
                return fileList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}