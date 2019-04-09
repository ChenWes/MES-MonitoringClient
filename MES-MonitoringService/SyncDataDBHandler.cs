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
    public class SyncDataDBHandler<T> where T : Model.SyncData
    {
        /// <summary>
        /// 操作类型
        /// </summary>
        public enum ActionType
        {
            ADD,
            EDIT,
            DELETE
        }

        //操作的Collection
        private IMongoCollection<T> operationCollection;        

        /// <summary>
        /// 同步数据方法
        /// </summary>
        /// <param name="dataJson">HTTP得到的data实体的JSON字符串</param>
        /// <param name="id">HTTP得到的的ID</param>
        /// <param name="action">HTTP得到的action</param>
        /// <returns></returns>
        public bool SyncData_DBHandler(string dataJson, string id, string action)
        {
            try
            {
                //反序列化为类实体
                T dataEntityClass= JsonConvert.DeserializeObject<T>(dataJson);

                if (action.ToUpper() == ActionType.ADD.ToString())
                {
                    return SyncData_Add(dataEntityClass);
                }
                else if (action.ToUpper() == ActionType.EDIT.ToString())
                {
                    return SyncData_Edit(dataEntityClass, id);
                }
                else if (action.ToUpper() == ActionType.DELETE.ToString())
                {
                    return SyncData_Delete(dataEntityClass, id);
                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 新增方法
        /// </summary>
        /// <param name="dataEntityClass">数据实体</param>
        /// <returns></returns>
        public bool SyncData_Add(T dataEntityClass)
        {
            try
            {
                operationCollection = Common.MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<T>(dataEntityClass.getCollectionName());                

                #region 新增

                operationCollection.InsertOne(dataEntityClass);

                #endregion

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 修改方法
        /// </summary>
        /// <param name="dataEntityClass">数据实体</param>
        /// <param name="id">实体ID</param>
        /// <returns></returns>
        public bool SyncData_Edit(T dataEntityClass, string id)
        {
            try
            {
                var collection = Common.MongodbHandler.GetInstance().GetCollection(dataEntityClass.getCollectionName());
                var filterID = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(id));
                
                #region 修改

                Common.ModelPropertyHelper<T> convertClass = new Common.ModelPropertyHelper<T>();
                var update = convertClass.GetModelClassProperty(dataEntityClass);
                Common.MongodbHandler.GetInstance().FindOneAndUpdate(collection, filterID, Builders<BsonDocument>.Update.Combine(update));

                #endregion

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 删除方法
        /// </summary>
        /// <param name="dataEntityClass">数据实体</param>
        /// <param name="id">实体ID</param>
        /// <returns></returns>
        public bool SyncData_Delete(T dataEntityClass, string id)
        {
            try
            {
                var collection = Common.MongodbHandler.GetInstance().GetCollection(dataEntityClass.getCollectionName());
                var filterID = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(id));

                #region 删除

                Common.MongodbHandler.GetInstance().FindOneAndDelete(collection, filterID);

                #endregion

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
