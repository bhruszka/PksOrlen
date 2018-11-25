using Orlen.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orlen.Interfaces
{
    public interface IDatabaseRepository
    {
        void ClearDatabase();
        void InserBusStop(TimeTableLineItem employe);
        List<TimeTableLineItem> GetAllBusStops();
    }
}
