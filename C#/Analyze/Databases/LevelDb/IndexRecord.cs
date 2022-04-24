using System;
using System.IO;
using System.Text;

namespace Analyze.Databases.LevelDb
{
    public class IndexRecord : BlockRecord
    {
        public long HandleLength { get => ValueLength; }
        public byte[] LargestKey { get => Key; }
        public BlockHandle Handle { get; private set; }
        public long DataBlockOffset { get => Handle.Offset; }
        public long DataBlockSize { get => Handle.Size; }

        public IndexRecord(Stream stream, byte[] prevKey) : base(stream, prevKey)
        {
            if (Value == null)
                throw new InvalidDataException($"Value of index record cannot be null.");

            Handle = new BlockHandle(Value);
        }

        public static new Func<Stream, byte[], IndexRecord> GetConstructor()
        {
            return (stream, prevKey) => new IndexRecord(stream, prevKey);
        }

        public override string ToString()
        {
            return $"{State} | {Encoding.UTF8.GetString(Key)} | {(Value == null ? string.Empty : Encoding.UTF8.GetString(Value))}";
        }
    }
}
