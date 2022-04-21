using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Common.Library.Type
{
    public class Varint64 : IEquatable<Varint64>
    {
        #region Constants
        public const long MaxValue = 0xFFFFFFFFFFFFFF;

        private const int MAX_LENGTH = 8;
        #endregion

        public long Value
        {
            get => _Value;
            set
            {
                if (value > MaxValue)
                    throw new ArgumentOutOfRangeException($"Max value is {MaxValue}.");

                _Value = value;
            }
        }

        #region Behind Properties
        private long _Value;
        #endregion

        public Varint64(long value = 0)
        {
            Value = value;
        }

        public Varint64(byte[] bytes)
        {
            var parser = new VarintParser();

            Value = parser.ToInt64(bytes);
        }

        public string GetString()
        {
            return Encoding.UTF8.GetString(GetBytes());
        }

        public string GetString(Encoding encoding)
        {
            return encoding.GetString(GetBytes());
        }

        public byte[] GetBytes()
        {
            var parser = new VarintParser();

            return parser.ToBytes(Value, MAX_LENGTH);
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        #region Equals
        public override int GetHashCode()
        {
            return GetBytes().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(int))
                return Equals((int)obj);
            else if (obj.GetType() == typeof(Varint64))
                return Equals((Varint64)obj);

            return false;
        }

        public bool Equals(Varint64 other)
        {
            return Value.Equals(other.Value);
        }

        public bool Equals(int other)
        {
            return Value.Equals(other);
        }

        public bool Equals(byte[] other)
        {
            return Enumerable.SequenceEqual(GetBytes(), other);
        }
        #endregion

        #region Static Methods
        public static Varint64 Get(Stream stream, long offset)
        {
            stream.Seek(offset, SeekOrigin.Begin);

            return Get(stream);
        }

        public static Varint64 Get(Stream stream)
        {
            var parser = new VarintParser();
            var bytes = parser.ReadBytes(stream, MAX_LENGTH);
            var value = parser.ToInt64(bytes);

            return new Varint64(value);
        }
        #endregion
    }
}