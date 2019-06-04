using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MES_MonitoringService.Model
{
    [BsonIgnoreExtraElements]
    public class JobOrder_MachineChangeLog
    {
        [BsonId]
        public string _id { get; set; }

        [BsonElement("MachineID")]
        public string MachineID { get; set; }

        [BsonElement("ChangeDate")]
        public DateTime ChangeDate { get; set; }

    }
}
