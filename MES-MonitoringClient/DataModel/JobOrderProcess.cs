using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace MES_MonitoringClient.DataModel
{
    [BsonIgnoreExtraElements]
    public class JobOrderProcess : SyncData
    {

        [BsonElement("JobOrderID")]
        public string JobOrderID { get; set; }

        [BsonElement("Status")]
        public string Status { get; set; }

        //开始的时间
        [BsonElement("StartDateTime")]
        public DateTime StartDateTime { get; set; }


        //已经生产的数量
        [BsonElement("ProductCount")]
        public int ProductCount { get; set; }

        //或错误的数量
        [BsonElement("ProductErrorCount")]
        public int ProductErrorCount { get; set; }

        public override string getCollectionName()
        {
            return Common.ConfigFileHandler.GetAppConfig("JobOrderProcessCollectionName");
        }
    }
}
