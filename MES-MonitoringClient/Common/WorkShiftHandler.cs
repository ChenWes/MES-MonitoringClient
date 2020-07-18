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
    public class WorkShiftHandler
    {
        /*MongoDB数据库*/
        /*-------------------------------------------------------------------------------------*/
        /// <summary>
        ///  打卡记录Mongodb集合
        /// </summary>
        private static IMongoCollection<DataModel.WorkShift> WorkShiftCollection;

        /// <summary>
        /// 打卡记录默认Mongodb集合名
        /// </summary>
        private static string defaultWorkShiftMongodbCollectionName = Common.ConfigFileHandler.GetAppConfig("WorkShiftCollectionName");

        /// <summary>
        /// 构造函数，处理初始化的参数
        /// </summary>
        public WorkShiftHandler()
        {
            WorkShiftCollection = MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.WorkShift>(defaultWorkShiftMongodbCollectionName);
        }

        /// <summary>
        /// 通过id找到班次
        /// </summary>
        /// <returns></returns>
        public static DataModel.WorkShift QueryWorkShiftByid(string id)
        {
            try
            {
                var collection = Common.MongodbHandler.GetInstance().GetCollection(defaultWorkShiftMongodbCollectionName);
                var newfilter = Builders<BsonDocument>.Filter.Eq("_id", id);
                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).ToList();

                foreach (var data in getdocument)
                {
                    //转换成类
                    var WorkShiftEntity = BsonSerializer.Deserialize<DataModel.WorkShift>(data);
                    return WorkShiftEntity;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 通过班次码找到班次
        /// </summary>
        /// <returns></returns>
        public static DataModel.WorkShift QueryWorkShiftByCode(string workShiftCode)
        {
            try
            {
                var collection = Common.MongodbHandler.GetInstance().GetCollection(defaultWorkShiftMongodbCollectionName);
                var newfilter = Builders<BsonDocument>.Filter.Eq("WorkShiftCode", workShiftCode);
                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).ToList();

                foreach (var data in getdocument)
                {
                    //转换成类
                    var WorkShiftEntity = BsonSerializer.Deserialize<DataModel.WorkShift>(data);
                    return WorkShiftEntity;
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
