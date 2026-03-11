using MyCrypt.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MyCrypt.Services
{
    internal class AesUtilService : IAesUtilService
    {
        private readonly IRngService _rngService;

        public AesUtilService(IRngService rngService)
        {
            _rngService = rngService;
        }

        public byte[] DecryptFile(byte[] content, byte[] key)
        {
            throw new NotImplementedException();
        }

        public void EncryptFile(Stream input, Stream output, byte[] key, string extension)
        {
            try
            {
                var extBytes = Encoding.UTF8.GetBytes(extension);

                output.Write(Encoding.ASCII.GetBytes("MYCR"));
                output.WriteByte((byte)extBytes.Length);
                output.Write(extBytes); 

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    byte[] iv = aes.IV;

                    output.Write(iv, 0, iv.Length);

                    using var crypto = new CryptoStream(output, aes.CreateEncryptor(), CryptoStreamMode.Write);

                    input.CopyTo(crypto);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Encryption failed. {ex}");
            }
        }

        public byte[] GenerateRandomKey()
        {
            return _rngService.GenerateRandomBytes(32);
        }
    }
}
