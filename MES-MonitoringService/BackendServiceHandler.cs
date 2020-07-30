﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Timers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;

using MongoDB.Bson.IO;
using System.Threading;

namespace MES_MonitoringService
{
    public class BackendServiceHandler
    {
        //服务运行间隔时间
        private static string defaultUploadDataIntervalMilliseconds = Common.ConfigFileHandler.GetAppConfig("UploadDataIntervalMilliseconds");
        //同步工单数据间隔时间
        private static string defaultUploaJobOrderdDataIntervalMilliseconds = Common.ConfigFileHandler.GetAppConfig("UploadJobOrderDataIntervalMilliseconds");
        //同步用户头像间隔时间
        private static string defaultSyneEmployeeImageIntervalMilliseconds = Common.ConfigFileHandler.GetAppConfig("SyncEmployeeImageIntervalMilliseconds");
        //同步工单第一次生产间隔时间
        private static string defaultSyneJobOrderFirstProduceLogIntervalMilliseconds = Common.ConfigFileHandler.GetAppConfig("SyncJobOrderFirstProduceLogIntervalMilliseconds");
        //同步生产数间隔时间
        private static string defaultSyneMachineProductionIntervalMilliseconds = Common.ConfigFileHandler.GetAppConfig("SyncMachineProductionIntervalMilliseconds");

        //机器状态日志Mongodb数据集名称
        private static string defaultMachineStatusLogMongodbCollectionName = Common.ConfigFileHandler.GetAppConfig("MachineStatusLogCollectionName");
        //工单Mongodb数据集名称
        private static string defaultJobOrderMongodbCollectionName = Common.ConfigFileHandler.GetAppConfig("JobOrderCollectionName");
        //机器注册表
        private static string defaultMachineRegisterMongodbCollectionName = Common.ConfigFileHandler.GetAppConfig("MachineRegisterCollectionName");
        //员工
        private static string defaultEmployeeMongodbCollectionName = Common.ConfigFileHandler.GetAppConfig("EmployeeCollectionName");
        //工单第一次生产记录
        private static string defaultJobOrderFirstProduceLogMongodbCollectionName = Common.ConfigFileHandler.GetAppConfig("JobOrderFirstProduceLogCollectionName");
        //生产数
        private static string defaultMachineProductionMongodbCollectionName = Common.ConfigFileHandler.GetAppConfig("MachineProductionCollectionName");

        //机器状态对应的交换机、路由、队列名称
        private static string defaultMachineStatus_ExchangeName = Common.ConfigFileHandler.GetAppConfig("MachineStatusLog_ExchangeName");
        private static string defaultMachineStatus_RoutingKey = Common.ConfigFileHandler.GetAppConfig("MachineStatusLog_RoutingKey");
        private static string defaultMachineStatus_QueueName = Common.ConfigFileHandler.GetAppConfig("MachineStatusLog_QueueName");


        //工单对应的交换机、路由、队列名称
        private static string defaultJobOrder_ExchangeName = Common.ConfigFileHandler.GetAppConfig("JobOrder_ExchangeName");
        private static string defaultJobOrder_RoutingKey = Common.ConfigFileHandler.GetAppConfig("JobOrder_RoutingKey");
        private static string defaultJobOrder_QueueName = Common.ConfigFileHandler.GetAppConfig("JobOrder_QueueName");

        //工单第一次生产记录对应的交换机、路由、队列名称
        private static string defaultJobOrderFirstProduceLogLog_ExchangeName = Common.ConfigFileHandler.GetAppConfig("JobOrderFirstProduceLog_ExchangeName");
        private static string defaultJobOrderFirstProduceLog_RoutingKey = Common.ConfigFileHandler.GetAppConfig("JobOrderFirstProduceLog_RoutingKey");
        private static string defaultJobOrderFirstProduceLog_QueueName = Common.ConfigFileHandler.GetAppConfig("JobOrderFirstProduceLog_QueueName");


