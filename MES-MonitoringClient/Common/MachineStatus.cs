using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_MonitoringClient.Common
{
    public class MachineStatus
    {
        public enum eumMachineStatus
        {
            //未分配
            Produce,
            CheckMould
        }
    }
}
