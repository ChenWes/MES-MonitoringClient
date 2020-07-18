using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_MonitoringService.Model
{
    /// <summary>
    /// 机器状态日志JSON格式(Nodejs端无法解析ObjectID和ISODate)
    /// </summary>
    public class MachineProduction_JSON
    {
        public string Id { get; set; }

        //日期
      
        public string Date { get; set; }
        //机器
        
        public string MachineID { get; set; }
        //工单
       
        public string JobOrderID { get; set; }
        //开始时间
       
        public string StartDateTime { get; set; }
        //结束时间
       
        public string EndDateTime { get; set; }
        //工单数
      
        public int ProduceCount { get; set; }
        //班次
       
        public string WorkShiftID { get; set; }
        //员工工时
        public List<EmployeeProductionTimeList> EmployeeProductionTimeList { get; set; }
        //工单生产工时
        public double JobOrderProductionTime { get; set; }

        public double ProduceSecond { get; set; }
        //不良品数
        
        public int ErrorCount { get; set; }
        //是否结束
        
        public bool IsStopFlag { get; set; }
    }
}
