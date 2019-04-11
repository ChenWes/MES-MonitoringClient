using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace MES_MonitoringClient.Common
{
    public static class SyncDataHelper
    {
        /// <summary>
        /// 基础表同步
        /// </summary>
        /// <returns></returns>
        public static bool SyncData_AllCollection()
        {
            try
            {
                DateTime startTime = System.DateTime.Now;

                //获取url并发起Http get请求
                string urlPath = Common.ConfigFileHandler.GetAppConfig("SyncDataUrlPath");
                string jsonString = Common.HttpHelper.HttpGetWithToken(urlPath);

                //机器状态
                SyncDataDBHelper<DataModel.MachineStatus> machineStatus_SyncHandlerClass = new SyncDataDBHelper<DataModel.MachineStatus>();
                machineStatus_SyncHandlerClass.SyncData_Process(Common.JsonHelper.GetJsonValue(jsonString, "machinestatus"));

                //部门
                SyncDataDBHelper<DataModel.Department> department_SyncHandlerClass = new SyncDataDBHelper<DataModel.Department>();
                department_SyncHandlerClass.SyncData_Process(Common.JsonHelper.GetJsonValue(jsonString, "department"));

                //组别
                SyncDataDBHelper<DataModel.Group> group_SyncHandlerClass = new SyncDataDBHelper<DataModel.Group>();
                group_SyncHandlerClass.SyncData_Process(Common.JsonHelper.GetJsonValue(jsonString, "group"));

                //岗位
                SyncDataDBHelper<DataModel.JobPositon> jobPosition_SyncHandlerClass = new SyncDataDBHelper<DataModel.JobPositon>();
                jobPosition_SyncHandlerClass.SyncData_Process(Common.JsonHelper.GetJsonValue(jsonString, "jobposition"));

                //班次
                SyncDataDBHelper<DataModel.WorkShift> workShift_SyncHandlerClass = new SyncDataDBHelper<DataModel.WorkShift>();
                workShift_SyncHandlerClass.SyncData_Process(Common.JsonHelper.GetJsonValue(jsonString, "workshift"));

                //员工
                SyncDataDBHelper<DataModel.Employee> employee_SyncHandlerClass = new SyncDataDBHelper<DataModel.Employee>();
                employee_SyncHandlerClass.SyncData_Process(Common.JsonHelper.GetJsonValue(jsonString, "employee"));
                


                TimeSpan timeSpan = System.DateTime.Now - startTime;
                Common.LogHandler.WriteLog("首次同步总运行时间：" + timeSpan.TotalSeconds.ToString());

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