        //生产数对应的交换机、路由、队列名称
        private static string defaultMachineProduction_ExchangeName = Common.ConfigFileHandler.GetAppConfig("MachineProduction_ExchangeName");
        private static string defaultMachineProduction_RoutingKey = Common.ConfigFileHandler.GetAppConfig("MachineProduction_RoutingKey");
        private static string defaultMachineProduction_QueueName = Common.ConfigFileHandler.GetAppConfig("MachineProduction_QueueName");

        //同步数据对应的队列名称
        private static string defaultSyncData_QueueName_Prefix = Common.ConfigFileHandler.GetAppConfig("SyncData_QueueName_Prefix");

        ////数据同步标识（作用开关，标识出系统是否正在数据同步）
        private bool MC_IsSyncDataFlag = false;
        //机器注册码
        private string MC_MachineRegisterID = string.Empty;

        //定时器

        //private readonly Timer _timer;
        //private readonly Timer _SyncEmployeeImageTimer;
        private System.Timers.Timer _timer;
        private System.Timers.Timer _SyncJobOrderTimer;
        private System.Timers.Timer _SyncEmployeeImageTimer;
        private System.Timers.Timer _SyncJobOrderFirstProduceLogTimer;
        private System.Timers.Timer _SyncMachineProductionTimer;


        /// <summary>
        /// 上传数据至服务器
        /// </summary>
        public BackendServiceHandler()
        {
        }

