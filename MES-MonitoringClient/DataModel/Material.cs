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
    public class Material : SyncData
    {
       
        [BsonElement("MaterialCode")]
        public string MaterialCode { get; set; }

        [BsonElement("MaterialName")]
        public string MaterialName { get; set; }

        [BsonElement("MaterialDesc")]
        public string MaterialDesc { get; set; }

        [BsonElement("MaterialSpecification")]
        public string MaterialSpecification { get; set; }

        [BsonElement("Remark")]
        public string Remark { get; set; }


        public override string getCollectionName()
        {
            return Common.ConfigFileHandler.GetAppConfig("MaterialCollectionName");
        }
    }
}
