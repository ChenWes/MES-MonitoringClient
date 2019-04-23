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
    public class Machine : SyncData
    {

        //机器
        //-------------------------------
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

        //车间
        //-------------------------------

        [BsonElement("WorkshopID")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string WorkshopID { get; set; }




        [BsonElement("Remark")]
        public string Remark { get; set; }

        public override string getCollectionName()
        {
            return Common.ConfigFileHandler.GetAppConfig("MachineCollectionName");
        }
    }
}
