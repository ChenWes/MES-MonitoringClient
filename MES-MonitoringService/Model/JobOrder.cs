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
    public class JobOrder : SyncData
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
        public DateTime DeliveryDate { get; set; }


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
        public DateTime CompletedDate { get; set; }

        [BsonElement("CompletedOperaterID")]
        public string CompletedOperaterID { get; set; }



        [BsonElement("MachineProcessLog")]
        public List<JobOrder_MachineProcessLog> MachineProcessLog { get; set; }


        [BsonElement("MachineChangeLog")]
        public List<JobOrder_MachineChangeLog> MachineChangeLog { get; set; }


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

        [BsonElement("MouldLifecycle")]
        public int MouldLifecycle { get; set; }

        [BsonElement("SumErrorCount")]
        public int SumErrorCount { get; set; }


        public override string getCollectionName()
        {
            return Common.ConfigFileHandler.GetAppConfig("JobOrderCollectionName");
        }
    }
}
