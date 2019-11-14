using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Timers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;

using MongoDB.Bson.IO;
using System.Threading;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.ServiceProcess;
using System.Diagnostics;

namespace MES_Service_Defend
{
    public class BackendServiceHandler
    {
        private static string currentExePath = AppDomain.CurrentDomain.BaseDirectory;//基目录
        //mongodb配置

        //RabbitMQ地址
        public static string RabbitMQServerProtocol = ConfigurationManager.AppSettings["RabbitMQServerProtocol"].ToString();
        public static string RabbitMQServerHostName = ConfigurationManager.AppSettings["RabbitMQServerHostName"].ToString();
        private static string RabbitMQServerPort = ConfigurationManager.AppSettings["RabbitMQServerPort"].ToString();
        public static string RabbitMQServerAPI = ConfigurationManager.AppSettings["RabbitMQServerAPI"].ToString();
        public static string RabbitMQServerVhost = ConfigurationManager.AppSettings["RabbitMQServerVhost"].ToString();
        private static string RabbitMQUserName = ConfigurationManager.AppSettings["RabbitMQUserName"].ToString();
        private static string RabbitMQPassword = ConfigurationManager.AppSettings["RabbitMQPassword"].ToString();
        private static string SyncData_QueueName_Prefix = ConfigurationManager.AppSettings["SyncData_QueueName_Prefix"].ToString();
        private static string Check_Unacked = ConfigurationManager.AppSettings["Check_Unacked"].ToString();
        private static string defaultMachineRegisterMongodbCollectionName = ConfigurationManager.AppSettings["MachineRegisterCollectionName"].ToString();
        private static string Service_Name = ConfigurationManager.AppSettings["Service_Name"].ToString();
        private static readonly int _timerInterval = Convert.ToInt32(ConfigurationManager.AppSettings["timerInterval"]) * 1000;
        /// <summary>
        /// 要守护的服务名
        /// </summary>
        private static readonly string toWatchServiceName = ConfigurationManager.AppSettings["toWatchServiceName"];
        private System.Timers.Timer _timer;
        /// <summary>
        /// 上传数据至服务器
        /// </summary>
        public BackendServiceHandler()
        {
        }

      
       

        private void TryRestartService()
        {
            try
            {
                var mres = new ManualResetEventSlim(false); // state is initially false
                while (!mres.Wait(5000)) // loop until state is true, checking every 3s
                {
                    try
                    {
                        //尝试启动服务
                        OnStart();

                        mres.Set(); // state set to true - breaks out of loop
                    }
                    catch (Exception ex)
                    {
                        Common.LogHandler.WriteLog("尝试启动MES守护服务出现错误：" + ex.Message, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.LogHandler.WriteLog("尝试启动MES守护服务出现错误：" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 开始方法
        /// </summary>
        public void Start()
        {
            TryRestartService();
        }

        /// <summary>
        /// 结束方法
        /// </summary>
        public void Stop()
        {
            OnStop();
        }
        protected  void OnStart()
        {
            //服务启动时开启定时器
            _timer = new System.Timers.Timer();
            _timer.Interval = _timerInterval;
            _timer.Enabled = true;
            _timer.AutoReset = true;
            _timer.Elapsed += _timer_Elapsed;
            Common.LogHandler.WriteLog("守护服务开启：" + currentExePath);
        }

        void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _timer.Stop();
            //如果服务状态为停止，则重新启动服务
            if (!CheckSericeStart(toWatchServiceName))
            {
                Common.LogHandler.WriteLog("检测到服务已停止");
                StartService(toWatchServiceName);
            }
            //如果存在Unacked，则重启服务
            else if (CheckUnacked())
            {
                StopService(toWatchServiceName);
                StartService(toWatchServiceName);

            }
            _timer.Start();
        }

        protected  void OnStop()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                Common.LogHandler.WriteLog("守护服务停止：" + currentExePath);
            }
        }
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="serviceName">要启动的服务名称</param>
        private void StartService(string serviceName)
        {
            try
            {
                ServiceController[] services = ServiceController.GetServices();
                foreach (ServiceController service in services)
                {
                    if (service.ServiceName.Trim() == serviceName.Trim())
                    {
                        service.Start();
                        //直到服务启动
                        service.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 30));
                        Common.LogHandler.WriteLog(string.Format("启动服务:{0}", serviceName));
                        if (CheckSericeStart(toWatchServiceName))
                        {
                            Common.LogHandler.WriteLog("启动服务成功！");
                        }
                       
                    }
                }
             
            }
            catch (Exception ex)
            {
               
                Common.LogHandler.WriteLog("StartService出错：", ex);
            
            }
        }
        /// <summary>
        /// 判断服务有无启动
        /// </summary>
        /// <param name="serviceName"></param>
        private bool CheckSericeStart(string serviceName)
        {
            bool result = true;
            try
            {
                ServiceController[] services = ServiceController.GetServices();
                foreach (ServiceController service in services)
                {
                    if (service.ServiceName.Trim() == serviceName.Trim())
                    {
                        //已停止
                        if (service.Status == ServiceControllerStatus.Stopped)
                        {
                            result = false;
                        }
                        //处理服务器一直处于正在停止状态
                        else if (service.Status == ServiceControllerStatus.StopPending)
                        {

                            service.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 0, 30));
                           
                            if (service.Status == ServiceControllerStatus.Stopped)
                            {
                                result = false;
                            }
                            else
                            {
                                Common.LogHandler.WriteLog("服务一直处于正在停止状态！");
                                KillProgram(Service_Name); 
                                result = false;
                            }
                        }
                        //处理服务器一直处于正在启动状态
                        else if (service.Status == ServiceControllerStatus.StartPending)
                        {
                            service.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 30));
                            if (service.Status == ServiceControllerStatus.StartPending)
                            {
                                Common.LogHandler.WriteLog("服务一直处于正在启动状态！");
                                KillProgram(Service_Name);
                                result = false;
                            }
                             
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.LogHandler.WriteLog("CheckSericeStart出错：", ex);
            }
            return result;
        }
        /// <summary>
        /// 判断有无unacked
        /// </summary>
        /// <param name="serviceName"></param>
        private bool CheckUnacked()
        {
            string url, username, password;
            string id = GetMachineRegisterID();
            url = RabbitMQServerProtocol + "://" + RabbitMQServerHostName + ":" + RabbitMQServerPort + RabbitMQServerAPI + "/" + RabbitMQServerVhost + "/" + SyncData_QueueName_Prefix + id;
            username = RabbitMQUserName;
            password = RabbitMQPassword;
            // bool result = true;
            try
            {
                // 如果认证页面是 https 的，请参考一下 jwt 认证的 HttpClientHandler
                // 创建  client 
                HttpClient client = new HttpClient();
                // 创建身份认证
                // using System.Net.Http.Headers;
                AuthenticationHeaderValue authentication = new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}")
                    ));
                client.DefaultRequestHeaders.Authorization = authentication;
                var response = client.GetAsync(url).Result;
                string json = response.Content.ReadAsStringAsync().Result;

