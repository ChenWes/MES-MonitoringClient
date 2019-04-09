using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_MonitoringClient.DataModel
{
    public class SyncData_LoopHelper<T> where T : SyncData
    {
        public T classType { get; set; }


        public string Url { get; set; }
    }
}
