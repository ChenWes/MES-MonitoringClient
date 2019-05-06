using System;
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
using MongoDB.Bson.Serialization;

namespace MES_MonitoringClient.Common
{
    /// <summary>
    /// 机器状态类
    /// </summary>
    public class MachineStatusHandler
    {
        /*MongoDB数据库*/
        /*-------------------------------------------------------------------------------------*/
        /// <summary>
        /// 机器状态Mongodb集合
        /// </summary>
        private IMongoCollection<DataModel.MachineStatusLog> machineStatusLogCollection;

        /// <summary>
        /// 机器状态日志默认Mongodb集合名
        /// </summary>
        private static string defaultMachineStatusMongodbCollectionName = "MachineStatusLog";


        /*机器生命周期参数处理（三个信号）*/
        /*-------------------------------------------------------------------------------------*/
        /// <summary>
        /// 机器生产状态
        /// </summary>
        public MachineProduceStatusHandler mc_MachineProduceStatusHandler = null;


        /*机器状态外部可访问参数声明*/
        /*-------------------------------------------------------------------------------------*/
        /// <summary>
        /// 机器状态ID
        /// </summary>
        public string MachineStatusID { get; set; }

        /// <summary>
        /// 机器状态编码
        /// </summary>
        public string MachineStatusCode { get; set; }

        /// <summary>
        /// 机器状态名称
        /// </summary>
        public string MachineStatusName { get; set; }

        /// <summary>
        /// 机器状态描述
        /// </summary>
        public string MachineStatusDesc { get; set; }

        /// <summary>
        /// 机器状态颜色（用于显示在状态灯中）
        /// </summary>
        public string MachineStatusColor { get; set; }


        /*状态持续时间外部可访问参数声明*/
        /*-------------------------------------------------------------------------------------*/

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndDateTime { get; set; }


        /*操作人外部可访问参数声明*/
        /*-------------------------------------------------------------------------------------*/

        /// <summary>
        /// 操作人姓名
        /// </summary>
        public string OperatePersonName { get; set; }

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
        /// 最后操作的数据库机器状态数据记录ID（用于回写结束标识及结束时间）
        /// </summary>
        public string LastOperationMachineStatusLogID { get; set; }

        /// <summary>
        /// 最后操作的时间
        /// </summary>
        public DateTime LastOperationDateTime { get; set; }



        /*更新界面委托方法*/
        /*-------------------------------------------------------------------------------------*/

        /// <summary>
        /// 更新机器状态使用时间饼图
        /// </summary>
        /// <param name="signalType"></param>
        public delegate void UpdateMachineStatusPieChart(List<DataModel.MachineStatusUseTime> user);
        public UpdateMachineStatusPieChart UpdateMachineStatusPieChartDelegate;

        /// <summary>
        /// 更新机器状态灯
        /// </summary>
        public delegate void UpdateMachineStatusLight();
        public UpdateMachineStatusLight UpdateMachineStatusLightDelegate;




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
            //机器产品生命周期声明（生产时获取的机器信号X1、X2、X3）
            mc_MachineProduceStatusHandler = new MachineProduceStatusHandler();

            //最后操作记录ID为空
            LastOperationMachineStatusLogID = string.Empty;

            //机器状态集合
            machineStatusLogCollection = MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.MachineStatusLog>(defaultMachineStatusMongodbCollectionName);
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
                //当前机器状态持续的总时间
                HoldStatusTotalMilliseconds += 1000;


