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
    public class WorkshopResponsiblePersonHandler
    {
        /*MongoDB数据库*/
        /*-------------------------------------------------------------------------------------*/
   

        /// <summary>
        /// 默认Mongodb集合名
        /// </summary>
        public static string defaultWorkshopResponsiblePersonMongodbCollectionName = Common.ConfigFileHandler.GetAppConfig("WorkshopResponsiblePersonCollectionName");
      

        ///<summary>
        ///通过车间id找到领班
        ///</summary>
        public static List<DataModel.WorkshopResponsiblePerson> findEmployeeByWorkshopID(string workshopID)
        {
            try
            {
                List<DataModel.WorkshopResponsiblePerson> workshopResponsiblePersons = new List<DataModel.WorkshopResponsiblePerson>();
                var collection = Common.MongodbHandler.GetInstance().GetCollection(defaultWorkshopResponsiblePersonMongodbCollectionName);
                var newfilter = Builders<BsonDocument>.Filter.Eq("WorkshopID", workshopID);
                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).ToList();

                foreach (var data in getdocument)
                {
                    //转换成类
                    var WorkshopResponsiblePersonEntity = BsonSerializer.Deserialize<DataModel.WorkshopResponsiblePerson>(data);
                    workshopResponsiblePersons.Add(WorkshopResponsiblePersonEntity);
                }
                return workshopResponsiblePersons;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
