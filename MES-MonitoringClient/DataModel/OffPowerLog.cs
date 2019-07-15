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
    public class OffPowerLog
    {

        //人员
        //-------------------------------
        [BsonElement("EmployeeID")]
        public string EmployeeID { get; set; }

        [BsonElement("EmployeeName")]
        public string EmployeeName { get; set; }

        [BsonElement("DateTime")]
        public DateTime DateTime { get; set; }

        [BsonElement("Remark")]
        public string Remark { get; set; }
    }
}
