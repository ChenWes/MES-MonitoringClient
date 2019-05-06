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

        public string StatusID { get; set; }

        public string StatusCode { get; set; }

        public string StatusName { get; set; }

        public string StatusDesc { get; set; }

        public string StartDateTime { get; set; }        

        public string EndDateTime { get; set; }
        
        public decimal UseTotalSeconds { get; set; }

        public bool IsStopFlag { get; set; }

        public string LocalMacAddress { get; set; }

        //机器注册ID        
        public string MachineID { get; set; }
    }
}
