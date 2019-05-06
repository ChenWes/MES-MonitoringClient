﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using MongoDB;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson;

namespace MES_MonitoringClient.DataModel
{
    /// <summary>
    /// 机器状态（本地数据库保存）
    /// </summary>
    public class MachineStatusLog
    {
        public ObjectId Id { get; set; }

        [BsonElement("StatusID")]
        public string StatusID { get; set; }

        [BsonElement("StatusCode")]
        public string StatusCode { get; set; }

        [BsonElement("StatusName")]
        public string StatusName { get; set; }

        [BsonElement("StatusDesc")]
        public string StatusDesc { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonElement("StartDateTime")]
        public DateTime? StartDateTime { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonElement("EndDateTime")]
        public DateTime? EndDateTime { get; set; }

        [BsonElement("IsStopFlag")]
        public bool IsStopFlag { get; set; }

        [BsonElement("UseTotalSeconds")]
        public int UseTotalSeconds { get; set; }


        /*上传及更新的标识*/
        /*-------------------------------------------------*/

        [BsonElement("IsUploadToServer")]
        public bool IsUploadToServer { get; set; }

        [BsonElement("IsUpdateToServer")]
        public bool IsUpdateToServer { get; set; }

        [BsonElement("LocalMacAddress")]
        public string LocalMacAddress { get; set; }

        //机器注册ID
        [BsonElement("MachineID")]
        public string MachineID { get; set; }
    }
}
