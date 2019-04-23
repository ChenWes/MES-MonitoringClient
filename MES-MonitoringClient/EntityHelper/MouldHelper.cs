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
    public class MouldHelper
    {
        public static DataModel.Mould GetMouldByID(string id)
        {
            try
            {
                string mouldCollection = Common.ConfigFileHandler.GetAppConfig("MouldCollectionName");

                var collection = Common.MongodbHandler.GetInstance().GetCollection(mouldCollection);
                var newfilter = Builders<BsonDocument>.Filter.Eq("_id", id);
                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).FirstOrDefault();

                if (getdocument != null) return BsonSerializer.Deserialize<DataModel.Mould>(getdocument);

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
