using Common.Library.Type;

using System.IO;
using System;
using System.Text;

namespace Analyze.Databases.LevelDb
{
    public class BlockRecord
    {
        public int SharedKeyLength { get; private set; }
        public int NonSharedKeyLength { get; private set; }
        public int ValueLength { get; private set; }
        public byte[] Key { get; private set; }
        public RecordState State { get; private set; }
        public long SequenceNumber { get; private set; }
        public byte[] Value { get; private set; }

        public BlockRecord(Stream stream, byte[] prevKey)
        {
            if (prevKey == null)
                prevKey = new byte[0];

            SharedKeyLength = GetSharedKeyLength(stream);
            NonSharedKeyLength = GetNonSharedKeyLength(stream);
            ValueLength = GetValueLength(stream);
            Key = GetKey(stream, prevKey);
            State = GetRecordState(stream);
            SequenceNumber = GetSequenceNumber(stream);

            if (ValueLength == 0)
                return;

            Value = GetValue(stream);
        }

        private int GetSharedKeyLength(Stream stream)
        {
            try
            {
                var len = Varint32.Get(stream).Value;

                return len;
            }
            catch (EndOfStreamException ex)
            {
                throw new EndOfStreamException("SharedKeyLength requires 1 to 4 bytes.", ex);
            }
        }

        private int GetNonSharedKeyLength(Stream stream)
        {
            try
            {
                var len = Varint32.Get(stream).Value;

                if (len < 8)
                    throw new InvalidDataException("NonSharedKeyLength is 0.");

                return len - 8;
            }
            catch (EndOfStreamException ex)
            {
                throw new EndOfStreamException("NonSharedKeyLength requires 1 to 4 bytes.", ex);
            }
        }

        private int GetValueLength(Stream stream)
        {
            try
            {
                var len = Varint32.Get(stream).Value;

                return len;
            }
            catch (EndOfStreamException ex)
            {
                throw new EndOfStreamException("ValueLength requires 1 to 4 bytes.", ex);
            }
        }

        private byte[] GetKey(Stream stream, byte[] prevKey)
        {
            if (prevKey.Length < SharedKeyLength)
                throw new InvalidDataException($"The length of previos key({prevKey.Length}) is less then the SharedKeyLength({SharedKeyLength}).");

            var bytes = new byte[SharedKeyLength + NonSharedKeyLength];
            Buffer.BlockCopy(prevKey, 0, bytes, 0, SharedKeyLength);

            var len = stream.Read(bytes, SharedKeyLength, NonSharedKeyLength);

            if (len < NonSharedKeyLength)
                throw new EndOfStreamException($"Nonshared key length is {NonSharedKeyLength}. But read {len} bytes.");

            return bytes;
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

        private long GetSequenceNumber(Stream stream)
        {
            var bytes = new byte[8];
            var len = stream.Read(bytes, 0, bytes.Length - 1);

            if(len < 7)
                throw new EndOfStreamException($"SequenceNumber requires 7 bytes. But read {len} bytes.");

            return BitConverter.ToInt64(bytes, 0);
        }

        private byte[] GetValue(Stream stream)
        {
            if (ValueLength == 0)
                throw new InvalidDataException("Value length is 0.");

            var bytes = new byte[ValueLength];
            var len = stream.Read(bytes, 0, bytes.Length);

            if (len < ValueLength)
                throw new EndOfStreamException($"Value length is {ValueLength}. But read {len} bytes.");

            return bytes;
        }

        public static Func<Stream, byte[], BlockRecord> GetConstructor()
        {
            return (stream, prevKey) => new BlockRecord(stream, prevKey);
        }

        public override string ToString()
        {
            return $"{State} | {Encoding.UTF8.GetString(Key)} | {(Value == null ? string.Empty : Encoding.UTF8.GetString(Value))}";
        }
    }
}