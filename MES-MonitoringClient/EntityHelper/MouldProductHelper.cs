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
    public class MouldProductHelper
    {
        /// <summary>
        /// 根据模具编号查询出模具对应产品出数资料
        /// </summary>
        /// <param name="MouldCode"></param>
        /// <returns></returns>
        public static DataModel.MouldProduct GetMmouldProductByMouldCode(string MouldCode)
        {
            try
            {
                string mouldProductCollection = Common.ConfigFileHandler.GetAppConfig("MouldProductCollectionName");

                var collection = Common.MongodbHandler.GetInstance().GetCollection(mouldProductCollection);
                var newfilter = Builders<BsonDocument>.Filter.Eq("MouldCode", MouldCode);
                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).FirstOrDefault();

                if (getdocument != null)
                {
                    DataModel.MouldProduct mouldProduct = BsonSerializer.Deserialize<DataModel.MouldProduct>(getdocument);                    

                    return mouldProduct;
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
