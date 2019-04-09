using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

using Newtonsoft.Json;

namespace MES_MonitoringService.Model
{
    [BsonIgnoreExtraElements]
    public class Department : SyncData
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        

        [BsonElement("DepartmentCode")]
        public string DepartmentCode { get; set; }

        [BsonElement("DepartmentName")]
        public string DepartmentName { get; set; }

        [BsonElement("DepartmentDesc")]
        public string DepartmentDesc { get; set; }


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
            return Common.ConfigFileHandler.GetAppConfig("DepartmentCollectionName");
        }
    }
}
