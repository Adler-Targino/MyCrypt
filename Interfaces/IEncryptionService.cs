using MyCrypt.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCrypt.Interfaces
{
    internal interface IEncryptionService
    {
        byte[] GenerateRandomKey();
        byte[] ParseKey(string key);
        void EncryptFile(Stream input, Stream output, byte[] key, EncryptedFileHeader fileHeader);
        void DecryptFile(Stream input, Stream output, byte[] key);
    }
}