        public void StartTimerFunction()
        {
            try
            {
                if (!Common.CommonFunction.ServiceRunning(Common.MongodbHandler.MongodbServiceName) && Common.ConfigFileHandler.GetAppConfig("CheckMongoDBService") == "1")
                {
                    //不存在MongoDB服务
                    throw new Exception("MES数据上传服务程序检测到该电脑暂时不存在Mongodb服务，服务未能正常运行，请管理员及时处理。");
                }
                else
                {
                    //MongoDB服务正常


                    #region 同步机器状态信息            

                    //定时任务间隔时间
                    long timeInterval = 0;
                    long.TryParse(defaultUploadDataIntervalMilliseconds, out timeInterval);

                    //定时任务
                    _timer = new System.Timers.Timer(timeInterval) { AutoReset = true };
                    _timer.Elapsed += TimerElapsed;

                    #endregion


                    #region 同步工单信息            

                    //定时任务间隔时间
                    timeInterval = 0;
                    long.TryParse(defaultUploaJobOrderdDataIntervalMilliseconds, out timeInterval);

                    //定时任务
                    _SyncJobOrderTimer = new System.Timers.Timer(timeInterval) { AutoReset = true };
                    _SyncJobOrderTimer.Elapsed += SyncJobOrderElapsed;

                    #endregion


                    #region 同步用户头像
                    //定时任务间隔时间
                    timeInterval = 0;
                    long.TryParse(defaultSyneEmployeeImageIntervalMilliseconds, out timeInterval);

                    //定时任务
                    _SyncEmployeeImageTimer = new System.Timers.Timer(timeInterval) { AutoReset = true };
                    _SyncEmployeeImageTimer.Elapsed += SyncEmployeeImageTimerElapsed;

                    #endregion

                    #region 同步第一次生产记录           

                    //定时任务间隔时间
                    timeInterval = 0;
                    long.TryParse(defaultSyneJobOrderFirstProduceLogIntervalMilliseconds, out timeInterval);

                    //定时任务
                    _SyncJobOrderFirstProduceLogTimer = new System.Timers.Timer(timeInterval) { AutoReset = true };
                    _SyncJobOrderFirstProduceLogTimer.Elapsed += SyncJobOrderFirstProduceLogElapsed;

                    #endregion

                    #region 同步机器生产记录           

                    //定时任务间隔时间
                    timeInterval = 0;
                    long.TryParse(defaultSyneMachineProductionIntervalMilliseconds, out timeInterval);

                    //定时任务
                    _SyncMachineProductionTimer = new System.Timers.Timer(timeInterval) { AutoReset = true };
                    _SyncMachineProductionTimer.Elapsed += SyncMachineProductionElapsed;

                    #endregion
                }


                if (_timer != null)
                {
                    _timer.Start();
                }

                if (_SyncEmployeeImageTimer != null)
                {
                    _SyncEmployeeImageTimer.Start();
                }

                if (_SyncJobOrderTimer != null)
                {
                    _SyncJobOrderTimer.Start();
                }
                if (_SyncJobOrderFirstProduceLogTimer != null)
                {
                    _SyncJobOrderFirstProduceLogTimer.Start();
                }
                if (_SyncMachineProductionTimer != null)
                {
                    _SyncMachineProductionTimer.Start();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 定时方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();
            //处理机器状态
            ProcessMachineStatusLog();            

            if (MC_IsSyncDataFlag == false)
            {
                //检测注册
                CheckMachineRegister();
            }
            _timer.Start();
        }

        private void SyncJobOrderElapsed(object sender, ElapsedEventArgs e)
        {
            _SyncJobOrderTimer.Stop();
            //处理工单
            ProcessJobOrder();
            _SyncJobOrderTimer.Start();
        }

        /// <summary>
        /// 同步用户头像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SyncEmployeeImageTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _SyncEmployeeImageTimer.Stop();
            //同步头像
            ProcessEmployeeImage();
            _SyncEmployeeImageTimer.Start();
        }

        /// <summary>
        /// 同步工单第一次生产
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SyncJobOrderFirstProduceLogElapsed(object sender, ElapsedEventArgs e)
        {
            _SyncJobOrderFirstProduceLogTimer.Stop();
            //处理工单第一次生产
            ProcessJobOrderFirstProduceLog();
            _SyncJobOrderFirstProduceLogTimer.Start();
        }
        /// <summary>
        /// 同步生产
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SyncMachineProductionElapsed(object sender, ElapsedEventArgs e)
        {
            _SyncMachineProductionTimer.Stop();
            ProcessMachineProduction();
            _SyncMachineProductionTimer.Start();
        }
        /// <summary>
        /// 检查DB后，调用一次的方法
        /// </summary>
        private void CheckDBCallOneTimeFunction()
        {
            //处理数据同步服务
            ProcessSyncDataAction();
        }

        /// <summary>
        /// 处理机器状态
        /// </summary>
        public void ProcessMachineStatusLog()
        {
            try
            {
                //Common.LogHandler.WriteLog("开始运行定时方法[ProcessMachineStatusLog]");


                //找到机器状态集合
                var collection = Common.MongodbHandler.GetInstance().GetCollection(defaultMachineStatusLogMongodbCollectionName);

                //找到没有上传的或者没有更新但已经停止的状态记录
                var newfilter = Builders<BsonDocument>.Filter.Or(
                    new FilterDefinition<BsonDocument>[] {
                Builders<BsonDocument>.Filter.Eq("IsUploadToServer", false),

                Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("IsUpdateToServer", false),
                    Builders<BsonDocument>.Filter.Eq("IsStopFlag", true)
                    )
                });
                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).ToList();
                //循环处理
                foreach (var data in getdocument)
                {
                    //转换成类
                    var machineStatusLogEntity = BsonSerializer.Deserialize<Model.MachineStatusLog>(data);

                    //JSON类，如果不将JSON字符串中的ObjectID和ISODate去除，则Nodejs解析JSON时会出现问题
                    Model.MachineStatusLog_JSON newMachineStatus_JSON = new Model.MachineStatusLog_JSON();
                    newMachineStatus_JSON.Id = machineStatusLogEntity.Id.ToString();//ObjectID转换成string

                    newMachineStatus_JSON.StatusID = machineStatusLogEntity.StatusID;
                    newMachineStatus_JSON.StatusCode = machineStatusLogEntity.StatusCode;
                    newMachineStatus_JSON.StatusName = machineStatusLogEntity.StatusName;

                    newMachineStatus_JSON.UseTotalSeconds = machineStatusLogEntity.UseTotalSeconds;//使用秒数

                    newMachineStatus_JSON.StartDateTime = machineStatusLogEntity.StartDateTime.Value.ToString("yyyy-MM-dd HH:mm:ss.fffffffK");//Date转换成string

                    if (machineStatusLogEntity.EndDateTime.HasValue)
                    {
                        newMachineStatus_JSON.EndDateTime = machineStatusLogEntity.EndDateTime.Value.ToString("yyyy-MM-dd HH:mm:ss.fffffffK");//Date转换成string
                    }
                    else
                    {
                        newMachineStatus_JSON.EndDateTime = null;
                    }

                    newMachineStatus_JSON.IsStopFlag = machineStatusLogEntity.IsStopFlag;
                    newMachineStatus_JSON.LocalMacAddress = machineStatusLogEntity.LocalMacAddress;
                    newMachineStatus_JSON.MachineID = machineStatusLogEntity.MachineID;
                    newMachineStatus_JSON.CardID = machineStatusLogEntity.CardID;

                    //Common.LogHandler.WriteLog("准备发送至队列=>" + JsonConvert.SerializeObject(newMachineStatus_JSON));

                    //读取Mongodb机器状态日志并上传至队列中
                    bool sendToServerFlag = Common.RabbitMQClientHandler.GetInstance().DirectExchangePublishMessageToServerAndWaitConfirm(defaultMachineStatus_ExchangeName, defaultMachineStatus_RoutingKey, defaultMachineStatus_QueueName, Newtonsoft.Json.JsonConvert.SerializeObject(newMachineStatus_JSON));
                    if (sendToServerFlag)
                    {
                        /*当上传至服务器以后，更改数据*/
                        /*---------------------------------------------------------*/
                        if (!machineStatusLogEntity.IsUploadToServer)
                        {
                            //使用ID作为条件
                            var filterID = Builders<BsonDocument>.Filter.Eq("_id", new BsonObjectId(machineStatusLogEntity.Id));
                            //更改值为已上传
                            var update = Builders<BsonDocument>.Update.Set("IsUploadToServer", true);
                            //查找并修改文档
                            Common.MongodbHandler.GetInstance().FindOneAndUpdate(collection, filterID, update);
                            Common.LogHandler.WriteLog("[" + machineStatusLogEntity.Id.ToString() + "][" + machineStatusLogEntity.StatusName + "]已上传至服务器中，请查看");
                        }
                        else if (!machineStatusLogEntity.IsUpdateToServer && machineStatusLogEntity.IsStopFlag)
                        {
                            //使用ID作为条件
                            var filterID = Builders<BsonDocument>.Filter.Eq("_id", new BsonObjectId(machineStatusLogEntity.Id));
                            //更改值为已上传
                            var update = Builders<BsonDocument>.Update.Set("IsUpdateToServer", true);
                            //查找并修改文档
                            Common.MongodbHandler.GetInstance().FindOneAndUpdate(collection, filterID, update);
                            Common.LogHandler.WriteLog("[" + machineStatusLogEntity.Id.ToString() + "][" + machineStatusLogEntity.StatusName + "]已更新至服务器中，请查看");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.LogHandler.WriteLog("MES数据上传服务程序发现错误，请管理员及时处理。" + ex.Message);
            }
        }

        public void ProcessJobOrder()
        {
            try
            {
                //找到工单集合
                var collection = Common.MongodbHandler.GetInstance().GetCollection(defaultJobOrderMongodbCollectionName);

                //找到更新标识工单记录
                var newfilter = Builders<BsonDocument>.Filter.Eq("IsSyncToServer", false);
                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).ToList();

                //循环处理
                foreach (var data in getdocument)
                {
                    //转换成类
                    var jobOrderEntity = BsonSerializer.Deserialize<Model.JobOrder>(data);
                    string id = jobOrderEntity._id;

                    Model.JobOrder_JSON jobOrder_JSON = new Model.JobOrder_JSON(jobOrderEntity);
                    
                    //jobOrderEntity.ToJson(jsonWriterSettings)
                    //读取Mongodb机器状态日志并上传至队列中
                    bool sendToServerFlag = Common.RabbitMQClientHandler.GetInstance().DirectExchangePublishMessageToServerAndWaitConfirm(
                            defaultJobOrder_ExchangeName,
                            defaultJobOrder_RoutingKey,
                            defaultJobOrder_QueueName,

                            Newtonsoft.Json.JsonConvert.SerializeObject(jobOrder_JSON)
                        );

                    if (sendToServerFlag)
                    {
                        /*当上传至服务器以后，更改数据*/
                        /*---------------------------------------------------------*/

                        //使用整个对象作为条件
                        var filterID = Builders<BsonDocument>.Filter.And(data);
                        //更改值为已上传
                        var update = Builders<BsonDocument>.Update.Set("IsSyncToServer", true);
                        //查找并修改文档
                        Common.MongodbHandler.GetInstance().FindOneAndUpdate(collection, filterID, update);

                    }
                }
            }
            catch (Exception ex)
            {
                Common.LogHandler.WriteLog("MES数据上传服务程序发现错误，请管理员及时处理。" + ex.Message);
            }
        }

        public void ProcessEmployeeImage()
        {
            try
            {
                //Common.LogHandler.WriteLog("开始同步下载员工照片信息");

                var collection = Common.MongodbHandler.GetInstance().GetCollection(defaultEmployeeMongodbCollectionName);

                //找到没有记录
                var newfilter = Builders<BsonDocument>.Filter.And(
                    new FilterDefinition<BsonDocument>[] {
                    Builders<BsonDocument>.Filter.Eq("IsSyncImage", false),
                    Builders<BsonDocument>.Filter.Eq("LocalFileName", ""),
                    Builders<BsonDocument>.Filter.Ne("Icon",BsonNull.Value),
                    Builders<BsonDocument>.Filter.Exists("Icon"),
                });
                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).ToList();

                //循环处理
                foreach (var data in getdocument)
                {
                    //转换成类
                    var employeeEntity = BsonSerializer.Deserialize<Model.Employee>(data);

                    //获取文件名
                    string jsonString = Common.HttpHelper.HttpPostWithToken(employeeEntity.Icon, null);
                    string filename = Common.JsonHelper.GetJsonValue(jsonString, "name");                    

                    //当前路径父文件夹
                    System.IO.DirectoryInfo topDir = System.IO.Directory.GetParent(System.Threading.Thread.GetDomain().BaseDirectory);

                    //当前路径父文件夹,并放入至Client文件夹
                    string newPath = topDir.Parent.FullName + "\\" + Common.ConfigFileHandler.GetAppConfig("ClientFolder");

                    //下载头像文件
                    bool SavetoLocal = Common.HttpHelper.HttpGetFileWithToken(employeeEntity.Icon, filename, newPath);
                    if (SavetoLocal)
                    {
                        //使用ID作为条件(就使用文本作为条件即可)
                        var filterID = Builders<BsonDocument>.Filter.Eq("_id", employeeEntity._id);                        

                        //更改值为已上传
                        var update = Builders<BsonDocument>.Update.Set("IsSyncImage", true).Set("LocalFileName", filename);
                        //查找并修改文档
                        Common.MongodbHandler.GetInstance().FindOneAndUpdate(collection, filterID, update);
                        Common.LogHandler.WriteLog("ID[" + employeeEntity._id + "]员工照片已经同步照片至本地，文件名称为" + filename);
                    }

                }
            }
            catch (Exception ex)
            {
                Common.LogHandler.WriteLog("MES同步照片程序发现错误，请管理员及时处理。" + ex.Message);
            }
        }

