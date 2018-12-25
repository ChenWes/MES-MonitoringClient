using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Threading;
using System.Diagnostics;

namespace MES_MonitoringClient
{
    public partial class frmMain : Form
    {
        private long COM7_SendDataCount = 0;
        private long COM7_SendDataErrorCount = 0;
        private long COM7_ReceiveDataCount = 0;


        private string defaultSendDataIntervalMilliseconds = Common.ConfigFileHandler.GetAppConfig("SendDataIntervalMilliseconds");
        private string defaultUploadDataServiceName = Common.ConfigFileHandler.GetAppConfig("UploadDataServiceName");
        /*---------------------------------------------------------------------------------------*/

        //向串口6发送的默认信号
        static string mc_DefaultSignal = Common.ConfigFileHandler.GetAppConfig("SendDataDefaultSignal");

        //必须的串口端口
        static string[] mc_DefaultRequiredSerialPortName = Common.ConfigFileHandler.GetAppConfig("CheckSerialPort").Split(',');

        //时间线程方法
        Thread DateTimeThreadClass = null;
        ThreadStart DateTimeThreadFunction = null;
        Common.TimmerHandler TTimerClass = null;

        //发送数据线程方法
        Thread SendDataThreadClass = null;
        ThreadStart SendDataThreadFunction = null;
        Common.TimmerHandler SDTimerClass = null;

        //状态操作类
        static Common.MachineStatusHandler mc_MachineStatusHander = null;

        /*---------------------------------------------------------------------------------------*/
        //后台线程变量
        Thread timerThread = null;

        //定时器变量
        //System.Timers.Timer TTimer;
        //System.Timers.Timer SendDataTimer;


        /*主窗口方法*/
        /*---------------------------------------------------------------------------------------*/

