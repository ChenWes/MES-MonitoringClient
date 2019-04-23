using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

using Newtonsoft.Json;

namespace MES_MonitoringClient.Common
{
    public static class MachineHelper
    {
        public static DataModel.Machine GetMachineByID(string id)
        {
            try
            {               
                var collection = Common.MongodbHandler.GetInstance().GetCollection(Common.ConfigFileHandler.GetAppConfig("MachineCollectionName"));
                var newfilter = Builders<BsonDocument>.Filter.Eq("_id", id);
                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).FirstOrDefault();

                //BsonSerializer.Deserialize<DataModel.Machine>(getdocument) 使用Bson转换，会要求Class拥有所有属性字段
                //JsonConvert.DeserializeObject<DataModel.Machine>(getdocument.ToJson())，使用Json转换，不会要求Class拥有所有属性字段
                if (getdocument != null) return JsonConvert.DeserializeObject<DataModel.Machine>(getdocument.ToJson());
                // BsonSerializer.Deserialize<DataModel.Machine>(getdocument);

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
