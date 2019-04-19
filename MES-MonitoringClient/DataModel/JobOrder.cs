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

        [BsonElement("CustomerID")]
        public string CustomerID { get; set; }

        [BsonElement("MaterialID")]
        public string MaterialID { get; set; }

        [BsonElement("MouldID")]
        public string MouldID { get; set; }

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
