using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.ServiceProcess;
using System.Windows.Forms;
using System.Threading;

namespace Mes_Update
{
    public partial class frmUpdate : Form
    {
        #region 字段/属性
        //下载超时时间
        private int timerInterval = Convert.ToInt32(ConfigurationManager.AppSettings["timerInterval"]) * 1000;
        //当前程序名
        private string processName = Process.GetCurrentProcess().ProcessName;   
        //守护服务名
        private string defendServiceName = ConfigurationManager.AppSettings["defendServiceName"].ToString();
        //守护进程名
        private string defendProgramName = ConfigurationManager.AppSettings["defendProgramName"].ToString();
        //MES服务名
        private string MESServiceName = ConfigurationManager.AppSettings["MESServiceName"].ToString();
        //MES服务进程名
        private string MESProgramName = ConfigurationManager.AppSettings["MESProgramName"].ToString();
        //MES系统进程名
        private string MESClientProgramName = ConfigurationManager.AppSettings["MESClientProgramName"].ToString();
        //MES程序目录  
        private string MESClientPath = @"C:\Program Files\MES-Monitoring-Client"+ ConfigurationManager.AppSettings["mesexe"].ToString();
        Process process = new Process();//定义新进程
        Thread thread_Update = null;
        #endregion

        #region 方法
        public frmUpdate(string[] arg)
        {
            InitializeComponent();
            //如果传参数为8个，则赋值
            if (arg.Length == 8)
            {
                this.txt_Download.Text = arg[0];//下载路径
                MESClientPath = arg[1]+ ConfigurationManager.AppSettings["mesexe"].ToString();//MES程序路径
                this.lab_Version.Text = "版本编号：" + arg[2];
                this.lab_Name.Text = "版本名称：" + arg[3];
                this.lab_Desc.Text = "版本描述：" + arg[4];
                this.richTextBox1.Text =  arg[5];
                this.lab_CreatAt.Text = "创建日期：" + arg[6];
                this.lab_UpdateAt.Text = "更新日期：" + arg[7];
            }
          
            
           

        }

        public frmUpdate()
        {
        }

        private void frmUpdate_Load(object sender, EventArgs e)
        {
            Download();
        }
      
