using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace MES_MonitoringClient.Common
{

    /// <summary>
    /// 工单处理记录
    /// </summary>
    public class JobOrderProcessHelper
    {
        public enum JobOrderProcessStatus
        {
            Process,
            Stop,
        }

        public static string MC_JobOrderProcessCollectionName = Common.ConfigFileHandler.GetAppConfig("JobOrderProcessCollectionName");

        
        public static DataModel.JobOrderProcess GetJobOrderProcessByID(string id)
        {
            try
            {               
                var collection = Common.MongodbHandler.GetInstance().GetCollection(MC_JobOrderProcessCollectionName);
                var newfilter = Builders<BsonDocument>.Filter.Eq("_id", new BsonObjectId(id));
                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).FirstOrDefault();

                if (getdocument != null) return BsonSerializer.Deserialize<DataModel.JobOrderProcess>(getdocument);

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 找到工单处理记录
        /// </summary>
        /// <param name="jobOrderID"></param>
        /// <returns></returns>
        public static DataModel.JobOrderProcess GetJobOrderProcessByJobOrderID(string jobOrderID)
        {
            try
            {                
                var collection = Common.MongodbHandler.GetInstance().GetCollection(MC_JobOrderProcessCollectionName);
                var newfilter = Builders<BsonDocument>.Filter.Eq("JobOrderID", jobOrderID);
                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).FirstOrDefault();

                if (getdocument != null) return BsonSerializer.Deserialize<DataModel.JobOrderProcess>(getdocument);

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 处理工单记录状态
        /// </summary>
        /// <param name="jobOrderProcessStatus"></param>
        /// <param name="jobOrderID"></param>
        /// <returns></returns>
        public static bool SetJobOrderProcessStatus(JobOrderProcessStatus jobOrderProcessStatus, string jobOrderID)
        {
            try
            {
                var collection = Common.MongodbHandler.GetInstance().GetCollection(MC_JobOrderProcessCollectionName);
                var filterID = Builders<BsonDocument>.Filter.Eq("JobOrderID", jobOrderID);

                var update = Builders<BsonDocument>.Update.Set("Status", jobOrderProcessStatus.ToString());

                //尝试找到原来的记录
                var getOldJobOrderProcess = collection.Find(filterID).FirstOrDefault();
                if (getOldJobOrderProcess == null)
                {
                    BsonDocument bsonElements = new BsonDocument();
                    bsonElements.Add(new BsonElement("StartDateTime", System.DateTime.Now));
                    bsonElements.Add(new BsonElement("ProductCount", 0));
                    bsonElements.Add(new BsonElement("ProductErrorCount", 0));

                    bsonElements.Add(new BsonElement("JobOrderID", jobOrderID));
                    bsonElements.Add(new BsonElement("Status", jobOrderProcessStatus.ToString()));

                    collection.InsertOne(bsonElements);
                }
                else
                {
                    getOldJobOrderProcess.SetElement(new BsonElement("Status", jobOrderProcessStatus.ToString()));
                    //直接修改                    
                    collection.FindOneAndUpdate(filterID, Builders<BsonDocument>.Update.Combine(getOldJobOrderProcess));
                }
                
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //更新数量
        public static bool SetJobOrderProcessCount(int processCount, string jobOrderID)
        {
            try
            {
                var collection = Common.MongodbHandler.GetInstance().GetCollection(MC_JobOrderProcessCollectionName);
                var filterID = Builders<BsonDocument>.Filter.Eq("JobOrderID", jobOrderID);
                

                //尝试找到原来的记录
                var getOldJobOrderProcess = collection.Find(filterID).FirstOrDefault();
                if (getOldJobOrderProcess != null)
                {
                    getOldJobOrderProcess.SetElement(new BsonElement("ProductCount", processCount));                    
                    collection.FindOneAndUpdate(filterID, Builders<BsonDocument>.Update.Combine(getOldJobOrderProcess));
                }                

                return true;                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //更新数量
        public static bool SetJobOrderProcessErrorCount(int processErrorCount, string jobOrderID)
        {
            try
            {
                var collection = Common.MongodbHandler.GetInstance().GetCollection(MC_JobOrderProcessCollectionName);
                var filterID = Builders<BsonDocument>.Filter.Eq("JobOrderID", jobOrderID);
                

                //尝试找到原来的记录
                var getOldJobOrderProcess = collection.Find(filterID).FirstOrDefault();
                if (getOldJobOrderProcess != null)
                {                    
                    getOldJobOrderProcess.SetElement(new BsonElement("ProductErrorCount", processErrorCount));                    
                    collection.FindOneAndUpdate(filterID, Builders<BsonDocument>.Update.Combine(getOldJobOrderProcess));
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
