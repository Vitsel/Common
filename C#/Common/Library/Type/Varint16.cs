using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Common.Library.Type
{
    public class Varint16 : IEquatable<Varint16>
    {
        #region Constants
        public const int MaxValue = 0x3FFF;

        private const int MAX_LENGTH = 2;
        #endregion

        public short Value
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
        private short _Value;
        #endregion

        public Varint16(short value = 0)
        {
            Value = value;
        }

        public Varint16(byte[] bytes)
        {
            var parser = new VarintParser();

            Value = parser.ToInt16(bytes);
        }

        public string GetString()
        {
            return GetString(Encoding.UTF8);
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
            else if (obj.GetType() == typeof(Varint16))
                return Equals((Varint16)obj);

            return false;
        }

        public bool Equals(Varint16 other)
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
        public static Varint16 Get(Stream stream, long offset)
        {
            stream.Seek(offset, SeekOrigin.Begin);

            return Get(stream);
        }

        public static Varint16 Get(Stream stream)
        {
            var parser = new VarintParser();
            var bytes = parser.ReadBytes(stream, MAX_LENGTH);
            var value = parser.ToInt16(bytes);

            return new Varint16(value);
        }
        #endregion
    }
}
