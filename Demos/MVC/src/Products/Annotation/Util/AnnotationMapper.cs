using GroupDocs.Annotation.Models;
using GroupDocs.Annotation.Models.AnnotationModels;
using GroupDocs.Annotation.Models.AnnotationModels.Interfaces.Properties;
using GroupDocs.Annotation.Options;
using GroupDocs.Total.MVC.Products.Annotation.Entity.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace GroupDocs.Total.MVC.Products.Annotation.Util
{
    public class AnnotationMapper
    {
        private AnnotationMapper()
        {
        }

        /// <summary>
        /// Map AnnotationInfo instances into AnnotationDataEntity
        /// </summary>
        /// <param name="annotations">AnnotationInfo[]</param>
        /// <param name="pageNumber">int</param>
        /// <returns></returns>
        public static AnnotationDataEntity[] MapForPage(AnnotationBase[] annotations, int pageNumber, PageInfo pageInfo, string documentType)
        {
            // initiate annotations data array
            IList<AnnotationDataEntity> pageAnnotations = new List<AnnotationDataEntity>();
            //  each annotation data - this functionality used since annotations data returned by the
            // GroupDocs.Annotation library are obfuscated
            for (int n = 0; n < annotations.Length; n++)
            {
                AnnotationBase annotationInfo = annotations[n];
                if (pageNumber == annotationInfo.PageNumber + 1)
                {
                    AnnotationDataEntity annotation = MapAnnotationDataEntity(annotationInfo, pageInfo, documentType);
                    pageAnnotations.Add(annotation);
                }
            }

            return pageAnnotations.ToArray();
        }

        /// <summary>
        /// Map AnnotationInfo instances into AnnotationDataEntity
        /// </summary>
        /// <param name="annotationInfo">AnnotationInfo</param>
        /// <returns>AnnotationDataEntity</returns>
        public static AnnotationDataEntity MapAnnotationDataEntity(AnnotationBase annotationInfo, PageInfo pageInfo, string documentType)
        {
            string annotationTypeName = Enum.GetName(typeof(AnnotationType), annotationInfo.Type);
            float maxY = 0, minY = 0, maxX = 0, minX = 0;
            float boxX = 0, boxY = 0, boxHeight = 0, boxWidth = 0;
            string svgPath = "";

            if (annotationInfo is IPoints)
            {
                List<Point> points = ((IPoints)annotationInfo).Points;
                maxY = points.Max(p => p.Y);
                minY = points.Min(p => p.Y);
                maxX = points.Max(p => p.X);
                minX = points.Min(p => p.X);
            }

            if (annotationInfo is IBox)
            {
                Rectangle box = ((IBox)annotationInfo).Box;
                boxX = box.X;
                boxY = box.Y;
                boxHeight = box.Height;
                boxWidth = box.Width;

                StringBuilder builder = new StringBuilder().
                Append("M").Append(box.X.ToString(CultureInfo.InvariantCulture)).
                Append(",").Append(box.Y.ToString(CultureInfo.InvariantCulture)).
                Append("L").Append(box.Width.ToString(CultureInfo.InvariantCulture)).
                Append(",").Append(box.Height.ToString(CultureInfo.InvariantCulture));

                svgPath = builder.ToString();
            }

            AnnotationDataEntity annotation = new AnnotationDataEntity();
            annotation.font = annotationInfo is IFontFamily ? ((IFontFamily)annotationInfo).FontFamily : "";
            double fontSize = annotationInfo is IFontSize ? Convert.ToDouble((((IFontSize)annotationInfo).FontSize == null) ? 0 : ((IFontSize)annotationInfo).FontSize) : (annotationInfo is ITextToReplace ? 10 : 0);
            annotation.fontSize = (float)fontSize;
            annotation.fontColor = annotationInfo is IFontColor ? ((((IFontColor)annotationInfo).FontColor == null) ? 0 : (int)((IFontColor)annotationInfo).FontColor) : 0;
            annotation.height = annotationInfo is IBox ? boxHeight : (annotationInfo is IPoints ? (maxY - minY) : 0);
            annotation.left = annotationInfo is IBox ? boxX : (annotationInfo is IPoints ? minX : 0);
            annotation.pageNumber = (int)annotationInfo.PageNumber + 1;
            annotation.svgPath = annotationInfo is ISvgPath ? (((ISvgPath)annotationInfo).SvgPath?.Replace("l", "L")) : svgPath;
            string text = annotationInfo is IText ? ((IText)annotationInfo).Text : (annotationInfo is ITextToReplace ? ((ITextToReplace)annotationInfo).TextToReplace : "");
            annotation.text = text;
            // TODO: remove comment after check all annotations types on main formats
            annotation.top = annotationInfo is IBox ? boxY : (annotationInfo is IPoints ? (!documentType.Equals("image") ? pageInfo.Height - maxY : minY) : 0);
            annotation.type = char.ToLowerInvariant(annotationTypeName[0]) + annotationTypeName.Substring(1);
            annotation.width = annotationInfo is IBox ? boxWidth : (annotationInfo is IPoints ? (maxX - minX) : 0);
            //  each reply data
            Reply[] replies = annotationInfo.Replies.ToArray();
            if (replies != null && replies.Length > 0)
            {
                CommentsEntity[] comments = new CommentsEntity[replies.Length];
                for (int m = 0; m < replies.Length; m++)
                {
                    CommentsEntity comment = new CommentsEntity();
                    Reply reply = replies[m];
                    comment.text = reply.Comment;
                    comment.time = reply.RepliedOn.ToString("yyyy-MM-dd HH:mm:ss");
                    comment.userName = reply.User.Name;
                    comments[m] = comment;
                }
                annotation.comments = comments;
            }
            return annotation;
        }
    }
}