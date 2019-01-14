using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_MonitoringClient.DataModel
{
    /// <summary>
    /// 机器各状态持续总时间
    /// </summary>
    public class MachineStatusUseTime
    {
        public string Status { get; set; }
        public int UseTotalSeconds { get; set; }
    }
}
