using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Analyze.Databases.LevelDb
{
    public class LdbFile
    {
        #region Constants
        public readonly byte[] SIGNATURE = { 0x57, 0xFB, 0x80, 0x8B, 0x24, 0x75, 0x47, 0xDB };
        
        private const int SIGNATURE_SIZE = 8;
        private const int FOOTER_SIZE = 48;
        #endregion

        public List<DataBlock> DataBlocks { get; private set; }
        public List<BlockRecord> DataRecords { get; private set; }
        public IndexBlock IndexBlock { get; private set; }

        public BlockHandle MetaIndexHandle { get; private set; }
        public BlockHandle IndexHandle { get; private set; }

        public LdbFile(Stream stream)
        {
            if (!CheckSignature(stream))
                throw new InvalidDataException("This is not ldb file.");

            SetFooter(stream);
            IndexBlock = new IndexBlock(stream, IndexHandle);
            SetDataBlocks(stream);
        }

        private bool CheckSignature(Stream stream)
        {
            if (stream.Length < SIGNATURE_SIZE)
                throw new EndOfStreamException($"Signature requires {SIGNATURE_SIZE} bytes.");

            stream.Seek(-SIGNATURE_SIZE, SeekOrigin.End);

            var bytes = new byte[SIGNATURE_SIZE];
            stream.Read(bytes, 0, bytes.Length);

            return Enumerable.SequenceEqual(SIGNATURE, bytes);
        }

        private void SetFooter(Stream stream)
        {
            if (stream.Length < FOOTER_SIZE)
                throw new EndOfStreamException($"Footer requires {FOOTER_SIZE} bytes.");

            stream.Seek(-FOOTER_SIZE, SeekOrigin.End);

            MetaIndexHandle = new BlockHandle(stream);
            IndexHandle = new BlockHandle(stream);
        }

        private void SetDataBlocks(Stream stream)
        {
            DataBlocks = new List<DataBlock>();
            DataRecords = new List<BlockRecord>();

            foreach(var handle in IndexBlock)
            {
                var block = new DataBlock(stream, handle);

                DataBlocks.Add(block);
                DataRecords.AddRange(block.Records);
            }

            if (DataBlocks.Count == 0)
                throw new InvalidDataException("There is no data blocks.");
        }

        public override string ToString()
        {
            return $"Record count: {DataRecords.Count}";
        }
    }
}