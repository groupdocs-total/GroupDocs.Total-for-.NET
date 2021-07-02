using GroupDocs.Annotation.Models;
using GroupDocs.Annotation.Models.AnnotationModels;
using GroupDocs.Annotation.Options;
using GroupDocs.Total.MVC.Products.Annotation.Entity.Web;
using System;
using System.Linq;

namespace GroupDocs.Total.MVC.Products.Annotation.Annotator
{
    /// <summary>
    /// BaseSigner
    /// </summary>
    public abstract class BaseAnnotator
    {
        public string Message = "Annotation of type {0} for this file type is not supported";
        protected AnnotationDataEntity annotationData;
        protected PageInfo pageInfo;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="annotationData"></param>
        /// <param name="pageInfo"></param>
        protected BaseAnnotator(AnnotationDataEntity annotationData, PageInfo pageInfo)
        {
            this.annotationData = annotationData;
            this.pageInfo = pageInfo;
        }

        /// <summary>
        /// Add area annotation into the Word document
        /// </summary>
        /// <returns>AnnotationBase</returns>
        public abstract AnnotationBase AnnotateWord();

        /// <summary>
        /// Add area annotation into the pdf document
        /// </summary>
        /// <returns>AnnotationBase</returns>
        public abstract AnnotationBase AnnotatePdf();

        ///// <summary>
        ///// Add area annotation into the Excel document
        ///// </summary>
        ///// <returns>AnnotationBase</returns>
        public abstract AnnotationBase AnnotateCells();

        /// <summary>
        /// Add area annotation into the Power Point document
        /// </summary>
        /// <returns>AnnotationBase</returns>
        public abstract AnnotationBase AnnotateSlides();

        /// <summary>
        /// Add area annotation into the image document
        /// </summary>
        /// <returns>AnnotationBase</returns>
        public abstract AnnotationBase AnnotateImage();

        /// <summary>
        /// Add area annotation into the document
        /// </summary>
        /// <returns>AnnotationBase</returns>
        public abstract AnnotationBase AnnotateDiagram();

        /// <summary>
        /// Initial for annotation info
        /// </summary>
        /// <returns>AnnotationBase</returns>
        protected AnnotationBase InitAnnotationBase(AnnotationBase annotationBase)
        {
            // set page number to add annotation
            annotationBase.PageNumber = annotationData.pageNumber - 1;
            // set annotation type
            annotationBase.Type = GetType();
            annotationBase.CreatedOn = DateTime.UtcNow;
            annotationBase.Id = annotationData.id;
            // add replies
            CommentsEntity[] comments = annotationData.comments;
            if (comments != null && comments.Length != 0)
            {
                Reply[] replies = new Reply[comments.Length];
                for (int i = 0; i < comments.Length; i++)
                {
                    Reply reply = GetAnnotationReplyInfo(comments[i]);
                    replies[i] = reply;
                }
                annotationBase.Replies = replies.ToList();
            }
            return annotationBase;
        }

        /// <summary>
        /// Initial for reply annotation info
        /// </summary>
        /// <param name="comment">CommentsEntity</param>
        /// <returns>AnnotationReplyInfo</returns>
        protected virtual Reply GetAnnotationReplyInfo(CommentsEntity comment)
        {
            Reply reply = new Reply();
            reply.Comment = comment.text;
            DateTime date;
            try
            {
                long unixDate = long.Parse(comment.time);
                DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                date = start.AddMilliseconds(unixDate).ToLocalTime();
            }
            catch (System.Exception)
            {
                date = DateTime.Parse(comment.time);
            }
            reply.RepliedOn = date;
            reply.User = new User { Name = comment.userName };
            return reply;
        }

        /// <summary>
        /// Get rectangle
        /// </summary>
        /// <returns>Rectangle</returns>
        protected virtual Rectangle GetBox()
        {
            return new Rectangle(annotationData.left, annotationData.top, annotationData.width, annotationData.height);
        }

        /// <summary>
        /// Get type of annotation
        /// </summary>
        /// <returns>byte</returns>
        protected new abstract AnnotationType GetType();

        /// <summary>
        /// Get Annotation info depending on document type
        /// </summary>
        /// <param name="documentType">string</param>
        /// <returns>AnnotationBase</returns>
        public AnnotationBase GetAnnotationBase(string documentType)
        {
            switch (documentType)
            {
                case "Portable Document Format":
                    return AnnotatePdf();
                case "Microsoft Word":
                case "Open Document Text":
                    return AnnotateWord();
                case "Rich Text Format":
                    return AnnotateWord();
                case "Microsoft PowerPoint":
                    return AnnotateSlides();
                case "image":
                    return AnnotateImage();
                case "Microsoft Excel":
                    return AnnotateCells();
                case "AutoCAD Drawing File Format":
                    return AnnotateDiagram();
                default:
                    throw new System.Exception("Wrong annotation data without document type!");
            }
        }

        /// <summary>
        /// Check if the current annotatin is supported
        /// </summary>
        /// <param name="documentType">string</param>
        /// <returns></returns>
        internal bool IsSupported(string documentType)
        {
            try
            {
                AnnotatorFactory.createAnnotator(annotationData, pageInfo).GetAnnotationBase(documentType);
                return true;
            }
            catch (NotSupportedException)
            {
                Message = string.Format(Message, annotationData.type);
                return false;
            }
        }
    }
}