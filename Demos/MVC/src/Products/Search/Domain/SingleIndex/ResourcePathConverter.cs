using System;
using System.Text;

namespace GroupDocs.Total.MVC.Products.Search.Domain.SingleIndex
{
    internal static class ResourcePathConverter
    {
        public static string GetContainerName(Guid folderNameGuid, string fileName, string resourceName)
        {
            int seed = SimpleHashAlgorithm.GetInt32Hash(resourceName);
            var random = new Random(seed);

            var folderNameGuidBytes = folderNameGuid.ToByteArray();
            var fileNameBytes = Encoding.UTF8.GetBytes(fileName);
            var bytes = new byte[folderNameGuidBytes.Length + fileNameBytes.Length];
            random.NextBytes(bytes);
            for (int i = 0; i < folderNameGuidBytes.Length; i++)
            {
                bytes[i] ^= folderNameGuidBytes[i];
            }
            for (int i = 0; i < fileNameBytes.Length; i++)
            {
                bytes[folderNameGuidBytes.Length + i] ^= fileNameBytes[i];
            }

            var result = ByteArrayToString(bytes);
            return result;
        }

        public static Guid GetFolderName(string containerName, string resourceName, out string fileName)
        {
            int seed = SimpleHashAlgorithm.GetInt32Hash(resourceName);
            var random = new Random(seed);

            var bytes = StringToByteArray(containerName);
            var randomBytes = new byte[bytes.Length];
            random.NextBytes(randomBytes);

            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] ^= randomBytes[i];
            }

            var folderNameGuidBytes = new byte[16];
            for (int i = 0; i < folderNameGuidBytes.Length; i++)
            {
                folderNameGuidBytes[i] = bytes[i];
            }
            var folderNameGuid = new Guid(folderNameGuidBytes);

            fileName = Encoding.UTF8.GetString(bytes, folderNameGuidBytes.Length, bytes.Length - folderNameGuidBytes.Length);
            return folderNameGuid;
        }

        private static string ByteArrayToString(byte[] bytes)
        {
            StringBuilder hex = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

        private static byte[] StringToByteArray(string hex)
        {
            int charCount = hex.Length;
            byte[] bytes = new byte[charCount / 2];
            for (int i = 0; i < charCount; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }
    }
}