        public frmMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 主窗口加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMain_Load(object sender, EventArgs e)
        {
            try
            {
                //检测代码运行时间
                //var sw = Stopwatch.StartNew();

                //最大化窗口
                this.WindowState = FormWindowState.Maximized;

                //设置默认状态
                mc_MachineStatusHander = new Common.MachineStatusHandler();
                mc_MachineStatusHander.ChangeStatus("Online", "运行", "WesChen", "001A");
                SettingLight();

                //检测端口
                CheckSerialPort(mc_DefaultRequiredSerialPortName);

                //发送数据串口默认配置
                sendDataSerialPortGetDefaultSetting();

                //检测MongoDB服务
                if (!Common.CommonFunction.ServiceRunning(Common.MongodbHandler.MongodbServiceName))
                {
                    if (MessageBox.Show("MongoDB服务未安装或未运行，是否继续运行应用？", "MongoDB服务", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        this.Close();
                    }
                }

                //打开端口
                serialPort6.Open();

                //开始后台进程（更新时间）                
                DateTimeThreadFunction = new ThreadStart(DateTimeTimer);
                DateTimeThreadClass = new Thread(DateTimeThreadFunction);
                DateTimeThreadClass.Start();

                //开始后台进程（定时发送指定数据至指定串口，并自动获取结果）
                SendDataThreadFunction = new ThreadStart(SendDataToSerialPortTimer);
                SendDataThreadClass = new Thread(SendDataThreadFunction);
                SendDataThreadClass.Start();


                //停止检测代码运行时间
                //MessageBox.Show("初始化共使用" + sw.ElapsedMilliseconds.ToString() + "毫秒");
                //sw.Stop();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message, "系统初始化");
                this.Close();
            }
        }

        /// <summary>
        /// 主窗口关闭中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("退出系统后，暂时不会收集到机器的数据，请知悉", "系统退出提醒", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                //定时器
                if (DateTimeThreadClass != null && TTimerClass != null)
                {
                    TTimerClass.StopTimmer();                    

                    DateTimeThreadClass.Abort();
                    DateTimeThreadClass.Join();                    
                }

                //发送数据
                if (SendDataThreadClass != null&& SDTimerClass!=null)
                {
                    SDTimerClass.StopTimmer();

                    SendDataThreadClass.Abort();
                    SendDataThreadClass.Join();
                }

                //串口关闭
                if (serialPort6.IsOpen)
                {
                    serialPort6.Close();
                }

                //关闭程序前先保存数据
                mc_MachineStatusHander.AppWillClose_SaveData();
                
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        /*定时器方法*/
        /*---------------------------------------------------------------------------------------*/

        /// <summary>
        /// 显示时间定时器
        /// </summary>
        private void DateTimeTimer()
        {
            try
            {
                TTimerClass = new Common.TimmerHandler(1000, true, (o, a) =>
                {
                    SetDateTime();
                }, true);
            }
            catch (Exception ex)
            {
                TTimerClass = null;
            }
        }

        /// <summary>
        /// 发送AA1086至串口7定时器
        /// </summary>
        private void SendDataToSerialPortTimer()
        {
            try
            {
                long timeInterval = 0;
                long.TryParse(defaultSendDataIntervalMilliseconds, out timeInterval);

                SDTimerClass = new Common.TimmerHandler(timeInterval, true, (o, a) =>
                {
                    SendDataToSerialPort(mc_DefaultSignal);
                }, true);
            }
            catch (Exception ex)
            {
                SDTimerClass = null;
            }
        }

        /*定时器委托*/
        /*---------------------------------------------------------------------------------------*/

        /// <summary>
        /// 声明显示当前时间委托
        /// </summary>
        private delegate void SetDateTimeDelegate();
        private void SetDateTime()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    try
                    {
                        this.Invoke(new SetDateTimeDelegate(SetDateTime));
                    }
                    catch (Exception ex)
                    {
                        //响铃并显示异常给用户
                        System.Media.SystemSounds.Beep.Play();
                    }
                }
                else
                {
                    try
                    {
                        lab_DateTime.Text = string.Format("当前时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                        lab_CurrentStatusTotalTime.Text = "当前状态:[" + mc_MachineStatusHander.StatusDescription + "][" + Common.CommonFunction.FormatMilliseconds(mc_MachineStatusHander.HoldStatusTotalMilliseconds) + "]";



                        //后台上传数据服务状态                        
                        Common.CommonFunction.ServiceStatus getServiceStatus = Common.CommonFunction.GetServiceStatus(defaultUploadDataServiceName);
                        //显示不同的文字及颜色
                        if (getServiceStatus == Common.CommonFunction.ServiceStatus.NoInstall)
                        {
                            lab_UploadDataServiceStatus.Text = "后台上传数据服务未安装，请联系管理员。";
                            lab_UploadDataServiceStatus.BackColor = System.Drawing.Color.FromArgb(255, 61, 0);
                        }
                        else if (getServiceStatus == Common.CommonFunction.ServiceStatus.Running)
                        {
                            lab_UploadDataServiceStatus.Text = "后台上传数据服务正常运行";
                            lab_UploadDataServiceStatus.BackColor = System.Drawing.Color.FromArgb(0, 230, 118);
                        }
                        else if (getServiceStatus == Common.CommonFunction.ServiceStatus.Stopped)
                        {
                            lab_UploadDataServiceStatus.Text = "后台上传数据服务已停止，请联系管理员。";
                            lab_UploadDataServiceStatus.BackColor = System.Drawing.Color.FromArgb(221, 221, 0);
                        }
                    }
                    catch (Exception ex)
                    {
                        //响铃并显示异常给用户
                        System.Media.SystemSounds.Beep.Play();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message, "设置当前时间错误");
                TTimerClass = null;
            }
        }

