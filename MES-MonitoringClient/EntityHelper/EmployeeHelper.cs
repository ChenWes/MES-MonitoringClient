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
    public class EmployeeHelper
    {

        /// <summary>
        /// 通过卡号找到人
        /// </summary>
        /// <param name="cardID"></param>
        /// <returns></returns>
        public static DataModel.Employee QueryEmployeeByEmployeeID(string employeeID)
        {
            try
            {
                var collection = Common.MongodbHandler.GetInstance().GetCollection(Common.ConfigFileHandler.GetAppConfig("EmployeeCollectionName"));
                var newfilter = Builders<BsonDocument>.Filter.Eq("_id", employeeID);
                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).FirstOrDefault();

                if (getdocument != null) return BsonSerializer.Deserialize<DataModel.Employee>(getdocument);

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 通过卡号找到人
        /// </summary>
        /// <param name="cardID"></param>
        /// <returns></returns>
        public static DataModel.Employee QueryEmployeeByCardID(string cardID)
        {
            try
            {               
                var collection = Common.MongodbHandler.GetInstance().GetCollection(Common.ConfigFileHandler.GetAppConfig("EmployeeCollectionName"));
                var newfilter = Builders<BsonDocument>.Filter.Eq("CardID", cardID);
                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).FirstOrDefault();

                if (getdocument != null) return BsonSerializer.Deserialize<DataModel.Employee>(getdocument);

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
