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

        public string MachineCode { get; set; }

        public string ProductCode { get; set; }

        public int PlanCount { get; set; }

        public string EmployeeID { get; set; }

        public string EmployeeName { get; set; }
 
        public int Beer { get; set; }

        public string ProduceTime { get; set; }

      
        public decimal ProduceCycle { get; set; }

        //版本
        public string Version { get; set; }

        //型腔穴数
        public string MouldOutput { get; set; }

        //机台吨位
        public string MachineTonnage { get; set; }

        //报价周期
        public decimal PlanCycle { get; set; }

        //差异周期
        public string DifferenceCycle { get; set; }
    }
}
