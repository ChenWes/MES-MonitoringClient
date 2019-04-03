using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

using Newtonsoft.Json;

namespace MES_MonitoringService.Model
{
    public class MachineStatus : SyncData
    {
        //public ObjectId Id { get; set; }

        [BsonElement("OriginalID")]
        public string OriginalID { get; set; }


        [BsonElement("MachineStatusCode")]
        public string MachineStatusCode { get; set; }

        [BsonElement("MachineStatusName")]
        public string MachineStatusName { get; set; }

        [BsonElement("MachineStatusDesc")]
        public string MachineStatusDesc { get; set; }


        [BsonElement("Remark")]
        public string Remark { get; set; }

        [BsonElement("IsActive")]
        public bool IsActive { get; set; }

        /// <summary>
        /// 状态的颜色
        /// </summary>
        [BsonElement("StatusColor")]
        public string StatusColor { get; set; }


        [BsonElement("CreateAt")]
        public DateTime CreateAt { get; set; }

        [BsonElement("LastUpdateAt")]
        public DateTime LastUpdateAt { get; set; }

    }
}
