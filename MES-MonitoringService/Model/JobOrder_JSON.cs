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

		[BsonElement("JobOrderID")]
		public string JobOrderID { get; set; }

		[BsonElement("JobOrderNumber")]
		public string JobOrderNumber { get; set; }

		[BsonElement("ProductCode")]
		public string ProductCode { get; set; }

		[BsonElement("ProductCategory")]
		public string ProductCategory { get; set; }

		[BsonElement("OrderCount")]
		public int OrderCount { get; set; }

		[BsonElement("MaterialCode")]
		public string MaterialCode { get; set; }



		[BsonElement("DeliveryDate")]
		public string DeliveryDate { get; set; }


		[BsonElement("MachineTonnage")]
		public int MachineTonnage { get; set; }


		[BsonElement("MouldStandardProduceSecond")]
		public decimal MouldStandardProduceSecond { get; set; }



		[BsonElement("Remark")]
		public string Remark { get; set; }

		[BsonElement("Status")]
		public string Status { get; set; }

		[BsonElement("MachineID")]
		public string MachineID { get; set; }


        [BsonElement("CompletedDate")]
        public string CompletedDate { get; set; }

        [BsonElement("CompletedOperaterID")]
        public string CompletedOperaterID { get; set; }


        [BsonElement("MachineProcessLog")]
        public List<JobOrder_MachineProcessLog_JSON> MachineProcessLog { get; set; }


        [BsonElement("MachineChangeLog")]
        public List<JobOrder_MachineChangeLog_JSON> MachineChangeLog { get; set; }


        [BsonElement("MachineAcceptLog")]
        public List<string> MachineAcceptLog { get; set; }

        [BsonElement("ProductDescription")]
        public string ProductDescription { get; set; }



        [BsonElement("MouldCode")]
        public string MouldCode { get; set; }

        [BsonElement("Sort")]
        public int Sort { get; set; }

        [BsonElement("ServiceDepartment")]
        public string ServiceDepartment { get; set; }

        public JobOrder_JSON(Model.JobOrder jobOrder)
        {
            _id = jobOrder._id;

            JobOrderID = jobOrder.JobOrderID;
            JobOrderNumber = jobOrder.JobOrderNumber;
            ProductCode = jobOrder.ProductCode;
            ProductCategory = jobOrder.ProductCategory;
            OrderCount = jobOrder.OrderCount;
            MaterialCode = jobOrder.MaterialCode;

            DeliveryDate = jobOrder.DeliveryDate.ToString("yyyy-MM-dd HH:mm:ss.fffffffK");
            MachineTonnage = jobOrder.MachineTonnage;


            MouldStandardProduceSecond = jobOrder.MouldStandardProduceSecond;
            Remark = jobOrder.Remark;
            Status = jobOrder.Status;
            MachineID = jobOrder.MachineID;

            CompletedDate = jobOrder.CompletedDate.ToString("yyyy-MM-dd HH:mm:ss.fffffffK");
            CompletedOperaterID = jobOrder.CompletedOperaterID;


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
            ProductDescription = jobOrder.ProductDescription;
            MouldCode = jobOrder.MouldCode;
            Sort = jobOrder.Sort;
            ServiceDepartment = jobOrder.ServiceDepartment;
        }


        public override string getCollectionName()
        {
            return Common.ConfigFileHandler.GetAppConfig("JobOrderCollectionName");
        }
    }
}
