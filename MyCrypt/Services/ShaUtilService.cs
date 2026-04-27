using MyCrypt.Helpers;
using System.Security.Cryptography;
using System.Text;

namespace MyCrypt.Services
{
    internal static class ShaUtilService
    {
        public static byte[] HashHMACSHA256(byte[] key, Stream input, long length)
        {
            using var hmac = new HMACSHA256(key);

            StreamHelper.ReadBuffered(input, length, (buffer, read) =>
            {
                hmac.TransformBlock(buffer, 0, read, null, 0);
            });

            hmac.TransformFinalBlock(Array.Empty<byte>(), 0, 0);

            return hmac.Hash!;
        }

        public static string ComputeSHA384(Stream input)
        {
            using (SHA384 sha384 = SHA384.Create())
            {
                using (var memoryStream = new MemoryStream())
                {
                    input.CopyTo(memoryStream);
                    
                    byte[] inputBytes = memoryStream.ToArray();
                    byte[] hashBytes = sha384.ComputeHash(inputBytes);
                    
                    StringBuilder sb = new StringBuilder();
                    foreach (byte b in hashBytes)
                        sb.Append(b.ToString("x2"));

                    return sb.ToString();
                }                
            }
        }

        public static bool ValidateSHA384(Stream input, string expectedHash)
        {
            string computedHash = ComputeSHA384(input);
            return string.Equals(computedHash, expectedHash, StringComparison.OrdinalIgnoreCase);
        }
    }
}
