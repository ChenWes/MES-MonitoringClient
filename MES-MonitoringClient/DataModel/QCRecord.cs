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
    public class QCRecord
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        [BsonElement("Date")]
        public DateTime Date { get; set; }

        [BsonElement("WorkShiftID")]
        public string WorkShiftID { get; set; }

        [BsonElement("JobOrderID")]
        public string JobOrderID { get; set; }

        [BsonElement("MachineProcessLogID")]
        public string MachineProcessLogID { get; set; }

        [BsonElement("MachineID")]
        public string MachineID { get; set; }

        [BsonElement("QCCheckCounts")]
        public List<QCCheckCount> QCCheckCounts { get; set; }

        [BsonElement("ErrorCount")]
        public int ErrorCount { get; set; }

        [BsonElement("EmployeeID")]
        public string EmployeeID { get; set; }

        [BsonElement("QCTime")]
        public DateTime QCTime { get; set; }

        [BsonElement("IsSyncToServer")]
        public bool IsSyncToServer { get; set; }
    }
    public class QCCheckCount
    {
        [BsonElement("DefectiveTypeID")]
        public string DefectiveTypeID { get; set; }

        [BsonElement("count")]
        public int Count { get; set; }
    }
}
