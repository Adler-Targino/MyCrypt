using System.Text;

namespace MyCrypt.Models
{
    //Encrypted file format
    //[MAGIC][VERSION][FLAGS][ENCRYPTION][COMPRESSION][MAC][EXT_LEN][EXT][INITDATA][DATA][MAC?]
    internal class EncryptedFileHeader
    {
        public byte Version { get; set; }
        public byte[] ExtensionBytes { get; set; } = new byte[0];
        public CryptoFlags Flags { get; set; }
        public EncryptionType Encryption { get; set; }
        public CompressionType Compression { get; set; }
        public MacType Mac { get; set; }

        public static void WriteHeaderToStream(EncryptedFileHeader fileHeader, Stream stream)
        {
            stream.Write(Encoding.ASCII.GetBytes("MYCR"));
            stream.WriteByte(fileHeader.Version);
            stream.WriteByte((byte)fileHeader.Flags);
            stream.WriteByte((byte)fileHeader.Encryption);
            stream.WriteByte((byte)fileHeader.Compression);
            stream.WriteByte((byte)fileHeader.Mac);
            stream.WriteByte((byte)fileHeader.ExtensionBytes.Length);
            stream.Write(fileHeader.ExtensionBytes);
        }

        public static EncryptedFileHeader ReadHeaderFromStream(Stream stream)
        {
            EncryptedFileHeader fileHeader = new EncryptedFileHeader();

            byte[] signature = new byte[4];
            stream.ReadExactly(signature);

            string magic = Encoding.ASCII.GetString(signature);
            if (magic != "MYCR")
                throw new InvalidDataException("Invalid MyCrypt file.");

            fileHeader.Version = (byte)stream.ReadByte();
            fileHeader.Flags = (CryptoFlags)stream.ReadByte();
            fileHeader.Encryption = (EncryptionType)stream.ReadByte();
            fileHeader.Compression = (CompressionType)stream.ReadByte();
            fileHeader.Mac = (MacType)stream.ReadByte();

            int extensionLength = stream.ReadByte();
            if (extensionLength < 0)
                throw new EndOfStreamException("Invalid ou corrupted file");

            fileHeader.ExtensionBytes = new byte[extensionLength];
            stream.ReadExactly(fileHeader.ExtensionBytes);

            return fileHeader;
        }

        public static int GetMacLength(MacType mac)
        {
            switch (mac)
            {
                case MacType.HmacSha256:
                    return 32;
                default:
                    return 0;
            }
        }
    }

    [Flags]
    public enum CryptoFlags : byte
    {
        None = 0,
        Hmac = 1 << 0,
        Compressed = 1 << 1,
    }

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
