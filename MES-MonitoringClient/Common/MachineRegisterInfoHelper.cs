using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

        /// <summary>
        /// 查询本机的机器注册信息
        /// </summary>
        /// <returns></returns>
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
            }
        }

        /// <summary>
        /// 机器码验证
        /// </summary>
        /// <param name="machineID"></param>
        /// <returns></returns>
        public string MachineIDCheck(string machineID)
        {
            try
            {
                string l_machineRegisterUrlPath = Common.ConfigFileHandler.GetAppConfig("MachineRegisterCheckUrlPath");

                //准备发送Http Post请求,顺带参数
                Common.HttpHelper httpHelperClass = new HttpHelper();
                FormUrlEncodedContent bodyData = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "id", machineID},                    
                });

                return httpHelperClass.HttpPost(l_machineRegisterUrlPath, bodyData);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 机器注册
        /// </summary>
        /// <param name="machineID"></param>
        /// <returns></returns>
        public string MachineRegister(string machineID)
        {
            try
            {
                string l_machineRegisterUrlPath = Common.ConfigFileHandler.GetAppConfig("MachineRegisterUrlPath");

                //准备发送Http Post请求,顺带参数
                Common.HttpHelper httpHelperClass = new HttpHelper();
                FormUrlEncodedContent bodyData = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "id", machineID},
                    { "MACAddress", Common.CommonFunction.getMacAddress()},
                    { "IPAddress",Common.CommonFunction.getIPAddress()}
                });

                return httpHelperClass.HttpPost(l_machineRegisterUrlPath, bodyData);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
