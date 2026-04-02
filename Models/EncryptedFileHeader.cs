namespace MyCrypt.Models
{
    //Encrypted file format
    //[MAGIC][VERSION][FLAGS][ENCRYPTION][COMPRESSION][MAC][EXT_LEN][EXT][INIT_LEN][INITDATA][DATA_LEN][DATA][MAC?]
    internal class EncryptedFileHeader
    {
        public byte Version { get; set; }
        public CryptoFlags Flags { get; set; }
        public EncryptionType Encryption { get; set; }
        public CompressionType Compression { get; set; }
        public MacType Mac { get; set; }
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
