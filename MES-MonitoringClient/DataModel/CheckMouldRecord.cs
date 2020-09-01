using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_MonitoringClient.DataModel
{
    [BsonIgnoreExtraElements]
    public class CheckMouldRecord
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        [BsonElement("MachineStatusLogID")]
        public string MachineStatusLogID { get; set; }


        [BsonElement("MouldCode")]
        public string MouldCode { get; set; }

        [BsonElement("ProductCode")]
        public string ProductCode { get; set; }

        [BsonElement("PlanCount")]
        public int PlanCount { get; set; }

        [BsonElement("EmployeeID")]
        public string EmployeeID { get; set; }

        [BsonElement("EmployeeName")]
        public string EmployeeName { get; set; }

        [BsonElement("CheckMouldLog")]
        public List<CheckMouldLog> CheckMouldLog { get; set; }
    }
    public class CheckMouldLog
    {
        [BsonElement("Beer")]
        public int Beer { get; set; }

        [BsonElement("ProduceTime")]
        public DateTime ProduceTime { get; set; }

        [BsonElement("ProduceCycle")]
        public decimal ProduceCycle { get; set; }
    }
}
