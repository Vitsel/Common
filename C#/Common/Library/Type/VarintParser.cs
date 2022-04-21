using System;
using System.IO;
using System.Collections.Generic;

namespace Common.Library.Type
{
    public class VarintParser
    {
        #region Constants
        private const int SPLIT_UNIT = 7;
        private const int SPLIT_MASK = 127;
        private const int MSB_VALUE = 128;

        private const int MAX_LENGTH_LIMIT = 8;

        private const int MAX_BITS_INT16 = 14;
        private const int MAX_BITS_INT32 = 28;
        private const int MAX_BITS_INT64 = 56;
        #endregion

        public byte[] ReadBytes(Stream stream, int maxLength)
        {
            if (maxLength > MAX_LENGTH_LIMIT)
                throw new ArgumentOutOfRangeException($"Argument \"maxLength\" must be less than {MAX_LENGTH_LIMIT}.");

            var bytes = new List<byte>();

            for (int i = 0; i < maxLength; i++)
            {
                var b = stream.ReadByte();
                if (b == -1)
                    throw new EndOfStreamException("There is no valid value.");

                bytes.Add((byte)b);

                if ((b & MSB_VALUE) == 0)
                    return bytes.ToArray();
            }

            throw new IndexOutOfRangeException($"Target is too big. Max length is {maxLength} bytes.");
        }

        public short ToInt16(byte[] bytes)
        {
            var result = (short)0;

            for (int i = 0; i < Math.Min(bytes.Length, sizeof(short)); i++)
            {
                result |= (short)((bytes[i] & SPLIT_MASK) << (SPLIT_UNIT * i));

                if ((bytes[i] & MSB_VALUE) == 0)
                    return result;
            }

            throw new IndexOutOfRangeException($"Target is too big. Max length is {MAX_BITS_INT16} bits.");
        }

        public int ToInt32(byte[] bytes)
        {
            var result = 0;

            for (int i = 0; i < Math.Min(bytes.Length, sizeof(int)); i++)
            {
                result |= (bytes[i] & SPLIT_MASK) << (SPLIT_UNIT * i);

                if ((bytes[i] & MSB_VALUE) == 0)
                    return result;
            }

            throw new IndexOutOfRangeException($"Target is too big. Max length is {MAX_BITS_INT32} bits.");
        }

        public long ToInt64(byte[] bytes)
        {
            var result = 0L;

            for (int i = 0; i < Math.Min(bytes.Length, sizeof(long)); i++)
            {
                result |= (long)(bytes[i] & SPLIT_MASK) << (SPLIT_UNIT * i);

                if ((bytes[i] & MSB_VALUE) == 0)
                    return result;
            }

            throw new IndexOutOfRangeException($"Target is too big. Max length is {MAX_BITS_INT64} bits.");
        }

        public byte[] ToBytes(long value, int maxLength)
        {
            var bytes = SplitBytes(value, maxLength);

            for (int i = 0; i < bytes.Count - 1; i++)
                bytes[i] |= 1 << 7;

            return bytes.ToArray();
        }

        #region Private Methods
        private List<byte> SplitBytes(long value, int maxLength)
        {
            var splited = new List<byte>();

            for (int i = 0; i < maxLength; i++)
            {
                splited.Add((byte)(value & SPLIT_MASK));

                value = value >> SPLIT_UNIT;
            }

            for (int i = splited.Count - 1; i >= 1; i--)
            {
                if (splited[i] > 0)
                    break;

                splited.RemoveAt(i);
            }

            return splited;
        }
        #endregion
    }
}
