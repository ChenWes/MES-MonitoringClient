using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_MonitoringClient.Common
{
    /// <summary>
    /// 机器工序
    /// </summary>
    public class MachineProcedure
    {
        /// <summary>
        /// 0800/0400/0222等原本的信号
        /// </summary>
        public string ProcedureID { get; set; }

        /// <summary>
        /// X01/X02/X03等转义过的信号
        /// </summary>
        public string ProcedureCode { get; set; }

        /// <summary>
        /// 工序名称
        /// </summary>
        public string ProcedureName { get; set; }

        /// <summary>
        /// 工序使用毫秒数
        /// </summary>
        public long UseMilliseconds { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndDateTime { get; set; }

        public override string ToString()
        {
            return "[" + ProcedureName + "]从[" + StartDateTime.ToString("yyyy-MM-dd HH:mm:tt") + "]至[" + EndDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "]，共使用了" + (UseMilliseconds / 1000).ToString() + "秒";
        }
    }
}
