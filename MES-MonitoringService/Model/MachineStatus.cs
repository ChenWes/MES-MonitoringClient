﻿using System;
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
    [BsonIgnoreExtraElements]
    public class MachineStatus : SyncData
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }


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
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreateAt { get; set; }

        [BsonElement("LastUpdateAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime LastUpdateAt { get; set; }

        public override string getCollectionName()
        {
            return Common.ConfigFileHandler.GetAppConfig("MachineStatusCollectionName");
        }
    }
}
