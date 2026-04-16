using System;
using System.Collections.Generic;
using System.Text;

namespace MyCrypt.Interfaces
{
    internal interface ICompressionService
    {
        void CompressData(Stream input, Stream output);
        void DecompressData(Stream input, Stream output);
    }
}
