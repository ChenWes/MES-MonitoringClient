using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;

namespace MES_MonitoringClient_ManualTest.Common
{
    /// <summary>
    /// 配置文件操作类
    /// </summary>
    public static class ConfigFileHandler
    {
        /// <summary>
        /// 读取配置节点对应值
        /// </summary>
        /// <param name="strKey"></param>
        /// <returns></returns>
        public static string GetAppConfig(string strKey)
        {
            return ConfigurationManager.AppSettings[strKey].ToString();
        }
    }
}
