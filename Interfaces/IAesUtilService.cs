using System;
using System.Collections.Generic;
using System.Text;

namespace MyCrypt.Interfaces
{
    internal interface IAesUtilService
    {
        byte[] GenerateRandomKey();
        byte[] ParseKey(string key);
        void EncryptFile(Stream input, Stream output, byte[] key, string extension);
        byte[] DecryptFile(byte[] content, byte[] key);
    }
}
