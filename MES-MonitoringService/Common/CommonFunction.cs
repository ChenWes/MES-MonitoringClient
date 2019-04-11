using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace MES_MonitoringService.Common
{
    public static class CommonFunction
    {
        /*-------------------------------------------------------------------------------------*/

        /// <summary>
        /// 格式化时间显示
        /// </summary>
        /// <param name="ms">总的毫秒数</param>
        /// <returns></returns>
        public static String FormatMilliseconds(long ms)
        {
            //基数
            int ss = 1000;
            int mi = ss * 60;
            int hh = mi * 60;
            int dd = hh * 24;

            //计算
            long day = ms / dd;
            long hour = (ms - day * dd) / hh;
            long minute = (ms - day * dd - hour * hh) / mi;
            long second = (ms - day * dd - hour * hh - minute * mi) / ss;
            long milliSecond = ms - day * dd - hour * hh - minute * mi - second * ss;

            //对应的文本
            String strDay = day < 10 ? "0" + day : "" + day; //天
            String strHour = hour < 10 ? "0" + hour : "" + hour;//小时
            String strMinute = minute < 10 ? "0" + minute : "" + minute;//分钟
            String strSecond = second < 10 ? "0" + second : "" + second;//秒
            String strMilliSecond = milliSecond < 10 ? "0" + milliSecond : "" + milliSecond;//毫秒

            strMilliSecond = milliSecond < 100 ? "0" + strMilliSecond : "" + strMilliSecond;

            //返回标准化的时间
            StringBuilder strTime = new StringBuilder();
            strTime.Append(day > 0 ? (strDay + "天") : "");
            strTime.Append(hour > 0 ? (strHour + "时") : "");
            strTime.Append(minute > 0 ? (strMinute + "分") : "");
            strTime.Append(second > 0 ? (strSecond + "秒") : "");
            strTime.Append(milliSecond > 0 ? (strMilliSecond + "毫秒") : "");

            return strTime.ToString();
        }

        /// <summary> 
        /// 获取MAC地址(返回第一个物理以太网卡的mac地址) 
        /// </summary> 
        /// <returns>成功返回mac地址，失败返回null</returns> 
        public static string getMacAddress()
        {
            string macAddress = null;
            try
            {
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in nics)
                {
                    if (adapter.NetworkInterfaceType.ToString().Equals("Ethernet")) //是以太网卡
                    {
                        string fRegistryKey = "SYSTEM\\CurrentControlSet\\Control\\Network\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\" + adapter.Id + "\\Connection";
                        RegistryKey rk = Registry.LocalMachine.OpenSubKey(fRegistryKey, false);
                        if (rk != null)
                        {
                            // 区分 PnpInstanceID     
                            // 如果前面有 PCI 就是本机的真实网卡    
                            // MediaSubType 为 01 则是常见网卡，02为无线网卡。    
                            string fPnpInstanceID = rk.GetValue("PnpInstanceID", "").ToString();
                            int fMediaSubType = Convert.ToInt32(rk.GetValue("MediaSubType", 0));
                            if (fPnpInstanceID.Length > 3 && fPnpInstanceID.Substring(0, 3) == "PCI") //是物理网卡
                            {
                                macAddress = adapter.GetPhysicalAddress().ToString();
                                break;
                            }
                            else if (fMediaSubType == 1) //虚拟网卡
                                continue;
                            else if (fMediaSubType == 2) //无线网卡(上面判断Ethernet时已经排除了)
                                continue;
                        }
                    }
                }
            }
            catch
            {
                macAddress = null;
            }
            return macAddress;
        }

        /// <summary>
        /// 确认服务是否在运行
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static bool ServiceRunning(string serviceName)
        {
            bool returnFlag = true;
            ServiceController[] services = ServiceController.GetServices();
            var service = services.FirstOrDefault(s => s.ServiceName == serviceName);

            if (service == null)
            {
                returnFlag = false;
            }
            else if (service.Status != ServiceControllerStatus.Running)
            {
                returnFlag = false;
            }

            return returnFlag;
        }

        /// <summary>
        /// 获取后台服务器完整URL地址
        /// </summary>
        /// <returns></returns>
        public static string GenerateBackendUri()
        {
            return Common.ConfigFileHandler.GetAppConfig("BackendServerProtocol") + "://" + Common.ConfigFileHandler.GetAppConfig("BackendServerHost") + ":" + Common.ConfigFileHandler.GetAppConfig("BackendServerPort");
        }
    }
}
