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
    public class MaterialHelper
    {
        public static DataModel.Material GetMaterialByID(string id)
        {
            try
            {
                string materialCollection = Common.ConfigFileHandler.GetAppConfig("MaterialCollectionName");

                var collection = Common.MongodbHandler.GetInstance().GetCollection(materialCollection);
                var newfilter = Builders<BsonDocument>.Filter.Eq("_id", id);
                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).FirstOrDefault();

                if (getdocument != null)
                {
                    DataModel.Material material = BsonSerializer.Deserialize<DataModel.Material>(getdocument);

                    //模具
                    if (!string.IsNullOrEmpty(material.MouldID))
                    {
                        material.Mould = MouldHelper.GetMouldByID(material.MouldID);
                    }

                    return material;
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
