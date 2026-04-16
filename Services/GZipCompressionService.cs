using MyCrypt.Interfaces;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;

namespace MyCrypt.Services
{
    internal class GZipCompressionService : ICompressionService
    {
        public void CompressData(Stream input, Stream output)
        {
            using var compressor = new GZipStream(output, CompressionMode.Compress, leaveOpen: true);
            input.CopyTo(compressor);
        }

        public void DecompressData(Stream input, Stream output)
        {
            using var decompressor = new GZipStream(input, CompressionMode.Decompress, leaveOpen: true);
            decompressor.CopyTo(output);
        }
    }
}
