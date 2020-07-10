using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MES_MonitoringClient.DataModel
{
    [BsonIgnoreExtraElements]
    public class WorkshopResponsiblePerson
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

       

        [BsonElement("WorkshopID")]
        public string WorkshopID { get; set; }

        [BsonElement("ResponsiblePersonID")]
        public string[] ResponsiblePersonID { get; set; }

    }


}
