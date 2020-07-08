using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_MonitoringClient.DataModel
{
     public class displayClockInRecord
    {
        public string _id { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string LocalFileName { get; set; }
        public DateTime StartDate { get; set; }
    }
}
