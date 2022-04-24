using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Analyze.Databases.LevelDb
{
    public class IndexBlock : Block, IEnumerable<BlockHandle>, IEnumerator<BlockHandle>
    {
        public BlockHandle this[int index] { get => Records[index].Handle; }
        public List<IndexRecord> Records { get; private set; }

        public IndexBlock(Stream stream, BlockHandle handle) : base(stream, handle)
        {
            Records = GetRecords(stream, IndexRecord.GetConstructor());
        }

        public override string ToString()
        {
            return $"{Handle.Offset} - {Handle.Size}";
        }

        #region Enumerable Implements
        public BlockHandle Current => Records[position].Handle;
        object IEnumerator.Current => Records[position].Handle;

        private int position = -1;

        public IEnumerator<BlockHandle> GetEnumerator()
        {
            foreach (var record in Records)
                yield return record.Handle;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var record in Records)
                yield return record.Handle;
        }

        public bool MoveNext()
        {
            if(position == Records.Count - 1)
            {
                Reset();
                return false;
            }

            position++;

            return position < Records.Count;
        }

        public void Reset()
        {
            position = -1;
        }

        public void Dispose() { }
        #endregion
    }
}