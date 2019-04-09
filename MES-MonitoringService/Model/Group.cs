using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using Newtonsoft.Json;


namespace MES_MonitoringService.Model
{
    [BsonIgnoreExtraElements]
    public class Group : SyncData
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }




        [BsonElement("GroupCode")]
        public string GroupCode { get; set; }

        [BsonElement("GroupName")]
        public string GroupName { get; set; }

        [BsonElement("GroupDesc")]
        public string GroupDesc { get; set; }


        [BsonElement("DepartmentID")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string DepartmentID { get; set; }

        [BsonElement("Remark")]
        public string Remark { get; set; }
        




        [BsonElement("CreateAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreateAt { get; set; }

        [BsonElement("LastUpdateAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime LastUpdateAt { get; set; }



        public override string getCollectionName()
        {
            return Common.ConfigFileHandler.GetAppConfig("GroupCollectionName");
        }
    }
}
