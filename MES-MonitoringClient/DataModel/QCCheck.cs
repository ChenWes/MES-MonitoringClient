using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_MonitoringClient.DataModel
{
    [BsonIgnoreExtraElements]
    public class QCCheck
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        [BsonElement("code")]
        public string code{ get; set; }

        [BsonElement("desc")]
        public string desc { get; set; }

   
    }
}
