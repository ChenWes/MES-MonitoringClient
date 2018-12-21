using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MES_MonitoringClient.DataModel
{
    /// <summary>
    /// 机器状态（本地数据库保存）
    /// </summary>
    public class MachineStatuTimeLine
    {   
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartDateTime { get; set; }           

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndDateTime { get; set; }

        //public string TimeLineString { get; set; }

        /// <summary>
        /// 是否已经上传至服务器标识
        /// </summary>
        public bool IsUploadToServer { get; set; }
    }
}
