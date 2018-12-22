using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;

namespace MES_MonitoringClient.Common
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
            //获取文件
            string file = System.Windows.Forms.Application.ExecutablePath;
            Configuration config = ConfigurationManager.OpenExeConfiguration(file);

            //获取节点对应值
            foreach (string key in config.AppSettings.Settings.AllKeys)
            {
                if (key == strKey)
                {
                    return config.AppSettings.Settings[strKey].Value.ToString();
                }
            }
            return null;
        }

        /// <summary>
        /// 设置配置节点对应值
        /// </summary>
        /// <param name="newKey"></param>
        /// <param name="newValue"></param>
        public static void SetAppConfig(string strKey, string newValue)
        {
            //获取文件
            string file = System.Windows.Forms.Application.ExecutablePath;
            Configuration config = ConfigurationManager.OpenExeConfiguration(file);

            //是否存在节点
            bool exist = false;
            foreach (string key in config.AppSettings.Settings.AllKeys)
            {
                if (key == strKey)
                {
                    exist = true;
                }
            }
            if (exist)
            {
                config.AppSettings.Settings.Remove(strKey);
            }

            //设置节点值
            config.AppSettings.Settings.Add(strKey, newValue);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
