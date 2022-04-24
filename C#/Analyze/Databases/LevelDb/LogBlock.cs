using Common.Library.Extensions;

using System.Collections.Generic;
using System.IO;

namespace Analyze.Databases.LevelDb
{
    public class LogBlock
    {
        #region Constants
        public const int MIN_SIZE = CHECKSUM_SIZE + DATA_LENTH_SIZE + BLOCK_TYPE_SIZE + SEQUENCE_NUMBER_SIZE + RECORD_COUNT_SIZE + LogRecord.MIN_SIZE;

        private const int CHECKSUM_SIZE = 4;
        private const int DATA_LENTH_SIZE = 2;
        private const int BLOCK_TYPE_SIZE = 1;
        private const int SEQUENCE_NUMBER_SIZE = 8;
        private const int RECORD_COUNT_SIZE = 4;
        #endregion


        public byte[] Checksum { get; private set; }
        public short DataLength { get; private set; }
        public BlockType Type { get; private set; }
        public int RecordCount { get; private set; }
        public List<LogRecord> Records { get; private set; }

        public LogBlock(Stream stream)
        {
            Checksum = GetChecksum(stream);
            DataLength = GetDataLength(stream);
            Type = GetBlockType(stream);
            var sequenceNumber = GetSequenceNumber(stream);
            RecordCount = GetRecordCount(stream);
            Records = GetRecords(stream, sequenceNumber);
        }

        private byte[] GetChecksum(Stream stream)
        {
            try
            {
                var checksum = new byte[CHECKSUM_SIZE];
                var len = stream.Read(checksum, 0, checksum.Length);

                if (len < checksum.Length)
                    throw new EndOfStreamException($"Checksum requires {CHECKSUM_SIZE} bytes. But read {len} bytes.");

                return checksum;
            }
            catch (IOException ex)
            {
                throw new IOException("Checksum is not valid.", ex);
            }
        }

        private short GetDataLength(Stream stream)
        {
            if (stream.GetRemain() < sizeof(ushort))
                throw new EndOfStreamException($"DataLength requires {sizeof(ushort)} bytes.");

            try
            {
                var len = stream.ReadInt16LE();

                if (len == 0)
                    throw new InvalidDataException("DataLength is 0.");

                return len;
            }
            catch (IOException ex)
            {
                throw new IOException("DataLength is not valid.", ex);
            }
        }

        private BlockType GetBlockType(Stream stream)
        {
            var type = stream.ReadByte();

            if (type == -1)
                throw new EndOfStreamException("Type of block requires 1 byte.");
            if (type < (byte)BlockType.Full || type > (byte)BlockType.Last)
                throw new InvalidDataException("Type of block is not valid.");

            return (BlockType)type;
        }

        private long GetSequenceNumber(Stream stream)
        {
            if (stream.GetRemain() < sizeof(ulong))
                throw new EndOfStreamException($"SequenceNumber requires {sizeof(ulong)} bytes.");

            try
            {
                return stream.ReadInt64LE();
            }
            catch (IOException ex)
            {
                throw new IOException("SequenceNumber is not valid.", ex);
            }
        }

        private int GetRecordCount(Stream stream)
        {
            if (stream.GetRemain() < sizeof(uint))
                throw new EndOfStreamException($"RecordCount requires {sizeof(uint)} bytes.");

            try
            {
                var cnt = stream.ReadInt32LE();

                if (cnt == 0)
                    throw new InvalidDataException("RecordCount is 0.");

                return cnt;
            }
            catch (IOException ex)
            {
                throw new IOException("RecordCount is not valid.", ex);
            }
        }

        private List<LogRecord> GetRecords(Stream stream, long startSequenceNum)
        {
            var records = new List<LogRecord>();

            for(int i = 0; i < RecordCount; i++)
            {
                var record = new LogRecord(stream, startSequenceNum++);

                records.Add(record);
            }

            return records;
        }

        public override string ToString()
        {
            return $"Counts: {RecordCount}";
        }
    }
}