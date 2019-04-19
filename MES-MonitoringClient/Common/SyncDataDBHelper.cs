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

namespace MES_MonitoringClient.Common
{
    public static class SyncDataDBHelper
    {
        /// <summary>
        /// 同步处理数据方法
        /// </summary>
        /// <param name="jsonString">传入JSON</param>
        /// <returns></returns>
        public static bool SyncData_Process(string collectionName, string jsonString)
        {
            try
            {
                //原本的JSON转成BSON
                IEnumerable<BsonDocument> bsonElements = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<IEnumerable<BsonDocument>>(jsonString);

                //删除数据集合
                Common.MongodbHandler.GetInstance().mc_MongoDatabase.DropCollection(collectionName);

                if (bsonElements != null && bsonElements.Count() > 0)
                {
                    //声明数据集合，插入数据
                    var collection = Common.MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<BsonDocument>(collectionName);
                    collection.InsertMany(bsonElements);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool SyncEmployee_Process(string collectionName, string jsonString)
        {
            try
            {
                //原本的JSON转成BSON
                IEnumerable<BsonDocument> bsonElements = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<IEnumerable<BsonDocument>>(jsonString);

                //删除数据集合
                Common.MongodbHandler.GetInstance().mc_MongoDatabase.DropCollection(collectionName);

                if (bsonElements != null && bsonElements.Count() > 0)
                {
                    //声明数据集合，插入数据
                    var collection = Common.MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<BsonDocument>(collectionName);

                    foreach (BsonDocument item in bsonElements)
                    {
                        item.Add(new BsonElement("IsSyncImage", false));
                        item.Add(new BsonElement("LocalFileName", ""));
                    }

                    collection.InsertMany(bsonElements);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
