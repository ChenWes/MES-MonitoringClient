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
    public class MachineRegisterInfo
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        //机器ID(只保存机器注册码，即机器ID)
        //与之关联的车间和工厂，可以在需要的时候直接关联，不需要写入至DB中
        //-------------------------------
        [BsonElement("MachineID")]
        public string MachineID { get; set; }

    }
}
