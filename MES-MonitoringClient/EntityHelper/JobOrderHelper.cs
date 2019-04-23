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

    public class JobOrderHelper
    {
        public static string MC_CustomerCollectionName = Common.ConfigFileHandler.GetAppConfig("CustomerCollectionName");
        public static string MC_MaterialCollectionName = Common.ConfigFileHandler.GetAppConfig("MaterialCollectionName");
        public static string MC_MouldCollectionName = Common.ConfigFileHandler.GetAppConfig("MouldCollectionName");
        public static string MC_JobOrderCollectionName = Common.ConfigFileHandler.GetAppConfig("JobOrderCollectionName");
        public static string MC_JobOrderProcessCollectionName = Common.ConfigFileHandler.GetAppConfig("JobOrderProcessCollectionName");


        /// <summary>
        /// 通过ID获取工单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DataModel.JobOrder GetJobOrderByID(string id)
        {
            try
            {
                //string jobOrderCollection = Common.ConfigFileHandler.GetAppConfig("JobOrderCollectionName");

                var collection = Common.MongodbHandler.GetInstance().GetCollection(MC_JobOrderCollectionName);
                var newfilter = Builders<BsonDocument>.Filter.Eq("_id", id);
                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).FirstOrDefault();

                if (getdocument != null) return BsonSerializer.Deserialize<DataModel.JobOrder>(getdocument);

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static object GetAllJobOrder()
        {
            try
            {
                var collection = Common.MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.JobOrder>(MC_JobOrderCollectionName);
                var getdocument = collection.Find(x => x._id != null).ToList();

                return getdocument;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 获取所有未开始的订单
        /// </summary>
        /// <returns></returns>
        public static object GetJobOrderByNoStart()
        {
            try
            {
                var customerCollection = Common.MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.Customer>(MC_CustomerCollectionName);
                var materialCollection = Common.MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.Material>(MC_MaterialCollectionName);
                var mouldCollection = Common.MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.Mould>(MC_MouldCollectionName);
                var jobOrderCollection = Common.MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.JobOrder>(MC_JobOrderCollectionName);
                var jobOrderProcessCollection = Common.MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.JobOrderProcess>(MC_JobOrderProcessCollectionName);


                //一种写法，会出错，不能绑定，估计是BsonDocument不能直接转成Class进行绑定
                //var collection = Common.MongodbHandler.GetInstance().GetCollection(Common.ConfigFileHandler.GetAppConfig("JobOrderCollectionName"));
                //var newfilter = Builders<BsonDocument>.Filter.Eq("Status", BsonNull.Value);
                //var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).ToList();

                //一种写法，找不到数据                
                //var filter = Builders<DataModel.JobOrder>.Filter.Eq("Status", BsonNull.Value);
                //var getdocument = jobOrderCollection.Find(filter).ToList();


                //一种写法，只匹配到其中一个
                //var getdocument = jobOrderCollection.Find(x => x.Status == null).ToList();    


                var jobFun = (from jop in jobOrderProcessCollection.AsQueryable() select jop.JobOrderID).ToArray();

                //一种写法，暂时未关联到客户外部信息
                var getdocument = (from jo in jobOrderCollection.AsQueryable()
                                   join cu in customerCollection.AsQueryable() on jo.CustomerID equals cu._id
                                   join ma in materialCollection.AsQueryable() on jo.MaterialID equals ma._id
                                   join mo in mouldCollection.AsQueryable() on jo.MouldID equals mo._id
                                   where !(jobFun).Contains(jo._id)
                                   select new
                                   {
                                       ID = jo._id,

                                       JobOrderCode = jo.JobOrderCode,
                                       JobOrderName = jo.JobOrderName,
                                       OrderCount = jo.OrderCount,
                                       JobOrderDesc = jo.JobOrderDesc,
                                       Status = "未开始",

                                       //客户
                                       CustomerCode = cu.CustomerCode,
                                       CustomerName = cu.CustomerName,

                                       //产品
                                       MaterialCode = ma.MaterialCode,
                                       MaterialName = ma.MaterialName,                                       
                                       MaterialSpecification = ma.MaterialSpecification,

                                       //模具
                                       MouldCode = mo.MouldCode,
                                       MouldName = mo.MouldName,                                       
                                       MouldSpecification = mo.MouldSpecification,
                                       StandardProduceSecond = mo.StandardProduceSecond,

                                   }
                                ).ToList();


                return getdocument;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 获取所有未完成的工单
        /// </summary>
        /// <returns></returns>
        public static object GetJobOrderByNoCompleted()
        {
            try
            {
                var customerCollection = Common.MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.Customer>(MC_CustomerCollectionName);
                var materialCollection = Common.MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.Material>(MC_MaterialCollectionName);
                var mouldCollection = Common.MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.Mould>(MC_MouldCollectionName);
                var jobOrderCollection = Common.MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.JobOrder>(MC_JobOrderCollectionName);
                var jobOrderProcessCollection = Common.MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.JobOrderProcess>(MC_JobOrderProcessCollectionName);


                //var jobFun = from jop in jobOrderProcessCollection.AsQueryable() select jop.JobOrderID;

                //一种写法，暂时未关联到客户外部信息
                var getdocument = (from jo in jobOrderCollection.AsQueryable()
                                   join cu in customerCollection.AsQueryable() on jo.CustomerID equals cu._id
                                   join ma in materialCollection.AsQueryable() on jo.MaterialID equals ma._id
                                   join mo in mouldCollection.AsQueryable() on jo.MouldID equals mo._id
                                   join jop in jobOrderProcessCollection.AsQueryable() on jo._id equals jop.JobOrderID
                                   select new
                                   {
                                       ID = jo._id,

                                       JobOrderCode = jo.JobOrderCode,
                                       JobOrderName = jo.JobOrderName,
                                       OrderCount = jo.OrderCount,
                                       JobOrderDesc = jo.JobOrderDesc,
                                       Status =
                                       (
                                            jop.Status == "Process" ? "生产中" :
                                            jop.Status == "Stop" ? "暂停" : "未知状态"
                                        ),

                                       //客户
                                       CustomerCode = cu.CustomerCode,
                                       CustomerName = cu.CustomerName,

                                       //产品
                                       MaterialCode = ma.MaterialCode,
                                       MaterialName = ma.MaterialName,                                       
                                       MaterialSpecification = ma.MaterialSpecification,

                                       //模具
                                       MouldCode = mo.MouldCode,
                                       MouldName = mo.MouldName,                                       
                                       MouldSpecification = mo.MouldSpecification,
                                       StandardProduceSecond = mo.StandardProduceSecond,

                                   }
                                ).ToList();


                return getdocument;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
