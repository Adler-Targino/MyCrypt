using System;
using System.Collections.Generic;
using System.Text;

namespace MyCrypt.Helpers
{
    //How to access part of a FileStream or MemoryStream
    //https://learn.microsoft.com/en-us/archive/msdn-technet-forums/c409b63b-37df-40ca-9322-458ffe06ea48
    internal class SubStream : Stream
    {
        private Stream baseStream;
        private readonly long length;
        private long position;
        public SubStream(Stream baseStream, long offset, long length)
        {
            ArgumentNullException.ThrowIfNull(baseStream);
            ArgumentOutOfRangeException.ThrowIfNegative(offset);
            if (!baseStream.CanRead) throw new ArgumentException("Unable to read base stream");

            this.baseStream = baseStream;
            this.length = length;

            if (baseStream.CanSeek)
            {
                baseStream.Seek(offset, SeekOrigin.Current);
            }
            else
            {
                byte[] buffer = new byte[StreamHelper.DefaultBufferSize];
                while (offset > 0)
                {
                    int read = baseStream.Read(buffer, 0, offset < StreamHelper.DefaultBufferSize ? (int)offset : StreamHelper.DefaultBufferSize);

                    if (read == 0)
                        throw new EndOfStreamException("Unexpected end of stream.");

                    offset -= read;
                }
            }
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            CheckDisposed();
            long remaining = length - position;
            if (remaining <= 0) return 0;
            if (remaining < count) count = (int)remaining;
            int read = baseStream.Read(buffer, offset, count);
            position += read;
            return read;
        }
        private void CheckDisposed()
        {
            if (baseStream == null) throw new ObjectDisposedException(GetType().Name);
        }
        public override long Length
        {
            get { CheckDisposed(); return length; }
        }
        public override bool CanRead
        {
            get { CheckDisposed(); return true; }
        }
        public override bool CanWrite
        {
            get { CheckDisposed(); return false; }
        }
        public override bool CanSeek
        {
            get { CheckDisposed(); return false; }
        }
        public override long Position
        {
            get
            {
                CheckDisposed();
                return position;
            }
            set { throw new NotSupportedException(); }
        }
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }
        public override void Flush()
        {
            CheckDisposed(); 
            baseStream.Flush();
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (baseStream != null)
                {
                    try { baseStream.Dispose(); }
                    catch { }
                    baseStream = null;
                }
            }
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }

}
