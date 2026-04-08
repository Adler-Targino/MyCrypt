using MyCrypt.Models;
using System.Text;

namespace MyCrypt.Helpers
{
    internal static class PathResolvingHelper
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
            EncryptedFileHeader fileHeader = EncryptedFileHeader.ReadHeaderFromStream(inputStream);
            inputStream.Close();

            string extension = Encoding.UTF8.GetString(fileHeader.ExtensionBytes);

            return !string.IsNullOrWhiteSpace(output) ?
                           Path.IsPathRooted(output) ?
                           Path.ChangeExtension(output, extension) :
                           Path.ChangeExtension(
                                Path.Combine(Directory.GetCurrentDirectory(), output),
                                extension) :
                           Path.ChangeExtension(input.FullName, extension);
        }

        public static string ResolveKeyFileName(string output)
        {
            return Path.IsPathRooted(output) ?
                   Path.ChangeExtension(output, ".myk") :
                   Path.ChangeExtension(
                       Path.Combine(Directory.GetCurrentDirectory(), output),
                       ".myk");
        }
    }
}
