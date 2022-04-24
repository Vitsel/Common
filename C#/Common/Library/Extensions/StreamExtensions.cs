using System;
using System.IO;

namespace Common.Library.Extensions
{
    public static class StreamExtensions
    {
        #region Read Integer
        public static short ReadInt16LE(this Stream stream, long offset)
        {
            stream.Seek(offset, SeekOrigin.Begin);

            return ReadInt16LE(stream);
        }

        public static short ReadInt16LE(this Stream stream)
        {
            if (stream.GetRemain() < 2)
                throw new EndOfStreamException("Int16 requires 2 bytes.");

            var bytes = new byte[2];

            stream.Read(bytes, 0, bytes.Length);

            return BitConverter.ToInt16(bytes, 0);
        }

        public static int ReadInt32LE(this Stream stream, long offset)
        {
            stream.Seek(offset, SeekOrigin.Begin);

            return ReadInt32LE(stream);
        }

        public static int ReadInt32LE(this Stream stream)
        {
            if (stream.GetRemain() < 4)
                throw new EndOfStreamException("Int32 requires 4 bytes.");

            var bytes = new byte[4];

            stream.Read(bytes, 0, bytes.Length);
            
            return BitConverter.ToInt32(bytes, 0);
        }

        public static long ReadInt64LE(this Stream stream, long offset)
        {
            stream.Seek(offset, SeekOrigin.Begin);

            return ReadInt64LE(stream);
        }

        public static long ReadInt64LE(this Stream stream)
        {
            if (stream.GetRemain() < 8)
                throw new EndOfStreamException("Int64 requires 8 bytes.");

            var bytes = new byte[8];

            stream.Read(bytes, 0, bytes.Length);

            return BitConverter.ToInt64(bytes, 0);
        }

        public static ushort ReadUInt16LE(this Stream stream, long offset)
        {
            stream.Seek(offset, SeekOrigin.Begin);

            return ReadUInt16LE(stream);
        }

        public static ushort ReadUInt16LE(this Stream stream)
        {
            if (stream.GetRemain() < 2)
                throw new EndOfStreamException("UInt16 requires 2 bytes.");

            var bytes = new byte[2];

            stream.Read(bytes, 0, bytes.Length);

            return BitConverter.ToUInt16(bytes, 0);
        }

        public static uint ReadUInt32LE(this Stream stream, long offset)
        {
            stream.Seek(offset, SeekOrigin.Begin);

            return ReadUInt32LE(stream);
        }

        public static uint ReadUInt32LE(this Stream stream)
        {
            if (stream.GetRemain() < 4)
                throw new EndOfStreamException("UInt32 requires 4 bytes.");

            var bytes = new byte[4];

            stream.Read(bytes, 0, bytes.Length);

            return BitConverter.ToUInt32(bytes, 0);
        }

        public static ulong ReadUInt64LE(this Stream stream, long offset)
        {
            stream.Seek(offset, SeekOrigin.Begin);

            return ReadUInt64LE(stream);
        }

        public static ulong ReadUInt64LE(this Stream stream)
        {
            if (stream.GetRemain() < 8)
                throw new EndOfStreamException("UInt64 requires 8 bytes.");

            var bytes = new byte[8];

            stream.Read(bytes, 0, bytes.Length);

            return BitConverter.ToUInt64(bytes, 0);
        }
        #endregion

        #region Status
        public static long GetRemain(this Stream stream)
        {
            return stream.Length - stream.Position;
        }
        #endregion
    }
}
