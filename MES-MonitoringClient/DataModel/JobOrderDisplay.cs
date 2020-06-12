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
    public class JobOrderDisplay
    {

        public string ID { get; set; }

        public string JobOrderID { get; set; }
		
		public string JobOrderNumber { get; set; }
		
		public string ProductCode { get; set; }
        
        public string ProductDescription { get; set; }
        
		public string ProductCategory { get; set; }
		
		public int OrderCount { get; set; }


        //已生产总数
        public int sumProduceCount { get; set; }

        //不良品总数
        public int sumErrorCount { get; set; }

        //未完成数量
        public int sumNoCompleted { get; set; }

        //剩余时间（秒数）
        public double sumNeedSecond { get; set; }

        //剩余时间（文本）
        public string sumNeedSecondDesc { get; set; }



        
		public string MaterialCode { get; set; }


		
		public string DeliveryDate { get; set; }

		
		public int MachineTonnage { get; set; }

		//[BsonElement("MouldCode")]
		//public string MouldCode { get; set; }


        public string MouldID { get; set; }

        
        public double MouldStandardProduceSecond { get; set; }



        //[BsonElement("Remark")]
        //public string Remark { get; set; }
        
		public string Status { get; set; }

        public string ServiceDepartment { get; set; }

        public DateTime ReceiveDate { get; set; }

        public int sort { get; set; }

        //[BsonElement("MachineID")]
        //public string MachineID { get; set; }


        //blic int Sort { get; set; }

    }
}
