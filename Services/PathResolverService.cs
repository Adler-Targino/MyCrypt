using System;
using System.Collections.Generic;
using System.Text;

namespace MyCrypt.Services
{
    internal static class PathResolverService
    {
        public static string ResolveEncryptedFileName(FileInfo input, string? output)
        {
            return !string.IsNullOrWhiteSpace(output) ?
                           Path.IsPathRooted(output) ?
                           Path.ChangeExtension(output, ".myc") :
                           Path.ChangeExtension(
                                Path.Combine(Directory.GetCurrentDirectory(), output),
                                ".myc") :
                           Path.ChangeExtension(input.FullName, ".myc");
        }

        public static string ResolveDecryptedtFileName(FileInfo input, string? output)
        {
            using var inputStream = input.OpenRead();
            byte[] magic = new byte[4];
            inputStream.ReadExactly(magic);

            if (Encoding.ASCII.GetString(magic) != "MYCR")
                throw new InvalidDataException("Invalid MyCrypt file.");

            int extLength = inputStream.ReadByte();
            if (extLength < 0)
                throw new EndOfStreamException("Invalid MyCrypt file.");
            byte[] extBytes = new byte[extLength];
            inputStream.ReadExactly(extBytes);

            inputStream.Close();

            string extension = Encoding.UTF8.GetString(extBytes);

            return !string.IsNullOrWhiteSpace(output) ?
                           Path.IsPathRooted(output) ?
                           Path.ChangeExtension(output, extension) :
                           Path.ChangeExtension(
                                Path.Combine(Directory.GetCurrentDirectory(), output),
                                extension) :
                           Path.ChangeExtension(input.FullName, extension);
        }
    }
}
