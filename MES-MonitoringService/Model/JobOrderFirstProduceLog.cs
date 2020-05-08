using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace MES_MonitoringService.Model
{
    [BsonIgnoreExtraElements]
    public class JobOrderFirstProduceLog
    {
      
        [BsonElement("JobOrderID")]
        public string JobOrderID { get; set; }

        [BsonElement("MachineID")]
        public string MachineID { get; set; }

        [BsonElement("StartDateTime")]
        public DateTime StartDateTime { get; set; }

    }

}
