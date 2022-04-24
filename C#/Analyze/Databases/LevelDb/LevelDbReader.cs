using Common.Library.Type.Comparer;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Analyze.Databases.LevelDb
{
    public class LevelDbReader : IDisposable
    {
        public string Path { get; private set; }
        public Dictionary<byte[], Record> Records { get; private set; }
        public bool IsDisposed { get; private set; }

        public LevelDbReader()
        {
            Path = null;
            Records = new Dictionary<byte[], Record>(new ByteArrayComparer());
            IsDisposed = false;
        }

        #region Connection
        public bool Open(string path)
        {
            if (!IsValidDb(path))
                throw new ArgumentException($"\"{path}\" is not valid LevelDB.");

            Path = path;

            return Analyze();
        }

        public void Close()
        {
            Path = null;
        }

        public bool IsValid()
        {
            return !IsDisposed && Path != null && IsValidDb(Path);
        }

        private bool IsValidDb(string path)
        {
            return Directory.Exists(path);
        }
        #endregion

        #region Analyze
        private bool Analyze()
        {
            Records.Clear();

            var db = new DirectoryInfo(Path);
            var excepted = false;

            foreach (var file in db.GetFiles())
            {
                try
                {
                    switch (file.Extension)
                    {
                        case ".log":
                        case ".LOG":
                            using (var fs = new FileStream(file.FullName, FileMode.Open))
                                AnalyzeLogFile(fs);
                            break;

                        case ".ldb":
                        case ".LDB":
                            using (var fs = new FileStream(file.FullName, FileMode.Open))
                                AnalyzeLdbFile(fs);
                            break;
                    }
                }
                catch
                {
                    return excepted = true;
                }
            }

            return !excepted && Records.Count > 0;
        }

        private void AnalyzeLogFile(Stream stream)
        {
            var logFile = new LogFile(stream);

            foreach (var log in logFile.Records)
                AddRecord(new Record(log.Key, log.Value, log.State, log.SequenceNumber));
        }

        private void AnalyzeLdbFile(Stream stream)
        {
            var ldb = new LdbFile(stream);

            foreach (var dataRec in ldb.DataRecords)
                AddRecord(new Record(dataRec.Key, dataRec.Value, dataRec.State, dataRec.SequenceNumber));
        }

        private void AddRecord(Record record)
        {
            var rec = GetRecord(record.Key);

            if (rec == null)
            {
                Records.Add(record.Key, record);
                record.AddLog(new Record(record.Key, record.Value, record.State, record.SequenceNumber));
            }
            else
            {
                rec.AddLog(record);
            }
        }
        #endregion

        #region GetRecord
        public List<Record> GetAllRecords()
        {
            return (from record in Records.Values
                    select record).ToList();
        }

        public Record GetRecord(byte[] key)
        {
            Records.TryGetValue(key, out Record rec);

            return rec;
        }

        public Record GetRecord(string key) => GetRecord(key, Encoding.UTF8);
        public Record GetRecord(string key, Encoding encoding)
        {
            var byteKey = encoding.GetBytes(key);

            return GetRecord(byteKey);
        }
        #endregion

        #region GetValue
        public byte[] GetByteValue(byte[] key)
        {
            var rec = GetRecord(key);

            if (rec == null)
                throw new KeyNotFoundException();

            return rec.Value;
        }

        public string GetStringValue(string key) => GetStringValue(key, Encoding.UTF8);
        public string GetStringValue(string key, Encoding encoding)
        {
            var rec = GetRecord(key, encoding);

            if (rec == null)
                throw new KeyNotFoundException();

            return rec.Value == null ? string.Empty : encoding.GetString(rec.Value);
        }
        #endregion

        #region GetState
        public RecordState GetState(byte[] key)
        {
            var rec = GetRecord(key);

            if (rec == null)
                throw new KeyNotFoundException();

            return rec.State;
        }

        public RecordState GetState(string key) => GetState(key, Encoding.UTF8);
        public RecordState GetState(string key, Encoding encoding)
        {
            var rec = GetRecord(key, encoding);

            if (rec == null)
                throw new KeyNotFoundException();

            return rec.State;
        }
        #endregion

        #region GetKeys
        public List<byte[]> GetByteKeys()
        {
            return Records.Keys.ToList();
        }

        public List<string> GetStringKeys() => GetStringKeys(Encoding.UTF8);
        public List<string> GetStringKeys(Encoding encoding)
        {
            return (from key in Records.Keys
                    select encoding.GetString(key)).ToList();
        }
        #endregion

        #region GetValues
        public List<byte[]> GetByteValues()
        {
            return (from record in Records.Values
                    select record.Value).ToList();
        }

        public List<string> GetStringValues() => GetStringValues(Encoding.UTF8);
        public List<string> GetStringValues(Encoding encoding)
        {
            return (from record in Records.Values
                    select encoding.GetString(record.Value)).ToList();
        }
        #endregion

        #region GetLog
        public List<Record> GetAllLogs()
        {
            return (from record in Records.Values
                    from log in record.Log
                    orderby log.SequenceNumber
                    select log).ToList();
        }

        public List<Record> GetLogs(byte[] key)
        {
            var rec = GetRecord(key);

            if (rec == null)
                throw new KeyNotFoundException();

            return rec.Log;
        }
        #endregion

        #region IDisposable Implements
        public void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (disposing)
            {
                Records = null;
            }

            Path = null;

            IsDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        ~LevelDbReader() => Dispose();
        #endregion

        public override string ToString()
        {
            return Path;
        }
    }
}
