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
    public class DefectiveTypeHelper
    {
        /// <summary>
        /// 获取所有的QC项
        /// </summary>
        /// <returns></returns>
        public static List<DataModel.DefectiveType> GetAllQCCheck()
        {
            try
            {
                string DefectiveTypeCollection = Common.ConfigFileHandler.GetAppConfig("DefectiveTypeCollectionName");

                var collection = Common.MongodbHandler.GetInstance().GetCollection(DefectiveTypeCollection);
                var newfilter = Builders<BsonDocument>.Filter.Exists("_id", true);

                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).ToList();

                if (getdocument != null)
                {
                    List<DataModel.DefectiveType> qCChecks = new List<DataModel.DefectiveType>();

                    foreach (var item in getdocument)
                    {
                        qCChecks.Add(BsonSerializer.Deserialize<DataModel.DefectiveType>(item));
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
