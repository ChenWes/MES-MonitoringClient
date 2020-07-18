﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_MonitoringService.Model
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
        //开始时间
        [BsonElement("StartDateTime")]
        public DateTime StartDateTime { get; set; }
        //结束时间
        [BsonElement("EndDateTime")]
        public DateTime EndDateTime { get; set; }
        //工单数
        [BsonElement("ProduceCount")]
        public int ProduceCount { get; set; }
        //班次
        [BsonElement("WorkShiftID")]
        public string WorkShiftID { get; set; }
        //员工工时
        [BsonElement("EmployeeProductionTimeList")]
        public List<EmployeeProductionTimeList> EmployeeProductionTimeList { get; set; }
        //工单生产工时
        [BsonElement("JobOrderProductionTime")]
        public double JobOrderProductionTime { get; set; }
        //周期
        [BsonElement("ProduceSecond")]
        public double ProduceSecond { get; set; }
        //不良品数
        [BsonElement("ErrorCount")]
        public int ErrorCount { get; set; }
        //是否结束
        [BsonElement("IsStopFlag")]
        public bool IsStopFlag { get; set; }
 
    }
    [BsonIgnoreExtraElements]
    public class EmployeeProductionTimeList
    {
        [BsonElement("EmployeeID")]
        public string EmployeeID { get; set; }
        [BsonElement("WorkHour")]
        public double WorkHour { get; set; }
    }
}
