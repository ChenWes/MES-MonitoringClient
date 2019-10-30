using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mes_Update
{
    public partial class frmUpdate : Form
    {
        #region 字段/属性
        Timer unload_timer = new Timer(); //卸载定时器
        Timer deleteService_timer = new Timer(); //删除定时器
        Timer install_timer = new Timer(); //安装定时器
        private string unloadName = null;//卸载程序进程名
        private string installName = null;//安装程序进程名
        private string serviceName = "MES-MonitoringService";//服务程序进程名
        private string serverPath=null;//mes目录
        private string service_folder = @"\Service";//Service文件夹
        private string log_folder = @"\log";//log文件夹
        private string uninsexe = @"C:\Program Files\MES-Monitoring-Client\unins000.exe";//卸载程序目录                                                                    
        private string processName = Process.GetCurrentProcess().ProcessName;   //当前程序名
        private int count = 0;//记时
        Process process = new Process();//定义新进程
        #endregion

        #region 方法
        public frmUpdate(string[] arg)
        {
            InitializeComponent();
            //设置定时器
            unload_timer.Enabled = false;
            unload_timer.Interval = 1000;
            deleteService_timer.Enabled = false;
            deleteService_timer.Interval = 1000;
            install_timer.Enabled = false;
            install_timer.Interval = 1000;
            //如果传参数为8个，则赋值
            if (arg.Length == 8)
            {
                this.txt_Download.Text = arg[0];//下载路径
                uninsexe = arg[1] + @"\unins000.exe";//卸载路径
                this.lab_Version.Text = "版本编号：" + arg[2];
                this.lab_Name.Text = "版本名称：" + arg[3];
                this.lab_Desc.Text = "版本描述：" + arg[4];
                this.lab_Remark.Text = "备    注：" + arg[5];
                this.lab_CreatAt.Text = "创建日期：" + arg[6];
                this.lab_UpdateAt.Text = "更新日期：" + arg[7];
            }
            this.lab_path.Text = "卸载程序所在目录：" + uninsexe;
        }

        public frmUpdate()
        {
        }

        private void frmUpdate_Load(object sender, EventArgs e)
        {
            //是否需要在外部打开程序
           /* if (this.txt_Download.Text == "")
            {
                //KillUpdateProgram();
            }*/
        }
      
        /// <summary>
        /// 下载
        /// </summary>
        //下载
        private void btn_Download_Click(object sender, EventArgs e)
        {
            //停用定时器
            unload_timer.Enabled = false;
            deleteService_timer.Enabled = false;
            install_timer.Enabled = false;
            //记录状态
            this.lab_out.Text = "正在下载";
            //停用安装按钮
            this.buttonInstall.Enabled = false;
            //使用WebClient下载
            using (WebClient wc = new WebClient())
            {

                if (txt_Download.Text.Trim() == "")
                    MessageBox.Show("请输入下载路径");
                else if (txt_SavePath.Text.Trim() == "")
                    MessageBox.Show("请输入文件存储名");
                else if (!txt_SavePath.Text.Trim().EndsWith(".exe"))
                {
                    MessageBox.Show("保存文件格式为exe");
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
                        //判断链接是否可访问，5s延时
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
                            this.buttonInstall.Enabled = true;
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
        /// 等待卸载完成定时器
        /// </summary>
        //等待卸载完成定时器
        private void unins_Tick(object o, EventArgs e)
        {
            //判断卸载进程及卸载程序是否存在
            if (Process.GetProcessesByName(unloadName).Length <= 0&&!File.Exists(uninsexe))
            {

                unload_timer.Enabled = false;
                lab_out.Text = "卸载成功,准备删除残留文件，等待后台服务关闭";
                //System.Threading.Thread.Sleep(2000);
                deleteService_timer.Enabled = true;
                deleteService_timer.Tick += new EventHandler(delete_Tick);
                return;
            }
             else if(Process.GetProcessesByName(unloadName).Length <= 0 && File.Exists(uninsexe))
            {
                EnableBtn(true);    
                this.lab_out.Text = "取消卸载，如取消更新，请注意重启后台服务";
            }
           
        }
        /// <summary>
        /// 等待删除完成定时器
        /// </summary>
        //等待删除完成
        private void delete_Tick(object o, EventArgs e)
        {
            count++;
            if (Process.GetProcessesByName(serviceName).Length <= 0&&!ifexistInstall(installName))
            {
                deleteService_timer.Enabled = false;
                string path = serverPath.Substring(0, serverPath.LastIndexOf(@"\")) + service_folder;
                DeleteDir(path);
                this.lab_out.Text ="成功删除："+ path;
                MessageBox.Show("成功删除："+ path);
                install();
                return;
            }
            //30s后强制结束服务
            if (Process.GetProcessesByName(serviceName).Length > 0 && count > 30)
            {
                KillProgram();
            }
        }
        /// <summary>
        /// 提示下载完成,是否继续安装
        /// </summary>
        //提示下载完成,是否继续安装
        private void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            
            
                this.lab_out.Text = "下载完成";
                this.buttonInstall.Enabled = true;
               //存在卸载程序
                if (File.Exists(this.txt_SavePath.Text) && File.Exists(uninsexe))
                {
                    this.lab_out.Text = "下载完成->等待确认";
                    DialogResult dialogResult = MessageBox.Show("下载完成，是否继续安装", "提示", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        unload();
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        this.lab_out.Text = "下载完成->取消安装";
                    }
                }
                //不存在卸载程序
                else if (File.Exists(this.txt_SavePath.Text) && !File.Exists(uninsexe))
                {
                    this.lab_out.Text = "下载完成->等待确认";
                    DialogResult dialogResult = MessageBox.Show("下载完成，因不存在卸载程序，请确认是否继续安装", "提示", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        install();
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        this.lab_out.Text = "下载完成->取消安装";
                    }

                }
          
          
          

        }
        /// <summary>
        /// 等待安装完成
        /// </summary>
        //等待安装完成
        private void install_Tick(object o, EventArgs e)
        {
            if (Process.GetProcessesByName(installName).Length <= 0&& Process.GetProcessesByName(serviceName).Length >0)
            {
                install_timer.Enabled = false;
                File.Delete(this.txt_SavePath.Text);
                this.lab_out.Text = "更新成功";
                MessageBox.Show("更新成功");
                //KillUpdateProgram();
                return;
            }
            else if(Process.GetProcessesByName(installName).Length <= 0&& Process.GetProcessesByName(serviceName).Length <=0)
            {
                EnableBtn(true);
                install_timer.Enabled = false;
                this.lab_out.Text = "取消安装";
                return;
            }
        }
        /// <summary>
        /// 定义进度条响应事件
        /// </summary>
        //定义进度条响应事件
        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar_Download.Value = e.ProgressPercentage;
        }
        /// <summary>
        /// 开始安装
        /// </summary>
        //开始安装
        private void buttonInstall_Click(object sender, EventArgs e)
        {
            unload();
        }
        /// <summary>
        /// 卸载程序
        /// </summary>
        //卸载程序
        private void unload()
        {
            if (File.Exists(this.txt_SavePath.Text) && File.Exists(uninsexe))
            {
                if (!ifexistInstall(uninsexe))
                {
                    if(Process.GetProcessesByName(serviceName).Length > 0)
                    {
                        KillProgram();
                    }
                    process.StartInfo.FileName = uninsexe;
                    process.Start();
                    System.Threading.Thread.Sleep(50);
                    unloadName = process.ProcessName;
                    serverPath = process.MainModule.FileName;
                    this.lab_out.Text = "正在卸载";
                    EnableBtn(false);
                    unload_timer.Enabled = true;
                    unload_timer.Tick += new EventHandler(unins_Tick);
                }
               
            }
            else if (File.Exists(this.txt_SavePath.Text) && !File.Exists(uninsexe))
            {
                this.lab_out.Text = "等待确认";
                DialogResult dialogResult = MessageBox.Show("因不存在卸载程序，请确认是否继续安装", "提示", MessageBoxButtons.YesNo);
                if ( dialogResult== DialogResult.Yes)
                {
                    install();
                }
                else if (dialogResult == DialogResult.No)
                {
                    EnableBtn(true);
                    this.lab_out.Text = "取消安装";
                }


            }
            else
            {
                this.lab_out.Text = "不存在安装包，请先下载";
                MessageBox.Show("不存在安装包，请先下载");
            }
               
           
        }
        /// <summary>
        /// 打开安装程序
        /// </summary>
        //打开安装程序
        private  void install()
        {
            if(File.Exists(this.txt_SavePath.Text))
            {
                if (!ifexistInstall(installName))
                {
                    process.StartInfo.FileName = this.txt_SavePath.Text;
                    process.Start();
                    System.Threading.Thread.Sleep(50);
                    installName = process.ProcessName;
                    this.lab_out.Text = "正在安装";
                    this.EnableBtn(false);
                    install_timer.Enabled = true;
                    install_timer.Tick += new EventHandler(install_Tick);
                }
            }
            else
            {
                MessageBox.Show("不存在安装包，请先下载");
                return;
            }
            
        }
        /// <summary>
        /// 删除残留文件
        /// </summary>
        //删除残留文件
        private void DeleteDir(string file)
        {
          if (Directory.Exists(file))
                {
                    foreach (string f in Directory.GetFileSystemEntries(file))
                    {
                        if (f == file+log_folder){  }
                        else if (File.Exists(f))
                        {
                            //如果有子文件删除文件
                            File.Delete(f);
                        }
                        else
                        {
                            //循环递归删除子文件夹
                            DeleteDir(f);
                        }
                    }
                    //删除空文件夹
                    //Directory.Delete(file,true);
                }
        }

      
        //关闭窗口前确认
        private void frmUpdate_FormClosing(object sender, FormClosingEventArgs e)
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
        /// <summary>
        /// 关闭服务进程
        /// </summary>
        private void KillProgram()
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
       /* /// <summary>
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
        }*/
        /// <summary>
        /// 控制按钮启用
        /// </summary>
        private void EnableBtn(bool status)
        {
                this.btn_Download.Enabled = status;
                this.buttonInstall.Enabled = status;
        }
        /// <summary>
        /// 判断链接是否可访问
        /// </summary>
        private bool CheckUrlVisit(string url)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Timeout = 5000;
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
        #endregion
    }
}
