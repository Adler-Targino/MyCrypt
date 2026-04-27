using System.Text;

namespace MyCrypt.Models
{
    //Encryption key file format
    //[MAGIC][VERSION][ENCRYPTION][DATA]
    internal class EncryptionKeyFile
    {
        private const int EncryptionKeyVersion = 1;
        private static readonly byte[] EncryptionKeySignature = { 77, 89, 67, 75 }; //MYCK

        public byte[] Magic { get; set; } = EncryptionKeySignature;
        public byte Version { get; set; } = EncryptionKeyVersion;
        public EncryptionType Encryption { get; set; }
        public byte[] KeyData { get; set; } = new byte[0];

        public static void ExportKey(byte[] key, EncryptionType encryption, string filePath)
        {
            EncryptionKeyFile encryptionKeyFile = new EncryptionKeyFile();
            encryptionKeyFile.Encryption = encryption;
            encryptionKeyFile.KeyData = key;

            using var stream = File.Create(filePath);

            stream.Write(encryptionKeyFile.Magic);
            stream.WriteByte(encryptionKeyFile.Version);
            stream.WriteByte((byte)encryptionKeyFile.Encryption);
            stream.Write(encryptionKeyFile.KeyData);
            stream.Dispose();
        }

        public static byte[] ImportKey(string filePath, EncryptionType encryption)
        {
            EncryptionKeyFile encryptionKeyFile = new EncryptionKeyFile();
            using var stream = File.OpenRead(filePath);

            byte[] magic = new byte[4];
            stream.ReadExactly(magic);

            encryptionKeyFile.Magic = magic;
            encryptionKeyFile.Version = (byte)stream.ReadByte();
            encryptionKeyFile.Encryption = (EncryptionType)stream.ReadByte();

            var buffer = new byte[(int)(stream.Length - stream.Position)];
            stream.ReadExactly(buffer);

            encryptionKeyFile.KeyData = buffer;

            if (!encryptionKeyFile.Magic.SequenceEqual(EncryptionKeySignature))
                throw new InvalidDataException("Invalid key file.");

            if (encryptionKeyFile.Version != EncryptionKeyVersion)
                throw new InvalidDataException("Incompatible key version.");

            if (encryptionKeyFile.Encryption != encryption)
                throw new InvalidDataException("Incompatible key file.");

            stream.Dispose();

            return encryptionKeyFile.KeyData;
        }
    }
}
