using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

using Newtonsoft;
using Newtonsoft.Json;

namespace MES_MonitoringClient.Common
{

    public class JobOrderHelper
    {
        public static string MC_CustomerCollectionName = Common.ConfigFileHandler.GetAppConfig("CustomerCollectionName");
        public static string MC_MaterialCollectionName = Common.ConfigFileHandler.GetAppConfig("MaterialCollectionName");
        public static string MC_MouldCollectionName = Common.ConfigFileHandler.GetAppConfig("MouldCollectionName");
        public static string MC_JobOrderCollectionName = Common.ConfigFileHandler.GetAppConfig("JobOrderCollectionName");


        /// <summary>
        /// 通过ID获取工单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DataModel.JobOrder GetJobOrderByID(string id)
        {
            try
            {
                var collection = Common.MongodbHandler.GetInstance().GetCollection(MC_JobOrderCollectionName);
                var newfilter = Builders<BsonDocument>.Filter.Eq("_id", id);
                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).FirstOrDefault();

                if (getdocument != null)
                {
                    DataModel.JobOrder jobOrder= BsonSerializer.Deserialize<DataModel.JobOrder>(getdocument);

                    ////获取客户
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

        /// <summary>
        /// 将类保存为文档
        /// </summary>
        /// <param name="jobOrder"></param>
        /// <returns></returns>
        public static DataModel.JobOrder UpdateJobOrder(DataModel.JobOrder jobOrder, bool GetLatestObject)
        {
            try
            {
                //查找ID
                var collection = Common.MongodbHandler.GetInstance().GetCollection(MC_JobOrderCollectionName);
                var filterid = Builders<BsonDocument>.Filter.Eq("_id", jobOrder._id);

                BsonDocument updatedocument = jobOrder.ToBsonDocument();
                updatedocument.Remove("_id");
                updatedocument.Set("IsSyncToServer", false);

                BsonDocument updateResult = collection.FindOneAndUpdate(filterid, updatedocument);


                if (updateResult != null && GetLatestObject == true)
                {
                    return GetJobOrderByID(jobOrder._id);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw;
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
        /// 获取所有分配（未开始）或正在生产的工单
        /// [Assigned][Producing]
        /// </summary>
        /// <returns></returns>
        public static object GetJobOrderByAssigned()
        {
            try
            {
                //var customerCollection = Common.MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.Customer>(MC_CustomerCollectionName);
                //var materialCollection = Common.MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.Material>(MC_MaterialCollectionName);
                //var mouldCollection = Common.MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.Mould>(MC_MouldCollectionName);
                var jobOrderCollection = Common.MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.JobOrder>(MC_JobOrderCollectionName);


                //一种写法，暂时未关联到客户外部信息
                var getdocument = (from jo in jobOrderCollection.AsQueryable()
                                       //join cu in customerCollection.AsQueryable() on jo.CustomerID equals cu._id
                                       //join ma in materialCollection.AsQueryable() on jo.MaterialID equals ma._id
                                       //join mo in mouldCollection.AsQueryable() on ma.MouldID equals mo._id
                                   where jo.Status == Common.JobOrderStatus.eumJobOrderStatus.Assigned.ToString() || jo.Status == Common.JobOrderStatus.eumJobOrderStatus.Producing.ToString()
                                   orderby jo.Sort, jo.DeliveryDate
                                   select new
                                   {
                                       ID = jo._id,

                                       JobOrderID = jo.JobOrderID,
                                       JobOrderNumber = jo.JobOrderNumber,
                                       ProductCode = jo.ProductCode,
                                       ProductCategory = jo.ProductCategory,
                                       OrderCount = jo.OrderCount,
                                       MaterialCode = jo.MaterialCode,
                                       DeliveryDate = jo.DeliveryDate,
                                       MachineTonnage = jo.MachineTonnage,
                                       MouldID = jo.MouldCode,
                                       MouldStandardProduceSecond = jo.MouldStandardProduceSecond,

                                       Status = (jo.Status == Common.JobOrderStatus.eumJobOrderStatus.Assigned.ToString() ? "未开始" : (jo.Status == Common.JobOrderStatus.eumJobOrderStatus.Producing.ToString() ? "生产中" : "未知")),                                       
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
        /// 获取所有暂停（已生产但暂停）的工单
        /// </summary>
        /// <returns></returns>
        public static object GetJobOrderBySuspend()
        {
            try
            {
                var jobOrderCollection = Common.MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.JobOrder>(MC_JobOrderCollectionName);


                //一种写法，暂时未关联到客户外部信息
                var getdocument = (from jo in jobOrderCollection.AsQueryable()
                                   where jo.Status == Common.JobOrderStatus.eumJobOrderStatus.Suspend.ToString()
                                   orderby jo.Sort, jo.DeliveryDate
                                   select new
                                   {
                                       ID = jo._id,

                                       JobOrderID = jo.JobOrderID,
                                       JobOrderNumber = jo.JobOrderNumber,
                                       ProductCode = jo.ProductCode,
                                       ProductCategory = jo.ProductCategory,
                                       OrderCount = jo.OrderCount,
                                       MaterialCode = jo.MaterialCode,
                                       DeliveryDate = jo.DeliveryDate,
                                       MachineTonnage = jo.MachineTonnage,
                                       MouldID = jo.MouldCode,
                                       MouldStandardProduceSecond = jo.MouldStandardProduceSecond,

                                       Status = (jo.Status == Common.JobOrderStatus.eumJobOrderStatus.Suspend.ToString() ? "暂停中" : "未知"),                                       
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
        /// 处理工单的生产记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool SettingMachineOperation(string id)
        {
            try
            {
                //找到机器信息
                Common.MachineRegisterInfoHelper machineRegisterInfoHelper = new MachineRegisterInfoHelper();
                DataModel.Machine machineInfo = machineRegisterInfoHelper.GetMachineRegisterInfo();


                //查找ID
                var collection = Common.MongodbHandler.GetInstance().GetCollection(MC_JobOrderCollectionName);
                var filterid = Builders<BsonDocument>.Filter.Eq("_id", id);
                BsonDocument jobOrderdocument = Common.MongodbHandler.GetInstance().Find(collection, filterid).FirstOrDefault();


                //找到机器
                BsonElement machineProcessLogElements = jobOrderdocument.GetElement("MachineProcessLog");
                if (machineProcessLogElements != null)
                {

                    if (machineProcessLogElements.Value.AsBsonArray.Count == 0)
                    {
                        //从未处理过
                        jobOrderdocument.Set("MachineProcessLog",
                            new BsonArray() {
                                new BsonDocument{
                                    { "MachineID", machineInfo._id },
                                    { "ProduceCount",0 },
                                    {"ErrorCount",0 },
                                    { "ProduceStartDate",System.DateTime.Now },
                                    { "ProduceEndDate",System.DateTime.Now },
                                    { "EmployeeID",BsonNull.Value},
                                }
                            }
                        );
                    }
                    else
                    {
                        //已经有机器处理过，本机处理过则不需要处理
                        bool machineHavaData = false;
                        foreach (BsonValue MachineProcessLogItem in machineProcessLogElements.Value.AsBsonArray)
                        {
                            BsonDocument bsonElements = (BsonDocument)MachineProcessLogItem;
                            if (bsonElements.GetElement("MachineID").Value == machineInfo._id) machineHavaData = true;
                        }

                        if (!machineHavaData)
                        {
                            //但本机没有处理过
                            machineProcessLogElements.Value.AsBsonArray.Add(
                                new BsonDocument{
                                    { "MachineID", machineInfo._id },
                                    { "ProduceCount",0 },
                                    {"ErrorCount",0 },
                                    { "ProduceStartDate",System.DateTime.Now },
                                    { "ProduceEndDate",System.DateTime.Now },
                                    { "EmployeeID",BsonNull.Value},
                                }
                            );
                        }
                    }
                }

                //未更新，且状态更新为工作中
                jobOrderdocument.Set("IsSyncToServer", false)
                    .Set("Status", "Working");


                //更新
                BsonDocument updateResult = collection.FindOneAndUpdate(filterid, jobOrderdocument);

                if (updateResult != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Common.LogHandler.WriteLog("处理工单出错，原因是：", ex);
                return false;
            }
        }

        public static bool SettingMachineProductCount(string id, int productCount, int errorCount)
        {
            try
            {
                //找到机器信息
                Common.MachineRegisterInfoHelper machineRegisterInfoHelper = new MachineRegisterInfoHelper();
                DataModel.Machine machineInfo = machineRegisterInfoHelper.GetMachineRegisterInfo();


                //查找ID
                var collection = Common.MongodbHandler.GetInstance().GetCollection(MC_JobOrderCollectionName);
                var filterid = Builders<BsonDocument>.Filter.Eq("_id", id);
                BsonDocument jobOrderdocument = Common.MongodbHandler.GetInstance().Find(collection, filterid).FirstOrDefault();


                //找到机器
                BsonElement machineProcessLogElements = jobOrderdocument.GetElement("MachineProcessLog");
                if (machineProcessLogElements != null)
                {
                    if (machineProcessLogElements.Value.AsBsonArray.Count == 0)
                    {
                        //从未处理过
                        jobOrderdocument.Set("MachineProcessLog",
                            new BsonArray() {
                                new BsonDocument{
                                    { "MachineID", machineInfo._id },
                                    { "ProduceCount",productCount },
                                    { "ErrorCount",errorCount },
                                    { "ProduceStartDate",System.DateTime.Now },
                                    { "ProduceEndDate",System.DateTime.Now },
                                    { "EmployeeID",BsonNull.Value},
                                }
                            }
                        );
                    }
                    else
                    {
                        //已经有机器处理过，本机处理过则不需要处理
                        bool machineHavaData = false;
                        int getindex = 0;
                        foreach (BsonValue MachineProcessLogItem in machineProcessLogElements.Value.AsBsonArray)
                        {
                            BsonDocument bsonElements = (BsonDocument)MachineProcessLogItem;
                            if (bsonElements.GetElement("MachineID").Value == machineInfo._id)
                            {
                                machineHavaData = true;
                            }

                            getindex++;
                        }

                        if (!machineHavaData)
                        {
                            //但本机没有处理过
                            machineProcessLogElements.Value.AsBsonArray.Add(
                                new BsonDocument{
                                    { "MachineID", machineInfo._id },
                                    { "ProduceCount",productCount },
                                    { "ErrorCount",errorCount },
                                    { "ProduceStartDate",System.DateTime.Now },
                                    { "ProduceEndDate",System.DateTime.Now },
                                    { "EmployeeID",BsonNull.Value},
                                }
                            );
                        }
                        else
                        {
                            machineProcessLogElements.Value.AsBsonArray[getindex] = new BsonDocument{
                                    { "MachineID", machineInfo._id },
                                    { "ProduceCount",productCount },
                                    { "ErrorCount",errorCount },
                                    { "ProduceStartDate",System.DateTime.Now },
                                    { "ProduceEndDate",System.DateTime.Now },
                                    { "EmployeeID",BsonNull.Value},
                                };
                        }
                    }
                }


                //更新
                BsonDocument updateResult = collection.FindOneAndUpdate(filterid, jobOrderdocument);

                if (updateResult != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Common.LogHandler.WriteLog("处理工单生产数量出错，原因是：", ex);
                return false;
            }
        }

        public static bool UpgradeMachineProductCount(string id, int productCount, int errorCount)
        {
            try
            {
                //找到机器信息
                Common.MachineRegisterInfoHelper machineRegisterInfoHelper = new MachineRegisterInfoHelper();
                DataModel.Machine machineInfo = machineRegisterInfoHelper.GetMachineRegisterInfo();


                //查找ID
                var collection = Common.MongodbHandler.GetInstance().GetCollection(MC_JobOrderCollectionName);
                var filterid = Builders<BsonDocument>.Filter.Eq("_id", id);
                BsonDocument jobOrderdocument = Common.MongodbHandler.GetInstance().Find(collection, filterid).FirstOrDefault();

               

                //找到机器
                BsonElement machineProcessLogElements = jobOrderdocument.GetElement("MachineProcessLog");
                if (machineProcessLogElements != null)
                {
                    if (machineProcessLogElements.Value.AsBsonArray.Count == 0)
                    {
                        //从未处理过
                        jobOrderdocument.Set("MachineProcessLog",
                            new BsonArray() {
                                new BsonDocument{
                                    { "MachineID", machineInfo._id },
                                    { "ProduceCount",1 },
                                    { "ErrorCount",0 },
                                    { "ProduceStartDate",System.DateTime.Now },
                                    { "ProduceEndDate",System.DateTime.Now },
                                    { "EmployeeID",BsonNull.Value},
                                }
                            }
                        );
                    }
                    else
                    {
                        //已经有机器处理过，本机处理过则不需要处理
                        bool machineHavaData = false;
                        int getindex = 0;
                        foreach (BsonValue MachineProcessLogItem in machineProcessLogElements.Value.AsBsonArray)
                        {
                            BsonDocument bsonElements = (BsonDocument)MachineProcessLogItem;
                            if (bsonElements.GetElement("MachineID").Value == machineInfo._id)
                            {
                                machineHavaData = true;
                            }

                            getindex++;
                        }

                        if (!machineHavaData)
                        {
                            //但本机没有处理过
                            machineProcessLogElements.Value.AsBsonArray.Add(
                                new BsonDocument{
                                    { "MachineID", machineInfo._id },
                                    { "ProduceCount",productCount },
                                    { "ErrorCount",errorCount },
                                    { "ProduceStartDate",System.DateTime.Now },
                                    { "ProduceEndDate",System.DateTime.Now },
                                    { "EmployeeID",BsonNull.Value},
                                }
                            );
                        }
                        else
                        {
                            machineProcessLogElements.Value.AsBsonArray[getindex] = new BsonDocument{
                                    { "MachineID", machineInfo._id },
                                    { "ProduceCount",productCount },
                                    { "ErrorCount",errorCount },
                                    { "ProduceStartDate",System.DateTime.Now },
                                    { "ProduceEndDate",System.DateTime.Now },
                                    { "EmployeeID",BsonNull.Value},
                                };
                        }
                    }
                }

                //更新
                BsonDocument updateResult = collection.FindOneAndUpdate(filterid, jobOrderdocument);

                if (updateResult != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Common.LogHandler.WriteLog("处理工单生产数量出错，原因是：", ex);
                return false;
            }
        }
    }
}
