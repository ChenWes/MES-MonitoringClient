using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;
using System.Diagnostics;

using MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections;

namespace MES_MonitoringClient.Common
{
    /// <summary>
    /// 最后操作工单类
    /// </summary>
    public class LastJobOrderHandler
    {
        /*MongoDB数据库*/
        /*-------------------------------------------------------------------------------------*/
        /// <summary>
        ///  最后操作工单Mongodb集合
        /// </summary>
        private static IMongoCollection<DataModel.LastJobOrder> LastJobOrderCollection;

        /// <summary>
        /// 最后操作工单默认Mongodb集合名
        /// </summary>
        private static string defaultLastJobOrderMongodbCollectionName = "LastJobOrder";

        /// <summary>
        /// 构造函数，处理初始化的参数
        /// </summary>
        public LastJobOrderHandler()
        {
            LastJobOrderCollection = MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.LastJobOrder>(defaultLastJobOrderMongodbCollectionName);
        }


        /// <summary>
        /// 最后操作工单保存
        /// </summary>
        /// <param name="LastJobOrde">最后操作工单保存</param>
        public void SaveLastJobOrder(DataModel.LastJobOrder LastJobOrder)
        {
            LastJobOrderCollection.InsertOne(LastJobOrder);
            
        }
        /// <summary>
        /// 最后操作工单删除
        /// </summary>
        /// <param name="DeleteLastJobOrder">最后操作工单删除</param>
        public void DeleteLastJobOrder()
        {
            var filterID = "{ }";
            LastJobOrderCollection.DeleteMany(filterID);

        }

        public List<DataModel.JobOrder> GetJLastJobOrderList()
        {
           
            List<DataModel.LastJobOrder> lastJobOrders=LastJobOrderCollection.AsQueryable().ToList();
            if (lastJobOrders.Count > 0)
            {
                string[] list = new string[lastJobOrders.Count];
                for (int i = 0; i < lastJobOrders.Count; i++)
                {
                    list[i] = lastJobOrders[i].JobOrderID;
                }

                var filterID = Builders<DataModel.JobOrder>.Filter.In("JobOrderID", list);



                return MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.JobOrder>("JobOrder").Find(filterID).ToList();
            }
            else
            {
                return null;
            }
            

           // machineProduceStatusHandler.CurrentProcessJobOrder = MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.JobOrder>("JobOrder").AsQueryable().ToList()[0];
           // machineProduceStatusHandler.CurrentMouldProduct = Common.MouldProductHelper.GetMmouldProductByMouldCode(MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.JobOrder>("JobOrder").Find(filterID).ToList()[0].MouldCode);


        }
    }
}
