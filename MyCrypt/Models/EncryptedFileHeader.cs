using System.Reflection;
using System.Text;

namespace MyCrypt.Models
{
    //Encrypted file format
    //[MAGIC][VERSION][FLAGS][ENCRYPTION][COMPRESSION][MAC][EXT_LEN][EXT][INITDATA][DATA][MAC?]
    internal class EncryptedFileHeader
    {
        private const int EncryptedFileVersion = 1;
        private static readonly byte[] EncryptedFileSignature = { 77, 89, 67, 82 }; //MYCR

        public byte[] Magic { get; set; } = EncryptedFileSignature;
        public byte Version { get; set; } = EncryptedFileVersion;
        public byte[] ExtensionBytes { get; set; } = new byte[0];
        public EncryptionType Encryption { get; set; }
        public CompressionType Compression { get; set; }
        public MacType Mac { get; set; }

        public static void WriteHeaderToStream(EncryptedFileHeader fileHeader, Stream stream)
        {
            stream.Write(fileHeader.Magic);
            stream.WriteByte(fileHeader.Version);
            stream.WriteByte((byte)fileHeader.Encryption);
            stream.WriteByte((byte)fileHeader.Compression);
            stream.WriteByte((byte)fileHeader.Mac);
            stream.WriteByte((byte)fileHeader.ExtensionBytes.Length);
            stream.Write(fileHeader.ExtensionBytes);
        }

        public static EncryptedFileHeader ReadHeaderFromStream(Stream stream)
        {
            EncryptedFileHeader fileHeader = new EncryptedFileHeader();

            byte[] magic = new byte[4];
            stream.ReadExactly(magic);
            
            fileHeader.Magic = magic;
            fileHeader.Version = (byte)stream.ReadByte();
            fileHeader.Encryption = (EncryptionType)stream.ReadByte();
            fileHeader.Compression = (CompressionType)stream.ReadByte();
            fileHeader.Mac = (MacType)stream.ReadByte();


            if (!fileHeader.Magic.SequenceEqual(EncryptedFileSignature))
                throw new InvalidDataException("Invalid MyCrypt file.");

            if (fileHeader.Version != EncryptedFileVersion)
                throw new InvalidDataException("Incompatible MyCrypt file version.");

            int extensionLength = stream.ReadByte();
            if (extensionLength < 0)
                throw new EndOfStreamException("Invalid ou corrupted file");

            fileHeader.ExtensionBytes = new byte[extensionLength];
            stream.ReadExactly(fileHeader.ExtensionBytes);

            return fileHeader;
        }

        public static int GetMacLength(MacType mac)
        {
            return mac switch
            {
                MacType.HMACSHA256 => 32,
                _ => 0
            };
        }
    }    
}
