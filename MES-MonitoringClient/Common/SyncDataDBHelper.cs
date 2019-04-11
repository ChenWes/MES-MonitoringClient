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

namespace MES_MonitoringClient.Common
{
    public class SyncDataDBHelper<T> where T : DataModel.SyncData
    {

        private IMongoCollection<T> operationCollection;

        public string getJsonStringFromUrl(T newclass)
        {
            try
            {
                string getUrl = newclass.getCollectionDataUrl();

                return Common.HttpHelper.HttpGetWithToken(getUrl);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 同步处理数据方法
        /// </summary>
        /// <param name="jsonString">传入JSON</param>
        /// <returns></returns>
        public bool SyncData_Process(string jsonString)
        {
            try
            {
                //反序列化
                List<T> dataEntityList = JsonConvert.DeserializeObject<List<T>>(jsonString);

                if (dataEntityList != null && dataEntityList.Count > 0)
                {
                    //删除数据集合
                    Common.MongodbHandler.GetInstance().mc_MongoDatabase.DropCollection(dataEntityList.FirstOrDefault().getCollectionName());
                    //获取数据集合
                    operationCollection = Common.MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<T>(dataEntityList.FirstOrDefault().getCollectionName());
                    //批量插入数据
                    operationCollection.InsertMany(dataEntityList);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
