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
    public class EmployeeWorkSchedule
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        [BsonElement("WorkShiftID")]
        public string WorkShiftID { get; set; }


        [BsonElement("ScheduleDate")]
        public string ScheduleDate { get; set; }

        [BsonElement("EmployeeID")]
        public string EmployeeID { get; set; }

        public DateTime startTime { get; set; }

        public DateTime endTime { get; set; }
    }
}