        //处理第一次生产记录
        private void ProcessJobOrderFirstProduceLog()
        {
            try
            {
                //找到工单集合
                var collection = Common.MongodbHandler.GetInstance().GetCollection(defaultJobOrderFirstProduceLogMongodbCollectionName);
                //找到更新标识工单记录
                var newfilter = Builders<BsonDocument>.Filter.Eq("IsSyncToServer", false);
                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).ToList();
                //循环处理
                foreach (var data in getdocument)
                {
                    //转换成类
                    var jobOrderFirstProduceLogEntity = BsonSerializer.Deserialize<Model.JobOrderFirstProduceLog>(data);

                    Common.LogHandler.WriteLog("开始发送，等待答复");

                    //jobOrderEntity.ToJson(jsonWriterSettings)
                    //读取Mongodb机器状态日志并上传至队列中
                    bool sendToServerFlag = Common.RabbitMQClientHandler.GetInstance().DirectExchangePublishMessageToServerAndWaitConfirm(
                            defaultJobOrderFirstProduceLogLog_ExchangeName,
                            defaultJobOrderFirstProduceLog_RoutingKey,
                            defaultJobOrderFirstProduceLog_QueueName,
                            Newtonsoft.Json.JsonConvert.SerializeObject(jobOrderFirstProduceLogEntity)
                        );

                    if (sendToServerFlag)
                    {
                        /*当上传至服务器以后，更改数据*/
                        /*---------------------------------------------------------*/

                        //使用整个对象作为条件
                        var filterID = Builders<BsonDocument>.Filter.And(data);
                        //更改值为已上传
                        var update = Builders<BsonDocument>.Update.Set("IsSyncToServer", true);
                        //查找并修改文档
                        Common.MongodbHandler.GetInstance().FindOneAndUpdate(collection, filterID, update);
                        Common.LogHandler.WriteLog("工单_id["+jobOrderFirstProduceLogEntity.JobOrderID+"]第一次生产已同步");

                    }
                }
            }
            catch (Exception ex)
            {
                Common.LogHandler.WriteLog("MES同步工单第一次生产记录发现错误，请管理员及时处理。" + ex.Message);
            }
        }
        //处理生产记录
        private void ProcessMachineProduction()
        {
            try
            {
                //找到工单集合
                var collection = Common.MongodbHandler.GetInstance().GetCollection(defaultMachineProductionMongodbCollectionName);
                //找到更新标识工单记录
                var newfilter = Builders<BsonDocument>.Filter.Eq("IsSyncToServer", false);
                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).ToList();
                //循环处理
                foreach (var data in getdocument)
                {
                    //转换成类
                    var machineProductionEntity = BsonSerializer.Deserialize<Model.MachineProduction>(data);
                    Model.MachineProduction_JSON machineProduction_JSON = new Model.MachineProduction_JSON();
                    machineProduction_JSON.Id = machineProductionEntity._id;
                    machineProduction_JSON.Date = machineProductionEntity.Date.ToString("yyyy-MM-dd HH:mm:ss.fffffffK");
                    machineProduction_JSON.MachineID = machineProductionEntity.MachineID;
                    machineProduction_JSON.JobOrderID = machineProductionEntity.JobOrderID;
                    machineProduction_JSON.StartDateTime= machineProductionEntity.StartDateTime.ToString("yyyy-MM-dd HH:mm:ss.fffffffK");
                    machineProduction_JSON.EndDateTime=machineProductionEntity.EndDateTime.ToString("yyyy-MM-dd HH:mm:ss.fffffffK");
                    machineProduction_JSON.ProduceCount = machineProductionEntity.ProduceCount;
                    machineProduction_JSON.WorkShiftID = machineProductionEntity.WorkShiftID;
                    machineProduction_JSON.EmployeeProductionTimeList = new List<Model.EmployeeProductionTimeList>();
                    foreach (var oldItem in machineProductionEntity.EmployeeProductionTimeList)
                    {
                        //true则新增
                        bool flag = true;
                        foreach(var newItem in machineProduction_JSON.EmployeeProductionTimeList)
                        {
                            if (newItem.EmployeeID == oldItem.EmployeeID)
                            {
                                newItem.WorkHour = Math.Round(newItem.WorkHour + oldItem.WorkHour,3);
                                flag = false;
                            }
                        }
                        if (flag)
                        {
                            machineProduction_JSON.EmployeeProductionTimeList.Add(oldItem);
                        }
                    }
                    machineProduction_JSON.JobOrderProductionTime = machineProductionEntity.JobOrderProductionTime;
                    machineProduction_JSON.ProduceSecond = machineProductionEntity.ProduceSecond;
                    machineProduction_JSON.ErrorCount = machineProductionEntity.ErrorCount;
                    machineProduction_JSON.IsStopFlag = machineProductionEntity.IsStopFlag;
                    //jobOrderEntity.ToJson(jsonWriterSettings)
                    //读取Mongodb机器状态日志并上传至队列中
                    bool sendToServerFlag = Common.RabbitMQClientHandler.GetInstance().DirectExchangePublishMessageToServerAndWaitConfirm(
                            defaultMachineProduction_ExchangeName,
                            defaultMachineProduction_RoutingKey,
                            defaultMachineProduction_QueueName,
                            Newtonsoft.Json.JsonConvert.SerializeObject(machineProduction_JSON)
                        );

                    if (sendToServerFlag)
                    {
                        /*当上传至服务器以后，更改数据*/
                        /*---------------------------------------------------------*/

                        //使用整个对象作为条件
                        var filterID = Builders<BsonDocument>.Filter.And(data);
                        //更改值为已上传
                        var update = Builders<BsonDocument>.Update.Set("IsSyncToServer", true);
                        //查找并修改文档
                        Common.MongodbHandler.GetInstance().FindOneAndUpdate(collection, filterID, update);
                       
                    }
                }
            }
            catch (Exception ex)
            {
                Common.LogHandler.WriteLog("MES同步工单生产记录发现错误，请管理员及时处理。" + ex.Message);
            }
        }
        /// <summary>
        /// 检查记录注册情况
        /// </summary>
        public void CheckMachineRegister()
        {
            try
            {
                if (MC_IsSyncDataFlag == false)
                {                    
                    var collection = Common.MongodbHandler.GetInstance().GetCollection(defaultMachineRegisterMongodbCollectionName);

                    //查找机器注册信息
                    var newfilter = Builders<BsonDocument>.Filter.Exists("MachineID", true);
                    var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).ToList();

                    if (getdocument != null && getdocument.Count > 0)
                    {
                        //注册的ID
                        MC_MachineRegisterID = getdocument.First().GetValue("MachineID").ToString();

                        Common.LogHandler.WriteLog("该机器已经注册过，机器注册码为[" + MC_MachineRegisterID + "]");

                        //调用处理
                        CheckDBCallOneTimeFunction();
                    }
                }                                
            }
            catch(Exception ex)
            {
                Common.LogHandler.WriteLog("MES检测程序程序发现错误，请管理员及时处理。" + ex.Message);
            }
        }

        /// <summary>
        /// 同步数据操作
        /// </summary>
        public void ProcessSyncDataAction()
        {
            try
            {
                /*手动处理时可用*/
                ////处理
                //Common.RabbitMQClientHandler.GetInstance().SyncDataFromServer(defaultSyncData_QueueName_Prefix);
                ////数据同步标识
                //MC_IsSyncDataFlag = true;

                //自动处理
                if (!string.IsNullOrEmpty(MC_MachineRegisterID))
                {
                    //处理
                    Common.RabbitMQClientHandler.GetInstance().SyncDataFromServer(defaultSyncData_QueueName_Prefix + MC_MachineRegisterID);

                    //数据同步标识
                    MC_IsSyncDataFlag = true;
                }
            }
            catch (Exception ex)
            {
                //数据同步标识
                MC_IsSyncDataFlag = false;                
                
                //出错重新调用自身
                ProcessSyncDataAction();

                Common.LogHandler.WriteLog("MES数据同步服务程序发现错误，请管理员及时处理。" + ex.Message);
            }
        }

        private void Cleanup()
        {
            try
            {
                if (_timer != null)
                {
                    _timer.Stop();
                }

                if (_SyncEmployeeImageTimer != null)
                {
                    _SyncEmployeeImageTimer.Stop();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void TryRestartService()
        {
            try
            {
                Cleanup();

                var mres = new ManualResetEventSlim(false); // state is initially false
                while (!mres.Wait(5000)) // loop until state is true, checking every 3s
                {
                    try
                    {
                        //尝试启动服务
                        StartTimerFunction();

                        mres.Set(); // state set to true - breaks out of loop
                    }
                    catch (Exception ex)
                    {
                        Common.LogHandler.WriteLog("尝试启动MES服务出现错误：" + ex.Message, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.LogHandler.WriteLog("尝试启动MES服务出现错误：" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 开始方法
        /// </summary>
        public void Start()
        {
            TryRestartService();
        }

        /// <summary>
        /// 结束方法
        /// </summary>
        public void Stop()
        {

            Cleanup();

            //if (_timer != null)
            //{
            //    _timer.Stop();
            //}

            //if (_SyncEmployeeImageTimer != null)
            //{
            //    _SyncEmployeeImageTimer.Stop();
            //}
        }
    }
}
