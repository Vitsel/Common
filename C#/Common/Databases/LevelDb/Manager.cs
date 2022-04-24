using LevelDB;

using System;
using System.IO;
using System.Collections.Generic;

namespace Common.Databases.LevelDb
{
    [Obsolete("Under development.")]
    public class Manager : IDisposable
    {
        public string Path { get; private set; }
        public DB Db { get; set; }
        public bool IsConnected { get; set; }
        public bool IsDisposed { get; private set; }

        private WriteBatch batch;

        public Manager(string path)
        {
            if (path == null)
                throw new ArgumentNullException(path);

            Path = path;
            Db = null;
            IsConnected = false;
            IsDisposed = false;

            batch = new WriteBatch();
        }

        #region Connection
        public void Open(string path = null)
        {
            var option = new Options();

            Connect(option);
        }

        public void Create(bool isOverride = false)
        {
            if (isOverride)
                Destroy();

            var option = new Options { ErrorIfExists = true };

            Connect(option);
        }

        public void OpenCreate()
        {
            var option = new Options { CreateIfMissing = true };

            Connect(option);
        }

        private void Connect(Options option)
        {
            if (Db != null)
                Close();

            Db = new DB(option, Path);

            IsConnected = true;
        }

        public void Close()
        {
            if (Db == null || !IsConnected)
                return;

            Db.Close();
            Db = null;

            IsConnected = false;
        }

        public void Destroy()
        {
            if (Directory.Exists(Path))
                Directory.Delete(Path, true);
        }
        #endregion

        #region Put
        public void Put(string key, string value)
        {
            Db.Put(key, value);
        }

        public void Put(IEnumerable<KeyValuePair<string, string>> datas)
        {
            using (var batch = new WriteBatch())
            {
                foreach (var data in datas)
                    batch.Put(data.Key, data.Value);

                Db.Write(batch);
            }
        }
        #endregion

        #region Delete
        public void Delete(string key)
        {
            Db.Delete(key);
        }
        #endregion

        #region Get
        public string Get(string key)
        {
            return Db.Get(key);
        }
        #endregion

        #region Batch
        public void PutBatch(WriteBatch batch)
        {
            Db.Write(batch);
        }

        public void PutBatch()
        {
            Db.Write(batch);
        }

        public void AddPutBatch(string key, string value)
        {
            batch.Put(key, value);
        }

        public void AddDeleteBatch(string key)
        {
            batch.Delete(key);
        }

        public void ClearBatch()
        {
            batch.Clear();
        }
        #endregion

        #region IDisposable Implements
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (disposing)
            {

            }

            Close();

            IsDisposed = true;
        }

        ~Manager() => Dispose(false);
        #endregion
    }
}