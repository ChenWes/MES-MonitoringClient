using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_MonitoringClient.Common
{
    public  class MachineProductionHandler
    {
        /*MongoDB数据库*/
        /*-------------------------------------------------------------------------------------*/


        /// <summary>
        /// 默认Mongodb集合名
        /// </summary>
        public static string defaulttMachineProductionMongodbCollectionName = Common.ConfigFileHandler.GetAppConfig("MachineProductionCollectionName");

        /// <summary>
        ///新增记录保存
        /// </summary>
        /// <param name="clockInRecord">新增记录保存</param>
        public  void SaveMachineProduction(DataModel.MachineProduction machineProduction)
        {
            MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.MachineProduction>(defaulttMachineProductionMongodbCollectionName).InsertOne(machineProduction);

        }

        ///<summary>
        ///通过工单及停止状态查找
        /// </summary>
        public  List<DataModel.MachineProduction> findRecordByID(string jobOrderID)
        {

            try
            {
                List<DataModel.MachineProduction> machineProductions = new List<DataModel.MachineProduction>();
                var collection = Common.MongodbHandler.GetInstance().GetCollection(defaulttMachineProductionMongodbCollectionName);
                var newfilter = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("JobOrderID", jobOrderID),
                    Builders<BsonDocument>.Filter.Eq("IsStopFlag", false)
                   );
                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).ToList();
                getdocument.Reverse();
                foreach (var data in getdocument)
                {
                    //转换成类
                    var MachineProductionEntity = BsonSerializer.Deserialize<DataModel.MachineProduction>(data);
                    machineProductions.Add(MachineProductionEntity);
                }
                return machineProductions;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        ///<summary>
        ///通过工单查找记录
        /// </summary>
        public List<DataModel.MachineProduction> findNewRecordByID(string jobOrderID)
        {

            try
            {
                List<DataModel.MachineProduction> machineProductions = new List<DataModel.MachineProduction>();
                var collection = Common.MongodbHandler.GetInstance().GetCollection(defaulttMachineProductionMongodbCollectionName);
                var newfilter = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("JobOrderID", jobOrderID)
                   );
                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).ToList();
                getdocument.Reverse();
                foreach (var data in getdocument)
                {
                    //转换成类
                    var MachineProductionEntity = BsonSerializer.Deserialize<DataModel.MachineProduction>(data);
                    machineProductions.Add(MachineProductionEntity);
                }
                return machineProductions;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        ///<summary>
        ///开始记录
        ///</summary>
        public void StartMachineProduction(DataModel.MachineProduction machineProduction,int count)
        {
            try
            {
                //查找ID
                var collection = Common.MongodbHandler.GetInstance().GetCollection(defaulttMachineProductionMongodbCollectionName);
                var filterid = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(machineProduction._id));
                BsonDocument updatedocument = machineProduction.ToBsonDocument();
                updatedocument.Remove("_id");
                updatedocument.Set("ProduceCount", count);
                updatedocument.Set("IsStopFlag", false);
                updatedocument.Set("IsSyncToServer", false);

                BsonDocument updateResult = collection.FindOneAndUpdate(filterid, updatedocument);

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        ///<summary>
        ///停止记录
        ///</summary>
        public  void StopMachineProduction(DataModel.MachineProduction machineProduction)
        {
            try
            {
                //查找ID
                var collection = Common.MongodbHandler.GetInstance().GetCollection(defaulttMachineProductionMongodbCollectionName);
                var filterid = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(machineProduction._id));
                BsonDocument updatedocument = machineProduction.ToBsonDocument();
                updatedocument.Remove("_id");
                updatedocument.Set("IsStopFlag", true);
                updatedocument.Set("IsSyncToServer", false);

                BsonDocument updateResult = collection.FindOneAndUpdate(filterid, updatedocument);
                
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        ///<summary>
        ///更新员工记录
        ///</summary>
        public BsonDocument AddEmployee(DataModel.MachineProduction machineProduction, BsonDocument oldMachineProduction)
        {
            try
            {
                //查找ID
                var collection = Common.MongodbHandler.GetInstance().GetCollection(defaulttMachineProductionMongodbCollectionName);
                
                BsonDocument updatedocument = machineProduction.ToBsonDocument();
                updatedocument.Remove("_id");
                updatedocument.Set("IsSyncToServer", false);
                var filterID = Builders<BsonDocument>.Filter.And(oldMachineProduction);
                BsonDocument updateResult = collection.FindOneAndUpdate(filterID, updatedocument);
                return updateResult;

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        ///<summary>
        ///更新不良品
        ///</summary>
        public void UpdateError(DataModel.MachineProduction machineProduction)
        {
            try
            {
                //查找ID
                var collection = Common.MongodbHandler.GetInstance().GetCollection(defaulttMachineProductionMongodbCollectionName);
                var filterid = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(machineProduction._id));
                BsonDocument updatedocument = machineProduction.ToBsonDocument();
                updatedocument.Remove("_id");
                updatedocument.Set("IsSyncToServer", false);
                BsonDocument updateResult = collection.FindOneAndUpdate(filterid, updatedocument);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        ///<summary>
        ///计算时间
        ///</summary>
        public void ComputingTime(DataModel.MachineProduction machineProduction, BsonDocument oldMachineProduction)
        {
            try
            {
                //查找ID
                var collection = Common.MongodbHandler.GetInstance().GetCollection(defaulttMachineProductionMongodbCollectionName);
               
                BsonDocument updatedocument = machineProduction.ToBsonDocument();
                updatedocument.Remove("_id");
                updatedocument.Set("IsSyncToServer", false);
                var filterID = Builders<BsonDocument>.Filter.And(oldMachineProduction);
                BsonDocument updateResult = collection.FindOneAndUpdate(filterID, updatedocument);

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        ///<summary>
        ///自动停止记录
        ///</summary>
        public BsonDocument AutoStopMachineProduction(DataModel.MachineProduction machineProduction, BsonDocument oldMachineProduction)
        {
            try
            {
                //查找ID
                var collection = Common.MongodbHandler.GetInstance().GetCollection(defaulttMachineProductionMongodbCollectionName);
              
                BsonDocument updatedocument = machineProduction.ToBsonDocument();
                updatedocument.Remove("_id");
                updatedocument.Set("IsStopFlag", true);
                updatedocument.Set("IsSyncToServer", false);
                var filterID = Builders<BsonDocument>.Filter.And(oldMachineProduction);
                BsonDocument updateResult = collection.FindOneAndUpdate(filterID, updatedocument);
                return updateResult;

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        ///<summary>
        ///查找所有false记录
        /// </summary>
        public List<DataModel.MachineProduction> findAllRecord()
        {

            try
            {
                List<DataModel.MachineProduction> machineProductions = new List<DataModel.MachineProduction>();
                var collection = Common.MongodbHandler.GetInstance().GetCollection(defaulttMachineProductionMongodbCollectionName);
                var newfilter = Builders<BsonDocument>.Filter.And(
                      Builders<BsonDocument>.Filter.Eq("IsStopFlag", false)
                   );
                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).ToList();
                foreach (var data in getdocument)
                {
                    //转换成类
                    var MachineProductionEntity = BsonSerializer.Deserialize<DataModel.MachineProduction>(data);
                    machineProductions.Add(MachineProductionEntity);
                }
                return machineProductions;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        ///<summary>
        ///按最后机器处理记录id及工单id查找
        /// </summary>
        public List<DataModel.MachineProduction> findRecordByProcessID(string jobOrderID)
        {

            try
            {
                List<DataModel.MachineProduction> machineProductions = new List<DataModel.MachineProduction>();
                var collection = Common.MongodbHandler.GetInstance().GetCollection(defaulttMachineProductionMongodbCollectionName);
                var newfilter = Builders<BsonDocument>.Filter.And(
                      Builders<BsonDocument>.Filter.Eq("JobOrderID", jobOrderID)
                   );
                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).ToList();
                foreach (var data in getdocument)
                {
                    //转换成类
                    var MachineProductionEntity = BsonSerializer.Deserialize<DataModel.MachineProduction>(data);
                    machineProductions.Add(MachineProductionEntity);
                }
                return machineProductions;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        ///<summary>
        ///增加数量
        ///</summary>
        public  void AddCount(DataModel.MachineProduction machineProduction, int count)
        {
            try
            {
                //查找ID
                var collection = Common.MongodbHandler.GetInstance().GetCollection(defaulttMachineProductionMongodbCollectionName);
                var filterid = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(machineProduction._id));
                BsonDocument updatedocument = machineProduction.ToBsonDocument();
                updatedocument.Remove("_id");
                updatedocument.Set("ProduceCount", count);
                updatedocument.Set("IsSyncToServer", false);

                BsonDocument updateResult = collection.FindOneAndUpdate(filterid, updatedocument);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
