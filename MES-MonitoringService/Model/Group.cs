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



        public override string getCollectionName()
        {
            return Common.ConfigFileHandler.GetAppConfig("GroupCollectionName");
        }
    }
}
