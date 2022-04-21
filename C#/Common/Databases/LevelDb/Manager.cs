using LevelDB;
using System;
using System.Collections.Generic;

namespace Common.Databases.LevelDb
{
    public class Manager : IDisposable
    {
        public string Path { get; set; }
        public DB Db { get; set; }
        public bool IsConnected { get; set; }
        public bool IsDisposed { get; private set; }

        private WriteBatch batch;

        public Manager(string path = null)
        {
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

            Connect(option, path);
        }

        public void Create(bool isOverride = false, string path = null)
        {
            var option = new Options { ErrorIfExists = true };

            Connect(option, path);
        }

        public void OpenCreate(string path = null)
        {
            var option = new Options { CreateIfMissing = true };

            Connect(option, path);
        }

        private void Connect(Options option, string path)
        {
            if (Db != null)
                Close();

            var dbPath = path ?? Path;
            if (dbPath == null)
                throw new ArgumentNullException(path, "The path to create db is not valid");

            Db = new DB(option, dbPath);

            IsConnected = true;
        }

        public void Close()
        {
            if (Db == null)
                return;

            Db.Close();
            Db = null;

            IsConnected = false;
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

        public void Put(WriteBatch batch)
        {
            Db.Write(batch);
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
        public void AddPutBatch(string key, string value)
        {
            batch.Put(key, value);
        }

        public void AddDeleteBatch(string key)
        {
            batch.Delete(key);
        }

        public void PutBatch()
        {
            Db.Write(batch);
        }

        public void ClearBatch()
        {
            batch.Clear();
        }
        #endregion

        #region IDisposable Implements
        public void Dispose() => Dispose(true);

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