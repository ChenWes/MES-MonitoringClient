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
    [BsonIgnoreExtraElements]
    public class JobPositon : SyncData
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }




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



        [BsonElement("CreateAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreateAt { get; set; }

        [BsonElement("LastUpdateAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime LastUpdateAt { get; set; }



        public override string getCollectionName()
        {
            return Common.ConfigFileHandler.GetAppConfig("JobPositionCollectionName");
        }
    }
}
