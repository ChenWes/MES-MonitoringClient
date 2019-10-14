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
    public class LastJobOrder
    {
        [BsonElement("JobOrderID")]
        public string JobOrderID { get; set; }

        [BsonElement("ChangeDate")]
        public DateTime ChangeDate { get; set; }

    }
}