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

namespace MES_MonitoringClient.DataModel
{
    public abstract class SyncData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }



        [BsonElement("CreateAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreateAt { get; set; }

        [BsonElement("LastUpdateAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime LastUpdateAt { get; set; }


        /// <summary>
        /// 获取当前实体对应的Collection数据集名称
        /// </summary>
        /// <returns></returns>
        public abstract string getCollectionName();

        public abstract string getCollectionDataUrl();
    }

}
