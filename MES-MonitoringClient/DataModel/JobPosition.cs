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
    public class JobPositon : SyncData
    {
        [BsonElement("JobPositionCode")]
        public string JobPositionCode { get; set; }

        [BsonElement("JobPositionName")]
        public string JobPositionName { get; set; }

        [BsonElement("JobPositionDesc")]
        public string JobPositionDesc { get; set; }

        [BsonElement("Remark")]
        public string Remark { get; set; }

        [BsonElement("MachineStatuss")]
        public IEnumerable<string> MachineStatuss { get; set; }




        public override string getCollectionName()
        {
            return Common.ConfigFileHandler.GetAppConfig("JobPositionCollectionName");
        }
    }
}
