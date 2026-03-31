using System;
using System.Collections.Generic;
using System.Text;

namespace MyCrypt.Interfaces
{
    internal interface IEncryptionService
    {
        byte[] GenerateRandomKey();
        byte[] ParseKey(string key);        
        void EncryptFile(Stream input, Stream output, byte[] key, string extension);
        void DecryptFile(Stream input, Stream output, byte[] key);
    }
}
