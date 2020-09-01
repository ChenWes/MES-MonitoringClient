using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_MonitoringClient.DataModel
{
    public class CheckMouldRecordDisplay
    {
        public string MouldCode { get; set; }

        public string ProductCode { get; set; }

        public int PlanCount { get; set; }

        public string EmployeeID { get; set; }

        public string EmployeeName { get; set; }
 
        public int Beer { get; set; }

        public string ProduceTime { get; set; }

      
        public decimal ProduceCycle { get; set; }
    }
}
