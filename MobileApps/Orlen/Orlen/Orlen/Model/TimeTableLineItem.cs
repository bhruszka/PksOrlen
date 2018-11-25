using System;
using System.Collections.Generic;
using System.Text;

namespace Orlen.Model
{
    public class TimeTableLineItem
    {
        public int LineId { get; set; }
        public int BusStopId { get; set; }
        public string BusStopName { get; set; }
        public double Long { get; set; }
        public double Lat { get; set; }
        public string TimeIN = "11.00 | 12.00 | 13.00 | 14.00 | 15.00";
    }
}
