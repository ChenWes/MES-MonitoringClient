﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace MES_MonitoringClient.Common
{
    public static class SyncDataHelper
    {
        public static string MC_MachineStatusCollectionName = Common.ConfigFileHandler.GetAppConfig("MachineStatusCollectionName");
        public static string MC_DepartmentCollectionName= Common.ConfigFileHandler.GetAppConfig("DepartmentCollectionName");
        public static string MC_GroupCollectionName = Common.ConfigFileHandler.GetAppConfig("GroupCollectionName");
        public static string MC_JobPositionCollectionName = Common.ConfigFileHandler.GetAppConfig("JobPositionCollectionName");
        public static string MC_WorkShiftCollectionName = Common.ConfigFileHandler.GetAppConfig("WorkShiftCollectionName");
        public static string MC_EmployeeCollectionName = Common.ConfigFileHandler.GetAppConfig("EmployeeCollectionName");
        public static string MC_CustomerCollectionName = Common.ConfigFileHandler.GetAppConfig("CustomerCollectionName");
        public static string MC_MaterialCollectionName = Common.ConfigFileHandler.GetAppConfig("MaterialCollectionName");
        public static string MC_MouldCollectionName = Common.ConfigFileHandler.GetAppConfig("MouldCollectionName");

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
