using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;
using System.Diagnostics;

using MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections;
using MongoDB.Bson.Serialization;

namespace MES_MonitoringClient.Common
{
    /// <summary>
    /// 最后操作工单类
    /// </summary>
    public class ClockInRecordHandler
    {
        /*MongoDB数据库*/
        /*-------------------------------------------------------------------------------------*/
        /// <summary>
        ///  打卡记录Mongodb集合
        /// </summary>
        private static IMongoCollection<DataModel.ClockInRecord> ClockInRecordCollection;

        /// <summary>
        /// 打卡记录默认Mongodb集合名
        /// </summary>
        private static string defaultClockInRecordMongodbCollectionName = "ClockInRecord";

        /// <summary>
        /// 构造函数，处理初始化的参数
        /// </summary>
        public ClockInRecordHandler()
        {
            ClockInRecordCollection = MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.ClockInRecord>(defaultClockInRecordMongodbCollectionName);
        }


        /// <summary>
        /// 打卡记录保存
        /// </summary>
        /// <param name="clockInRecord">打卡记录保存</param>
        public void SaveClockInRecord(DataModel.ClockInRecord clockInRecord)
        {
            ClockInRecordCollection.InsertOne(clockInRecord);

        }
        /// <summary>
        /// 通过工号找到未结束记录
        /// </summary>
        /// <returns></returns>
        public  DataModel.ClockInRecord QueryClockInRecordByEmployeeID(string employeeID)
        {
            try
            {
                var collection = Common.MongodbHandler.GetInstance().GetCollection(defaultClockInRecordMongodbCollectionName);
                var newfilter = Builders<BsonDocument>.Filter.Eq("EmployeeID", employeeID);
                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).ToList();


                foreach (var data in getdocument)
                {
                    //转换成类
                    var ClockInRecordEntity = BsonSerializer.Deserialize<DataModel.ClockInRecord>(data);
                    if (ClockInRecordEntity.StartDate == ClockInRecordEntity.EndDate)
                    {
                        return ClockInRecordEntity;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //获取未结束记录列表
        public List<DataModel.ClockInRecord> GetClockInRecordList()
        {

            List<DataModel.ClockInRecord> clockInRecords = ClockInRecordCollection.AsQueryable().ToList().FindAll(t=>t.StartDate==t.EndDate);
            if (clockInRecords.Count > 0)
            {
                return clockInRecords;
            }
            else
            {
                return new List<DataModel.ClockInRecord>();
            }


            // machineProduceStatusHandler.CurrentProcessJobOrder = MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.JobOrder>("JobOrder").AsQueryable().ToList()[0];
            // machineProduceStatusHandler.CurrentMouldProduct = Common.MouldProductHelper.GetMmouldProductByMouldCode(MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.JobOrder>("JobOrder").Find(filterID).ToList()[0].MouldCode);


        }
        ///<summary>
        ///更新记录（签退）
        ///</summary>
        public void UpdateClockInRecord(DataModel.ClockInRecord clockInRecord,bool isAuto)
        {
            try
            {
                //查找ID
                var collection = Common.MongodbHandler.GetInstance().GetCollection(defaultClockInRecordMongodbCollectionName);
                var filterid = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(clockInRecord._id));
                BsonDocument updatedocument = clockInRecord.ToBsonDocument();
                updatedocument.Remove("_id");
                updatedocument.Set("EndDate", DateTime.Now);
                updatedocument.Set("IsAuto", isAuto);
                updatedocument.Set("IsUploadToServer", false);

                BsonDocument updateResult = collection.FindOneAndUpdate(filterid, updatedocument);
             
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
