using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MES_MonitoringClient.DataModel
{
    [BsonIgnoreExtraElements]
    public class ClockInRecord
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        [BsonElement("MachineID")]
        public string MachineID { get; set; }


        [BsonElement("EmployeeID")]
        public string EmployeeID { get; set; }

        [BsonElement("StartDate")]
        public DateTime StartDate { get; set; }

        [BsonElement("EndDate")]
        public DateTime EndDate { get; set; }

        [BsonElement("IsUploadToServer")]
        public bool IsUploadToServer { get; set; }

        [BsonElement("IsAuto")]
        public bool IsAuto { get; set; }
    }
}
