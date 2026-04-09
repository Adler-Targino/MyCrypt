using MyCrypt.Helpers;
using MyCrypt.Interfaces;
using MyCrypt.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MyCrypt.Services
{
    internal class AesEncryptionService : IEncryptionService
    {
        private readonly IRngService _rngService;
        private static readonly int[] AesValidStringKeySizes = { 24, 32, 44 };
        public AesEncryptionService(IRngService rngService)
        {
            _rngService = rngService;
        }

        public void DecryptFile(Stream input, Stream output, byte[] key, EncryptedFileHeader fileHeader)
        {
            long dataStartPosition = input.Position;
            int macSize = EncryptedFileHeader.GetMacLength(fileHeader.Mac);

            //HMAC VALIDATION
            if (fileHeader.Mac != MacType.None)
            {
                input.Position = 0;
                long contentSize = input.Length - macSize;

                byte[] hmac = fileHeader.Mac switch
                {
                    MacType.HMACSHA256 => ShaUtilService.HashHMACSHA256(key, input, contentSize),
                    _ => new byte[0]
                };

                byte[] storedHmac = new byte[macSize];
                input.ReadExactly(storedHmac);

                if(!CryptographicOperations.FixedTimeEquals(hmac, storedHmac))
                    throw new CryptographicException("HMAC validation failed. The file may be corrupted or the key is incorrect.");

                input.Position = dataStartPosition;
            }

            //DATA DECOMPRESSION


            //DECRYPTION
            using (Aes aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                byte[] iv = new byte[aes.BlockSize / 8];
                input.ReadExactly(iv);

                aes.Key = key;
                aes.IV = iv;

                long encryptedDataSize = input.Length - input.Position - macSize;

                using var crypto = new CryptoStream(output, aes.CreateDecryptor(), CryptoStreamMode.Write);

                StreamHelper.ReadBuffered(input, encryptedDataSize, (buffer, read) =>
                {
                    crypto.Write(buffer, 0, read);
                });

                crypto.FlushFinalBlock();
            }
        }

        public void EncryptFile(Stream input, Stream output, byte[] key, EncryptedFileHeader fileHeader)
        {
            // WRITE HEADER
            EncryptedFileHeader.WriteHeaderToStream(fileHeader, output);            

            //DATA ENCRYPTION
            using (Aes aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                aes.Key = key;
                byte[] iv = aes.IV;

                output.Write(iv, 0, iv.Length);

                using var crypto = new CryptoStream(output, aes.CreateEncryptor(), CryptoStreamMode.Write, leaveOpen: true);

                input.CopyTo(crypto);
            }

            //DATA COMPRESSION

            //HMAC
            if(fileHeader.Mac != MacType.None)
            {
                output.Position = 0;

                byte[] hmac = new byte[0];
                switch (fileHeader.Mac)
                {
                    case MacType.HMACSHA256:
                        hmac = ShaUtilService.HashHMACSHA256(key, output, output.Length);
                        break;
                }

                output.Position = output.Length;
                output.Write(hmac);
            }
        }

        public byte[] GenerateRandomKey()
        {
            return _rngService.GenerateRandomBytes(32);
        }

        public byte[] ParseKey(string key)
        {
            if (AesValidStringKeySizes.Contains(key.Length))
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
