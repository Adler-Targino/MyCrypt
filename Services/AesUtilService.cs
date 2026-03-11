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
        private static readonly int[] AesValidKeySizes = { 16, 24, 32 };
        public AesUtilService(IRngService rngService)
        {
            _rngService = rngService;
        }

        public void DecryptFile(Stream input, Stream output, byte[] key)
        {
            try
            {
                using (Aes aes = Aes.Create())
                {
                    input.Seek(4, SeekOrigin.Current);

                    int extLength = input.ReadByte();
                    input.Seek(extLength, SeekOrigin.Current);

                    byte[] iv = new byte[aes.BlockSize / 8];
                    input.ReadExactly(iv);

                    aes.Key = key;
                    aes.IV = iv;

                    using var crypto = new CryptoStream(input, aes.CreateDecryptor(), CryptoStreamMode.Read);

                    crypto.CopyTo(output);
                }

            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Decryption failed. {ex}");
            }
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

        public byte[] ParseKey(string key)
        {
            if (AesValidKeySizes.Contains(key.Length))
            {
                return Convert.FromBase64String(key);
            }
            else
            {
                using var sha = SHA256.Create();
                return sha.ComputeHash(Encoding.UTF8.GetBytes(key));
            }
        }
    }
}
