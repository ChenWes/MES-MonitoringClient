using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_MonitoringService.Model
{
    /// <summary>
    /// 机器状态日志JSON格式(Nodejs端无法解析ObjectID和ISODate)
    /// </summary>
    public class MachineStatusLog_JSON
    {
        public string Id { get; set; }        

        public string Status { get; set; }    

        public string StartDateTime { get; set; }        

        public string EndDateTime { get; set; }
        
        public decimal UseTotalSeconds { get; set; }

        public bool IsStopFlag { get; set; }

        public string LocalMacAddress { get; set; }                     
    }
}
