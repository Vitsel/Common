using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Common.Library.Type
{
    public class Varint32 : IEquatable<Varint32>
    {
        #region Constants
        public const int MAX_VALUE = 0xFFFFFFF;

        public const int MAX_SIZE = 4;
        #endregion

        public int Value
        {
            get => _Value;
            set
            {
                if (value > MAX_VALUE)
                    throw new ArgumentOutOfRangeException($"Max value is {MAX_VALUE}.");

                _Value = value;
            }
        }

        #region Behind Properties
        private int _Value;
        #endregion

        public Varint32(int value = 0)
        {
            Value = value;
        }

        public Varint32(byte[] bytes)
        {
            var parser = new VarintParser();

            Value = parser.ToInt32(bytes);
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

            return parser.ToBytes(Value, MAX_SIZE);
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
            else if (obj.GetType() == typeof(Varint32))
                return Equals((Varint32)obj);

            return false;
        }

        public bool Equals(Varint32 other)
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
        public static Varint32 Get(Stream stream, long offset, SeekOrigin origin = SeekOrigin.Begin)
        {
            stream.Seek(offset, origin);

            return Get(stream);
        }

        public static Varint32 Get(Stream stream)
        {
            var value = new VarintParser().ReadInt32(stream);

            return new Varint32(value);
        }
        #endregion
    }
}