using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MES_MonitoringClient.Common
{

    public class JobOrderProcessHelper
    {        
        public static string MC_JobOrderCollectionName = Common.ConfigFileHandler.GetAppConfig("JobOrderProcessCollectionName");


        /// <summary>
        /// 通过ID获取工单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DataModel.JobOrder GetJobOrderProcessByID(string id)
        {
            try
            {
                var collection = Common.MongodbHandler.GetInstance().GetCollection(MC_JobOrderCollectionName);
                var newfilter = Builders<BsonDocument>.Filter.Eq("_id", id);
                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).FirstOrDefault();

                if (getdocument != null)
                {
                    DataModel.JobOrder jobOrder= BsonSerializer.Deserialize<DataModel.JobOrder>(getdocument);

                    //获取客户
                    //if (!string.IsNullOrEmpty(jobOrder.CustomerID))
                    //{
                    //    jobOrder.Customer = CustomerHelper.GetCustomerByID(jobOrder.CustomerID);
                    //}

                    ////获取产品
                    //if (!string.IsNullOrEmpty(jobOrder.MaterialID))
                    //{
                    //    jobOrder.Material = MaterialHelper.GetMaterialByID(jobOrder.MaterialID);
                    //}

                    return jobOrder;
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
