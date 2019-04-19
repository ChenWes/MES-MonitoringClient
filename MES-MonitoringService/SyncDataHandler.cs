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
    public class SyncDataHandler
    {
        public static string MC_MachineStatusCollectionName = Common.ConfigFileHandler.GetAppConfig("MachineStatusCollectionName");
        public static string MC_DepartmentCollectionName = Common.ConfigFileHandler.GetAppConfig("DepartmentCollectionName");
        public static string MC_GroupCollectionName = Common.ConfigFileHandler.GetAppConfig("GroupCollectionName");
        public static string MC_JobPositionCollectionName = Common.ConfigFileHandler.GetAppConfig("JobPositionCollectionName");
        public static string MC_WorkShiftCollectionName = Common.ConfigFileHandler.GetAppConfig("WorkShiftCollectionName");
        public static string MC_EmployeeCollectionName = Common.ConfigFileHandler.GetAppConfig("EmployeeCollectionName");
        public static string MC_CustomerCollectionName = Common.ConfigFileHandler.GetAppConfig("CustomerCollectionName");
        public static string MC_MaterialCollectionName = Common.ConfigFileHandler.GetAppConfig("MaterialCollectionName");
        public static string MC_MouldCollectionName = Common.ConfigFileHandler.GetAppConfig("MouldCollectionName");
        public static string MC_JobOrderCollectionName = Common.ConfigFileHandler.GetAppConfig("JobOrderCollectionName");

        public enum SyncDataType
        {
            Factory,
            Workshop,
            Machine,

            MachineStatus,
            WorkShift,
            JobPosition,
            Department,
            Group,

            Customer,
            Material,
            Mould,

            Employee,
            JobOrder,
        }

        /// <summary>
        /// 处理同步的数据
        /// {
        /// 	"type": "XXX",
        /// 	"id": "5cb80cbc6375a2558cd10b99",
        /// 	"action": "EDIT",
        /// 	"data": {
        /// 		"_id": "5cb80cbc6375a2558cd10b99",
        /// 		"Code": "003",
        /// 	}
        /// }
        /// </summary>
        /// <param name="jsonString">通过同步获取的JSON数据</param>
        /// <returns></returns>
        public bool ProcessSyncData(string jsonString)
        {
            try
            {
                //处理RabbitMQ传入的JSON数据
                //同步的数据实体类型（操作的是什么）
                string type = Common.JsonHelper.GetJsonValue(jsonString, "type");
                //同步的操作类型（ADD\EDIT\DELETE）（怎么操作）
                string action = Common.JsonHelper.GetJsonValue(jsonString, "action");
                //同步的数据ID（操作的ID，用于修改和删除操作）
                string id = Common.JsonHelper.GetJsonValue(jsonString, "id");
                //同步的数据实体（用于新增和修改操作）
                string dataJson = Common.JsonHelper.GetJsonValue(jsonString, "data");


                if (type == SyncDataType.Factory.ToString() || type == SyncDataType.Workshop.ToString() || type == SyncDataType.Machine.ToString()
                   || type == SyncDataType.MachineStatus.ToString() || type == SyncDataType.WorkShift.ToString() || type == SyncDataType.JobPosition.ToString()
                   || type == SyncDataType.Department.ToString() || type == SyncDataType.Group.ToString()
                   || type == SyncDataType.Customer.ToString() || type == SyncDataType.Material.ToString() || type == SyncDataType.Mould.ToString()
               )
                {
                    #region 正常处理数据

                    return SyncDataDBHandler.SyncData_DBHandler(MC_GroupCollectionName, dataJson, id, action);

                    #endregion
                }
                else if (type == SyncDataType.Employee.ToString())
                {
                    #region 员工（特殊处理）

                    return SyncDataDBHandler.SyncEmployee_DBHandler(MC_EmployeeCollectionName, dataJson, id, action);

                    #endregion
                }
                else if (type == SyncDataType.JobOrder.ToString())
                {
                    #region 工单（特殊处理）

                    SyncDataDBHandler.SyncOrder_DBHandler(MC_JobOrderCollectionName, dataJson, id, action);

                    #endregion
                }

                return true;

            }
            catch (Exception ex)
            {
                Common.LogHandler.WriteLog("处理同步数据出错，原因：" + ex.Message);
                return false;
            }
        }
    }
}