        /// <summary>
        /// 声明发送数据至串口委托
        /// </summary>
        /// <param name="defaultSignal"></param>
        private delegate void SendDataToSerialPortDelegate(string defaultSignal);
        private void SendDataToSerialPort(string defaultSignal)
        {
            try
            {

                if (this.InvokeRequired)
                {
                    SendDataToSerialPortDelegate sendDataToSerialPortDelegate = SendDataToSerialPort;
                    try
                    {
                        this.Invoke(sendDataToSerialPortDelegate, new object[] { defaultSignal });
                    }
                    catch (Exception ex)
                    {
                        //响铃并显示异常给用户
                        System.Media.SystemSounds.Beep.Play();
                    }
                }
                else
                {
                    try
                    {
                        if (serialPort6.IsOpen)
                        {
                            //转码后再发送
                            //byte[] byteArray = System.Text.Encoding.Default.GetBytes(defaultSignal);
                            //serialPort7.Write(byteArray, 0, byteArray.Length);

                            //不转码直接发送
                            serialPort6.Write(defaultSignal);

                            //发送的大小
                            COM7_SendDataCount += 1;
                            lab_SendSuccessCount.Text = "发送成功：" + COM7_SendDataCount;
                            lab_SendSuccessCount.BackColor = System.Drawing.Color.FromArgb(0, 230, 118);
                        }
                    }
                    catch (Exception ex)
                    {
                        COM7_SendDataErrorCount += 1;
                        lab_SendErrorCount.Text = "发送失败：" + COM7_SendDataErrorCount;
                        lab_SendErrorCount.BackColor= System.Drawing.Color.FromArgb(221, 221, 0);

                        //响铃并显示异常给用户
                        System.Media.SystemSounds.Beep.Play();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message, "发送数据至串口错误");
                
                SDTimerClass = null;
            }
        }


        /*窗口公共方法*/
        /*---------------------------------------------------------------------------------------*/

        /// <summary>
        /// 检测串口
        /// </summary>
        /// <param name="checkPortList"></param>
        private void CheckSerialPort(string[] checkPortList)
        {
            string[] havePortList = System.IO.Ports.SerialPort.GetPortNames();

            if (checkPortList != null && checkPortList.Length > 0)
            {
                foreach (string needPort in checkPortList)
                {
                    bool checkErrorFlag = true;
                    foreach (string havePort in havePortList)
                    {
                        if (havePort.ToUpper() == needPort.ToUpper())
                        {
                            checkErrorFlag = false;
                            continue;
                        }
                    }

                    if (checkErrorFlag)
                    {
                        throw new Exception("电脑无法检测到串口[" + needPort + "]，请联系管理员");
                    }
                }
            }
        }

        /// <summary>
        /// 设置红绿灯
        /// </summary>
        private void SettingLight()
        {
            btn_StatusLight.Text = mc_MachineStatusHander.StatusDescription;
            btn_StatusLight.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);            

            if (mc_MachineStatusHander.mc_StatusLight == Common.MachineStatusHandler.enumStatusLight.Red)
            {
                btn_StatusLight.BackColor = System.Drawing.Color.FromArgb(255, 61, 0);// Common.CommonFunction.colorHx16toRGB(Common.CommonFunction.colorRGBtoHx16(255, 61, 0));
            }
            else if (mc_MachineStatusHander.mc_StatusLight == Common.MachineStatusHandler.enumStatusLight.Green)
            {
                btn_StatusLight.BackColor = System.Drawing.Color.FromArgb(0, 230, 118);// Common.CommonFunction.colorHx16toRGB(Common.CommonFunction.colorRGBtoHx16(0, 230, 118));
            }
            else if (mc_MachineStatusHander.mc_StatusLight == Common.MachineStatusHandler.enumStatusLight.Yellow)
            {
                btn_StatusLight.BackColor = System.Drawing.Color.FromArgb(221, 221, 0);// Common.CommonFunction.colorHx16toRGB(Common.CommonFunction.colorRGBtoHx16(221, 221, 0));
            }
        }

