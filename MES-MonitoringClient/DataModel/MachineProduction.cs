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
    public class MachineProduction
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        //日期
        [BsonElement("Date")]
        public DateTime Date { get; set; }
        //机器
        [BsonElement("MachineID")]
        public string MachineID { get; set; }
        //工单
        [BsonElement("JobOrderID")]
        public string JobOrderID { get; set; }

        //生产明细
        [BsonElement("ProductionDetails")]
        public List<ProductionDetails> ProductionDetails { get; set; }
       
        //生产数
        [BsonElement("ProduceCount")]
        public int ProduceCount { get; set; }
        //班次
        [BsonElement("WorkShiftID")]
        public string WorkShiftID { get; set; }
        //员工工时
        [BsonElement("EmployeeProductionTimeList")]
        public List<EmployeeProductionTimeList> EmployeeProductionTimeList { get; set; }
        //周期
        [BsonElement("ProduceSecond")]
        public double ProduceSecond { get; set; }

        //工单生产工时
        [BsonElement("JobOrderProductionTime")]
        public double JobOrderProductionTime { get; set; }
        //工单生产记录
        [BsonElement("JobOrderProductionLog")]
        public List<JobOrderProductionLog> JobOrderProductionLog { get; set; }
        //不良品数
        [BsonElement("ErrorCount")]
        public int ErrorCount { get; set; }
        //是否结束
        [BsonElement("IsStopFlag")]
        public bool IsStopFlag { get; set; }
        //是否同步
        [BsonElement("IsSyncToServer")]
        public bool IsSyncToServer { get; set; }

        
    }

    public class ProductionDetails
    {
        //开始时间
        [BsonElement("StartDateTime")]
        public DateTime StartDateTime { get; set; }
        //结束时间
        [BsonElement("EndDateTime")]
        public DateTime EndDateTime { get; set; }
        //生产数
        [BsonElement("ProduceCount")]
        public int ProduceCount { get; set; }
    }

    public class JobOrderProductionLog
    {
        [BsonElement("MachineProcessLogID")]
        public string MachineProcessLogID { get; set; }

        [BsonElement("ProduceStartDate")]
        public DateTime ProduceStartDate { get; set; }

        [BsonElement("ProduceEndDate")]
        public DateTime ProduceEndDate { get; set; }
    }

    public class EmployeeProductionTimeList
    {
        [BsonElement("EmployeeID")]
        public string EmployeeID { get; set; }

        [BsonElement("StartTime")]
        public DateTime StartTime { get; set; }

        [BsonElement("EndTime")]
        public DateTime EndTime { get; set; }

        [BsonElement("WorkHour")]
        public double WorkHour { get; set; }
    }
}
