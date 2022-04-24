using Common.Library.Extensions;

using System;
using System.Collections.Generic;
using System.IO;

namespace Analyze.Databases.LevelDb
{
    public class Block
    {
        #region Constants
        private const int RESTART_SIZE = 4;
        private const int RESTART_COUNT_SIZE = 4;
        private const int CHECKSUM_SIZE = 4;
        #endregion

        public BlockHandle Handle { get; private set; }
        public List<uint> Restarts { get; private set; }
        public bool IsCompressed { get; private set; }
        public byte[] Checksum { get; private set; }
        public long EndOfDataOffset
        {
            get
            {
                if (Handle == null || Restarts == null)
                    return 0;

                return Handle.EndOfBlock - ((Restarts.Count * RESTART_SIZE) + RESTART_COUNT_SIZE);
            }
        }

        public Block(Stream stream, BlockHandle handle)
        {
            if (stream.Length < handle.EndOfTrailer)
                throw new InvalidDataException("Stream's length less than end of block offset.");

            Handle = handle;
            var restartCount = GetRestartCount(stream);
            Restarts = GetRestarts(stream, restartCount);
            IsCompressed = GetIsCompressed(stream);
            Checksum = GetChecksum(stream);
        }

        protected List<T> GetRecords<T>(Stream stream, Func<Stream, byte[], T> recordConstruct) where T : BlockRecord
        {
            stream.Position = Handle.Offset;

            var blocks = new List<T>();
            var prevKey = new byte[0];

            while (stream.Position < EndOfDataOffset)
            {
                var dataBlock = recordConstruct(stream, prevKey);

                prevKey = dataBlock.Key;

                blocks.Add(dataBlock);
            }

            if (blocks.Count == 0)
                throw new InvalidDataException("There is no DataBlock.");

            return blocks;
        }

        private int GetRestartCount(Stream stream)
        {
            stream.Position = Handle.RestartCountOffset;

            var count = stream.ReadInt32LE();

            if (count == 0)
                throw new InvalidDataException("Restart count is 0.");

            return count;
        }

        private List<uint> GetRestarts(Stream stream, int restartCount)
        {
            stream.Position = Handle.RestartCountOffset - (RESTART_SIZE * restartCount);

            var restarts = new List<uint>();

            for (int i = 0; i < restartCount; i++)
                restarts.Add(stream.ReadUInt32LE());

            return restarts;
        }

        private bool GetIsCompressed(Stream stream)
        {
            stream.Position = Handle.CompressionTypeOffset;

            var compress = stream.ReadByte();

            if (compress != 0 && compress != 1)
                throw new InvalidDataException($"Compression type is {compress}. Compression type must be 0 or 1.");

            return compress == 1;
        }

        private byte[] GetChecksum(Stream stream)
        {
            stream.Position = Handle.ChecksumOffset;

            var checksum = new byte[CHECKSUM_SIZE];
            var len = stream.Read(checksum, 0, checksum.Length);

            if (len != CHECKSUM_SIZE)
                throw new InvalidDataException($"Checksum's length is {len}. Checksum requires {CHECKSUM_SIZE} bytes.");

            return checksum;
        }

        public override string ToString()
        {
            return $"{Handle.Offset} - {Handle.Size}";
        }
    }
}
