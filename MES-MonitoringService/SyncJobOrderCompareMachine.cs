using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;

namespace MES_MonitoringService
{
    /// <summary>
    /// 工单匹配本地机器
    /// </summary>
    public static class SyncJobOrderCompareMachine
    {
        /// <summary>
        /// 工单匹配本地机器
        /// </summary>
        /// <param name="dataJson"></param>
        /// <returns></returns>
        public static bool MatchLocalMachine(string dataJson)
        {
            try
            {
                //本地机器
                Model.MachineInfo machineInfo = new Common.MachineRegisterInfoHelper().GetMachineRegisterInfo();
                //反序列化为类实体
                BsonDocument bsonElements = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(dataJson);

                //是否匹配本地机器
                return machineInfo.MachineID == bsonElements.GetValue("MachineID").ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
