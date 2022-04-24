using Common.Library.Type;

using System;
using System.IO;

namespace Analyze.Databases.LevelDb
{
    public class BlockHandle
    {
        #region Constants
        private const int RESTART_CNT_SIZE = 4;
        private const int COMPRESSION_TYPE_SIZE = 1;
        private const int CHECKSUM_SIZE = 4;
        #endregion

        public long Offset { get; set; }
        public long Size { get; set; }
        public long EndOfBlock { get => Offset + Size; }
        public long RestartCountOffset { get => EndOfBlock - RESTART_CNT_SIZE; }
        public long CompressionTypeOffset { get => EndOfBlock; }
        public long ChecksumOffset { get => CompressionTypeOffset + COMPRESSION_TYPE_SIZE; }
        public long EndOfTrailer { get => ChecksumOffset + CHECKSUM_SIZE; }

        public BlockHandle(long offset, long size)
        {
            Offset = offset;
            Size = size;
        }

        public BlockHandle(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException("bytes");

            var stream = new MemoryStream(bytes);

            InitFromStream(stream);
        }

        public BlockHandle(Stream stream)
        {
            InitFromStream(stream);
        }

        private void InitFromStream(Stream stream)
        {
            try
            {
                Offset = Varint64.Get(stream).Value;
                Size = Varint64.Get(stream).Value;
            }
            catch (Exception ex)
            {
                throw new IndexOutOfRangeException("There is no valid data for block handle.", ex);
            }
        }

        public override string ToString()
        {
            return $"{Offset} - {Size}";
        }
    }
}
