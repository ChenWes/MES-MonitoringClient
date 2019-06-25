//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using MongoDB;
//using MongoDB.Bson;
//using MongoDB.Bson.Serialization.Attributes;
//using MongoDB.Bson.Serialization.IdGenerators;

//using Newtonsoft.Json;

//namespace MES_MonitoringClient.DataModel
//{
//    [BsonIgnoreExtraElements]
//    public class Mould : SyncData
//    {
       
//        [BsonElement("MouldCode")]
//        public string MouldCode { get; set; }

//        [BsonElement("MouldName")]
//        public string MouldName { get; set; }

//        [BsonElement("MouldDesc")]
//        public string MouldDesc { get; set; }

//        [BsonElement("MouldSpecification")]
//        public string MouldSpecification { get; set; }

//        [BsonElement("StandardProduceSecond")]
//        [BsonRepresentation(BsonType.Double)]
//        public decimal StandardProduceSecond { get; set; }

//        [BsonElement("Remark")]
//        public string Remark { get; set; }



//        public override string getCollectionName()
//        {
//            return Common.ConfigFileHandler.GetAppConfig("MouldCollectionName");
//        }
//    }
//}
