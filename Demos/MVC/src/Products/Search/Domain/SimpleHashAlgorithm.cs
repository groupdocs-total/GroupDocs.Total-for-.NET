namespace GroupDocs.Total.MVC.Products.Search.Domain
{
    internal static class SimpleHashAlgorithm
    {
        public static int GetInt32Hash(string text)
        {
            ulong hash = GetUInt64Hash(text);
            return unchecked((int)hash);
        }

        public static ulong GetUInt64Hash(string text)
        {
            ulong value = 3074457345618258791u;
            for (int i = 0; i < text.Length; i++)
            {
                value += text[i];
                value *= 3074457345618258799u;
            }
            return value;
        }
    }
}