        /// <summary>
        /// 显示系统错误信息
        /// </summary>
        /// <param name="errorTitle">错误标题</param>
        /// <param name="errorMessage">错误</param>
        private void ShowErrorMessage(string errorMessage, string errorTitle)
        {
            MessageBox.Show(errorMessage, errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 查找后台上传数据服务状态
        /// </summary>
        //private void ShowBackupServiceStatus()
        //{
        //    Common.CommonFunction.ServiceStatus getServiceStatus = Common.CommonFunction.GetServiceStatus(defaultUploadDataServiceName);

        //    if (getServiceStatus == Common.CommonFunction.ServiceStatus.NoInstall)
        //    {
        //        lab_UploadDataServiceStatus.Text = "后台上传数据服务未安装，请联系管理员。";
        //    }
        //    else if (getServiceStatus == Common.CommonFunction.ServiceStatus.Running)
        //    {
        //        lab_UploadDataServiceStatus.Text = "后台上传数据服务正常运行";
        //    }
        //    else if (getServiceStatus == Common.CommonFunction.ServiceStatus.Stopped)
        //    {
        //        lab_UploadDataServiceStatus.Text = "后台上传数据服务已停止，请联系管理员。";
        //    }
        //}

        /*获取串口数据事件*/
        /*---------------------------------------------------------------------------------------*/

        /// <summary>
        /// 串口6获取数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serialPort6_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {                
                System.IO.Ports.SerialPort COM = (System.IO.Ports.SerialPort)sender;

                StringBuilder stringBuilder = new StringBuilder();
                do
                {
                    int count = COM.BytesToRead;
                    if (count <= 0)
                        break;
                    byte[] readBuffer = new byte[count];

                    Application.DoEvents();
                    COM.Read(readBuffer, 0, count);

                    stringBuilder.Append(System.Text.Encoding.Default.GetString(readBuffer));

                } while (COM.BytesToRead > 0);

                //更改状态
                mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal(stringBuilder.ToString());

                this.Invoke((EventHandler)(delegate
                {
                    richTextBox1.AppendText(stringBuilder.ToString() + "\r\n");

                    COM7_ReceiveDataCount += 1;
                    lab_ReceviedDataCount.Text = "接收成功：" + COM7_ReceiveDataCount;
                    lab_ReceviedDataCount.BackColor= System.Drawing.Color.FromArgb(0, 230, 118);


                    lab_ProductCount.Text = "累计生产数量：" + mc_MachineStatusHander.mc_MachineProduceStatusHandler.LifeCycleCount;
                    lab_LastLifeCycleTime.Text = "最后一次生产用时：" + Common.CommonFunction.FormatMilliseconds(mc_MachineStatusHander.mc_MachineProduceStatusHandler.LastLifeCycleMilliseconds);
                }
                   )
                );

                #region 老代码

                //接收数据
                //string getData = "";
                //do
                //{
                //    int byteCount = COM.BytesToRead;
                //    if (byteCount <= 0)
                //        break;
                //    byte[] readBuffer = new byte[byteCount];

                //    Application.DoEvents();

                //    COM.Read(readBuffer, 0, byteCount);
                //    getData += System.Text.Encoding.Default.GetString(readBuffer);


                //} while (COM.BytesToRead > 0);

                //    因为要访问UI资源，所以需要使用invoke方式同步ui
                //    this.Invoke((EventHandler)(delegate
                //    {
                //        receviedDataCount += byteCount;
                //        label3.Text = "接收成功：" + receviedDataCount;

                //        改变状态
                //        mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal(getData);
                //    }
                //       )
                //    );

                #endregion

            }
            catch (Exception ex)
            {
                //响铃并显示异常给用户
                System.Media.SystemSounds.Beep.Play();
            }
        }

