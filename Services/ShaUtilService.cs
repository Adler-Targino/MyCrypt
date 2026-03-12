using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MyCrypt.Services
{
    internal static class ShaUtilService
    {
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
