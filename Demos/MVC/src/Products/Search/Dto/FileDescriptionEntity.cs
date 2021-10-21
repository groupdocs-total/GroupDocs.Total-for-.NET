﻿namespace GroupDocs.Total.MVC.Products.Search.Dto
{
    public class FileDescriptionEntity
    {
        /// <summary>
        /// Absolute path to the file/directory.
        /// </summary>
        public string guid { get; set; }

        /// <summary>
        /// Name of the file/directory.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Document type.
        /// </summary>
        public string docType { get; set; }

        /// <summary>
        /// File or directory flag.
        /// </summary>
        public bool isDirectory { get; set; }

        /// <summary>
        /// File size.
        /// </summary>
        public long size { get; set; }
    }
}
