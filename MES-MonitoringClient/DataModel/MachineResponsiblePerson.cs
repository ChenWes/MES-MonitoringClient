using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_MonitoringClient.DataModel
{
    public class MachineResponsiblePerson
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        [BsonElement("ResponsiblePersonID")]
        public string[] ResponsiblePersonID { get; set; }

        [BsonElement("MachineID")]
        public string[] MachineID{ get; set; }

        [BsonElement("CreateAt")]
        public string CreateAt { get; set; }

        [BsonElement("LastUpdateAt")]
        public string LastUpdateAt { get; set; }

        [BsonElement("__v")]
        public int __v { get; set; }
    }
}
