using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;

namespace MES_MonitoringService
{
    public class SyncDataHandler
    {
        public enum ActionType
        {
            ADD,
            EDIT,
            DELETE
        }

        private IMongoCollection<Model.MachineStatus> machineStatusCollection;
        private static string defaultMachineStatusMongodbCollectionName = "MachineStatus";


        public SyncDataHandler()
        {
            machineStatusCollection = Common.MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<Model.MachineStatus>(defaultMachineStatusMongodbCollectionName);
        }

        /// <summary>
        /// 处理同步的数据
        /// </summary>
        /// <param name="jsonString">通过同步获取的JSON数据</param>
        /// <returns></returns>
        public bool ProcessData(string jsonString)
        {
            try
            {
                //处理一级参数
                string type = Common.JsonHelper.GetJsonValue(jsonString, "type");
                string action = Common.JsonHelper.GetJsonValue(jsonString, "action");
                string id = Common.JsonHelper.GetJsonValue(jsonString, "id");

                string dataJson = Common.JsonHelper.GetJsonValue(jsonString, "data");

                if (type == "MachineStatus")
                {
                    //解析成类，并且默认增加一个参数
                    Model.MachineStatus syncDataClass = JsonConvert.DeserializeObject<Model.MachineStatus>(dataJson);
                    syncDataClass.OriginalID = id;

                    if (action.ToUpper() == ActionType.ADD.ToString())
                    {
                        #region 新增

                        machineStatusCollection.InsertOne(syncDataClass);

                        #endregion
                    }
                    else if (action.ToUpper() == ActionType.EDIT.ToString() || action.ToUpper() == ActionType.DELETE.ToString())
                    {
                        var collection = Common.MongodbHandler.GetInstance().GetCollection(defaultMachineStatusMongodbCollectionName);
                        var filterID = Builders<BsonDocument>.Filter.Eq("OriginalID", id);

                        if (action.ToUpper() == ActionType.EDIT.ToString())
                        {
                            #region 修改

                            Common.ModelPropertyHelper<Model.MachineStatus> convertClass = new Common.ModelPropertyHelper<Model.MachineStatus>();
                            var update = convertClass.GetModelClassProperty(syncDataClass);
                            Common.MongodbHandler.GetInstance().FindOneAndUpdate(collection, filterID, Builders<BsonDocument>.Update.Combine(update));

                            #endregion
                        }
                        else if (action.ToUpper() == ActionType.DELETE.ToString())
                        {
                            #region 删除

                            Common.MongodbHandler.GetInstance().FindOneAndDelete(collection, filterID);

                            #endregion
                        }
                    }
                }

                return true;

            }
            catch (Exception ex)
            {
                Common.LogHandler.Log("处理同步数据出错，原因：" + ex.Message);
                return false;
            }
        }
    }
}
