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
    public class EmployeeWorkScheduleHandler
    {
        /*MongoDB数据库*/
        /*-------------------------------------------------------------------------------------*/


        /// <summary>
        /// 默认Mongodb集合名
        /// </summary>
        public static string defaulttEmployeeWorkScheduleMongodbCollectionName = Common.ConfigFileHandler.GetAppConfig("EmployeeWorkScheduleCollectionName");

    
        ///<summary>
        ///通过时间及员工查找
        /// </summary>
        public static List<DataModel.EmployeeWorkSchedule> findRecordByIDAndTime(string maxTime,string minTime,string employeeID)
        {
           
            try
            {
                List<DataModel.EmployeeWorkSchedule> employeeWorkSchedules = new List<DataModel.EmployeeWorkSchedule>();
                var collection = Common.MongodbHandler.GetInstance().GetCollection(defaulttEmployeeWorkScheduleMongodbCollectionName);
                var newfilter = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("EmployeeID", employeeID),
                    Builders<BsonDocument>.Filter.Gte("ScheduleDate", minTime),
                    Builders<BsonDocument>.Filter.Lte("ScheduleDate", maxTime)
                   );
                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).ToList();

                foreach (var data in getdocument)
                {
                    //转换成类
                    var EmployeeSchedulingEntity = BsonSerializer.Deserialize<DataModel.EmployeeWorkSchedule>(data);
                    employeeWorkSchedules.Add(EmployeeSchedulingEntity);
                }
                return employeeWorkSchedules;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
    }
}
