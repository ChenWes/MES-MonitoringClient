using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MES_MonitoringClient.DataModel
{
    [BsonIgnoreExtraElements]
    public class DefectiveType
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        public int sort { get; set; }
        


        [BsonElement("DefectiveTypeCode")]
        public string DefectiveTypeCode{ get; set; }

        [BsonElement("DefectiveTypeName")]
        public string DefectiveTypeName { get; set; }

   
    }
}
