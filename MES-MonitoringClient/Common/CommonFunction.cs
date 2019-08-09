using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.NetworkInformation;
using Microsoft.Win32;
using System.ServiceProcess;
using System.Net.Sockets;
using System.Net;

namespace MES_MonitoringClient.Common
{
    /// <summary>
    /// 公共方法类
    /// </summary>
    public static class CommonFunction
    {
        public enum ServiceStatus
        {
            NoInstall = 0,
            Stopped = 1,
            StartPending = 2,
            StopPending = 3,
            Running = 4,
            ContinuePending = 5,
            PausePending = 6,
            Paused = 7
        }
        /*-------------------------------------------------------------------------------------*/

        /// <summary>
        /// 格式化时间显示（毫秒）
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
            strTime.Append(hour > 0 ? (strHour + "小时") : "");
            strTime.Append(minute > 0 ? (strMinute + "分钟") : "");
            strTime.Append(second > 0 ? (strSecond + "秒") : "");
            strTime.Append(milliSecond > 0 ? (strMilliSecond + "毫秒") : "");

            return strTime.ToString();
        }

        /// <summary>
        /// 格式化时间显示（秒）
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static String FormatSeconds_D(double s)
        {
            long l = Convert.ToInt64(s);
            return FormatSeconds(l);
        }

        /// <summary>
        /// 格式化时间显示（秒）
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static String FormatSeconds(long s)
        {
            //基数
            int ss = 1;
            int mi = ss * 60;
            int hh = mi * 60;
            int dd = hh * 24;

            //计算
            long day = s / dd;
            long hour = (s - day * dd) / hh;
            long minute = (s - day * dd - hour * hh) / mi;
            long second = (s - day * dd - hour * hh - minute * mi) / ss;
            long milliSecond = s - day * dd - hour * hh - minute * mi - second * ss;

            //对应的文本
            String strDay = day < 10 ? "0" + day : "" + day; //天
            String strHour = hour < 10 ? "0" + hour : "" + hour;//小时
            String strMinute = minute < 10 ? "0" + minute : "" + minute;//分钟
            String strSecond = second < 10 ? "0" + second : "" + second;//秒
            //String strMilliSecond = milliSecond < 10 ? "0" + milliSecond : "" + milliSecond;//毫秒
            //strMilliSecond = milliSecond < 100 ? "0" + strMilliSecond : "" + strMilliSecond;

            //返回标准化的时间
            StringBuilder strTime = new StringBuilder();
            strTime.Append(day > 0 ? (strDay + "天") : "");
            strTime.Append(hour > 0 ? (strHour + "小时") : "");
            strTime.Append(minute > 0 ? (strMinute + "分钟") : "");
            strTime.Append(second > 0 ? (strSecond + "秒") : "");
            //strTime.Append(milliSecond > 0 ? (strMilliSecond + "毫秒") : "");

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

        public static string getIPAddress()
        {
            try
            {
                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        return IpEntry.AddressList[i].ToString();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        /*-------------------------------------------------------------------------------------*/
        /*颜色转换*/

        /// <summary>
        /// HTML颜色转换成ARGB颜色
        /// </summary>
        /// <param name="strHxColor"></param>
        /// <returns></returns>
        public static System.Drawing.Color colorHx16toRGB(string strHxColor)
        {
            try
            {
                if (strHxColor.Length == 0)
                {
                    //如果为空
                    return System.Drawing.Color.FromArgb(0, 0, 0);//设为黑色
                }
                else
                {
                    //转换颜色
                    return System.Drawing.Color.FromArgb(System.Int32.Parse(strHxColor.Substring(1, 2), System.Globalization.NumberStyles.AllowHexSpecifier), System.Int32.Parse(strHxColor.Substring(3, 2), System.Globalization.NumberStyles.AllowHexSpecifier), System.Int32.Parse(strHxColor.Substring(5, 2), System.Globalization.NumberStyles.AllowHexSpecifier));
                }
            }
            catch
            {
                //设为黑色
                return System.Drawing.Color.FromArgb(0, 0, 0);
            }
        }

        /// <summary>
        /// RGB颜色转换成HTML颜色结构
        /// </summary>
        /// <param name="R">红</param>
        /// <param name="G">绿</param>
        /// <param name="B">蓝</param>
        /// <returns></returns>
        public static string colorRGBtoHx16(int R, int G, int B)
        {
            return System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(R, G, B));
        }

        /*-------------------------------------------------------------------------------------*/
        /*服务状态*/

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
        /// 获取服务状态
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static ServiceStatus GetServiceStatus(string serviceName)
        {
            ServiceStatus returnFlag = ServiceStatus.NoInstall;

            ServiceController[] services = ServiceController.GetServices();
            var service = services.FirstOrDefault(s => s.ServiceName == serviceName);

            if (service == null)
            {
                returnFlag = ServiceStatus.NoInstall;
            }
            else
            {
                returnFlag = (ServiceStatus)service.Status;
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
