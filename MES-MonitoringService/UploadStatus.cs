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


namespace MES_MonitoringService
{
    public class UploadStatus
    {
        //服务运行间隔时间
        private static string defaultUploadDataIntervalMilliseconds = Common.ConfigFileHandler.GetAppConfig("UploadDataIntervalMilliseconds");

        //机器状态日志队列名称
        private static string defaultMachineStatusQueueName = Common.ConfigFileHandler.GetAppConfig("MachineStatusLogQueueName");
        //机器状态日志Mongodb数据集名称
        private static string defaultMachineStatusMongodbCollectionName = Common.ConfigFileHandler.GetAppConfig("MachineStatusCollectionName");

        //定时器
        private readonly Timer _timer;

        /// <summary>
        /// 上传数据至服务器
        /// </summary>
        public UploadStatus()
        {
            if (!Common.CommonFunction.ServiceRunning(Common.MongodbHandler.MongodbServiceName))
            {
                //不存在MongoDB服务
                Common.LogHandler.Log("MES数据上传服务程序检测到该电脑暂时不存在Mongodb服务，服务未能正常运行，请管理员及时处理。");
            }
            else
            {
                //时间
                long timeInterval = 0;
                long.TryParse(defaultUploadDataIntervalMilliseconds, out timeInterval);

                //MongoDB服务正常
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
            var collection = Common.MongodbHandler.GetInstance().GetCollection(defaultMachineStatusMongodbCollectionName);
            //var filter = Builders<BsonDocument>.Filter.Eq("IsUploadToServer", false);

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
                var machineStatusEntity = BsonSerializer.Deserialize<Model.MachineStatus>(data);

                //读取Mongodb机器状态日志并上传至队列中
                Common.RabbitMQClientHandler.GetInstance().publishMessageToServer(defaultMachineStatusQueueName, machineStatusEntity.ToBsonDocument().ToJson());

                /*当上传至服务器以后，更改数据*/
                /*---------------------------------------------------------*/
                if (!machineStatusEntity.IsUploadToServer)
                {
                    //使用ID作为条件
                    var filterID = Builders<BsonDocument>.Filter.Eq("_id", new BsonObjectId(machineStatusEntity.Id));
                    //更改值为已上传
                    var update = Builders<BsonDocument>.Update.Set("IsUploadToServer", true);
                    //查找并修改文档
                    Common.MongodbHandler.GetInstance().FindOneAndUpdate(collection, filterID, update);
                    Common.LogHandler.Log("[" + machineStatusEntity.Id.ToString() + "][" + machineStatusEntity.Status + "]已上传至服务器中，请查看");
                }
                else if (!machineStatusEntity.IsUpdateToServer && machineStatusEntity.IsStopFlag)
                {
                    //使用ID作为条件
                    var filterID = Builders<BsonDocument>.Filter.Eq("_id", new BsonObjectId(machineStatusEntity.Id));
                    //更改值为已上传
                    var update = Builders<BsonDocument>.Update.Set("IsUpdateToServer", true);
                    //查找并修改文档
                    Common.MongodbHandler.GetInstance().FindOneAndUpdate(collection, filterID, update);
                    Common.LogHandler.Log("[" + machineStatusEntity.Id.ToString() + "][" + machineStatusEntity.Status + "]已更新至服务器中，请查看");
                }
            }
        }

        public void CheckFunction()
        {
            var collection = Common.MongodbHandler.GetInstance().GetCollection(defaultMachineStatusMongodbCollectionName);
            //var filter = Builders<BsonDocument>.Filter.Eq("IsUploadToServer", false);

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
                var machineStatusEntity = BsonSerializer.Deserialize<Model.MachineStatus>(data);

                //读取Mongodb机器状态日志并上传至队列中
                Common.RabbitMQClientHandler.GetInstance().publishMessageToServer(defaultMachineStatusQueueName, machineStatusEntity.ToBsonDocument().ToJson());

                /*当上传至服务器以后，更改数据*/
                /*---------------------------------------------------------*/
                if (!machineStatusEntity.IsUploadToServer)
                {
                    //使用ID作为条件
                    var filterID = Builders<BsonDocument>.Filter.Eq("_id", new BsonObjectId(machineStatusEntity.Id));
                    //更改值为已上传
                    var update = Builders<BsonDocument>.Update.Set("IsUploadToServer", true);
                    //查找并修改文档
                    Common.MongodbHandler.GetInstance().FindOneAndUpdate(collection, filterID, update);
                    Common.LogHandler.Log("[" + machineStatusEntity.Id.ToString() + "][" + machineStatusEntity.Status + "]已上传至服务器中，请查看");
                }
                else if (!machineStatusEntity.IsUpdateToServer && machineStatusEntity.IsStopFlag)
                {
                    //使用ID作为条件
                    var filterID = Builders<BsonDocument>.Filter.Eq("_id", new BsonObjectId(machineStatusEntity.Id));
                    //更改值为已上传
                    var update = Builders<BsonDocument>.Update.Set("IsUpdateToServer", true);
                    //查找并修改文档
                    Common.MongodbHandler.GetInstance().FindOneAndUpdate(collection, filterID, update);
                    Common.LogHandler.Log("[" + machineStatusEntity.Id.ToString() + "][" + machineStatusEntity.Status + "]已更新至服务器中，请查看");
                }
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