                /*这里需要更改，什么状态才算是停机时间*/
                //停机时间累加
                if (this.MachineStatusCode == "Stop" || this.MachineStatusCode == "Error")
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
        /// 会将修改的状态保存至DB中
        /// </summary>
        /// <param name="newMachineStatusID">机器状态ID</param>
        /// <param name="newMachineStatusCode">机器状态编号</param>
        /// <param name="newMachineStatusName">机器状态名称</param>
        /// <param name="newMachineStatusDesc">机器状态描述</param>
        /// <param name="newMachineStatusColor">机器状态颜色</param>
        /// <param name="employeeName">员工姓名</param>
        /// <param name="employeeCardID">员工IC卡号</param>
        public void ChangeStatus(string newMachineStatusID, string newMachineStatusCode, string newMachineStatusName, string newMachineStatusDesc, string newMachineStatusColor, string employeeName, string employeeCardID)
        {
            try
            {
                #region 上一条未完成的机器状态日志记录

                //重新开始计时器及线程
                if (DateTimeThreadClass != null && TTimerClass != null)
                {
                    //取消线程
                    DateTimeThreadClass.Abort();
                    Thread.SpinWait(1000);

                    //停止定时器
                    TTimerClass.StopTimmer();
                    TTimerClass = null;

                    //更新最后时间
                    EndDateTime = DateTime.Now.ToLocalTime();

                    //保存至DB中/*******************/嫁动率的主要数据来源

                    //如果有Mongodb才保存至DB中
                    if (Common.CommonFunction.ServiceRunning(Common.MongodbHandler.MongodbServiceName))
                    {
                        //判断最后一个记录ID（来自于没有结束的机器状态日志记录ID）
                        if (!string.IsNullOrEmpty(LastOperationMachineStatusLogID))
                        {
                            //计算相隔时间（秒数）
                            int useTotalSecond = 0;
                            if (LastOperationDateTime != null)
                            {
                                TimeSpan timeSpan = EndDateTime - LastOperationDateTime;
                                useTotalSecond = (int)timeSpan.TotalSeconds;
                            }

                            //找到单个记录
                            var filterID = Builders<DataModel.MachineStatusLog>.Filter.Eq("_id", ObjectId.Parse(LastOperationMachineStatusLogID));

                            //修改（写入结束标识、结束时间、中间持续的时间秒数）
                            var update = Builders<DataModel.MachineStatusLog>.Update
                                .Set("EndDateTime", EndDateTime)
                                .Set("UseTotalSeconds", useTotalSecond)
                                .Set("IsStopFlag", true);

                            //更新数据库
                            machineStatusLogCollection.UpdateOne(filterID, update);
                        }
                    }
                }

                #endregion

                #region 当前记录操作

                //当前时间
                StartDateTime = DateTime.Now.ToLocalTime();

                //更新状态
                MachineStatusID = newMachineStatusID;
                MachineStatusCode = newMachineStatusCode;
                MachineStatusName = newMachineStatusName;
                MachineStatusDesc = newMachineStatusDesc;
                MachineStatusColor = newMachineStatusColor;

                //更新人员
                OperatePersonName = employeeName;
                OperatePersonCardID = employeeCardID;

                //清空时间开始计时
                HoldStatusTotalMilliseconds = 0;


                //开始一个新线程，处理状态的时间
                DateTimeThreadFunction = new ThreadStart(DateTimeTimer);
                DateTimeThreadClass = new Thread(DateTimeThreadFunction);
                DateTimeThreadClass.Start();

                //当前机器状态记录写入DB
                if (Common.CommonFunction.ServiceRunning(Common.MongodbHandler.MongodbServiceName))
                {
                    #region 上次退出界面，但未完成机器状态的完成记录

                    //查询数据库，是否有未完成的数据（上次退出界面，但未完成机器状态的完成记录）
                    //如果有，先完成未完成的数据
                    //如果没有，直接新增
                    if (string.IsNullOrEmpty(LastOperationMachineStatusLogID))
                    {
                        var collection = Common.MongodbHandler.GetInstance().GetCollection(defaultMachineStatusMongodbCollectionName);
                        var filter = Builders<BsonDocument>.Filter.And(new FilterDefinition<BsonDocument>[] {
                            Builders<BsonDocument>.Filter.Eq("IsStopFlag", false),
                            Builders<BsonDocument>.Filter.Eq("UseTotalSeconds", 0),
                            Builders<BsonDocument>.Filter.Eq("EndDateTime", BsonNull.Value),
                        });

                        var result = Common.MongodbHandler.GetInstance().Find(collection, filter).FirstOrDefault();

                        if (result != null)
                        {
                            var dataEntity = BsonSerializer.Deserialize<DataModel.MachineStatusLog>(result);

                            LastOperationMachineStatusLogID = dataEntity.Id.ToString();

                            //最后时间
                            EndDateTime = System.DateTime.Now.ToLocalTime();
                            //计算相隔时间（秒数）
                            int useTotalSecond = 0;
                            if (dataEntity.StartDateTime != null)
                            {
                                TimeSpan timeSpan = EndDateTime - dataEntity.StartDateTime.Value;
                                useTotalSecond = (int)timeSpan.TotalSeconds;
                            }

                            //找到单个记录
                            var filterID = Builders<DataModel.MachineStatusLog>.Filter.Eq("_id", ObjectId.Parse(LastOperationMachineStatusLogID));

                            //修改（写入结束标识、结束时间、中间持续的时间秒数）
                            var update = Builders<DataModel.MachineStatusLog>.Update
                                .Set("EndDateTime", EndDateTime)
                                .Set("UseTotalSeconds", useTotalSecond)
                                .Set("IsStopFlag", true);

                            //更新数据库
                            machineStatusLogCollection.UpdateOne(filterID, update);                            
                        }
                    }

                    #endregion


                    #region 当前机器状态日志记录保存至DB
                    
                    //新增的机器状态日志记录实体
                    DataModel.MachineStatusLog newMachineStatusLog = new DataModel.MachineStatusLog();

                    newMachineStatusLog.StatusID = MachineStatusID;
                    newMachineStatusLog.StatusCode = MachineStatusCode;
                    newMachineStatusLog.StatusName = MachineStatusName;
                    newMachineStatusLog.StatusDesc = MachineStatusDesc;

                    //开始与结束时间一致
                    newMachineStatusLog.StartDateTime = StartDateTime;
                    //此时EndDateTime为Null，方便下次查询未完成的机器状态日志记录
                    //newMachineStatus.EndDateTime = null;
                    //使用的秒数
                    newMachineStatusLog.UseTotalSeconds = 0;

                    //未结束标识
                    //未上传标识与未更新至服务器标识
                    newMachineStatusLog.IsStopFlag = false;
                    newMachineStatusLog.IsUploadToServer = false;
                    newMachineStatusLog.IsUpdateToServer = false;

                    //MAC地址
                    newMachineStatusLog.LocalMacAddress = Common.CommonFunction.getMacAddress();

                    //机器ID不为空，则保存到数据库中
                    if (mc_MachineProduceStatusHandler.MC_machine != null)
                    {
                        newMachineStatusLog.MachineID = mc_MachineProduceStatusHandler.MC_machine._id;
                    }

                    //插入DB
                    machineStatusLogCollection.InsertOne(newMachineStatusLog);

                    //暂存ID（方便下一次更改机器状态时，可以结束掉现在插入的机器状态日志记录）
                    LastOperationMachineStatusLogID = newMachineStatusLog.Id.ToString();
                    //最后操作时间
                    LastOperationDateTime = StartDateTime;

                    #endregion
                }

                #endregion

                #region 更新界面状态灯&更新时间占比图

                //回调更新界面，状态灯
                UpdateMachineStatusLightDelegate();

                //回调更新界面，各状态占比时间
                UpdateMachineStatusPieChartDelegate(GetMachineUseTimeList());

                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /*修改机器状态*/
        /*-------------------------------------------------------------------------------------*/

        /// <summary>
        /// 获取机器各状态持续总时间
        /// </summary>
        /// <returns></returns>
        public List<DataModel.MachineStatusUseTime> GetMachineUseTimeList()
        {
            List<DataModel.MachineStatusUseTime> returnList = new List<DataModel.MachineStatusUseTime>();

            //数据库中的机器状态日志的分组计算
            var result = machineStatusLogCollection.Aggregate().Group(key => key.StatusID,
                value => new { StatusKey = value.Key, UseTotalSeconds = value.Sum(key => key.UseTotalSeconds) }).ToList();

            //数据库的所有机器状态记录
            List<DataModel.MachineStatus> machineStatuseList = Common.MachineStatusHelper.GetAllMachineStatus();


            foreach (var item in result)
            {
                //机器状态日志循环，并找到对应的机器状态记录
                //Linq语法
                var findMachineStatus = (from msl
                                         in machineStatuseList
                                         where msl._id == item.StatusKey
                                         select msl).FirstOrDefault();

                if (findMachineStatus != null)
                {
                    //加入到记录列表中
                    returnList.Add(new DataModel.MachineStatusUseTime()
                    {
                        StatusText = findMachineStatus.MachineStatusName + "(" + findMachineStatus.MachineStatusCode + ")",
                        UseTotalSeconds = item.UseTotalSeconds,
                        StatusColor = findMachineStatus.StatusColor
                    });
                }
            }

            return returnList;
        }


        /// <summary>
        /// 自动获取数据的饼图
        /// </summary>
        public void ShowStatusPieChart()
        {
            if (Common.CommonFunction.ServiceRunning(Common.MongodbHandler.MongodbServiceName))
            {
                //回调更新界面，各状态占比时间
                UpdateMachineStatusPieChartDelegate(GetMachineUseTimeList());                
            }
        }
    }
}
