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
    public class JobOrder_JSON : SyncData
    {

        [BsonElement("JobOrderCode")]
        public string JobOrderCode { get; set; }

        [BsonElement("JobOrderName")]
        public string JobOrderName { get; set; }

        [BsonElement("JobOrderDesc")]
        public string JobOrderDesc { get; set; }

        [BsonElement("OrderCount")]
        public int OrderCount { get; set; }

        [BsonElement("OrderDate")]
        public string OrderDate { get; set; }

        [BsonElement("DeliveryDate")]
        public string DeliveryDate { get; set; }



        [BsonElement("CustomerID")]
        public string CustomerID { get; set; }


        [BsonElement("MaterialID")]
        public string MaterialID { get; set; }


        [BsonElement("MachineID")]
        public string MachineID { get; set; }



        [BsonElement("Remark")]
        public string Remark { get; set; }

        [BsonElement("Status")]
        public string Status { get; set; }



        [BsonElement("MachineProcessLog")]
        public List<JobOrder_MachineProcessLog_JSON> MachineProcessLog { get; set; }


        [BsonElement("MachineChangeLog")]
        public List<JobOrder_MachineChangeLog_JSON> MachineChangeLog { get; set; }


        [BsonElement("MachineAcceptLog")]
        public List<string> MachineAcceptLog { get; set; }


        public JobOrder_JSON(Model.JobOrder jobOrder)
        {
            _id = jobOrder._id;

            JobOrderCode = jobOrder.JobOrderCode;
            JobOrderName = jobOrder.JobOrderName;
            JobOrderDesc = jobOrder.JobOrderDesc;
            OrderCount = jobOrder.OrderCount;
            OrderDate = jobOrder.OrderDate.ToString("yyyy-MM-dd HH:mm:ss.fffffffK");
            DeliveryDate = jobOrder.DeliveryDate.ToString("yyyy-MM-dd HH:mm:ss.fffffffK");

            CustomerID = jobOrder.CustomerID;
            MaterialID = jobOrder.MaterialID;
            MachineID = jobOrder.MachineID;

            Remark = jobOrder.Remark;
            Status = jobOrder.Status;

            CreateAt = jobOrder.CreateAt;
            CreateBy = jobOrder.CreateBy;
            LastUpdateAt = jobOrder.LastUpdateAt;
            LastUpdateBy = jobOrder.LastUpdateBy;


            MachineProcessLog = new List<JobOrder_MachineProcessLog_JSON>();
            foreach (var item in jobOrder.MachineProcessLog)
            {
                MachineProcessLog.Add(new JobOrder_MachineProcessLog_JSON(item));
            }

            MachineChangeLog = new List<JobOrder_MachineChangeLog_JSON>();
            foreach (var item in jobOrder.MachineChangeLog)
            {
                MachineChangeLog.Add(new JobOrder_MachineChangeLog_JSON(item));
            }

            MachineAcceptLog = jobOrder.MachineAcceptLog;
        }


        public override string getCollectionName()
        {
            return Common.ConfigFileHandler.GetAppConfig("JobOrderCollectionName");
        }
    }
}
