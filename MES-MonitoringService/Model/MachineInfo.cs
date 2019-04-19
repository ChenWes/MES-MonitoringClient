using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace MES_MonitoringService.Model
{
    public class MachineInfo
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        [BsonElement("MachineID")]
        public string MachineID { get; set; }

        [BsonElement("MachineCode")]
        public string MachineCode { get; set; }

        [BsonElement("MachineName")]
        public string MachineName { get; set; }

        [BsonElement("MachineDesc")]
        public string MachineDesc { get; set; }

        [BsonElement("MACAddress")]
        public string MACAddress { get; set; }

        [BsonElement("Tonnage")]
        public string Tonnage { get; set; }


        [BsonElement("WorkshopCode")]
        public string WorkshopCode { get; set; }
        [BsonElement("WorkshopName")]
        public string WorkshopName { get; set; }


        [BsonElement("FactoryCode")]
        public string FactoryCode { get; set; }
        [BsonElement("FactoryName")]
        public string FactoryName { get; set; }

    }
}
