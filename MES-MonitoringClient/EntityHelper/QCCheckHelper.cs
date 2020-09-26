using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_MonitoringClient.EntityHelper
{
    public class QCCheckHelper
    {
        /// <summary>
        /// 获取所有的QC项
        /// </summary>
        /// <returns></returns>
        public static List<DataModel.QCCheck> GetAllQCCheck()
        {
            try
            {
                string jobpositionCollection = Common.ConfigFileHandler.GetAppConfig("QCCheckCollectionName");

                var collection = Common.MongodbHandler.GetInstance().GetCollection(jobpositionCollection);
                var newfilter = Builders<BsonDocument>.Filter.Exists("_id", true);

                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).ToList();

                if (getdocument != null)
                {
                    List<DataModel.QCCheck> qCChecks = new List<DataModel.QCCheck>();

                    foreach (var item in getdocument)
                    {
                        qCChecks.Add(BsonSerializer.Deserialize<DataModel.QCCheck>(item));
                    }

                    return qCChecks;
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
