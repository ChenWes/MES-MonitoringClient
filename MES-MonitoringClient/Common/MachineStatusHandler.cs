using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading;

using System.Diagnostics;

namespace MES_MonitoringClient.Common
{
    /// <summary>
    /// 机器状态类
    /// </summary>
    public class MachineStatusHandler
    {
        /// <summary>
        /// 状态灯
        /// </summary>
        public enum enumStatusLight
        {
            Green,
            Yellow,
            Red
        }

        /// <summary>
        /// 机器生产状态
        /// </summary>
        public MachineProduceStatusHandler mc_MachineProduceStatusHandler = null;

        public enumStatusLight mc_StatusLight = enumStatusLight.Green;

        /// <summary>
        /// 状态编码
        /// </summary>
        public string StatusCode { get; set; }

        /// <summary>
        /// 状态描述==》这个是指示灯的判断指标
        /// </summary>
        public string StatusDescription { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndDateTime { get; set; }

        /// <summary>
        /// 操作人姓名
        /// </summary>
        public string OperatePerson { get; set; }

        /// <summary>
        /// 当前状态时间
        /// </summary>
        public long HoldStatusTotalMilliseconds { get; set; }

        /// <summary>
        /// 时间线程
        /// </summary>
        ThreadHandler DateTimeThreadHandler = null;

        /// <summary>
        /// 时间线程方法
        /// </summary>
        ThreadStart DateTimeThreadStart = null;

        /// <summary>
        /// 定时器
        /// </summary>
        Common.TimmerHandler TTimerClass = null;

        /// <summary>
        /// 操作人卡号
        /// </summary>
        public string OperatePersonCardID { get; set; }




        public MachineStatusHandler()
        {
            //生产状态
            mc_MachineProduceStatusHandler = new MachineProduceStatusHandler();            
        }        



        /// <summary>
        /// 计算时间定时器
        /// </summary>
        private void DateTimeTimer()
        {
            TTimerClass = new Common.TimmerHandler(1000, true, (o, a) =>
            {
                AddDateTime();
            }, true);
        }

        /// <summary>
        /// 声明计算时间委托
        /// </summary>
        private delegate void SetDateTimeDelegate();
        private void AddDateTime()
        {            
            HoldStatusTotalMilliseconds += 1000;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newStatusCode"></param>
        /// <param name="newStatusDescription"></param>
        public void ChangeStatus(string newStatusCode, string newStatusDescription,string newOperatePerson,string newOperateCardID)
        {
            //当前时间
            StartDateTime = System.DateTime.Now;

            //更新状态
            StatusCode = newStatusCode;
            StatusDescription = newStatusDescription;

            //更新人员
            OperatePerson = newOperatePerson;
            OperatePersonCardID = newOperateCardID;

            //清空时间开始计时
            HoldStatusTotalMilliseconds = 0;

            //重新开始计时器及线程
            if (DateTimeThreadHandler != null && DateTimeThreadStart != null)
            {
                //线程
                DateTimeThreadHandler._TThread.Abort();
                DateTimeThreadHandler._TThread.Join();
                //定时器
                TTimerClass.StopTimmer();
                TTimerClass = null;

                //更新最后时间
                EndDateTime = System.DateTime.Now;

                //保存至DB中/*******************/嫁动率的主要数据来源

            }

            DateTimeThreadStart = new ThreadStart(DateTimeTimer);
            DateTimeThreadHandler = new ThreadHandler(DateTimeThreadStart, false, true);
        }
    }
}