                var jObject = JObject.Parse(json);
                var messages_unacknowledged = jObject[Check_Unacked].ToString();
                //Common.LogHandler.WriteLog(messages_unacknowledged);
                if (messages_unacknowledged != "0" && messages_unacknowledged != string.Empty)
                {
                    Common.LogHandler.WriteLog("Unacked数量为：" + messages_unacknowledged);
                    return true;
                }
                else
                {
                    if (messages_unacknowledged == string.Empty)
                    {
                        Common.LogHandler.WriteLog("Unacked数量为：" + messages_unacknowledged);
                    }
                    return false;
                }

            }
            catch (Exception ex)
            {
                //return ex.ToString();
                Common.LogHandler.WriteLog("CheckUnacked出错：", ex);
                return false;
            }
        }
        /// <summary>
        /// 查询获取本机的机器注册信息
        /// </summary>
        /// <returns></returns>
        public string GetMachineRegisterID()
        {
            try
            {
                var collection = Common.MongodbHandler.GetInstance().GetCollection(defaultMachineRegisterMongodbCollectionName);

                var newfilter = Builders<BsonDocument>.Filter.Exists("MachineID", true);
                var getdocument = Common.MongodbHandler.GetInstance().Find(collection, newfilter).ToList();

                if (getdocument != null && getdocument.Count > 0)
                {
                    //注册的ID
                    return getdocument.First().GetValue("MachineID").ToString();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Common.LogHandler.WriteLog("GetMachineRegisterID出错：", ex);
                return null;
            }
        }
        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="serviceName"></param>
        private void StopService(string serviceName)
        {
            try
            {
                ServiceController[] services = ServiceController.GetServices();
                foreach (ServiceController service in services)
                {
                    if (service.ServiceName.Trim() == serviceName.Trim())
                    {
                        service.Stop();
                        //直到服务停止
                        service.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 0, 30));
                        Common.LogHandler.WriteLog(string.Format("停止服务:{0}", serviceName));
                    }
                }
            }
            catch (Exception ex)
            {
                Common.LogHandler.WriteLog("StopService出错：", ex);
            }
        }
        /// <summary>
        /// 关闭系统进程
        /// </summary>
        private void KillProgram(string name)
        {
            try
            {
                Process[] processList = Process.GetProcesses();
                foreach (Process process in processList)
                {
                    if (process.ProcessName == name)
                    {
                        process.Kill();
                        Common.LogHandler.WriteLog("强制停止服务");
                    }
                }
            }
            catch (Exception ex)
            {
                Common.LogHandler.WriteLog("KillProgram出错：", ex);
            }
          
        }
    }
}
