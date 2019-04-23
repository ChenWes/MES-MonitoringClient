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
    public class JobPositionHelper
    {
        public static DataModel.JobPositon GetJobPositon(string id)
        {
            try
            {
                string jobpositionCollection = Common.ConfigFileHandler.GetAppConfig("JobPositionCollectionName");

                var collection = Common.MongodbHandler.GetInstance().GetCollection(jobpositionCollection);
                var newfilter = Builders<BsonDocument>.Filter.Eq("_id", id);
                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).FirstOrDefault();

                if (getdocument != null) return BsonSerializer.Deserialize<DataModel.JobPositon>(getdocument);

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
