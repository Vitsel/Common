using System.Collections.Generic;
using System.IO;

namespace Analyze.Databases.LevelDb
{
    public class DataBlock : Block
    {
        public List<BlockRecord> Records { get; private set; }

        public DataBlock(Stream stream, BlockHandle handle) : base(stream, handle)
        {
            Records = GetRecords(stream, BlockRecord.GetConstructor());
        }

        public override string ToString()
        {
            return $"{Handle.Offset} - {Handle.Size}";
        }
    }
}
