using Orlen.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Orlen.Database
{
    class DatabaseHelper
    {
        static SQLiteConnection sqliteconnection;
        public const string DbFileName = "Database.db";

        public DatabaseHelper()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DbFileName);
            sqliteconnection = new SQLiteConnection(path);
            sqliteconnection.CreateTable<TimeTableLineItem>();
        }
        public void DelateTable()
        {
            sqliteconnection.DeleteAll<TimeTableLineItem>();
        }
        public void InsertClient(TimeTableLineItem customer)
        {
            
                sqliteconnection.Insert(customer);
        }

        internal List<TimeTableLineItem> GetAllBusStops()
        {
            return sqliteconnection.Table<TimeTableLineItem>().ToList();

        }
    }

}
