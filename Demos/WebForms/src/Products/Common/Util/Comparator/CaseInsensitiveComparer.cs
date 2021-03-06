using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroupDocs.Total.WebForms.Products.Common.Util.Comparator
{
    /// <summary>
    /// Compares two strings for equivalence, ignoring the case.
    /// </summary>
    public class CaseInsensitiveComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
        }
    }
}