using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_MonitoringService.Model
{
    [BsonIgnoreExtraElements]
    public class QCRecord
    {
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
    }
    public class QCCheckCount
    {
        [BsonElement("DefectiveTypeID")]
        public string DefectiveTypeID { get; set; }

        [BsonElement("count")]
        public int Count { get; set; }
    }
}
