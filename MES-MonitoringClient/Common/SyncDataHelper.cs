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
        public static string MC_FactoryCollectionName = Common.ConfigFileHandler.GetAppConfig("FactoryCollectionName");
        public static string MC_WorkshopCollectionName = Common.ConfigFileHandler.GetAppConfig("WorkshopCollectionName");
        public static string MC_MachineCollectionName = Common.ConfigFileHandler.GetAppConfig("MachineCollectionName");
        public static string MC_MachineStatusCollectionName = Common.ConfigFileHandler.GetAppConfig("MachineStatusCollectionName");
        public static string MC_DepartmentCollectionName= Common.ConfigFileHandler.GetAppConfig("DepartmentCollectionName");
        public static string MC_GroupCollectionName = Common.ConfigFileHandler.GetAppConfig("GroupCollectionName");
        public static string MC_JobPositionCollectionName = Common.ConfigFileHandler.GetAppConfig("JobPositionCollectionName");
        public static string MC_WorkShiftCollectionName = Common.ConfigFileHandler.GetAppConfig("WorkShiftCollectionName");
        public static string MC_EmployeeCollectionName = Common.ConfigFileHandler.GetAppConfig("EmployeeCollectionName");
        public static string MC_CustomerCollectionName = Common.ConfigFileHandler.GetAppConfig("CustomerCollectionName");
        public static string MC_MaterialCollectionName = Common.ConfigFileHandler.GetAppConfig("MaterialCollectionName");
        public static string MC_MouldCollectionName = Common.ConfigFileHandler.GetAppConfig("MouldCollectionName");
        public static string MC_MouldProductCollectionName = Common.ConfigFileHandler.GetAppConfig("MouldProductCollectionName");
        public static string MC_WorkshopResponsiblePersonCollectionName = Common.ConfigFileHandler.GetAppConfig("WorkshopResponsiblePersonCollectionName");
        public static string MC_MachineResponsiblePersonCollectionName = Common.ConfigFileHandler.GetAppConfig("MachineResponsiblePersonCollectionName");
        public static string MC_EmployeeWorkScheduleCollectionName = Common.ConfigFileHandler.GetAppConfig("EmployeeWorkScheduleCollectionName");
        public static string MC_DefectiveTypeCollectionName = Common.ConfigFileHandler.GetAppConfig("DefectiveTypeCollectionName");
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

                //工厂
                SyncDataDBHelper.SyncData_Process(MC_FactoryCollectionName, Common.JsonHelper.GetJsonValue(jsonString, "factory"));

                //车间                
                SyncDataDBHelper.SyncData_Process(MC_WorkshopCollectionName, Common.JsonHelper.GetJsonValue(jsonString, "workshop"));

                //机器                
                SyncDataDBHelper.SyncData_Process(MC_MachineCollectionName, Common.JsonHelper.GetJsonValue(jsonString, "machine"));

                //机器状态                
                SyncDataDBHelper.SyncData_Process(MC_MachineStatusCollectionName, Common.JsonHelper.GetJsonValue(jsonString, "machinestatus"));

                //部门                
                SyncDataDBHelper.SyncData_Process(MC_DepartmentCollectionName, Common.JsonHelper.GetJsonValue(jsonString, "department"));

                //组别
                SyncDataDBHelper.SyncData_Process(MC_GroupCollectionName, Common.JsonHelper.GetJsonValue(jsonString, "group"));

                //岗位
                SyncDataDBHelper.SyncData_Process(MC_JobPositionCollectionName, Common.JsonHelper.GetJsonValue(jsonString, "jobposition"));

                //班次
                SyncDataDBHelper.SyncData_Process(MC_WorkShiftCollectionName, Common.JsonHelper.GetJsonValue(jsonString, "workshift"));

                //员工--单独处理（增加自定义字段）
                SyncDataDBHelper.SyncEmployee_Process(MC_EmployeeCollectionName, Common.JsonHelper.GetJsonValue(jsonString, "employee"));

                //客户
                SyncDataDBHelper.SyncData_Process(MC_CustomerCollectionName, Common.JsonHelper.GetJsonValue(jsonString, "customer"));

                //产品
                SyncDataDBHelper.SyncData_Process(MC_MaterialCollectionName, Common.JsonHelper.GetJsonValue(jsonString, "material"));

                //模具
                SyncDataDBHelper.SyncData_Process(MC_MouldCollectionName, Common.JsonHelper.GetJsonValue(jsonString, "mould"));

                //模具对应产品出数
                SyncDataDBHelper.SyncData_Process(MC_MouldProductCollectionName, Common.JsonHelper.GetJsonValue(jsonString, "mouldProduct"));

                //车间负责人
                SyncDataDBHelper.SyncData_Process(MC_WorkshopResponsiblePersonCollectionName, Common.JsonHelper.GetJsonValue(jsonString, "WorkshopResponsiblePerson"));

                //机器负责人
                SyncDataDBHelper.SyncData_Process(MC_MachineResponsiblePersonCollectionName, Common.JsonHelper.GetJsonValue(jsonString, "MachineResponsiblePerson"));

                //员工排班
                SyncDataDBHelper.SyncData_Process(MC_EmployeeWorkScheduleCollectionName, Common.JsonHelper.GetJsonValue(jsonString, "EmployeeWorkScheduleList"));

                //疵品类型
                SyncDataDBHelper.SyncData_Process(MC_DefectiveTypeCollectionName, Common.JsonHelper.GetJsonValue(jsonString, "DefectiveType"));

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