        private void start_Update()
        {
            try
            {
                //如果在运行就关闭
                if (CheckSericeStart(defendServiceName, defendProgramName))
                {
                    StopService(defendServiceName, defendProgramName);
                }
                if (CheckSericeStart(MESServiceName, MESProgramName))
                {
                    StopService(MESServiceName, MESProgramName);
                }
                if (ifexistInstall(MESClientProgramName))
                {
                    KillProgram(MESClientProgramName);
                }
                this.Invoke(new Action(() =>
                {
                    this.lab_out.Text = "开始更新文件";
                }));
                string savePath = new DirectoryInfo(Application.StartupPath).Parent.FullName;
                string zipPath = this.txt_SavePath.Text;
                if (Compress(savePath, zipPath, null))
                {
                    this.Invoke(new Action(() =>
                    {
                        this.lab_out.Text = "更新完成";
                    }));
                    //启动服务和程序
                    if (!CheckSericeStart(defendServiceName, defendProgramName))
                    {
                        StartService(defendServiceName, defendProgramName);
                    }
                    if (!CheckSericeStart(MESServiceName, MESProgramName))
                    {
                        StartService(MESServiceName, MESProgramName);
                    }
                    if (!ifexistInstall(MESClientProgramName))
                    {
                        process.StartInfo.FileName = MESClientPath;
                        process.Start();
                    }
                    if (CheckSericeStart(MESServiceName, MESProgramName) && CheckSericeStart(defendServiceName, defendProgramName) && ifexistInstall(MESClientProgramName))
                    {
                        KillUpdateProgram();
                    }
                }
                

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 下载
        /// </summary>
        //下载
        private void btn_Download_Click(object sender, EventArgs e)
        {

            Download();
            //停用安装按钮
           
     
            

        }
        /// <summary>
        /// 下载zip包
        /// </summary>
        private void Download()
        {
            this.lab_out.Text = "正在下载";
            //使用WebClient下载
             using (WebClient wc = new WebClient())
              {

                  if (txt_Download.Text.Trim() == "")
                      MessageBox.Show("请输入下载路径");
                  else if (txt_SavePath.Text.Trim() == "")
                      MessageBox.Show("请输入文件存储名");
                  else if (!txt_SavePath.Text.Trim().EndsWith(".zip"))
                  {
                      MessageBox.Show("保存文件格式为zip");
                  }
                  else
                  {

                      try
                      {
                          //判断目标文件夹是否存在，如不在则创建
                          if (!Directory.Exists(this.txt_SavePath.Text.Substring(0, this.txt_SavePath.Text.LastIndexOf(@"\"))))
                          {
                              Directory.CreateDirectory(this.txt_SavePath.Text.Substring(0, this.txt_SavePath.Text.LastIndexOf(@"\")));
                          }
                          //判断链接是否可访问，20s延时
                          if (CheckUrlVisit(this.txt_Download.Text.Trim()))
                          {
                              wc.Proxy = null;
                              Uri address = new Uri(txt_Download.Text.ToString());
                              wc.DownloadFileAsync(address, txt_SavePath.Text.ToString());
                              //进度条响应
                              wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
                              //下载完成后
                              wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadFileCompleted);
                          }
                          else
                          {
                              MessageBox.Show("下载超时，请检查下载地址和网络并重试");
                          }

                      }
                      catch (Exception ex)
                      {
                          MessageBox.Show(ex.Message);
                      }
                  }
              }

        }
        /// <summary>
        /// 下载完成后开始更新
        /// </summary>
        private void wc_DownloadFileCompleted(Object sender,AsyncCompletedEventArgs e)
        {
            this.btn_Download.Enabled = false;
            ThreadStart threadStart_Update = new ThreadStart(start_Update);//通过ThreadStart委托告诉子线程执行什么方法　　
            thread_Update = new Thread(threadStart_Update);
            thread_Update.Start();//启动新线程
        }
        /// <summary>
        /// 定义进度条响应事件
        /// </summary>
        //定义进度条响应事件
        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar_Download.Value = e.ProgressPercentage;
        }
        //关闭窗口前确认
        private void frmUpdate_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (thread_Update.IsAlive)
            {
                MessageBox.Show("正在更新，不能关闭");
                e.Cancel = false;
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("确定退出吗", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = true;
                }
            }
          
        }
        /// <summary>
        /// 关闭服务进程
        /// </summary>
        private void KillProgram(string serviceName)
        {
            try
            {
                Process[] processList = Process.GetProcesses();
                foreach (Process process in processList)
                {
                    if (process.ProcessName == serviceName)
                    {
                        process.Kill();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            
        }
        /// <summary>
        /// 判断程序是否启动
        /// </summary>
        private Boolean ifexistInstall(string name)
        {
            Process[] processList = Process.GetProcesses();
            foreach (Process process in processList)
            {
                if (process.ProcessName == name)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 关闭系统进程
        /// </summary>
        private void KillUpdateProgram()
        {
            Process[] processList = Process.GetProcesses();
            foreach (Process process in processList)
            {
                if (process.ProcessName == processName)
                {
                    process.Kill();

                }
            }
        }
       
        /// <summary>
        /// 判断链接是否可访问
        /// </summary>
        private bool CheckUrlVisit(string url)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Proxy = null;
                req.Method = "HEAD";
                req.Timeout = timerInterval;
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    resp.Close();
                    return true;
                }
            }
            catch (WebException webex)
            {
                return false;
            }

            return false;

        }

        /// <summary>
        /// 解压Zip
        /// </summary>
        /// <param name="DirPath">解压后存放路径</param>
        /// <param name="ZipPath">Zip的存放路径</param>
        /// <param name="ZipPWD">解压密码（null代表无密码）</param>
        /// <returns></returns>
        private Boolean Compress(string DirPath, string ZipPath, string ZipPWD)
        {
            FastZip fz = new FastZip();
            try
            {
                fz.Password = ZipPWD;
                fz.ExtractZip(ZipPath, DirPath, null);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
           
        }
        /// <summary>
        /// 判断服务有无启动
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// /// <param name="programName">进程名</param>
        private bool CheckSericeStart(string serviceName,string programName)
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
                               
                                KillProgram(programName);
                                result = false;
                            }
                        }
                        //处理服务器一直处于正在启动状态
                        else if (service.Status == ServiceControllerStatus.StartPending)
                        {
                            service.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 30));
                            if (service.Status == ServiceControllerStatus.StartPending)
                            {
                               
                                KillProgram(programName);
                                result = false;
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="serviceName">要启动的服务名称</param>
        private void StartService(string serviceName,string programName)
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
                        if (CheckSericeStart(serviceName,programName))
                        {
                            //lab_out.Text = serviceName+"服务启动成功";
                            break;
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="serviceName">要停止的服务名称</param>
        private void StopService(string serviceName, string programName)
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
                        if (!CheckSericeStart(serviceName, programName))
                        {
                            break;
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion
    }
}
