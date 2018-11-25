
using Orlen.Database;
using Orlen.Interfaces;
using Orlen.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orlen.Services
{
    public class DatabaseRepository : IDatabaseRepository
    {
        readonly DatabaseHelper _databaseHelper;
 
        public void ClearDatabase()
        {
            _databaseHelper.DelateTable();
        }

        public DatabaseRepository()
        {
            _databaseHelper = new DatabaseHelper();
        }
        public void InserBusStop(TimeTableLineItem employe)
        {
            _databaseHelper.InsertClient(employe);
        }

        public List<TimeTableLineItem> GetAllBusStops()
        {
            return _databaseHelper.GetAllBusStops();
        }
    }
}
