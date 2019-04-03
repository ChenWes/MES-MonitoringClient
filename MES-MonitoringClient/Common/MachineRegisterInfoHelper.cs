using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;

namespace MES_MonitoringClient.Common
{
    public class MachineRegisterInfoHelper
    {
        private IMongoCollection<DataModel.MachineInfo> machineRegisterCollection;
        private static string defaultMachineRegisterMongodbCollectionName = Common.ConfigFileHandler.GetAppConfig("MachineRegisterCollectionName");



        public MachineRegisterInfoHelper()
        {
            machineRegisterCollection = Common.MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.MachineInfo>(defaultMachineRegisterMongodbCollectionName);
        }

        public DataModel.MachineInfo GetMachineRegisterInfo()
        {
            try
            {
                var collection = Common.MongodbHandler.GetInstance().GetCollection(defaultMachineRegisterMongodbCollectionName);

                var newfilter = Builders<BsonDocument>.Filter.Exists("MachineID", true);
                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).ToList();

                if (getdocument != null && getdocument.Count > 0)
                {
                    var machineRegisterEntity = BsonSerializer.Deserialize<DataModel.MachineInfo>(getdocument.First());
                    return machineRegisterEntity;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
                return null;
            }
        }
    }
}
