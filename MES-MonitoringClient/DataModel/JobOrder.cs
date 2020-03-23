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

        [BsonElement("ProductDescription")]
        public string ProductDescription { get; set; }

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


        /// <summary>
        /// 2019-08-08 特别留意，Mongodb不支持decimal类型，如果是decimal保存到数据库会变成文本，需要转换成double才可以
        /// https://stackoverflow.com/questions/20036529/need-to-store-high-precision-decimal-values-in-mongodb
        /// </summary>
		[BsonElement("MouldStandardProduceSecond")]
		public double MouldStandardProduceSecond { get; set; }



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


        [BsonElement("Sort")]
        public int Sort { get; set; }

        [BsonElement("ServiceDepartment")]
        public string ServiceDepartment { get; set; }

        [BsonElement("CollectionNum")]
        public int CollectionNum { get; set; }

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
