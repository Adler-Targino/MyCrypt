namespace MyCrypt.Helpers
{
    internal static class StreamHelper
    {
        private const int DefaultBufferSize = 16384;

        public static void ReadBuffered(Stream input, long length, Action<byte[], int> onChunk, int bufferSize = DefaultBufferSize)
        {
            byte[] buffer = new byte[bufferSize];
            long remainingBytes = length;

            while (remainingBytes > 0)
            {
                int bytesToRead = (int)Math.Min(buffer.Length, remainingBytes);
                int bytesReaden = input.Read(buffer, 0, bytesToRead);

                if (bytesReaden == 0)
                    throw new EndOfStreamException(
                        "Unexpected end of stream.");

                onChunk(buffer, bytesReaden);

                remainingBytes -= bytesReaden;
            }
        }
    }
}
