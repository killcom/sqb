using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sqb.db
{
    public class Sqlite
    {
#pragma warning disable IDE1006
        public string src { get; private set; }
#pragma warning restore IDE1006
        public const string srcMem = ":memory:";
        protected SQLiteConnection con;
        public bool IsOpen { get; private set; }

        public Sqlite(string src) { }
        ~Sqlite()
        {
            Close(false);
        }

        public void Close(bool IsForce = false)
        {
            if (!IsForce)
                Backup();
            // TODO : Already Close
            // , Con Null Control
            // , Backup/Close Failed Control
            con?.Close();
            IsOpen = false;
            con = null;
        }
        public void Open(string src)
        {
            if (IsOpen)
            {
                // TODO : Already Opened
            }
            this.src = GetConnectionStr(src);

            // TODO : Open Failed
            var localCon = new SQLiteConnection(GetConnectionStr(src));
            localCon.Open();
            // TODO : Open Failed
            var memCon = new SQLiteConnection(GetConnectionStr(srcMem));
            // TODO : Backup Failed
            localCon.BackupDatabase(memCon, "main", "main", -1, null, 1000);
            // TODO? : delegate SQLiteBackupCallback
            localCon.Close();
            con = memCon;

            IsOpen = true;
        }
        private string GetConnectionStr(string src)
        {
            return new SQLiteConnectionStringBuilder
            {
                DataSource = src,
                SyncMode = SynchronizationModes.Normal,
                JournalMode = SQLiteJournalModeEnum.Wal
            }.ToString();
        }
        public void Backup()
        {
            if (!IsOpen)
            {
                // TODO Connenction Closed Exception
                return;
            }

            var backupCon = new SQLiteConnection(GetConnectionStr(src));
            backupCon.Open();
            con.BackupDatabase(backupCon, "main", "main", -1, null, 1000);
            backupCon.Close();
        }
    }
}
