using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_MonitoringClient.Common
{
    public class MachineResponsiblePersonHandler
    {
        /*MongoDB数据库*/
        /*-------------------------------------------------------------------------------------*/
        /// <summary>
        ///  Mongodb集合
        /// </summary>
        private static IMongoCollection<DataModel.MachineResponsiblePerson> MachineResponsiblePersonCollection;

        /// <summary>
        /// 默认Mongodb集合名
        /// </summary>
        public static string defaultMachineResponsiblePersonMongodbCollectionName = Common.ConfigFileHandler.GetAppConfig("MachineResponsiblePersonCollectionName");



        ///<summary>
        ///通过机器找到员工
        ///</summary>
        public static List<DataModel.MachineResponsiblePerson> findChargeAreaByMachineID(string machineID)
        {
            MachineResponsiblePersonCollection = MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.MachineResponsiblePerson>(defaultMachineResponsiblePersonMongodbCollectionName);
            List<DataModel.MachineResponsiblePerson>  machineResponsiblePersons = MachineResponsiblePersonCollection.AsQueryable().ToList();
            foreach(var item in machineResponsiblePersons.ToArray())
            {
                if(!item.MachineID.Contains(machineID))
                {
                    machineResponsiblePersons.Remove(item);
                }
            }
            return machineResponsiblePersons;
        }

    }
}
