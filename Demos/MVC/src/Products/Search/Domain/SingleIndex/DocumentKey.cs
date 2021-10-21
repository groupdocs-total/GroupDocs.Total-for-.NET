using System;

namespace GroupDocs.Total.MVC.Products.Search.Domain.SingleIndex
{
    internal class DocumentKey
    {
        private readonly string _userId;
        private readonly string _fileName;

        public DocumentKey(string userId, string fileName)
        {
            if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException(nameof(userId));
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException(nameof(fileName));

            _userId = userId;
            _fileName = fileName;
        }

        public static bool TryCreateFromString(string documentKeyString, out DocumentKey documentKey)
        {
            int separatorIndex = documentKeyString.IndexOf(Settings.DocumentKeySeparator);
            if (separatorIndex < 0)
            {
                documentKey = null;
                return false;
            }

            var userId = documentKeyString.Substring(0, separatorIndex);
            var fileName = documentKeyString.Substring(separatorIndex + 1);
            documentKey = new DocumentKey(userId, fileName);
            return true;
        }

        public string UserId => _userId;

        public string FileName => _fileName;

        public override int GetHashCode()
        {
            return _userId.GetHashCode() ^ _fileName.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as DocumentKey;
            if (other == null)
            {
                return false;
            }

            return _userId == other._userId && _fileName == other._fileName;
        }

        public override string ToString()
        {
            return _userId + Settings.DocumentKeySeparator + _fileName;
        }
    }
}
