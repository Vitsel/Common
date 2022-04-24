using System.Collections.Generic;
using System.Text;

namespace Analyze.Databases.LevelDb
{
    public class Record
    {
        public byte[] Key { get; private set; }
        public byte[] Value { get; private set; }
        public RecordState State { get; private set; }
        public long SequenceNumber { get; private set; }
        public List<Record> Log { get; private set; }

        public Record(byte[] key, byte[] value, RecordState state, long seq)
        {
            Key = key;
            Value = value;
            State = state;
            SequenceNumber = seq;
            Log = null;
        }

        public void AddLog(Record record)
        {
            if (Log == null)
                Log = new List<Record>();

            var i = Log.BinarySearch(record, new RecordSeqComparer());

            if (i < 0)
                Log.Insert(~i, record);

            if (record.SequenceNumber > SequenceNumber)
            {
                Key = record.Key;
                Value = record.Value;
                State = record.State;
                SequenceNumber = record.SequenceNumber;
            }
        }

        public override string ToString()
        {
            return $"{State} | {Encoding.UTF8.GetString(Key)} | {(Value == null ? string.Empty : Encoding.UTF8.GetString(Value))}";
        }
    }

    public class RecordSeqComparer : IComparer<Record>
    {
        public int Compare(Record x, Record y)
        {
            if (x.SequenceNumber == y.SequenceNumber)
                return 0;

            return x.SequenceNumber > y.SequenceNumber ? -1 : 1;
        }
    }
}
