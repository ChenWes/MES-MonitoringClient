using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;

namespace MES_MonitoringService.Common
{
    class MachineRegisterInfoHelper
    {
        private IMongoCollection<Model.MachineInfo> machineRegisterCollection;
        private static string defaultMachineRegisterMongodbCollectionName = Common.ConfigFileHandler.GetAppConfig("MachineRegisterCollectionName");


        public MachineRegisterInfoHelper()
        {
            machineRegisterCollection = Common.MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<Model.MachineInfo>(defaultMachineRegisterMongodbCollectionName);
        }

        /// <summary>
        /// 查询本机的机器注册信息
        /// </summary>
        /// <returns></returns>
        public Model.MachineInfo GetMachineRegisterInfo()
        {
            try
            {
                var collection = Common.MongodbHandler.GetInstance().GetCollection(defaultMachineRegisterMongodbCollectionName);

                var newfilter = Builders<BsonDocument>.Filter.Exists("MachineID", true);
                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).ToList();

                if (getdocument != null && getdocument.Count > 0)
                {
                    var machineRegisterEntity = BsonSerializer.Deserialize<Model.MachineInfo>(getdocument.First());
                    return machineRegisterEntity;
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
