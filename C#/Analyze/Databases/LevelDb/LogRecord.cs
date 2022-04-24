using Common.Library.Type;
using Common.Library.Extensions;

using System.IO;
using System.Text;

namespace Analyze.Databases.LevelDb
{
    public class LogRecord
    {
        public const int MIN_SIZE = 5;

        public long SequenceNumber { get; private set; }
        public RecordState State { get; private set; }
        public int KeyLength { get; private set; }
        public byte[] Key { get; private set; }
        public int ValueLength { get; private set; }
        public byte[] Value { get; private set; }

        public LogRecord(Stream stream, long sequnceNum)
        {
            SequenceNumber = sequnceNum;
            State = GetRecordState(stream);
            KeyLength = GetKeyLength(stream);
            Key = GetKey(stream);

            if (State == RecordState.Delete)
                return;

            ValueLength = GetValueLength(stream);
            Value = GetValue(stream);
        }

        private RecordState GetRecordState(Stream stream)
        {
            var state = stream.ReadByte();

            if (state == -1)
                throw new EndOfStreamException("Record state requires 1 byte.");
            if (state != 0 && state != 1)
                throw new InvalidDataException($"Type of record is {state}.");

            return (RecordState)state;
        }

        private int GetKeyLength(Stream stream)
        {
            try
            {
                var len = Varint32.Get(stream).Value;

                if (len == 0)
                    throw new InvalidDataException("KeyLength is 0.");

                return len;
            }
            catch (EndOfStreamException ex)
            {
                throw new EndOfStreamException("KeyLength requires 1 to 4 bytes.", ex);
            }
        }

        private byte[] GetKey(Stream stream)
        {
            if (stream.GetRemain() < KeyLength)
                throw new EndOfStreamException($"Key length is {KeyLength}.");

            var key = new byte[KeyLength];
            stream.Read(key, 0, key.Length);

            return key;
        }

        private int GetValueLength(Stream stream)
        {
            try
            {
                var len = Varint32.Get(stream).Value;

                if (len == 0)
                    throw new InvalidDataException("ValueLength is 0.");

                return len;
            }
            catch (EndOfStreamException ex)
            {
                throw new EndOfStreamException("ValueLength requires 1 to 4 bytes.", ex);
            }
        }

        private byte[] GetValue(Stream stream)
        {
            if (stream.GetRemain() < ValueLength)
                throw new EndOfStreamException($"Value length is {ValueLength}.");

            var value = new byte[ValueLength];
            stream.Read(value, 0, value.Length);

            return value;
        }

        public override string ToString()
        {
            return $"{State} | {Encoding.UTF8.GetString(Key)} | {(Value == null ? string.Empty : Encoding.UTF8.GetString(Value))}";
        }
    }
}
