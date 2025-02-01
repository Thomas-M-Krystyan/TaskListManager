namespace TaskList.ConsoleApp.Tests.Functional._Utilities
{
    internal sealed class BlockingStream : Stream
    {
        private readonly Stream _underlyingStream;

        internal BlockingStream(Stream underlyingStream)
        {
            _underlyingStream = underlyingStream;
        }

        public override void Flush()
        {
            lock (_underlyingStream)
            {
                _underlyingStream.Flush();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int read = 0;

            while (true)
            {
                lock (_underlyingStream)
                {
                    read = _underlyingStream.Read(buffer, offset, count);
                    if (read > 0)
                    {
                        return read;
                    }
                }

                _ = Thread.Yield();
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            lock (_underlyingStream)
            {
                return _underlyingStream.Seek(offset, origin);
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            lock (_underlyingStream)
            {
                _underlyingStream.Write(buffer, offset, count);
            }
        }

        public override void SetLength(long value)
        {
            _underlyingStream.SetLength(value);
        }

        public override bool CanRead
        {
            get
            {
                lock (_underlyingStream)
                {
                    return _underlyingStream.CanRead;
                }
            }
        }

        public override bool CanSeek
        {
            get
            {
                lock (_underlyingStream)
                {
                    return _underlyingStream.CanSeek;
                }
            }
        }

        public override bool CanWrite
        {
            get
            {
                lock (_underlyingStream)
                {
                    return _underlyingStream.CanWrite;
                }
            }
        }

        public override long Length
        {
            get
            {
                lock (_underlyingStream)
                {
                    return _underlyingStream.Length;
                }
            }
        }

        public override long Position
        {
            get
            {
                lock (_underlyingStream)
                {
                    return _underlyingStream.Position;
                }
            }
            set
            {
                lock (_underlyingStream)
                {
                    _underlyingStream.Position = value;
                }
            }
        }
    }
}
