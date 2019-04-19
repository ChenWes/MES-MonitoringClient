using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_MonitoringClient.DataModel
{
    /// <summary>
    /// 机器各状态持续总时间
    /// 用于在主页面的机器状态对比饼图中
    /// </summary>
    public class MachineStatusUseTime
    {
        /// <summary>
        /// 机器状态标题
        /// </summary>
        public string StatusText { get; set; }

        /// <summary>
        /// 使用总秒数
        /// </summary>
        public int UseTotalSeconds { get; set; }

        /// <summary>
        /// 机器状态显示的颜色（与状态灯一致）
        /// </summary>
        public string StatusColor { get; set; }
    }
}
