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
    public class JobOrder_MachineChangeLog_JSON
    {
        [BsonId]
        public string _id { get; set; }

        [BsonElement("MachineID")]
        public string MachineID { get; set; }

        [BsonElement("ChangeDate")]
        public string ChangeDate { get; set; }

        public JobOrder_MachineChangeLog_JSON(Model.JobOrder_MachineChangeLog jobOrder_MachineChangeLog)
        {
            _id = jobOrder_MachineChangeLog._id;

            MachineID = jobOrder_MachineChangeLog.MachineID;

            ChangeDate = jobOrder_MachineChangeLog.ChangeDate.ToString("yyyy-MM-dd HH:mm:ss.fffffffK");
        }

    }
}