        /// <summary>
        /// 发送数据串口默认配置
        /// </summary>
        private void sendDataSerialPortGetDefaultSetting()
        {
            //端口名称
            serialPort6.PortName = Common.ConfigFileHandler.GetAppConfig("SendDataSerialPortName");

            //波特率
            int defaultBaudRate = 0;
            int.TryParse(Common.ConfigFileHandler.GetAppConfig("SendDataSerialBaudRate"), out defaultBaudRate);
            serialPort6.BaudRate = defaultBaudRate;

            //奇偶性验证
            string defaultParity = Common.ConfigFileHandler.GetAppConfig("SendDataSerialParity");
            if (defaultParity.ToUpper() == System.IO.Ports.Parity.None.ToString().ToUpper())
            {
                serialPort6.Parity = System.IO.Ports.Parity.None;
            }
            else if (defaultParity.ToUpper() == System.IO.Ports.Parity.Odd.ToString().ToUpper())
            {
                serialPort6.Parity = System.IO.Ports.Parity.Odd;
            }
            else if (defaultParity.ToUpper() == System.IO.Ports.Parity.Even.ToString().ToUpper())
            {
                serialPort6.Parity = System.IO.Ports.Parity.Even;
            }
            else if (defaultParity.ToUpper() == System.IO.Ports.Parity.Mark.ToString().ToUpper())
            {
                serialPort6.Parity = System.IO.Ports.Parity.Mark;
            }
            else if (defaultParity.ToUpper() == System.IO.Ports.Parity.Space.ToString().ToUpper())
            {
                serialPort6.Parity = System.IO.Ports.Parity.Space;
            }

            //数据位
            int defaultDataBits = 0;
            int.TryParse(Common.ConfigFileHandler.GetAppConfig("SendDataSerialDataBits"), out defaultDataBits);            
            serialPort6.DataBits = defaultDataBits;

            //停止位
            string defaultStopBits = Common.ConfigFileHandler.GetAppConfig("SendDataSerialStopBits");
            if (defaultStopBits.ToUpper() == System.IO.Ports.StopBits.None.ToString().ToUpper())
            {
                serialPort6.StopBits = System.IO.Ports.StopBits.None;
            }
            else if (defaultStopBits.ToUpper() == System.IO.Ports.StopBits.One.ToString().ToUpper())
            {
                serialPort6.StopBits = System.IO.Ports.StopBits.One;
            }
            else if (defaultStopBits.ToUpper() == System.IO.Ports.StopBits.OnePointFive.ToString().ToUpper())
            {
                serialPort6.StopBits = System.IO.Ports.StopBits.OnePointFive;
            }
            else if (defaultStopBits.ToUpper() == System.IO.Ports.StopBits.Two.ToString().ToUpper())
            {
                serialPort6.StopBits = System.IO.Ports.StopBits.Two;
            }

        }

        /*按钮事件*/
        /*---------------------------------------------------------------------------------------*/

        /// <summary>
        /// 模拟信号，验证生产逻辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {            
            //X03
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");


            //X01+X03   开模
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            //X03
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            //X02+X03   射胶
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0600ZZ");
            //X03
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");


            //X01+X03   开模
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            //X03
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            //X02+X03   射胶
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0600ZZ");
            //X03
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");


            //X01+X03   开模
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            //X03
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            //X02+X03   射胶
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0600ZZ");
            //X03
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");


            //X01+X03   开模
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            //X03
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
        }

        /// <summary>
        /// 关闭系统
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_CloseWindow_Click(object sender, EventArgs e)
        {            
            this.Close();
        }

        /// <summary>
        /// 更改机器状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_StatusLight_Click(object sender, EventArgs e)
        {
            try
            {
                frmChangeStatus newfrmChangeStatus = new frmChangeStatus();
                newfrmChangeStatus.ShowDialog();

                //返回的参数
                string newOperatePersonCardID = newfrmChangeStatus.OperatePersonCardID;
                string newOperatePersonName = newfrmChangeStatus.OperatePersonName;

                string strNewStatusCode = newfrmChangeStatus.NewStatusCode;
                string strNewStatusString = newfrmChangeStatus.NewStatusString;

                //更新状态
                if (!string.IsNullOrEmpty(strNewStatusString) && strNewStatusString != mc_MachineStatusHander.StatusDescription)
                {
                    mc_MachineStatusHander.ChangeStatus(strNewStatusCode, strNewStatusString, newOperatePersonName, newOperatePersonCardID);
                    SettingLight();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message, "更新机器状态错误");
            }
        }
    }
}
