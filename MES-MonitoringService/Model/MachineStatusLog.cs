using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using MongoDB;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;


namespace MES_MonitoringService.Model
{
    /// <summary>
    /// 机器状态（本地数据库保存）
    /// </summary>
    public class MachineStatusLog
    {
        public ObjectId Id { get; set; }

        [BsonElement("Status")]
        public string Status { get; set; }

        [BsonElement("StartDateTime")]
        public DateTime StartDateTime { get; set; }

        [BsonElement("EndDateTime")]
        public DateTime EndDateTime { get; set; }

        [BsonElement("IsStopFlag")]
        public bool IsStopFlag { get; set; }



        /*上传及更新的标识*/
        /*-------------------------------------------------*/

        [BsonElement("IsUploadToServer")]
        public bool IsUploadToServer { get; set; }

        [BsonElement("IsUpdateToServer")]
        public bool IsUpdateToServer { get; set; }

        [BsonElement("LocalMacAddress")]
        public string LocalMacAddress { get; set; }
    }
}
