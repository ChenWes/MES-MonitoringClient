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
                    #region 正常反序列化为实体类并处理
                    Model.MachineStatus dataEntityClass = JsonConvert.DeserializeObject<Model.MachineStatus>(dataJson);

                    SyncDataDBHandler<Model.MachineStatus> machineStatus_SyncHandlerClass = new SyncDataDBHandler<Model.MachineStatus>();
                    return machineStatus_SyncHandlerClass.SyncData_DBHandler(dataEntityClass, id, action);

                    #endregion
                }
                else if (type == "WorkShift")
                {
                    #region 正常反序列化为实体类并处理

                    Model.WorkShift dataEntityClass = JsonConvert.DeserializeObject<Model.WorkShift>(dataJson);

                    SyncDataDBHandler<Model.WorkShift> machineStatus_SyncHandlerClass = new SyncDataDBHandler<Model.WorkShift>();
                    return machineStatus_SyncHandlerClass.SyncData_DBHandler(dataEntityClass, id, action);

                    #endregion
                }
                else if (type == "JobPosition")
                {
                    #region 自定义反序列化

                    Model.JobPositon dataEntityClass = JsonConvert.DeserializeObject<Model.JobPositon>(dataJson);

                    SyncDataDBHandler<Model.JobPositon> machineStatus_SyncHandlerClass = new SyncDataDBHandler<Model.JobPositon>();
                    return machineStatus_SyncHandlerClass.SyncData_DBHandler(dataEntityClass, id, action);

                    #endregion
                }
                else if (type == "Department")
                {
                    #region 正常反序列化为实体类并处理

                    Model.Department dataEntityClass = JsonConvert.DeserializeObject<Model.Department>(dataJson);

                    SyncDataDBHandler<Model.Department> machineStatus_SyncHandlerClass = new SyncDataDBHandler<Model.Department>();
                    return machineStatus_SyncHandlerClass.SyncData_DBHandler(dataEntityClass, id, action);

                    #endregion
                }
                else if (type == "Group")
                {
                    #region 自定义反序列化

                    Model.Group dataEntityClass = JsonConvert.DeserializeObject<Model.Group>(dataJson);

                    SyncDataDBHandler<Model.Group> machineStatus_SyncHandlerClass = new SyncDataDBHandler<Model.Group>();
                    return machineStatus_SyncHandlerClass.SyncData_DBHandler(dataEntityClass, id, action);

                    #endregion
                }
                else if (type == "Employee")
                {
                    #region 自定义反序列化

                    Model.Employee dataEntityClass = JsonConvert.DeserializeObject<Model.Employee>(dataJson);

                    SyncDataDBHandler<Model.Employee> machineStatus_SyncHandlerClass = new SyncDataDBHandler<Model.Employee>();
                    return machineStatus_SyncHandlerClass.SyncData_DBHandler(dataEntityClass, id, action);

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
