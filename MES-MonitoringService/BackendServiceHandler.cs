using System;
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

namespace MES_MonitoringService
{
    public class BackendServiceHandler
    {
        //服务运行间隔时间
        private static string defaultUploadDataIntervalMilliseconds = Common.ConfigFileHandler.GetAppConfig("UploadDataIntervalMilliseconds");

        //机器状态日志Mongodb数据集名称
        private static string defaultMachineStatusMongodbCollectionName = Common.ConfigFileHandler.GetAppConfig("MachineStatusCollectionName");
        //机器注册表
        private static string defaultMachineRegisterMongodbCollectionName = Common.ConfigFileHandler.GetAppConfig("MachineRegisterCollectionName");

        //机器状态对应的交换机、路由、队列名称
        private static string defaultMachineStatus_ExchangeName = Common.ConfigFileHandler.GetAppConfig("MachineStatusLog_ExchangeName");
        private static string defaultMachineStatus_RoutingKey = Common.ConfigFileHandler.GetAppConfig("MachineStatusLog_RoutingKey");
        private static string defaultMachineStatus_QueueName = Common.ConfigFileHandler.GetAppConfig("MachineStatusLog_QueueName");


        //同步数据对应的队列名称
        private static string defaultUpdateData_QueueName_Prefix = Common.ConfigFileHandler.GetAppConfig("UpdateData_QueueName_Prefix");

        ////数据同步标识（作用开关，标识出系统是否正在数据同步）
        private bool MC_IsSyncDataFlag = false;
        //机器注册码
        private string MC_MachineRegisterID = string.Empty;

        //定时器
        private readonly Timer _timer;

        /// <summary>
        /// 上传数据至服务器
        /// </summary>
        public BackendServiceHandler()
        {
            if (!Common.CommonFunction.ServiceRunning(Common.MongodbHandler.MongodbServiceName))
            {
                //不存在MongoDB服务
                Common.LogHandler.Log("MES数据上传服务程序检测到该电脑暂时不存在Mongodb服务，服务未能正常运行，请管理员及时处理。");
            }
            else
            {
                //MongoDB服务正常


                //定时任务间隔时间
                long timeInterval = 0;
                long.TryParse(defaultUploadDataIntervalMilliseconds, out timeInterval);

                //定时任务
                _timer = new Timer(timeInterval) { AutoReset = true };
                _timer.Elapsed += TimerElapsed;
            }
        }

        /// <summary>
        /// 定时方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            //处理机器状态
            ProcessMachineStatusLog();

            if (MC_IsSyncDataFlag == false)
            {
                //检测注册
                CheckMachineRegister();
            }
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
                //Common.LogHandler.Log("开始运行定时方法[ProcessMachineStatusLog]");


                //找到机器状态集合
                var collection = Common.MongodbHandler.GetInstance().GetCollection(defaultMachineStatusMongodbCollectionName);

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
                    newMachineStatus_JSON.Status = machineStatusLogEntity.Status;

                    newMachineStatus_JSON.UseTotalSeconds = machineStatusLogEntity.UseTotalSeconds;//使用秒数

                    newMachineStatus_JSON.StartDateTime = machineStatusLogEntity.StartDateTime.ToString("yyyy-MM-dd HH:mm:ss.fffffffK");//Date转换成string
                    newMachineStatus_JSON.EndDateTime = machineStatusLogEntity.EndDateTime.ToString("yyyy-MM-dd HH:mm:ss.fffffffK");//Date转换成string

                    newMachineStatus_JSON.IsStopFlag = machineStatusLogEntity.IsStopFlag;
                    newMachineStatus_JSON.LocalMacAddress = machineStatusLogEntity.LocalMacAddress;

                    //Common.LogHandler.Log("准备发送至队列=>" + JsonConvert.SerializeObject(newMachineStatus_JSON));

                    //读取Mongodb机器状态日志并上传至队列中
                    bool sendToServerFlag = Common.RabbitMQClientHandler.GetInstance().DirectExchangePublishMessageToServerAndWaitConfirm(defaultMachineStatus_ExchangeName, defaultMachineStatus_RoutingKey, defaultMachineStatus_QueueName, JsonConvert.SerializeObject(newMachineStatus_JSON));
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
                            Common.LogHandler.Log("[" + machineStatusLogEntity.Id.ToString() + "][" + machineStatusLogEntity.Status + "]已上传至服务器中，请查看");
                        }
                        else if (!machineStatusLogEntity.IsUpdateToServer && machineStatusLogEntity.IsStopFlag)
                        {
                            //使用ID作为条件
                            var filterID = Builders<BsonDocument>.Filter.Eq("_id", new BsonObjectId(machineStatusLogEntity.Id));
                            //更改值为已上传
                            var update = Builders<BsonDocument>.Update.Set("IsUpdateToServer", true);
                            //查找并修改文档
                            Common.MongodbHandler.GetInstance().FindOneAndUpdate(collection, filterID, update);
                            Common.LogHandler.Log("[" + machineStatusLogEntity.Id.ToString() + "][" + machineStatusLogEntity.Status + "]已更新至服务器中，请查看");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.LogHandler.Log("MES数据上传服务程序发现错误，请管理员及时处理。" + ex.Message);
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
                    //新增
                    //var collection = Common.MongodbHandler.GetInstance().GetCollection(defaultMachineRegisterMongodbCollectionName);
                    //var document = new BsonDocument
                    //{
                    //  {"MachineCode", BsonValue.Create("D1")},
                    //  {"MachineID", new BsonString("5c7f21627d2e4914c075bb2b")}
                    //};
                    //Common.MongodbHandler.GetInstance().InsertOne(collection, document);

                    var collection = Common.MongodbHandler.GetInstance().GetCollection(defaultMachineRegisterMongodbCollectionName);

                    //查找机器注册信息
                    var newfilter = Builders<BsonDocument>.Filter.Exists("MachineID", true);
                    var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).ToList();

                    if (getdocument != null && getdocument.Count > 0)
                    {
                        //注册的ID
                        MC_MachineRegisterID = getdocument.First().GetValue("MachineID").ToString();

                        //调用处理
                        CheckDBCallOneTimeFunction();
                    }
                }                                
            }
            catch(Exception ex)
            {
                Common.LogHandler.Log("MES检测程序程序发现错误，请管理员及时处理。" + ex.Message);
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
                //Common.RabbitMQClientHandler.GetInstance().TopicExchangeConsumeMessageFromServer(defaultUpdateData_QueueName_Prefix);
                ////数据同步标识
                //MC_IsSyncDataFlag = true;

                //自动处理
                if (!string.IsNullOrEmpty(MC_MachineRegisterID))
                {
                    //处理
                    Common.RabbitMQClientHandler.GetInstance().TopicExchangeConsumeMessageFromServer(defaultUpdateData_QueueName_Prefix + MC_MachineRegisterID);

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

                Common.LogHandler.Log("MES数据同步服务程序发现错误，请管理员及时处理。" + ex.Message);
            }
        }

        /// <summary>
        /// 开始方法
        /// </summary>
        public void Start()
        {
            if (_timer != null)
            {
                _timer.Start();
            }
        }

        /// <summary>
        /// 结束方法
        /// </summary>
        public void Stop()
        {
            if (_timer != null)
            {
                _timer.Stop();
            }
        }
    }
}
