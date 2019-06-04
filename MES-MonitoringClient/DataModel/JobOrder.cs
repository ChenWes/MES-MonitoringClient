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

        [BsonElement("JobOrderCode")]
        public string JobOrderCode { get; set; }

        [BsonElement("JobOrderName")]
        public string JobOrderName { get; set; }

        [BsonElement("JobOrderDesc")]
        public string JobOrderDesc { get; set; }

        [BsonElement("OrderCount")]
        public int OrderCount { get; set; }

        [BsonElement("OrderDate")]
        public DateTime OrderDate { get; set; }

        [BsonElement("DeliveryDate")]
        public DateTime DeliveryDate { get; set; }


        [BsonElement("CustomerID")]
        public string CustomerID { get; set; }

        [BsonIgnore]
        public DataModel.Customer Customer { get; set; }

        [BsonElement("MaterialID")]
        public string MaterialID { get; set; }

        [BsonIgnore]
        public DataModel.Material Material { get; set; }

        [BsonElement("MachineID")]
        public string MachineID { get; set; }



        [BsonElement("Remark")]
        public string Remark { get; set; }

        [BsonElement("Status")]
        public string Status { get; set; }



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
