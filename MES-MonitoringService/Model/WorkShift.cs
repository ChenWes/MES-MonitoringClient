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

        [BsonElement("WorkShiftType")]
        public string WorkShiftType { get; set; }



        public override string getCollectionName()
        {
            return Common.ConfigFileHandler.GetAppConfig("WorkShiftCollectionName");
        }
    }
}
