namespace MyCrypt.Services
{
    internal static class KeyManagementService
    {
        public static void ExportKey(byte[] key, string filePath)
        {
            using var stream = File.Create(filePath);
            using var writer = new BinaryWriter(stream);

            writer.Write("MYCK");
            writer.Write(key.Length);
            writer.Write(key);
        }

        public static byte[] ImportKey(string filePath)
        {
            using var stream = File.OpenRead(filePath);
            using var reader = new BinaryReader(stream);

            string magic = reader.ReadString();

            if (magic != "MYCK")
                throw new InvalidDataException("Invalid key file.");

            int length = reader.ReadInt32();

            return reader.ReadBytes(length);
        }
    }
}