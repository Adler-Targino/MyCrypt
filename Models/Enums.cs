using System;
using System.Collections.Generic;
using System.Text;

namespace MyCrypt.Models
{
    public enum EncryptionType : byte
    {
        Aes = 0,
    }

    public enum CompressionType : byte
    {
        None = 0,
        GZip = 1,
    }

    public enum MacType : byte
    {
        None = 0,
        HmacSha256 = 1,
    }
}
