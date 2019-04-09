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
    public class WorkShift : SyncData
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }



        [BsonElement("WorkShiftCode")]
        public string WorkShiftCode { get; set; }

        [BsonElement("WorkShiftName")]
        public string WorkShiftName { get; set; }

        [BsonElement("WorkShiftDesc")]
        public string WorkShiftDesc { get; set; }

        [BsonElement("Remark")]
        public string Remark { get; set; }

        [BsonElement("WorkShiftStartTime")]
        public string WorkShiftStartTime { get; set; }

        [BsonElement("WorkShiftEndTime")]
        public string WorkShiftEndTime { get; set; }




        [BsonElement("CreateAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreateAt { get; set; }

        [BsonElement("LastUpdateAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime LastUpdateAt { get; set; }



        public override string getCollectionName()
        {
            return Common.ConfigFileHandler.GetAppConfig("WorkShiftCollectionName");
        }
    }
}
