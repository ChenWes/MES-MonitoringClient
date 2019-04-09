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

        /// <summary>
        /// 处理同步的数据
        /// </summary>
        /// <param name="jsonString">通过同步获取的JSON数据</param>
        /// <returns></returns>
        public bool ProcessSyncData(string jsonString)
        {
            try
            {
                //处理一级参数
                string type = Common.JsonHelper.GetJsonValue(jsonString, "type");
                string action = Common.JsonHelper.GetJsonValue(jsonString, "action");
                string id = Common.JsonHelper.GetJsonValue(jsonString, "id");

                string dataJson = Common.JsonHelper.GetJsonValue(jsonString, "data");

                if (type == "MachineStatus")
                {
                    #region 处理数据

                    SyncDataDBHandler<Model.MachineStatus> machineStatus_SyncHandlerClass = new SyncDataDBHandler<Model.MachineStatus>();
                    return machineStatus_SyncHandlerClass.SyncData_DBHandler(dataJson, id, action);

                    #endregion
                }
                else if (type == "WorkShift")
                {
                    #region 处理数据

                    SyncDataDBHandler<Model.WorkShift> machineStatus_SyncHandlerClass = new SyncDataDBHandler<Model.WorkShift>();
                    return machineStatus_SyncHandlerClass.SyncData_DBHandler(dataJson, id, action);

                    #endregion
                }
                else if (type == "JobPosition")
                {
                    #region 处理数据

                    SyncDataDBHandler<Model.JobPositon> machineStatus_SyncHandlerClass = new SyncDataDBHandler<Model.JobPositon>();
                    return machineStatus_SyncHandlerClass.SyncData_DBHandler(dataJson, id, action);

                    #endregion
                }
                else if (type == "Department")
                {
                    #region 处理数据

                    SyncDataDBHandler<Model.Department> machineStatus_SyncHandlerClass = new SyncDataDBHandler<Model.Department>();
                    return machineStatus_SyncHandlerClass.SyncData_DBHandler(dataJson, id, action);

                    #endregion
                }
                else if (type == "Group")
                {
                    #region 处理数据

                    SyncDataDBHandler<Model.Group> machineStatus_SyncHandlerClass = new SyncDataDBHandler<Model.Group>();
                    return machineStatus_SyncHandlerClass.SyncData_DBHandler(dataJson, id, action);

                    #endregion
                }
                else if (type == "Employee")
                {
                    #region 处理数据

                    SyncDataDBHandler<Model.Employee> machineStatus_SyncHandlerClass = new SyncDataDBHandler<Model.Employee>();
                    return machineStatus_SyncHandlerClass.SyncData_DBHandler(dataJson, id, action);

                    #endregion
                }

                return true;

            }
            catch (Exception ex)
            {
                Common.LogHandler.Log("处理同步数据出错，原因：" + ex.Message);
                return false;
            }
        }
    }
}
