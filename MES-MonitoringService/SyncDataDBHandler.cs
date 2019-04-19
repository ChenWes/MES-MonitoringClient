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
    /// <summary>
    /// 数据同步数据库操作帮助类
    /// </summary>
    public static class SyncDataDBHandler
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


        /// <summary>
        /// 标准同步数据方法
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="dataJson"></param>
        /// <param name="id"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool SyncData_DBHandler(string collectionName, string dataJson, string id, string action)
        {
            try
            {
                //JSON转成BSON
                BsonDocument bsonDocument = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(dataJson);

                if (action.ToUpper() == ActionType.ADD.ToString())
                {
                    return SyncData_Add(collectionName, bsonDocument);
                }
                else if (action.ToUpper() == ActionType.EDIT.ToString())
                {
                    return SyncData_Edit(collectionName, id, bsonDocument);
                }
                else if (action.ToUpper() == ActionType.DELETE.ToString())
                {
                    return SyncData_Delete(collectionName, id);
                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 同步工单
        /// 简单的逻辑，如果该单是现在这台机器的，按原来的流程（新增、修改、删除）来处理
        /// 如果不是现在这台机器的，那就删除
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="dataJson"></param>
        /// <param name="id"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool SyncOrder_DBHandler(string collectionName, string dataJson, string id, string action)
        {
            try
            {
                BsonDocument bsonDocument = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(dataJson);

                //查看工单是否匹配到当前机器，
                //匹配则继续按原有流程处理
                //不匹配则直接删除工单
                if (SyncJobOrderCompareMachine.MatchLocalMachine(dataJson))
                {
                    #region 原有流程

                    if (action.ToUpper() == ActionType.ADD.ToString())
                    {
                        return SyncJobOrder_Add(collectionName, bsonDocument);
                    }
                    else if (action.ToUpper() == ActionType.EDIT.ToString())
                    {
                        return SyncJobOrder_Edit(collectionName, id, bsonDocument);
                    }
                    else if (action.ToUpper() == ActionType.DELETE.ToString())
                    {
                        return SyncData_Delete(collectionName, id);
                    }

                    #endregion
                }
                else
                {
                    #region 直接删除工单

                    return SyncData_Delete(collectionName, id);

                    #endregion
                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 同步员工
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="dataJson"></param>
        /// <param name="id"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool SyncEmployee_DBHandler(string collectionName, string dataJson, string id, string action)
        {
            try
            {
                BsonDocument bsonDocument = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(dataJson);

                //查看工单是否匹配到当前机器，
                //匹配则继续按原有流程处理
                //不匹配则直接删除工单
                if (SyncJobOrderCompareMachine.MatchLocalMachine(dataJson))
                {
                    #region 原有流程

                    if (action.ToUpper() == ActionType.ADD.ToString())
                    {
                        return SyncJobOrder_Add(collectionName, bsonDocument);
                    }
                    else if (action.ToUpper() == ActionType.EDIT.ToString())
                    {
                        return SyncJobOrder_Edit(collectionName, id, bsonDocument);
                    }
                    else if (action.ToUpper() == ActionType.DELETE.ToString())
                    {
                        return SyncData_Delete(collectionName, id);
                    }

                    #endregion
                }
                else
                {
                    #region 直接删除工单

                    return SyncData_Delete(collectionName, id);

                    #endregion
                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }





        //标准处理
        //--------------------------------------------------------------------------------------

        /// <summary>
        /// 新增方法
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="bsonDocument"></param>
        /// <returns></returns>
        public static bool SyncData_Add(string collectionName, BsonDocument bsonDocument)
        {
            try
            {
                #region 新增

                var collection = Common.MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<BsonDocument>(collectionName);
                collection.InsertOne(bsonDocument);

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
        /// <param name="collectionName"></param>
        /// <param name="id"></param>
        /// <param name="bsonDocument"></param>
        /// <returns></returns>
        public static bool SyncData_Edit(string collectionName, string id, BsonDocument bsonDocument)
        {
            try
            {

                #region 修改

                var collection = Common.MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<BsonDocument>(collectionName);
                var filterID = new BsonDocument("_id", id);

                bsonDocument.Remove("_id");
                collection.FindOneAndUpdate(filterID, Builders<BsonDocument>.Update.Combine(bsonDocument));


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
        /// <param name="collectionName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool SyncData_Delete(string collectionName, string id)
        {
            try
            {

                #region 删除

                var collection = Common.MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<BsonDocument>(collectionName);
                var filterID = new BsonDocument("_id", id);
                collection.FindOneAndDelete(filterID);

                #endregion

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }





        //工单特殊处理
        //--------------------------------------------------------------------------------------

        /// <summary>
        /// 工单新增方法
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="bsonDocument"></param>
        /// <returns></returns>
        public static bool SyncJobOrder_Add(string collectionName, BsonDocument bsonDocument)
        {
            try
            {
                #region 新增

                var collection = Common.MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<BsonDocument>(collectionName);                
                collection.InsertOne(bsonDocument);

                #endregion

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 工单修改方法
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="id"></param>
        /// <param name="bsonDocument"></param>
        /// <returns></returns>
        public static bool SyncJobOrder_Edit(string collectionName, string id, BsonDocument bsonDocument)
        {
            try
            {

                #region 修改

                var collection = Common.MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<BsonDocument>(collectionName);
                var filterID = new BsonDocument("_id", id);

                var getCollection = collection.Find(filterID).FirstOrDefault();
                if (getCollection == null)
                {                    
                    //collection.InsertOne(bsonDocument);
                    //调用新增方法
                    return SyncJobOrder_Add(collectionName, bsonDocument);
                }
                else
                {
                    //直接修改（移除_id，如果修改_id将出错）
                    bsonDocument.Remove("_id");
                    collection.FindOneAndUpdate(filterID, Builders<BsonDocument>.Update.Combine(bsonDocument));
                }


                #endregion

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }




        //员工特殊处理
        //--------------------------------------------------------------------------------------

        /// <summary>
        /// 工单新增方法
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="bsonDocument"></param>
        /// <returns></returns>
        public static bool SyncEmployee_Add(string collectionName, BsonDocument bsonDocument)
        {
            try
            {
                bsonDocument.Add(new BsonElement("IsSyncImage", false));
                bsonDocument.Add(new BsonElement("LocalFileName", ""));

                #region 新增

                var collection = Common.MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<BsonDocument>(collectionName);
                bsonDocument.Add(new BsonElement("Status", BsonNull.Value));
                collection.InsertOne(bsonDocument);

                #endregion

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 工单修改方法
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="id"></param>
        /// <param name="bsonDocument"></param>
        /// <returns></returns>
        public static bool SyncEmployee_Edit(string collectionName, string id, BsonDocument bsonDocument)
        {
            try
            {
                bsonDocument.Add(new BsonElement("IsSyncImage", false));
                bsonDocument.Add(new BsonElement("LocalFileName", ""));

                #region 修改

                var collection = Common.MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<BsonDocument>(collectionName);
                var filterID = new BsonDocument("_id", id);

                var getCollection = collection.Find(filterID).FirstOrDefault();
                if (getCollection == null)
                {
                    collection.InsertOne(bsonDocument);
                }
                else
                {
                    bsonDocument.Remove("_id");
                    collection.FindOneAndUpdate(filterID, Builders<BsonDocument>.Update.Combine(bsonDocument));
                }


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
