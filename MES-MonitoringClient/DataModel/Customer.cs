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
    [BsonIgnoreExtraElements]
    public class Customer : SyncData
    {
       
        [BsonElement("CustomerCode")]
        public string CustomerCode { get; set; }

        [BsonElement("CustomerName")]
        public string CustomerName { get; set; }

        [BsonElement("CustomerDesc")]
        public string CustomerDesc { get; set; }


        [BsonElement("Remark")]
        public string Remark { get; set; }



        public override string getCollectionName()
        {
            return Common.ConfigFileHandler.GetAppConfig("CustomerCollectionName");
        }
    }
}
