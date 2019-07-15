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
    public class MachineStatusHelper
    {
        /// <summary>
        /// 通过ID数组找到多个机器状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<DataModel.MachineStatus> GetMachineStatusByIDArray(string[] id)
        {
            try
            {
                string jobpositionCollection = Common.ConfigFileHandler.GetAppConfig("MachineStatusCollectionName");

                var collection = Common.MongodbHandler.GetInstance().GetCollection(jobpositionCollection);
                var newfilter = Builders<BsonDocument>.Filter.In("_id", id);


                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).ToList();

                if (getdocument != null)
                {
                    List<DataModel.MachineStatus> machineList = new List<DataModel.MachineStatus>();

                    foreach (var item in getdocument)
                    {
                        machineList.Add(BsonSerializer.Deserialize<DataModel.MachineStatus>(item));
                    }

                    return machineList;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 通过ID找到机器状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DataModel.MachineStatus GetMachineStatusByID(string id)
        {
            try
            {
                string jobpositionCollection = Common.ConfigFileHandler.GetAppConfig("MachineStatusCollectionName");

                var collection = Common.MongodbHandler.GetInstance().GetCollection(jobpositionCollection);
                var newfilter = Builders<BsonDocument>.Filter.Eq("_id", id);

                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).FirstOrDefault();
                if (getdocument != null) return BsonSerializer.Deserialize<DataModel.MachineStatus>(getdocument);

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 获取所有的机器状态
        /// </summary>
        /// <returns></returns>
        public static List<DataModel.MachineStatus> GetAllMachineStatus()
        {
            try
            {
                string jobpositionCollection = Common.ConfigFileHandler.GetAppConfig("MachineStatusCollectionName");

                var collection = Common.MongodbHandler.GetInstance().GetCollection(jobpositionCollection);
                var newfilter = Builders<BsonDocument>.Filter.Exists("_id", true);

                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).ToList();

                if (getdocument != null)
                {
                    List<DataModel.MachineStatus> machineList = new List<DataModel.MachineStatus>();

                    foreach (var item in getdocument)
                    {
                        machineList.Add(BsonSerializer.Deserialize<DataModel.MachineStatus>(item));
                    }

                    return machineList;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 通过Code找到机器状态
        /// </summary>
        /// <param name="machineStatusCode"></param>
        /// <returns></returns>
        public static DataModel.MachineStatus GetMachineStatusByCode(string machineStatusCode)
        {
            try
            {
                string machineStatusCollection = Common.ConfigFileHandler.GetAppConfig("MachineStatusCollectionName");

                var collection = Common.MongodbHandler.GetInstance().GetCollection(machineStatusCollection);
                var newfilter = Builders<BsonDocument>.Filter.Eq("MachineStatusCode", machineStatusCode);

                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).FirstOrDefault();
                if (getdocument != null) return BsonSerializer.Deserialize<DataModel.MachineStatus>(getdocument);

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
