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
    [BsonIgnoreExtraElements]
    public class JobOrder : SyncData
    {

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

		[BsonElement("MouldCode")]
		public string MouldCode { get; set; }


		[BsonElement("MouldStandardProduceSecond")]
		public int MouldStandardProduceSecond { get; set; }



		[BsonElement("Remark")]
		public string Remark { get; set; }

		[BsonElement("Status")]
		public string Status { get; set; }

		[BsonElement("MachineID")]
		public string MachineID { get; set; }



		[BsonElement("MachineProcessLog")]
        public List<JobOrder_MachineProcessLog> MachineProcessLog { get; set; }


        [BsonElement("MachineChangeLog")]
        public List<JobOrder_MachineChangeLog> MachineChangeLog { get; set; }


        [BsonElement("MachineAcceptLog")]
        public List<string> MachineAcceptLog { get; set; }


        public override string getCollectionName()
        {
            return Common.ConfigFileHandler.GetAppConfig("JobOrderCollectionName");
        }
    }
}
