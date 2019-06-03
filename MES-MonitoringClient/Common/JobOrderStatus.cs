using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_MonitoringClient.Common
{
    public class JobOrderStatus
    {
        public enum eumJobOrderStatus
        {
            //未分配
            Unallocated,

            //已分配
            Assigned,

            //生产中
            Producing,

            //暂停
            Suspend,

            //完成
            Completed
        }
    }
}
