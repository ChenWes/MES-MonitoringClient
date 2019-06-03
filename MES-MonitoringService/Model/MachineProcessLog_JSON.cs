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
    public class JobOrder_MachineProcessLog_JSON
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
        public string ProduceStartDate { get; set; }

        [BsonElement("ProduceEndDate")]
        public string ProduceEndDate { get; set; }

        [BsonElement("EmployeeID")]
        public string EmployeeID { get; set; }


        public JobOrder_MachineProcessLog_JSON(Model.JobOrder_MachineProcessLog jobOrder_MachineProcessLog)
        {
            _id = jobOrder_MachineProcessLog._id;

            MachineID = jobOrder_MachineProcessLog.MachineID;

            ProduceCount = jobOrder_MachineProcessLog.ProduceCount;
            ErrorCount = jobOrder_MachineProcessLog.ErrorCount;

            ProduceStartDate = jobOrder_MachineProcessLog.ProduceStartDate.ToString("yyyy-MM-dd HH:mm:ss.fffffffK");
            ProduceEndDate = jobOrder_MachineProcessLog.ProduceEndDate.ToString("yyyy-MM-dd HH:mm:ss.fffffffK");

            EmployeeID = jobOrder_MachineProcessLog.EmployeeID;
        }

    }
}
