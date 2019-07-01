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
    public class MachineStatusLogHelper
    {

        /// <summary>
        /// 找到最后一次机器状态日志
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DataModel.MachineStatusLog GetLatestMachineStatusLog()
        {
            try
            {
                string machineStatusLogCollection = Common.ConfigFileHandler.GetAppConfig("MachineStatusLogCollectionName");

                var collection = Common.MongodbHandler.GetInstance().GetCollection(machineStatusLogCollection);
                var newfilter = Builders<BsonDocument>.Filter.Eq("IsStopFlag", false);


                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).FirstOrDefault();
                if (getdocument != null) return BsonSerializer.Deserialize<DataModel.MachineStatusLog>(getdocument);

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
