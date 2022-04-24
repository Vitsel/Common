using Common.Library.Extensions;

using System.Collections.Generic;
using System.IO;

namespace Analyze.Databases.LevelDb
{
    public class LogFile
    {
        #region Constants
        private const int BLOCK_BORDER_SIZE = 0x8000;
        #endregion

        public List<LogBlock> Blocks { get; private set; }
        public List<LogRecord> Records { get; private set; }

        public LogFile(Stream stream)
        {
            Blocks = new List<LogBlock>();
            Records = new List<LogRecord>();

            while (stream.GetRemain() > LogBlock.MIN_SIZE)
            {
                var block = new LogBlock(stream);

                Blocks.Add(block);
                Records.AddRange(block.Records);

                //Check that blocks are before border.
                //If remaining size to border smaller than min size of log block,
                //move stream's offset to next border.
                var remain = BLOCK_BORDER_SIZE - (stream.GetRemain() % BLOCK_BORDER_SIZE);
                if (remain < LogBlock.MIN_SIZE && stream.Length > stream.Position + remain)
                    stream.Seek(remain, SeekOrigin.Current);
            }

            if (Blocks.Count == 0)
                throw new InvalidDataException("There is no log blocks.");
        }

        public override string ToString()
        {
            return $"Record count: {Records.Count}";
        }
    }
}
