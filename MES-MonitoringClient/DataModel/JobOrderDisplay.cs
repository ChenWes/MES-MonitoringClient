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
    public class JobOrderDisplay : SyncData
    {

        [BsonElement("JobOrderCode")]
        public string JobOrderCode { get; set; }

        [BsonElement("JobOrderName")]
        public string JobOrderName { get; set; }

        [BsonElement("JobOrderDesc")]
        public string JobOrderDesc { get; set; }

        [BsonElement("OrderCount")]
        public int OrderCount { get; set; }


        //客户----------------------------------------------------------------


        [BsonElement("CustomerID")]
        public string CustomerID { get; set; }

        [BsonElement("CustomerCode")]
        public string CustomerCode { get; set; }

        [BsonElement("CustomerName")]
        public string CustomerName { get; set; }



        //产品----------------------------------------------------------------

        [BsonElement("MaterialID")]
        public string MaterialID { get; set; }

        [BsonElement("MaterialCode")]
        public string MaterialCode { get; set; }

        [BsonElement("MaterialName")]
        public string MaterialName { get; set; }

        [BsonElement("MaterialDesc")]
        public string MaterialDesc { get; set; }

        [BsonElement("MaterialSpecification")]
        public string MaterialSpecification { get; set; }


        //模具----------------------------------------------------------------

        [BsonElement("MouldID")]
        public string MouldID { get; set; }

        [BsonElement("MouldCode")]
        public string MouldCode { get; set; }

        [BsonElement("MouldName")]
        public string MouldName { get; set; }

        [BsonElement("MouldDesc")]
        public string MouldDesc { get; set; }

        [BsonElement("MouldSpecification")]
        public string MouldSpecification { get; set; }

        [BsonElement("StandardProduceSecond")]
        [BsonRepresentation(BsonType.Double)]
        public decimal StandardProduceSecond { get; set; }

        //机器----------------------------------------------------------------

        [BsonElement("MachineID")]
        public string MachineID { get; set; }





        [BsonElement("Remark")]
        public string Remark { get; set; }

        [BsonElement("Status")]
        public string Status { get; set; }


        public override string getCollectionName()
        {
            return Common.ConfigFileHandler.GetAppConfig("JobOrderCollectionName");
        }
    }
}
