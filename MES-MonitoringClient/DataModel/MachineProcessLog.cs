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
    public class JobOrder_MachineProcessLog
    {
        [BsonId]
        public string _id { get; set; }

        [BsonElement("MachineID")]
        public string MachineID { get; set; }

        [BsonElement("ProduceCount")]
        public int ProduceCount { get; set; }

        [BsonElement("ErrorCount")]
        public int ErrorCount { get; set; }

        [BsonElement("ProduceStartDate")]
        public DateTime ProduceStartDate { get; set; }

        [BsonElement("ProduceEndDate")]
        public DateTime ProduceEndDate { get; set; }

        [BsonElement("EmployeeID")]
        public string EmployeeID { get; set; }
        
    }
}
