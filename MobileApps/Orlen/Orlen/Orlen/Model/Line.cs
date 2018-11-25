using System;
using System.Collections.Generic;
using System.Text;

namespace Orlen.Model
{
    public class Line
    {
        public int Id { get; set; }
        public string LineNumber { get; set; }
        public string LineFrom { get; set; }
        public string LineTo { get; set; }
        public List<TimeTableLineItem> lineItems { get; set; }
    }
}
