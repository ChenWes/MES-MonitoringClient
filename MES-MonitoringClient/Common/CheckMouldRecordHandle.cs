using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_MonitoringClient.Common
{
     public class CheckMouldRecordHandle
    {
 
        private static IMongoCollection<DataModel.CheckMouldRecord> CheckMouldRecordCollection;

    
        private static string defaultCheckMouldRecordMongodbCollectionName = "CheckMouldRecord";

     
        public CheckMouldRecordHandle()
        {
            CheckMouldRecordCollection = MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.CheckMouldRecord>(defaultCheckMouldRecordMongodbCollectionName);
        }


        /// <summary>
        /// 记录保存
        /// </summary>
        /// <param name="clockInRecord">记录</param>
        public void SaveClockInRecord(DataModel.CheckMouldRecord checkMouldRecord)
        {
            CheckMouldRecordCollection.InsertOne(checkMouldRecord);

        }

        //更新记录
        public void addCheckMouldLog(DataModel.CheckMouldRecord checkMouldRecord)
        {
            try
            {
                //查找ID
                var collection = Common.MongodbHandler.GetInstance().GetCollection(defaultCheckMouldRecordMongodbCollectionName);
                var filterid = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(checkMouldRecord._id));
                BsonDocument updatedocument = checkMouldRecord.ToBsonDocument();
                updatedocument.Remove("_id");

                BsonDocument updateResult = collection.FindOneAndUpdate(filterid, updatedocument);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 通过机器状态id
        /// </summary>
        /// <returns></returns>
        public static DataModel.CheckMouldRecord QueryCheckMouldRecord(string machineStatusLogID)
        {
            try
            {
                var collection = Common.MongodbHandler.GetInstance().GetCollection(defaultCheckMouldRecordMongodbCollectionName);
                var newfilter = Builders<BsonDocument>.Filter.Eq("MachineStatusLogID", machineStatusLogID);
                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).ToList();

                foreach (var data in getdocument)
                {
                    //转换成类
                    var MachineStatusLogIDEntity = BsonSerializer.Deserialize<DataModel.CheckMouldRecord>(data);
                    return MachineStatusLogIDEntity;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
