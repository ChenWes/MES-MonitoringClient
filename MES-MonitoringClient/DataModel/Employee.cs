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

namespace MES_MonitoringClient.DataModel
{
    [BsonIgnoreExtraElements]
    public class Employee : SyncData
    {
        [BsonElement("EmployeeCode")]
        public string EmployeeCode { get; set; }

        [BsonElement("EmployeeName")]
        public string EmployeeName { get; set; }

        [BsonElement("CardID")]
        public string CardID { get; set; }

        [BsonElement("MobileNumber")]
        public string MobileNumber { get; set; }

        [BsonElement("Email")]
        public string Email { get; set; }

        [BsonElement("Icon")]
        public string Icon { get; set; }



        [BsonElement("GroupID")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string GroupID { get; set; }

        [BsonElement("WorkShiftID")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string WorkShiftID { get; set; }

        [BsonElement("JobPostionID")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string JobPostionID { get; set; }




        [BsonElement("JobPostionRank")]
        public string JobPostionRank { get; set; }

        [BsonElement("JobPostionResponsibility")]
        public string JobPostionResponsibility { get; set; }

        [BsonElement("Remark")]
        public string Remark { get; set; }

        [BsonElement("IsActive")]
        public bool IsActive { get; set; }


        [BsonElement("IsSyncImage")]
        public bool IsSyncImage { get; set; }

        [BsonElement("LocalFileName")]
        public string LocalFileName { get; set; }



        public override string getCollectionName()
        {
            return Common.ConfigFileHandler.GetAppConfig("EmployeeCollectionName");
        }
    }
}
