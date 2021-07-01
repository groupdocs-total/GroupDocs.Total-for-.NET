using System.Collections.Generic;
using System.IO;

namespace GroupDocs.Total.WebForms.Products.Common.Util.Comparator
{
    /// <summary>
    /// FileTypeComparator.
    /// </summary>
    public class FileTypeComparator : IComparer<string>
    {
        /// <summary>
        /// Compare file types.
        /// </summary>
        /// <param name="x">First string to compare.</param>
        /// <param name="y">Second string to compare.</param>
        /// <returns>Compare result as integer.</returns>
        public int Compare(string x, string y)
        {
            string strExt1 = Path.GetExtension(x);
            string strExt2 = Path.GetExtension(y);

            if (strExt1.Equals(strExt2))
            {
                return string.CompareOrdinal(x, y);
            }
            else
            {
                return string.CompareOrdinal(strExt1, strExt2);
            }
        }
    }
}