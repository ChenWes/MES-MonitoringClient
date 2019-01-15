﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading;

using MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;

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

        //机器状态Mongodb集合
        private IMongoCollection<DataModel.MachineStatus> machineStatusLogCollection;


        /// <summary>
        /// 机器状态默认Mongodb集合名
        /// </summary>
        private static string defaultMachineStatusMongodbCollectionName = "MachineStatusLog";

        /// <summary>
        /// 灯
        /// </summary>
        public enumStatusLight mc_StatusLight = enumStatusLight.Green;

        /// <summary>
        /// 机器生产状态
        /// </summary>
        public MachineProduceStatusHandler mc_MachineProduceStatusHandler = null;

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
        /// 操作人卡号
        /// </summary>
        public string OperatePersonCardID { get; set; }

        /// <summary>
        /// 当前状态等待时间（毫秒级）
        /// </summary>
        public long HoldStatusTotalMilliseconds { get; set; }

        /// <summary>
        /// 停机状态等待时间（毫秒级）
        /// </summary>
        public long StopStatusTotalMilliseconds{get;set;}            

        /// <summary>
        /// 最后操作的数据库ID
        /// </summary>
        public string LastOperationMachineStatusID { get; set; }

        /// <summary>
        /// 最后操作的时间
        /// </summary>
        public DateTime LastOperationDateTime { get; set; }


        /// <summary>
        /// 起工时间
        /// </summary>
        public DateTime? StartWorkTime { get; set; }

        /// <summary>
        /// 预计完成时间
        /// </summary>
        public DateTime? PlanCompleteDateTime { get; set; }

        /// <summary>
        /// 更新机器使用时间返回至界面
        /// </summary>
        /// <param name="signalType"></param>
        public delegate void UpdateMachineUseTime(List<DataModel.MachineStatusUseTime> user);
        public UpdateMachineUseTime UpdateMachineUseTimeDelegate;


        /// <summary>
        /// 更新机器使用时间返回至界面
        /// </summary>
        public delegate void UpdateMachineCompleteDateTime();
        public UpdateMachineCompleteDateTime UpdateMachineCompleteDateTimeDelegate;

        /// <summary>
        /// 更新机器状态持续总时间
        /// </summary>
        public delegate void UpdateMachineStatusTotalDateTime();
        public UpdateMachineStatusTotalDateTime UpdateMachineStatusTotalDateTimeDelegate;


        /*处理线程等*/
        /*-------------------------------------------------------------------------------------*/

        /// <summary>
        /// 时间线程
        /// </summary>        
        Thread DateTimeThreadClass = null;

        /// <summary>
        /// 时间线程方法
        /// </summary>
        ThreadStart DateTimeThreadFunction = null;

        /// <summary>
        /// 定时器
        /// </summary>
        Common.TimmerHandler TTimerClass = null;


        /*构造函数*/
        /*-------------------------------------------------------------------------------------*/

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="uiCallbackFunction"></param>
        public MachineStatusHandler()
        {
            //生产状态
            mc_MachineProduceStatusHandler = new MachineProduceStatusHandler();

            //最后操作记录ID为空
            LastOperationMachineStatusID = string.Empty;

            //机器状态集合
            machineStatusLogCollection = MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.MachineStatus>(defaultMachineStatusMongodbCollectionName);
        }


        /*状态计时器*/
        /*-------------------------------------------------------------------------------------*/

        /// <summary>
        /// 计算时间定时器
        /// </summary>
        private void DateTimeTimer()
        {
            try
            {
                TTimerClass = new Common.TimmerHandler(1000, true, (o, a) =>
                {
                    AddDateTime();
                }, true);
            }
            catch (Exception ex)
            {
                TTimerClass = null;
            }
        }

        /// <summary>
        /// 声明计算时间委托
        /// </summary>
        private delegate void SetDateTimeDelegate();
        private void AddDateTime()
        {
            try
            {
                //增加时间
                HoldStatusTotalMilliseconds += 1000;
                if (this.StatusDescription == "停机" || this.StatusDescription == "故障")
                {
                    StopStatusTotalMilliseconds += 1000;
                }

                //更新界面
                UpdateMachineStatusTotalDateTimeDelegate();
            }
            catch (Exception ex)
            {                
                TTimerClass = null;
            }
        }        


        /*修改机器状态*/
        /*-------------------------------------------------------------------------------------*/

        /// <summary>
        /// 主动修改当前机器状态
        /// ##会将修改的状态保存至DB中
        /// </summary>
        /// <param name="newStatusCode"></param>
        /// <param name="newStatusDescription"></param>
        public void ChangeStatus(string newStatusCode, string newStatusDescription, string newOperatePerson, string newOperateCardID)
        {
            try
            {
                #region 上一条记录操作

                //重新开始计时器及线程
                if (DateTimeThreadClass != null && TTimerClass != null)
                {
                    //线程
                    DateTimeThreadClass.Abort();
                    Thread.SpinWait(1000);

                    //定时器
                    TTimerClass.StopTimmer();
                    TTimerClass = null;

                    //更新最后时间
                    EndDateTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

                    //保存至DB中/*******************/嫁动率的主要数据来源

                    //如果有Mongodb才保存至DB中
                    if (Common.CommonFunction.ServiceRunning(Common.MongodbHandler.MongodbServiceName))
                    {
                        //判断最后一个
                        if (!string.IsNullOrEmpty(LastOperationMachineStatusID))
                        {
                            int useTotalSecond = 0;
                            if (LastOperationDateTime != null)
                            {
                                TimeSpan timeSpan = EndDateTime - LastOperationDateTime;
                                useTotalSecond = (int)timeSpan.TotalSeconds;
                            }

                            //找到单个记录
                            //var dataID = new ObjectId(LastOperationMachineStatusID);
                            //var getMachineStatusEntity = machineStatusLogCollection.AsQueryable<DataModel.MachineStatus>().SingleOrDefault(m => m.Id == dataID);

                            var filterID = Builders<DataModel.MachineStatus>.Filter.Eq("_id", ObjectId.Parse(LastOperationMachineStatusID));

                            var update = Builders<DataModel.MachineStatus>.Update
                                .Set("EndDateTime", EndDateTime)
                                .Set("UseTotalSeconds", useTotalSecond)
                                .Set("IsStopFlag", true);                              

                            var result = machineStatusLogCollection.UpdateOne(filterID, update);
                        }
                    }                                        
                }

                #endregion

                #region 当前记录操作

                //当前时间
                StartDateTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

                //更新状态
                StatusCode = newStatusCode;
                StatusDescription = newStatusDescription;

                //更新人员
                OperatePerson = newOperatePerson;
                OperatePersonCardID = newOperateCardID;

                //清空时间开始计时
                HoldStatusTotalMilliseconds = 0;

                //修改灯
                SettingLight(newStatusDescription);

                //开始一个新线程，处理状态的时间
                DateTimeThreadFunction = new ThreadStart(DateTimeTimer);
                DateTimeThreadClass = new Thread(DateTimeThreadFunction);
                DateTimeThreadClass.Start();

                if (Common.CommonFunction.ServiceRunning(Common.MongodbHandler.MongodbServiceName))
                {
                    DataModel.MachineStatus newMachineStatus = new DataModel.MachineStatus();

                    newMachineStatus.Status = StatusDescription;
                    //开始与结束时间一致
                    newMachineStatus.StartDateTime = StartDateTime;
                    newMachineStatus.EndDateTime = StartDateTime;
                    //使用的秒数
                    newMachineStatus.UseTotalSeconds = 0;
                    //未结束与上传标识
                    newMachineStatus.IsStopFlag = false;
                    newMachineStatus.IsUploadToServer = false;
                    newMachineStatus.IsUpdateToServer = false;
                    //MAC地址
                    newMachineStatus.LocalMacAddress = Common.CommonFunction.getMacAddress();

                    //插入
                    machineStatusLogCollection.InsertOne(newMachineStatus);
                    //暂存ID
                    LastOperationMachineStatusID = newMachineStatus.Id.ToString();
                    //最后操作时间
                    LastOperationDateTime = StartDateTime;
                }

                //回调更新界面，各状态占比时间
                UpdateMachineUseTimeDelegate(GetMachineUseTimeList());

                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 获取机器各状态持续总时间
        /// </summary>
        /// <returns></returns>
        public List<DataModel.MachineStatusUseTime> GetMachineUseTimeList()
        {
            List<DataModel.MachineStatusUseTime> returnList = new List<DataModel.MachineStatusUseTime>();

            var result = machineStatusLogCollection.Aggregate().Group(key => key.Status,
                value => new { Status = value.Key, UseTotalSeconds = value.Sum(key => key.UseTotalSeconds) }).ToList();

            foreach (var item in result)
            {
                returnList.Add(new DataModel.MachineStatusUseTime()
                {
                    Status = item.Status,
                    UseTotalSeconds = item.UseTotalSeconds
                });
            }

            return returnList;
        }


        /// <summary>
        /// 自动获取数据的饼图
        /// </summary>
        public void ShowStatusPieChart()
        {
            //回调更新界面，各状态占比时间
            UpdateMachineUseTimeDelegate(GetMachineUseTimeList());
        }

        /*红绿灯操作*/
        /*-------------------------------------------------------------------------------------*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newStatusDescription"></param>
        public void SettingLight(string newStatusDescription)
        {
            if (newStatusDescription == "运行") mc_StatusLight = enumStatusLight.Green;
            else if (newStatusDescription == "故障") mc_StatusLight = enumStatusLight.Red;
            else if (newStatusDescription == "停机") mc_StatusLight = enumStatusLight.Yellow;
        }

        /// <summary>
        /// 机器预计完成时间设置
        /// </summary>
        public void SettingMachineCompleteDateTime()
        {
            UpdateMachineCompleteDateTimeDelegate();
        }


        /*应用关闭时处理事件*/
        /*-------------------------------------------------------------------------------------*/

        public void AppWillClose_SaveData()
        {
            try
            {
                //重新开始计时器及线程
                if (DateTimeThreadClass != null && TTimerClass != null)
                {
                    //更新最后时间
                    EndDateTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

                    //保存至DB中/*******************/嫁动率的主要数据来源
                    if (Common.CommonFunction.ServiceRunning(Common.MongodbHandler.MongodbServiceName))
                    {
                        //判断最后一个
                        if (!string.IsNullOrEmpty(LastOperationMachineStatusID))
                        {
                            int useTotalSecond = 0;
                            if (LastOperationDateTime != null)
                            {
                                TimeSpan timeSpan = EndDateTime - LastOperationDateTime;
                                useTotalSecond = (int)timeSpan.TotalSeconds;
                            }

                            //找到单个记录
                            //var dataID = new ObjectId(LastOperationMachineStatusID);
                            //var getMachineStatusEntity = machineStatusLogCollection.AsQueryable<DataModel.MachineStatus>().SingleOrDefault(m => m.Id == dataID);

                            var filterID = Builders<DataModel.MachineStatus>.Filter.Eq("_id", ObjectId.Parse(LastOperationMachineStatusID));

                            var update = Builders<DataModel.MachineStatus>.Update
                                .Set("EndDateTime", EndDateTime)
                                .Set("UseTotalSeconds", useTotalSecond)
                                .Set("IsStopFlag", true);

                            var result = machineStatusLogCollection.UpdateOne(filterID, update);

                            if (result.ModifiedCount == 1)
                            {

                            }
                        }
                    }

                    //定时器
                    TTimerClass.StopTimmer();
                    TTimerClass = null;

                    //线程
                    DateTimeThreadClass.Abort();
                    Thread.Sleep(1000);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }   
    }
}
