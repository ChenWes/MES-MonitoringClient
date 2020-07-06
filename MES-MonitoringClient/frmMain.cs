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

using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;

using MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Globalization;

namespace MES_MonitoringClient
{
    public partial class frmMain : Form
    {
        //数字为发送及接收数据状态的灯
        //默认是0，发送数据后变成255，而后10毫秒减10，直至0
        //从而形成闪烁的效果
        private int SendDataSuccessColor = 0;
        private int ReceiveDataSuccessColor = 0;

        //关机日志
        private IMongoCollection<DataModel.OffPowerLog> MC_OffPowerLogCollection;
        static string defaultOffPowerLogMongodbCollectionName = Common.ConfigFileHandler.GetAppConfig("OffPowerLogCollectionName");

        //默认的生产的编号
        //private string MC_ProduceStatusCode = "Produce";

        //发送串口数据信号时间间隔
        private long sendDataTimeInterval = 0;        

        //发送数据时间间隔，以毫秒计
        private string defaultSendDataIntervalMilliseconds = Common.ConfigFileHandler.GetAppConfig("SendDataIntervalMilliseconds");

        //数据上传服务名称，作为界面中显示使用
        private string defaultUploadDataServiceName = Common.ConfigFileHandler.GetAppConfig("UploadDataServiceName");

        //本机使用
        private DataModel.Machine MC_Machine = null;

        /*---------------------------------------------------------------------------------------*/

        //向串口6发送的默认信号（默认是AA10086）
        static string mc_DefaultSignal = Common.ConfigFileHandler.GetAppConfig("SendDataDefaultSignal");
        //必须的串口端口
        static string[] mc_DefaultRequiredSerialPortName = {Common.ConfigFileHandler.GetAppConfig("RFIDSerialPortName"), Common.ConfigFileHandler.GetAppConfig("SendDataSerialPortName") };

        //时间线程方法
        Thread DateTimeThreadClass = null;
        ThreadStart DateTimeThreadFunction = null;
        Common.TimmerHandler TTimerClass = null;

        //发送数据线程方法
        Thread SendDataThreadClass = null;
        ThreadStart SendDataThreadFunction = null;
        Common.TimmerHandler SDTimerClass = null;

        //发送数据线程方法
        Thread MachineTemperatureThreadClass = null;
        ThreadStart MachineTemperatureThreadFunction = null;
        Common.TimmerHandler MachineTemperatureTimerClass = null;

        //状态灯
        Thread StatusLightThreadClass = null;
        ThreadStart StatusLightThreadFunction = null;
        Common.TimmerHandler StatusLightTimerClass = null;

        //检测更新定时器
        Common.TimmerHandler checkUpdateTimerClass = null;
        Thread thread_getJsonTimer = null;
        //更新图片定时器
        Thread UpdateImageThreadClass = null;
        ThreadStart UpdateImageThreadFunction = null;
        Common.TimmerHandler UpdateImageTimerClass = null;
        /*---------------------------------------------------------------------------------------*/

        //状态操作类
        static Common.MachineStatusHandler mc_MachineStatusHander = null;

        //温度状态表
        public ChartValues<DataModel.MachineTemperature> MachineTemperature1ChartValues { get; set; }

        /*---------------------------------------------------------------------------------------*/

        /*---------------------------------------------------------------------------------------*/
        //当前程序名
        private string processName = Process.GetCurrentProcess().ProcessName;
        //更新地址
        private string update_Path = new DirectoryInfo(Application.StartupPath).Parent.Parent.FullName+ @"\Mes_Update\Mes_Update.exe";
        //旧版本
        string oldVersion = Common.ConfigFileHandler.GetAppConfig("Version");
        //更新密码
        string password= Common.ConfigFileHandler.GetAppConfig("Update_Password");
        //获取服务端新版本信息
        string jsonString = null;
        private int click =0;
        private int Registerclick = 0;
        private DateTime clickTime ;
        private bool listening = false;//是否没有执行完invoke相关操作
        private bool closing = false;//是否正在关闭串口，执行Application.DoEvents，并阻止再次invoke
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
        bool haveEmployee = false;
        bool haveQC = false;
        bool haveHead = false;
        bool haveCharge = false;
        //记录领班
        public static DataModel.Employee head = null;
        //记录组长
        public static DataModel.Employee charge = null;
        //当前班次开始时间
        private DateTime? nowStartTime = null;
        //当前班次结束时间
        private DateTime? nowEndTime = null;
        //照片地址
        string imagePath= Common.ConfigFileHandler.GetAppConfig("EmployeeImageFolder");

        //安装包名
        //private string nstallation_package_name = null;
        //使用WebClient下载
        //启动程序目录
        //后台线程变量
        //Thread timerThread = null;

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
                //最大化窗口
                this.WindowState = FormWindowState.Maximized;

                //检测端口
                CheckSerialPort(mc_DefaultRequiredSerialPortName);

                //发送数据串口默认配置
                sendDataSerialPortGetDefaultSetting();

                //检测MongoDB服务
                if (!Common.CommonFunction.ServiceRunning(Common.MongodbHandler.MongodbServiceName) && Common.ConfigFileHandler.GetAppConfig("CheckMongoDBService") == "1")
                {
                    //if (MessageBox.Show("MongoDB服务未安装或未运行，请关闭后重新启动", "MongoDB服务", MessageBoxButtons.OK, MessageBoxIcon.Question) == DialogResult.OK)
                    //{
                    //    this.Close();
                    //}
                    throw new Exception("MongoDB服务未安装或未运行，请关闭应用后并确认MongoDB服务状态再重新启动");
                }

                //设置机器默认状态
                mc_MachineStatusHander = new Common.MachineStatusHandler();
                mc_MachineStatusHander.UpdateMachineStatusPieChartDelegate += UpdateMachineStatusPieChart;//状态更新方法（更新饼图）
                mc_MachineStatusHander.UpdateMachineStatusLightDelegate += UpdateMachineStatusLight;//状态灯更新方法（状态灯）
                mc_MachineStatusHander.UpdateMachineStatusTotalDateTimeDelegate += UpdateMachineStatusTotalDateTime;//状态总时间更新方法（状态总时间）

                //设置机器生命周期信号状态
                mc_MachineStatusHander.mc_MachineProduceStatusHandler.UpdateMachineCompleteDateTimeDelegate += UpdateMachineCompleteDateTime;//预计完成时间更新方法（预计完成时间）
                mc_MachineStatusHander.mc_MachineProduceStatusHandler.UpdateMachineSignalDelegate += UpdateMachineSignalStatus;//信号更新方法（更新信号灯）
                mc_MachineStatusHander.mc_MachineProduceStatusHandler.UpdateMachineLifeCycleTimeDelegate += UpdateMachineLifeCycleTime;//更新产品生命周期（实际生产时间）
                mc_MachineStatusHander.mc_MachineProduceStatusHandler.UpdateMachineNondefectiveCountDelegate += UpdateMachineNondefectiveCount;//良品更新方法（良品数量）
                mc_MachineStatusHander.mc_MachineProduceStatusHandler.UpdateMachineNoCompleteCountDelegate += UpdateMachineNoCompletedCount;//未完成产品数量更新方法（未完成产品数量）
                mc_MachineStatusHander.mc_MachineProduceStatusHandler.ShowJobOrderBasicInfoDelegate += ShowJobOrderBiaisInfo;//显示工单基本信息

                //显示机器名称                
                CheckMachineRegister();
                //获取饼图
                mc_MachineStatusHander.ShowStatusPieChart();


             
                //初始化最后一次机器状态
                mc_MachineStatusHander.GetLatestMachineStatusLog();

              
                //打开端口
                if (!serialPort6.IsOpen)
                {
                    serialPort6.Open();
                }

                //开始后台进程（更新时间）                
                DateTimeThreadFunction = new ThreadStart(DateTimeTimer);
                DateTimeThreadClass = new Thread(DateTimeThreadFunction);
                DateTimeThreadClass.Start();

                //发送串口数据信号时间间隔
                long.TryParse(defaultSendDataIntervalMilliseconds, out sendDataTimeInterval);

                //开始后台进程（定时发送指定数据至指定串口，并自动获取结果）
                SendDataThreadFunction = new ThreadStart(SendDataToSerialPortTimer);
                SendDataThreadClass = new Thread(SendDataThreadFunction);
                SendDataThreadClass.Start();


                //开始后台进程（状态灯）                
                StatusLightThreadFunction = new ThreadStart(StatusLightTimer);
                StatusLightThreadClass = new Thread(StatusLightThreadFunction);
                StatusLightThreadClass.Start();

                //显示版本号
                this.lab_Version.Text = this.lab_Version.Text+oldVersion;

                this.lab_log.Text = "正在检测";

               

                //检测更新
                ThreadStart threadStart_getJsonTimer = new ThreadStart(CheckUpdateTimer);　
                thread_getJsonTimer = new Thread(threadStart_getJsonTimer);
                thread_getJsonTimer.Start();//启动新线程
                //显示打卡
                showClockIn();

                if (MC_Machine != null)
                {
                    //显示最后一次工单信息
                    ShowLastJobOrderBiaisInfo(0);
                    UpdateImageThreadFunction = new ThreadStart(UpdateImageTimer);
                    UpdateImageThreadClass = new Thread(UpdateImageThreadFunction);
                    UpdateImageThreadClass.Start();//启动新线程
                }
              
             
                //NowVersion();
                
                #region 开机后设置默认参数，直接运行，该功能只作为收集机器信号稳定性测试，正式功能需要删除该代码

                //txt_WorkOrderCount.Text = "999999";
                //txt_PlanWorkTime.Text = "50";                
                //btn_Start_Click(null, null);

                #endregion

                #region 温度代码
                //开始后台进程（定时获取机器温度）
                //MachineTemperatureThreadFunction = new ThreadStart(MachineTemperatureTimer);
                //MachineTemperatureThreadClass = new Thread(MachineTemperatureThreadFunction);
                //MachineTemperatureThreadClass.Start();

                //转换成
                //var mapper = Mappers.Xy<DataModel.MachineTemperature>()
                //.X(model => model.DateTime.Ticks)   //use DateTime.Ticks as X
                //.Y(model => model.Temperature);           //use the value property as Y

                //lets save the mapper globally.
                //Charting.For<DataModel.MachineTemperature>(mapper);
                //实例化数据列表
                //MachineTemperature1ChartValues = new ChartValues<DataModel.MachineTemperature>();
                //图示例
                //cartesianChart_temperature.DataTooltip.Background =System.Windows.Media.Brushes.White;

                //图表数据
                //cartesianChart_temperature.Series = new SeriesCollection
                //{
                //    new LineSeries
                //    {
                //        Title="机器状态",
                //        Values = MachineTemperature1ChartValues,
                //        PointGeometrySize = 5,
                //        //PointGeometry = null,
                //        PointGeometry = DefaultGeometries.Circle,
                //        StrokeThickness = 1,                        
                //    }
                //};

                //图表时间间隔
                //cartesianChart_temperature.AxisX.Add(new Axis
                //{
                //    DisableAnimations = true,
                //    LabelFormatter = value => new System.DateTime((long)value).ToString("HH:mm:ss"),
                //    Separator = new Separator
                //    {
                //        Step = TimeSpan.FromSeconds(10).Ticks
                //    }
                //});

                //SetAxisLimits(System.DateTime.Now);
                #endregion

            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message, "系统初始化");
                Common.LogHandler.WriteLog("系统初始化失败", ex);
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
            try
            {
                if (!Common.CommonFunction.ServiceRunning(Common.MongodbHandler.MongodbServiceName) && Common.ConfigFileHandler.GetAppConfig("CheckMongoDBService") == "1")
                {
                    e.Cancel = false;
                }
                else
                {
                    if (MessageBox.Show("退出系统后，暂时不会收集到机器的数据，请知悉", "系统退出提醒", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                    {
                        frmScanRFID newfrmScanRFID = new frmScanRFID();
                        newfrmScanRFID.MC_OperationType = frmScanRFID.OperationType.OffPower;
                        newfrmScanRFID.MC_OperationType_Prompt = frmScanRFID.OperationType_Prompt.Quit;
                        newfrmScanRFID.ShowDialog();

                        if (!newfrmScanRFID.MC_IsManualCancel)
                        {
                            MC_OffPowerLogCollection = Common.MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.OffPowerLog>(defaultOffPowerLogMongodbCollectionName);

                            //保存一条关机记录至数据库
                            DataModel.OffPowerLog offPowerLog = new DataModel.OffPowerLog();

                            offPowerLog.EmployeeID = newfrmScanRFID.MC_EmployeeInfo._id;
                            offPowerLog.EmployeeName = newfrmScanRFID.MC_EmployeeInfo.EmployeeName;
                            offPowerLog.DateTime = System.DateTime.Now;
                            offPowerLog.Remark = "操作员主动关闭应用";

                            //保存数据
                            MC_OffPowerLogCollection.InsertOne(offPowerLog);
                        }
                        else
                        {
                            //员工不想刷卡，退出
                            e.Cancel = true;
                            return;
                        }
                        //检测更新
                        if (thread_getJsonTimer != null && checkUpdateTimerClass != null)
                        {
                            checkUpdateTimerClass.StopTimmer();
                            thread_getJsonTimer.Abort();
                        }
                        //更新照片
                        if (UpdateImageThreadClass != null && UpdateImageTimerClass != null)
                        {
                            UpdateImageTimerClass.StopTimmer();
                            UpdateImageThreadClass.Abort();
                        }
                        //定时器
                        if (DateTimeThreadClass != null && TTimerClass != null)
                        {
                            TTimerClass.StopTimmer();
                            DateTimeThreadClass.Abort();
                        }

                        //发送数据
                        if (SendDataThreadClass != null && SDTimerClass != null)
                        {
                            SDTimerClass.StopTimmer();
                            SendDataThreadClass.Abort();
                        }

                        //状态灯
                        if (StatusLightThreadClass != null && StatusLightTimerClass != null)
                        {
                            StatusLightTimerClass.StopTimmer();
                            StatusLightThreadClass.Abort();
                        }

                        //串口关闭
                        if (serialPort6.IsOpen)
                        {
                            closing = true;

                            while (listening) Application.DoEvents();
                            serialPort6.Close();
                        }

                        //关闭程序前先保存数据
                        if (mc_MachineStatusHander != null)
                        {
                            //mc_MachineStatusHander.AppWillClose_SaveData();
                        }

                        e.Cancel = false;
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message, "退出系统错误");
                Common.LogHandler.WriteLog("退出系统错误", ex);
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
        /// 显示状态灯
        /// </summary>
        private void StatusLightTimer()
        {
            try
            {
                StatusLightTimerClass = new Common.TimmerHandler(10, true, (o, a) =>
                {
                    SetStatusLight();
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
                SDTimerClass = new Common.TimmerHandler(sendDataTimeInterval, true, (o, a) =>
                {
                    SendDataToSerialPort(mc_DefaultSignal);
                }, true);
            }
            catch (Exception ex)
            {
                SDTimerClass = null;
            }
        }
        /// <summary>
        /// 定时检测版本
        /// </summary>
        private void CheckUpdateTimer()
        {
            //1小时检测
            getJson();
            checkUpdateTimerClass = new Common.TimmerHandler(60*60*1000, true, (o, a) =>
            {
                getJson();
            }, true);
        }

        /// <summary>
        /// 定时更新图片
        /// </summary>
        private void UpdateImageTimer()
        {
            //1分钟检测
            showHeadAndCharge();
            UpdateImageTimerClass = new Common.TimmerHandler(10*1000, true, (o, a) =>
            {
                showHeadAndCharge();
            }, true);

        }
        /// <summary>
        /// 获取温度定时器
        /// </summary>
        private void MachineTemperatureTimer()
        {
            try
            {
                MachineTemperatureTimerClass = new Common.TimmerHandler(1000, true, (o, a) =>
                {
                    GetMachineTemperature();

                }, true);
            }
            catch (Exception ex)
            {
                MachineTemperatureTimerClass = null;
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
                Common.LogHandler.WriteLog("设置当前时间错误", ex);
                TTimerClass = null;
            }
        }

        /// <summary>
        /// 发送信号及接收信号状态灯委托
        /// </summary>
        private delegate void SetStatusLightDelegate();
        private void SetStatusLight()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    try
                    {
                        this.Invoke(new SetDateTimeDelegate(SetStatusLight));
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
                        if (SendDataSuccessColor != 0)
                        {
                            btn_SendDataLight.BackColor = Color.FromArgb(SendDataSuccessColor, SendDataSuccessColor, SendDataSuccessColor);

                            SendDataSuccessColor = SendDataSuccessColor - 30;
                            if (SendDataSuccessColor <= 0)
                            {
                                SendDataSuccessColor = 0;
                            }
                        }

                        if (ReceiveDataSuccessColor != 0)
                        {
                            btn_ReceiveDataLight.BackColor = Color.FromArgb(ReceiveDataSuccessColor, ReceiveDataSuccessColor, ReceiveDataSuccessColor);

                            ReceiveDataSuccessColor = ReceiveDataSuccessColor - 30;
                            if (ReceiveDataSuccessColor <= 0)
                            {
                                ReceiveDataSuccessColor = 0;
                            }
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
                ShowErrorMessage(ex.Message, "设置信号灯错误");
                Common.LogHandler.WriteLog("设置信号灯错误", ex);
                StatusLightTimerClass = null;
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
                            //闪灯
                            SendDataSuccessColor = 255;                         
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
                ShowErrorMessage(ex.Message, "发送数据至串口错误");
                Common.LogHandler.WriteLog("发送数据至串口错误", ex);
                SDTimerClass = null;
            }
        }

        /// <summary>
        /// 声明获取机器温度委托
        /// </summary>
        private delegate void GetMachineTemperatureDelegate();
        private void GetMachineTemperature()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    try
                    {
                        this.Invoke(new GetMachineTemperatureDelegate(GetMachineTemperature));
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
                        //定时增加数字
                        MachineTemperature1ChartValues.Add(new DataModel.MachineTemperature
                        {
                            DateTime = System.DateTime.Now,
                            Temperature = new System.Random().Next(90, 100)
                        });

                        //超过一定数值则删除
                        if (MachineTemperature1ChartValues.Count > 100) MachineTemperature1ChartValues.RemoveAt(0);

                        //处理横轴
                        //SetAxisLimits(System.DateTime.Now);
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
                ShowErrorMessage(ex.Message, "获取当前机器温度");
                Common.LogHandler.WriteLog("获取当前机器温度", ex);
                MachineTemperatureTimerClass = null;
            }
        }



        /// <summary>
        /// 获取机器信号，并更新界面信号灯
        /// </summary>
        /// <param name="singnal">机器信号</param>
        delegate void UpdateMachineProduceSignalDelegate(Common.MachineProduceStatusHandler.SignalType singnal);
        private void UpdateMachineSignalStatus(Common.MachineProduceStatusHandler.SignalType signal)
        {
            if (InvokeRequired)
            {
                this.Invoke(new UpdateMachineProduceSignalDelegate(delegate (Common.MachineProduceStatusHandler.SignalType s)
                {
                    if (signal.ToString().IndexOf("X01") != -1)
                    {
                        btn_SignalX01.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
                        btn_SignalX01.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
                    }
                    else
                    {
                        btn_SignalX01.BackColor = System.Drawing.Color.FromArgb(38, 45, 58);
                        btn_SignalX01.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
                    }

                    if (signal.ToString().IndexOf("X02") != -1)
                    {
                        btn_SignalX02.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
                        btn_SignalX02.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
                    }
                    else
                    {
                        btn_SignalX02.BackColor = System.Drawing.Color.FromArgb(38, 45, 58);
                        btn_SignalX02.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
                    }

                    if (signal.ToString().IndexOf("X03") != -1)
                    {
                        btn_SignalX03.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
                        btn_SignalX03.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
                    }
                    else
                    {
                        btn_SignalX03.BackColor = System.Drawing.Color.FromArgb(38, 45, 58);
                        btn_SignalX03.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
                    }
                }), signal);
            }
            else
            {
                if (signal.ToString().IndexOf("X01") != -1)
                {
                    btn_SignalX01.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
                    btn_SignalX01.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
                }
                else
                {
                    btn_SignalX01.BackColor = System.Drawing.Color.FromArgb(38, 45, 58);
                    btn_SignalX01.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
                }

                if (signal.ToString().IndexOf("X02") != -1)
                {
                    btn_SignalX02.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
                    btn_SignalX02.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
                }
                else
                {
                    btn_SignalX02.BackColor = System.Drawing.Color.FromArgb(38, 45, 58);
                    btn_SignalX02.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
                }

                if (signal.ToString().IndexOf("X03") != -1)
                {
                    btn_SignalX03.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
                    btn_SignalX03.ForeColor = System.Drawing.Color.FromArgb(0, 0, 0);
                }
                else
                {
                    btn_SignalX03.BackColor = System.Drawing.Color.FromArgb(38, 45, 58);
                    btn_SignalX03.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
                }
            }
        }

        /// <summary>
        /// 更新机器生命周期（毫秒数转成文字）
        /// </summary>
        delegate void UpdateMachineLifeCycleTimeDelegate();
        private void UpdateMachineLifeCycleTime()
        {
            if (InvokeRequired)
            {
                this.Invoke(new UpdateMachineLifeCycleTimeDelegate(UpdateMachineLifeCycleTime));
            }
            else
            {
                txt_ActualWorkTime.Text = Common.CommonFunction.FormatMilliseconds(mc_MachineStatusHander.mc_MachineProduceStatusHandler.LastProductUseMilliseconds);
            }
        }

        /// <summary>
        /// 获取机器状态信息，更新状态饼图        
        /// </summary>
        /// <param name="singnal"></param>
        delegate void UpdateMachineStatusPieChartDelegate(List<DataModel.MachineStatusUseTime> useTimeList);
        private void UpdateMachineStatusPieChart(List<DataModel.MachineStatusUseTime> useTimeList)
        {
            if (InvokeRequired)
            {
                this.Invoke(new UpdateMachineStatusPieChartDelegate(delegate (List<DataModel.MachineStatusUseTime> s)
                {
                    
                }), useTimeList);
            }
            else
            {
                
                #region 机器状态对比饼图

                //显示标签
                Func<ChartPoint, string> labelPoint = chartPoint => string.Format("{0}", string.Format("{0:D2}天{1:D2}时{2:D2}分{3:D2}秒", TimeSpan.FromSeconds(chartPoint.Y).Days, TimeSpan.FromSeconds(chartPoint.Y).Hours, TimeSpan.FromSeconds(chartPoint.Y).Minutes, TimeSpan.FromSeconds(chartPoint.Y).Seconds, TimeSpan.FromSeconds(chartPoint.Y).Milliseconds));
                SeriesCollection seriesViews = new SeriesCollection();

                var converter = new System.Windows.Media.BrushConverter();
                foreach (var item in useTimeList)
                {                   
                    seriesViews.Add(new PieSeries
                    {
                        Title = item.StatusText,
                        Values = new ChartValues<double> { item.UseTotalSeconds },                        
                        PushOut = 5,
                        Fill = (System.Windows.Media.Brush)converter.ConvertFromString(item.StatusColor),
                        DataLabels = true,
                        LabelPoint = labelPoint
                    });
                }
                //数据
                pieChart_MachineStatus.Series = seriesViews;
                //图表示例
                pieChart_MachineStatus.LegendLocation = LegendLocation.Right;
                pieChart_MachineStatus.DefaultLegend.Background = System.Windows.Media.Brushes.White;
                pieChart_MachineStatus.DefaultLegend.Foreground = System.Windows.Media.Brushes.White;
                pieChart_MachineStatus.DefaultLegend.FontSize = 20;
                pieChart_MachineStatus.DefaultLegend.BorderBrush = System.Windows.Media.Brushes.Red;
                //数据提示栏
                pieChart_MachineStatus.DataTooltip.Background = System.Windows.Media.Brushes.White;
                //pieChart_MachineStatus.DataClick += pieChart_MachineStatusOnDataClick;

                #endregion
            }
        }

        /// <summary>
        /// 状态灯，文字及背景色
        /// </summary>
        delegate void UpdateMachineStatusLightDelegate();
        private void UpdateMachineStatusLight()
        {
            if (InvokeRequired)
            {
                this.Invoke(new UpdateMachineStatusLightDelegate(UpdateMachineStatusLight));
            }
            else
            {
                #region 机器状态灯

                //设置状态灯
                btn_StatusLight.Text = mc_MachineStatusHander.MachineStatusName;
                //btn_StatusLight.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
                btn_StatusLight.BackColor = Common.CommonFunction.colorHx16toRGB(mc_MachineStatusHander.MachineStatusColor);
                btn_StatusLight.ForeColor = Common.CommonFunction.getReverseForeColor(btn_StatusLight.BackColor);

                #endregion
            }
        }
        /// <summary>
        /// 之前工单
        /// </summary>
        private void ShowLastJobOrderBiaisInfo(int orderindex)
        {
            Common.LastJobOrderHandler lastJobOrderHandler = new Common.LastJobOrderHandler();
            List<DataModel.JobOrder> ProcessJobOrderList = lastJobOrderHandler.GetJLastJobOrderList();
            
            if (ProcessJobOrderList!=null && ProcessJobOrderList.Count>orderindex)
            {
                DataModel.JobOrder CurrentProcessJobOrder = ProcessJobOrderList[orderindex];
                if (CurrentProcessJobOrder != null )
                {
                    //mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder = CurrentProcessJobOrder;
                    mc_MachineStatusHander.mc_MachineProduceStatusHandler.ProcessJobOrderList = ProcessJobOrderList;
                    mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeCurrentProcessJobOrder(orderindex);
                    //ShowJobOrderBiaisInfo();
                    UpdateMachineStatusTotalDateTime();
                    UpdateMachineCompleteDateTime();
                    UpdateMachineNondefectiveCount();
                    UpdateMachineNoCompletedCount();
                }
            }
             
        }

        /// <summary>
        /// 显示工单基础信息
        /// </summary>
        delegate void ShowJobOrderBasicInfoDelegate();
        private void ShowJobOrderBiaisInfo()
        {
            if (InvokeRequired)
            {
                this.Invoke(new ShowJobOrderBasicInfoDelegate(ShowJobOrderBiaisInfo));
            }
            else
            {
                //删除旧工单
                Common.LastJobOrderHandler lastJobOrderHandler = new Common.LastJobOrderHandler();
                lastJobOrderHandler.DeleteLastJobOrder();
                #region 工单基础信息
                //防止加入控件时发生闪烁
                this.tableLayoutPanel9.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(this.tableLayoutPanel9, true, null);
                //清空第一行控件
                this.tableLayoutPanel9.Controls.Remove(this.tableLayoutPanel9.GetControlFromPosition(0, 0));

                if (mc_MachineStatusHander.mc_MachineProduceStatusHandler.ProcessJobOrderList != null && mc_MachineStatusHander.mc_MachineProduceStatusHandler.ProcessJobOrderList.Count > 0)
                {
                    int int_jobOrderIndex = 0;
                    int int_jobOrderCount = mc_MachineStatusHander.mc_MachineProduceStatusHandler.ProcessJobOrderList.Count;

                    //增加一个表格，用来装按钮
                    TableLayoutPanel tlp = new TableLayoutPanel();
                    tlp.Dock = DockStyle.Fill;
                    tlp.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
                    tlp.AutoScroll = true;

                    //增加列，计算每列的宽度
                    tlp.ColumnCount = int_jobOrderCount;
                    for (int i = 0; i < int_jobOrderCount; i++)
                    {
                        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100 / int_jobOrderCount));
                    }

                    tlp.SuspendLayout();

                    //获取所有可用的机器状态               
                    foreach (var item in mc_MachineStatusHander.mc_MachineProduceStatusHandler.ProcessJobOrderList)
                    {
                        //保存当前工单id
                            DataModel.LastJobOrder lastJobOrder = new DataModel.LastJobOrder();
                            lastJobOrder.JobOrderID = item.JobOrderID;
                            lastJobOrder.ChangeDate = DateTime.Now;
                            lastJobOrderHandler.SaveLastJobOrder(lastJobOrder);                   
                        //声明一个按钮
                        Button jobOrderButton = new Button
                        {
                            Text = "工单 " + item.JobOrderID,
                            Anchor = AnchorStyles.None,
                            FlatStyle = FlatStyle.Flat,
                            ForeColor = Color.Black,
                            Tag = int_jobOrderIndex,
                            Margin = new Padding(),
                            Dock = DockStyle.Fill,
                            BackColor = Color.WhiteSmoke
                        };
                        if (mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder.JobOrderID == item.JobOrderID)
                        {
                            jobOrderButton.BackColor = Color.PaleGreen;
                        }
                        //事件
                        jobOrderButton.Click += new System.EventHandler(btnSelectJobOrder_click);
                        //将可用的工单动态用按钮的形式加载到表格中
                        tlp.Controls.Add(jobOrderButton, int_jobOrderIndex, 0);

                        int_jobOrderIndex++;
                    }

                    tlp.ResumeLayout();

                    //将表格加入至大表格
                    this.tableLayoutPanel9.Controls.Add(tlp, 0, 0);
                }

                if (mc_MachineStatusHander.mc_MachineProduceStatusHandler.ProcessJobOrderList != null && mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder != null)
                {
                    //工单编号
                    txt_JobOrderCode.Text = mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder.JobOrderNumber;

                    //送达部门
                    this.txt_Dept.Text = mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder.ServiceDepartment;

                    //产品
                    txt_MaterialCode.Text = mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder.ProductCode;
                    
                    txt_MaterialName.Text = mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder.ProductDescription+"(1出" + getUnits().ToString() + ")";
                    //txt_MaterialSpecification.Text = mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder.Material.MaterialSpecification;


                    //模具标准周期
                    txt_StandardProduceSecond.Text = mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder.MouldStandardProduceSecond.ToString();
                    txt_MouldCode.Text = mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder.MouldCode.ToString();


                    //工单数
                    txt_WorkOrderCount.Text = mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder.OrderCount.ToString();


                    var sumProductCount = mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder.MachineProcessLog.Sum(t => t.ProduceCount);
                    var sumErrorCount = mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder.MachineProcessLog.Sum(t => t.ErrorCount);
                    //找到当前处理的一次操作记录
                    var machineProcessLog = mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder.MachineProcessLog.Find(t => t.MachineID == MC_Machine._id && t.ProduceStartDate == t.ProduceEndDate);
                    //产量
                    txt_ProductTotalCount.Text = sumProductCount.ToString() + " / " + machineProcessLog.ProduceCount.ToString();
                    //总不良品数量
                    txt_TotalRejectsCount.Text = sumErrorCount.ToString();
                    //不良品数
                    txt_RejectsCount.Text = machineProcessLog.ErrorCount.ToString();
                    //工单进度条

                    //进度条
                    this.circleProgramBar.MaxValue = mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder.OrderCount;
                    this.circleProgramBar.Progress = sumProductCount-sumErrorCount;             
                }
                else
                {
                    //进度条
                    this.circleProgramBar.Progress = 0;
                    //工单号
                    txt_JobOrderCode.Text = "";

                    //送达部门
                    this.txt_Dept.Text = "";
                    //产品
                    txt_MaterialCode.Text = "";
                    txt_MaterialName.Text = "";
                    txt_MouldCode.Text = "";

                    //模具标准周期
                    txt_StandardProduceSecond.Text = "";

                    //工单数
                    txt_WorkOrderCount.Text = "";
                    //产量
                    txt_ProductTotalCount.Text = "";
                    //总不良品数量
                    txt_TotalRejectsCount.Text = "";
                    //不良品数
                    txt_RejectsCount.Text = "";

                    //实际周期
                    txt_ActualWorkTime.Text = "";
                }

                #endregion
            }
        }

        /// <summary>
        /// 更新状态持续总时间
        /// </summary>
        delegate void UpdateMachineStatusTotalDateTimeDelegate();
        private void UpdateMachineStatusTotalDateTime()
        {
            if (InvokeRequired)
            {
                this.Invoke(new UpdateMachineStatusTotalDateTimeDelegate(UpdateMachineStatusTotalDateTime));
            }
            else
            {
                txt_UseTotalTime.Text = Common.CommonFunction.FormatMilliseconds(mc_MachineStatusHander.HoldStatusTotalMilliseconds);
                txt_MachineStopTotalTime.Text = Common.CommonFunction.FormatMilliseconds(mc_MachineStatusHander.StopStatusTotalMilliseconds);
            }
        }

        /// <summary>
        /// 预计完成时间处理
        /// </summary>
        delegate void UpdateMachineCompleteDateTimeDelegate();
        private void UpdateMachineCompleteDateTime()
        {
            if (InvokeRequired)
            {
                this.Invoke(new UpdateMachineCompleteDateTimeDelegate(UpdateMachineCompleteDateTime));
            }
            else
            {

                if (mc_MachineStatusHander.mc_MachineProduceStatusHandler.ProcessJobOrderList != null && mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder != null)
                {

                    //标准生命周期
                    double dbl_PlanWorkTime = 0;
                    double.TryParse(txt_StandardProduceSecond.Text.Trim(), out dbl_PlanWorkTime);
                    //实际生命周期
                    double RealWorkTime=0; 
                    RealWorkTime = mc_MachineStatusHander.mc_MachineProduceStatusHandler.LastProductUseMilliseconds / 1000.000;
                    //未完成数
                    int uncompletednum = 0;
                    int.TryParse(this.txt_NoCompletedCount.Text,out uncompletednum);
                    //找到本机
                    var machineProcessLog = mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder.MachineProcessLog.FindLast(t => t.MachineID == MC_Machine._id);

                    if (machineProcessLog.ProduceStartDate != null && mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder.OrderCount > 0)
                    {
                        txt_StartDateTime.BackColor = System.Drawing.Color.FromArgb(208, 208, 208);
                        txt_StartDateTime.Text = machineProcessLog.ProduceStartDate.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");

                        //预计完成工时间
                        System.DateTime PlanDateTime;
                        int units = getUnits();
                        if (units > 0)
                        {
                            if (uncompletednum > 0)
                            {
                                if (RealWorkTime > 0)
                                {
                                    PlanDateTime = DateTime.Now.AddSeconds(uncompletednum * RealWorkTime / units);
                                }
                                else
                                {
                                    PlanDateTime = DateTime.Now.AddSeconds(uncompletednum * dbl_PlanWorkTime / units);

                                }
                                txt_PlanCompleteDateTime.Text = PlanDateTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else
                            {
                                txt_PlanCompleteDateTime.Text = DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            
                        }
                       
                        
                    }

                }
                else
                {
                    txt_StartDateTime.Text = "";
                    txt_PlanCompleteDateTime.Text = "";
                }
            }
        }

        /// <summary>
        /// 良品数量处理
        /// </summary>
        delegate void UpdateMachineNondefectiveCountDelegate();
        private void UpdateMachineNondefectiveCount()
        {
            if (InvokeRequired)
            {
                this.Invoke(new UpdateMachineNondefectiveCountDelegate(UpdateMachineNondefectiveCount));
            }
            else
            {
                if (mc_MachineStatusHander.mc_MachineProduceStatusHandler.ProcessJobOrderList != null && mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder != null)
                {

                    int orderCount = mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder.OrderCount;

                    var sumProductCount = mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder.MachineProcessLog.Sum(t => t.ProduceCount);
                    var sumErrorCount = mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder.MachineProcessLog.Sum(t => t.ErrorCount);
                    var machineProcessLog = mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder.MachineProcessLog.Find(t => t.MachineID == MC_Machine._id && t.ProduceStartDate == t.ProduceEndDate);

                    if (sumProductCount > sumErrorCount)
                    {
                        //良品                    
                        txt_NondefectiveCount.Text = (machineProcessLog.ProduceCount - machineProcessLog.ErrorCount).ToString();
                    }
                    else
                    {
                        txt_NondefectiveCount.Text = "0";
                    }

                    //总不良品
                    txt_TotalRejectsCount.Text = sumErrorCount.ToString();
                    //未完成数量
                    txt_NoCompletedCount.Text = (orderCount - sumProductCount + sumErrorCount).ToString();
                }
                else
                {
                    //良品
                    txt_NondefectiveCount.Text = "";
                    //总不良品
                    txt_TotalRejectsCount.Text = "";
                    //未完成数量
                    txt_NoCompletedCount.Text = "";
                }
            }
        }
        /// <summary>
        /// 获取1出几
        /// </summary>
        private int getUnits()
        {
            DataModel.MouldProduct CurrentMouldProduct= Common.MouldProductHelper.GetMmouldProductByMouldCode(mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder.MouldCode);
            if (CurrentMouldProduct == null)
            {
               return  1;
            }
            else
            {
                var MouldProductItem = CurrentMouldProduct.ProductList.Find(t => t.ProductCode== mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder.ProductCode);
                if (MouldProductItem != null)
                {
                    return MouldProductItem.ProductCount;
                }
                else
                {
                    return 1;
                }
            }
        }

        /// <summary>
        /// 未完成数量处理
        /// </summary>
        delegate void UpdateMachineNoCompletedCountDelegate();
        private void UpdateMachineNoCompletedCount()
        {
            if (InvokeRequired)
            {
                this.Invoke(new UpdateMachineNoCompletedCountDelegate(UpdateMachineNoCompletedCount));
            }
            else
            {
                if (mc_MachineStatusHander.mc_MachineProduceStatusHandler.ProcessJobOrderList != null && mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder != null)
                {

                    int orderCount = mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder.OrderCount;

                    var sumProductCount = mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder.MachineProcessLog.Sum(t => t.ProduceCount);
                    var sumErrorCount = mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder.MachineProcessLog.Sum(t => t.ErrorCount);
                    var machineProcessLog = mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder.MachineProcessLog.Find(t => t.MachineID == MC_Machine._id && t.ProduceStartDate == t.ProduceEndDate);

                    //产量
                    txt_ProductTotalCount.Text = sumProductCount.ToString() + " / " + machineProcessLog.ProduceCount.ToString();

                    //总不良品
                    txt_TotalRejectsCount.Text = sumErrorCount.ToString();
                    //未完成数量
                    txt_NoCompletedCount.Text = (orderCount - sumProductCount + sumErrorCount).ToString();
                    txt_MaterialCode.Text = mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder.ProductCode;
                    txt_MaterialName.Text = mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder.ProductDescription+"(1出" + getUnits().ToString() + ")";

                    //进度条

                    this.circleProgramBar.MaxValue = mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder.OrderCount;
                    this.circleProgramBar.Progress = sumProductCount-sumErrorCount;
                    //this.progressBar_JobOrder.Visible = false;
                    //this.circleProgramBar.Progress = mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder.OrderCount;
                    // this.label24.Text = "超产" + (sumProductCount - mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder.OrderCount);
                    //this.label24.BackColor = Color.Red;


                    //double value = sumProductCount * 100 / this.progressBar_JobOrder.Maximum;
                    //this.label24.Text = value.ToString() + "%";

                }
                else
                {
                    this.circleProgramBar.Progress = 0;
                }

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
        private void SettingMachineStatusLight()
        {
            btn_StatusLight.Text = mc_MachineStatusHander.MachineStatusName;
            //btn_StatusLight.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            btn_StatusLight.BackColor = Common.CommonFunction.colorHx16toRGB(mc_MachineStatusHander.MachineStatusColor);
            btn_StatusLight.ForeColor = Common.CommonFunction.getReverseForeColor(btn_StatusLight.BackColor);
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

        //private void SetAxisLimits(System.DateTime now)
        //{
        //    cartesianChart_temperature.AxisX[0].MaxValue = now.Ticks + TimeSpan.FromSeconds(1).Ticks; // lets force the axis to be 100ms ahead
        //    cartesianChart_temperature.AxisX[0].MinValue = now.Ticks - TimeSpan.FromSeconds(60).Ticks; //we only care about the last 8 seconds
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
            if (closing) return;//如果正在关闭，忽略操作，直接返回，尽快的完成串口监听线程的一次循环
            try
            {
                listening = true;//设置标记，已经开始处理数据。
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

                //如果系统还未开始处理，则不会更改信号
                if (!string.IsNullOrEmpty(mc_MachineStatusHander.MachineStatusID))
                {
                    //更改状态
                    mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal(stringBuilder.ToString());
                    //闪灯
                    ReceiveDataSuccessColor = 255;
                }

                //this.Invoke((EventHandler)(delegate
                //{
                    //Common.LogHandler.Log(stringBuilder.ToString());                    

                    //if (COM7_ReceiveDataCount + 100 > long.MaxValue) COM7_ReceiveDataCount = 0;
                    //COM7_ReceiveDataCount += 1;
                    //lab_ReceviedDataCount.Text = "接收成功：" + COM7_ReceiveDataCount;
                    //lab_ReceviedDataCount.BackColor= System.Drawing.Color.FromArgb(0, 230, 118);

                //}
                //   )
                //);

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
                //System.Media.SystemSounds.Beep.Play();                
                Common.LogHandler.WriteLog("串口6获取数据错误", ex);
            }
            finally

            {

                listening = false;//用完了，ui可以关闭串口

            }
        }

        /// <summary>
        /// 串口6获取数据错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serialPort6_ErrorReceived(object sender, System.IO.Ports.SerialErrorReceivedEventArgs e)
        {
            Common.LogHandler.WriteLog("串口6获取到错误数据");
        }

        /// <summary>
        /// 发送数据串口默认配置
        /// </summary>
        private void sendDataSerialPortGetDefaultSetting()
        {
            try
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
            catch (Exception ex)
            {
                Common.LogHandler.WriteLog("串口获取默认设置",ex);
                throw;
            }

        }

        /*按钮事件*/
        /*---------------------------------------------------------------------------------------*/

        /// <summary>
        /// 模拟信号，验证生产逻辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTest_Click(object sender, EventArgs e)
        {
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0600ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0600ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0600ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0600ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0600ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0600ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0600ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0600ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0600ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0600ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0600ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0600ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0600ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0600ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0600ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0600ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0A00ZZ");
            mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeSignal("AA0200ZZ");


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
                //刷卡窗口指定类型
                frmScanRFID newfrmScanRFID = new frmScanRFID();
                //指定为选择机器状态
                newfrmScanRFID.MC_OperationType = frmScanRFID.OperationType.ChangeMachineType;
                newfrmScanRFID.MC_OperationType_Prompt = frmScanRFID.OperationType_Prompt.ChangeMachineType;
                newfrmScanRFID.ShowDialog();

                //如果主动返回则不处理
                if (!newfrmScanRFID.MC_IsManualCancel)
                {
                    if (newfrmScanRFID.MC_EmployeeInfo == null)
                    {
                        throw new Exception("用户未刷卡");
                    }

                    if (newfrmScanRFID.MC_frmChangeMachineStatusPara == null)
                    {
                        throw new Exception("用户未选择机器状态");
                    }


                    //如果选择的机器状态是[生产中]
                    if (newfrmScanRFID.MC_frmChangeMachineStatusPara.machineStatusCode == Common.MachineStatus.eumMachineStatus.Produce.ToString())
                    {
                        //生产中
                        if (mc_MachineStatusHander.mc_MachineProduceStatusHandler.ProcessJobOrderList != null && mc_MachineStatusHander.mc_MachineProduceStatusHandler.ProcessJobOrderList.Count > 0)
                        {
                            //工单还在界面上
                            mc_MachineStatusHander.mc_MachineProduceStatusHandler.SetJobOrder(mc_MachineStatusHander.mc_MachineProduceStatusHandler.ProcessJobOrderList, newfrmScanRFID.MC_EmployeeInfo._id);
                        }
                        else
                        {
                            throw new Exception("修改机器状态变为[生产中]，你需要先选择工单");
                        }
                    }
                    else
                    {
                        //暂停工单
                        mc_MachineStatusHander.mc_MachineProduceStatusHandler.StopJobOrder();
                    }

                    //更新状态                
                    mc_MachineStatusHander.ChangeStatus(
                        newfrmScanRFID.MC_frmChangeMachineStatusPara.machineStatusID,
                        newfrmScanRFID.MC_frmChangeMachineStatusPara.machineStatusCode,
                        newfrmScanRFID.MC_frmChangeMachineStatusPara.machineStatusName,
                        newfrmScanRFID.MC_frmChangeMachineStatusPara.machineStatusDesc,
                        newfrmScanRFID.MC_frmChangeMachineStatusPara.machineStatusColor,

                        newfrmScanRFID.MC_EmployeeInfo.EmployeeName,
                        newfrmScanRFID.MC_EmployeeInfo._id
                        );
                    showNoEmployee();
                }

            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message, "更新机器状态错误");
                Common.LogHandler.WriteLog("更新机器状态错误", ex);
            }
        }

        /// <summary>
        /// 应用标题事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lab_Title_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show(Common.CommonFunction.getMacAddress() + "\r\n", "系统信息");
        }


        /*文本框事件*/
        /*---------------------------------------------------------------------------------------*/

        /// <summary>
        /// 不良品只能输入数字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txt_RejectsCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;

            if (!Char.IsDigit(ch) && ch != 8 && ch != 46)
            {
                e.Handled = true;
            }
        }


        /// <summary>
        /// 不良品==》修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txt_RejectsCount_TextChanged(object sender, EventArgs e)
        {
            int int_RejectsCount = 0;
            int.TryParse(txt_RejectsCount.Text.Trim(), out int_RejectsCount);

            //一定要有工单
            if (mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder != null)
            {
                //找到当前记录
                var machineProcessLog = mc_MachineStatusHander.mc_MachineProduceStatusHandler.CurrentProcessJobOrder.MachineProcessLog.Find(t => t.MachineID == MC_Machine._id && t.ProduceStartDate == t.ProduceEndDate);

                if (int_RejectsCount >= 0 && int_RejectsCount <= machineProcessLog.ProduceCount)
                {
                    //不良品数量
                    if (!mc_MachineStatusHander.mc_MachineProduceStatusHandler.SettingProductErrorCount(int_RejectsCount))
                    {
                        txt_RejectsCount.Text = "";
                    }
                }
                else
                {
                    //不良品数量
                    if (!mc_MachineStatusHander.mc_MachineProduceStatusHandler.SettingProductErrorCount(0))
                    {
                        txt_RejectsCount.Text = "";
                    }
                }
            }
            else
            {
                txt_RejectsCount.Text = "";
            }
        }


        /*菜单按钮事件*/
        /*---------------------------------------------------------------------------------------*/

        /// <summary>
        /// 机器注册按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_MachineName_Click(object sender, EventArgs e)
        {
            Common.MachineRegisterInfoHelper machineRegisterInfoHelperClass = new Common.MachineRegisterInfoHelper();
            DataModel.Machine machineInfoEntity = machineRegisterInfoHelperClass.GetMachineRegisterInfo();

            if (machineInfoEntity == null)
            {
                frmMachineRegister newfrmMachineRegister = new frmMachineRegister();
                newfrmMachineRegister.ShowDialog();
                CheckMachineRegister();
            }
            else
            {
                if (Registerclick > 0)
                {
                    Registerclick = 0;
                    TimeSpan span = DateTime.Now - clickTime;
                    if (span.Milliseconds < SystemInformation.DoubleClickTime)
                    {
                            ThreadStart threadStart_SyncData = new ThreadStart(start_SyncData);//通过ThreadStart委托告诉子线程执行什么方法　　
                            Thread thread_SyncData = new Thread(threadStart_SyncData);
                            thread_SyncData.Start();//启动新线程
                    }
                }
                else
                {
                    Registerclick++;
                    clickTime = DateTime.Now;
                }
            }
           
            
        }
        /// <summary>
        /// 同步基础表
        /// </summary>
        /// 开始同步
        private void start_SyncData()
        {
            string name = this.btn_MachineName.Text;
            try
            {
               
                this.Invoke(new Action(() =>
                {
                    this.btn_MachineName.Text = "正在同步";
                    this.btn_MachineName.Enabled = false;
                }));
                //同步
                if (Common.SyncDataHelper.SyncData_AllCollection())
                {
                    this.Invoke(new Action(() =>
                    {
                        this.btn_MachineName.Text = "同步成功";
                        ShowJobOrderBiaisInfo();
                    }));
                }
                //4s后按钮值为初始值
                Thread.Sleep(3000);
                this.Invoke(new Action(() =>
                {
                    this.btn_MachineName.Text = name;
                    this.btn_MachineName.Enabled = true;
                }));
               
            }
            catch
            {
                this.Invoke(new Action(() =>
                {
                    this.btn_MachineName.Text = "同步失败";
                }));
                Thread.Sleep(3000);
                this.Invoke(new Action(() =>
                {
                    this.btn_MachineName.Text =name;
                    this.btn_MachineName.Enabled = true;
                }));
               
                return;
            }
        }
        /// <summary>
        /// 开始按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Start_Click(object sender, EventArgs e)
        {
            try
            {
				//if (btn_StatusLight.Text != "生产中")
				//{
					//if (mc_MachineStatusHander.MachineStatusCode != Common.MachineStatus.eumMachineStatus.Produce.ToString())
					//{
					frmScanRFID newfrmScanRFID = new frmScanRFID();
					newfrmScanRFID.MC_OperationType = frmScanRFID.OperationType.StartJobOrder;
					newfrmScanRFID.MC_OperationType_Prompt = frmScanRFID.OperationType_Prompt.StartJobOrder;
					newfrmScanRFID.ShowDialog();

					if (!newfrmScanRFID.MC_IsManualCancel)
					{
						//获取订单
						List<DataModel.JobOrder> jobOrderList = newfrmScanRFID.MC_frmChangeJobOrderPara;
						if (jobOrderList == null || jobOrderList.Count == 0) throw new Exception("未选择工单！");

						//设置订单，无论是开始还是恢复
						mc_MachineStatusHander.mc_MachineProduceStatusHandler.SetJobOrder(jobOrderList, newfrmScanRFID.MC_EmployeeInfo._id);

						//找到生产中状态
						DataModel.MachineStatus machineStatus = Common.MachineStatusHelper.GetMachineStatusByCode(Common.MachineStatus.eumMachineStatus.Produce.ToString());

						//更改状态
						mc_MachineStatusHander.ChangeStatus(
							machineStatus._id,
							machineStatus.MachineStatusCode,
							machineStatus.MachineStatusName,
							machineStatus.MachineStatusDesc,
							machineStatus.StatusColor,

							newfrmScanRFID.MC_EmployeeInfo.EmployeeName,
							newfrmScanRFID.MC_EmployeeInfo._id
						);
                        showNoEmployee();
                    }
				//}
				//else
				//{
				//	MessageBox.Show("工单【生产中】，请【暂停】工单后，重新选择【开始工单】", "提示");
				//}

				//}
			}
            catch (Exception ex)
            {
                Common.LogHandler.WriteLog("开始工单错误", ex);
                ShowErrorMessage("开始工单错误，原因：" + ex.Message, "开始工单错误");
            }
        }

        /// <summary>
        /// 暂停按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                if (mc_MachineStatusHander.mc_MachineProduceStatusHandler.ProcessJobOrderList != null && mc_MachineStatusHander.MachineStatusCode == Common.MachineStatus.eumMachineStatus.Produce.ToString())
                {
                    frmScanRFID newfrmScanRFID = new frmScanRFID();
                    newfrmScanRFID.MC_OperationType = frmScanRFID.OperationType.StopJobOrder;
                    newfrmScanRFID.MC_OperationType_Prompt = frmScanRFID.OperationType_Prompt.StopJobOrder;
                    newfrmScanRFID.ShowDialog();

                    if (!newfrmScanRFID.MC_IsManualCancel)
                    {
                        if (newfrmScanRFID.MC_frmChangeMachineStatusPara.machineStatusCode == Common.MachineStatus.eumMachineStatus.Produce.ToString()) throw new Exception("暂停工单时，机器状态不可选择为[生产中]"); 

                        //暂停工单
                        mc_MachineStatusHander.mc_MachineProduceStatusHandler.StopJobOrder();

                        //更新状态，状态来自于机器状态选择窗体
                        mc_MachineStatusHander.ChangeStatus(
                            newfrmScanRFID.MC_frmChangeMachineStatusPara.machineStatusID,
                            newfrmScanRFID.MC_frmChangeMachineStatusPara.machineStatusCode,
                            newfrmScanRFID.MC_frmChangeMachineStatusPara.machineStatusName,
                            newfrmScanRFID.MC_frmChangeMachineStatusPara.machineStatusDesc,
                            newfrmScanRFID.MC_frmChangeMachineStatusPara.machineStatusColor,

                            newfrmScanRFID.MC_EmployeeInfo.EmployeeName,
                            newfrmScanRFID.MC_EmployeeInfo._id
                            );
                        showNoEmployee();
                    }
                }
                else
                {
                    ShowErrorMessage("没有正在处理的工单或者当前机器状态并不是[生产中]", "工单停止失败");
                }
            }
            catch (Exception ex)
            {
                Common.LogHandler.WriteLog("暂停工单错误", ex);
                ShowErrorMessage("暂停工单错误，原因是：" + ex.Message, "暂停工单错误");
            }
        }

        /// <summary>
        /// 完成工单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_CompleteJobOrder_Click(object sender, EventArgs e)
        {
            try
            {
                if (mc_MachineStatusHander.mc_MachineProduceStatusHandler.ProcessJobOrderList != null && mc_MachineStatusHander.MachineStatusCode == Common.MachineStatus.eumMachineStatus.Produce.ToString())
                {
                    frmScanRFID newfrmScanRFID = new frmScanRFID();
                    newfrmScanRFID.MC_OperationType = frmScanRFID.OperationType.StopJobOrder;
                    newfrmScanRFID.MC_OperationType_Prompt = frmScanRFID.OperationType_Prompt.finishJobOrder;
                    newfrmScanRFID.ShowDialog();

                    if (!newfrmScanRFID.MC_IsManualCancel)
                    {
                        if (newfrmScanRFID.MC_frmChangeMachineStatusPara.machineStatusCode == Common.MachineStatus.eumMachineStatus.Produce.ToString()) throw new Exception("完成工单时，机器状态不可选择为[生产中]");

                        //清空工单
                        mc_MachineStatusHander.mc_MachineProduceStatusHandler.CompleteJobOrder(newfrmScanRFID.MC_EmployeeInfo._id);

                        //更新状态，状态来自于机器状态选择窗体
                        mc_MachineStatusHander.ChangeStatus(
                            newfrmScanRFID.MC_frmChangeMachineStatusPara.machineStatusID,
                            newfrmScanRFID.MC_frmChangeMachineStatusPara.machineStatusCode,
                            newfrmScanRFID.MC_frmChangeMachineStatusPara.machineStatusName,
                            newfrmScanRFID.MC_frmChangeMachineStatusPara.machineStatusDesc,
                            newfrmScanRFID.MC_frmChangeMachineStatusPara.machineStatusColor,

                            newfrmScanRFID.MC_EmployeeInfo.EmployeeName,
                            newfrmScanRFID.MC_EmployeeInfo._id
                            );
                        showNoEmployee();
                    }
                }
                else
                {
                    ShowErrorMessage("没有正在处理的工单或者当前机器状态并不是[生产中]", "工单停止失败");
                }
            }
            catch (Exception ex)
            {
                Common.LogHandler.WriteLog("完成工单错误", ex);
                ShowErrorMessage("完成工单错误，原因是：" + ex.Message, "完成工单错误");
            }
        }

        /// <summary>
        /// 恢复按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Recovery_Click(object sender, EventArgs e)
        {
            try
            {
				//if (btn_StatusLight.Text != "生产中")
				//{ 
					//if (mc_MachineStatusHander.MachineStatusCode != Common.MachineStatus.eumMachineStatus.Produce.ToString())
					//{
					frmScanRFID newfrmScanRFID = new frmScanRFID();
                    newfrmScanRFID.MC_OperationType = frmScanRFID.OperationType.ResumeJobOrder;
                    newfrmScanRFID.MC_OperationType_Prompt = frmScanRFID.OperationType_Prompt.ResumeJobOrder;
                    newfrmScanRFID.ShowDialog();

                    if (!newfrmScanRFID.MC_IsManualCancel)
                    {

                        //获取订单
                        List<DataModel.JobOrder> jobOrderList = newfrmScanRFID.MC_frmChangeJobOrderPara;
                        if (jobOrderList == null || jobOrderList.Count == 0) throw new Exception("未选择工单！");

						
						//设置订单，无论是开始还是恢复
						mc_MachineStatusHander.mc_MachineProduceStatusHandler.SetJobOrder(jobOrderList, newfrmScanRFID.MC_EmployeeInfo._id);
					
						

					//找到生产中状态
					DataModel.MachineStatus machineStatus = Common.MachineStatusHelper.GetMachineStatusByCode(Common.MachineStatus.eumMachineStatus.Produce.ToString());

                        //更改状态
                        mc_MachineStatusHander.ChangeStatus(
                            machineStatus._id,
                            machineStatus.MachineStatusCode,
                            machineStatus.MachineStatusName,
                            machineStatus.MachineStatusDesc,
                            machineStatus.StatusColor,

                            newfrmScanRFID.MC_EmployeeInfo.EmployeeName,
                            newfrmScanRFID.MC_EmployeeInfo._id
                        );
                    showNoEmployee();
                    }
				//}
				//else
				//{
				//	MessageBox.Show("工单【生产中】，请【暂停】工单后，重新选择【恢复工单】", "提示");
				//}
				//}
			}
			catch (Exception ex)
            {
                Common.LogHandler.WriteLog("恢复工单错误", ex);
                ShowErrorMessage("恢复工单错误，原因是：" + ex.Message, "恢复工单错误");
            }
        }

        /// <summary>
        /// 窗口最小化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_MinimizeWindows_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        /// <summary>
        /// 关闭系统
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_CloseWindow_Click(object sender, EventArgs e)
        {
            this.Close();

			//暂停工单
			//mc_MachineStatusHander.mc_MachineProduceStatusHandler.StopJobOrder();
		}

        /// <summary>
        /// 机器注册
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_MachineRegister_Click(object sender, EventArgs e)
        {
            try
            {
                frmMachineRegister newfrmMachineRegister = new frmMachineRegister();
                newfrmMachineRegister.ShowDialog();

                CheckMachineRegister();
            }
            catch (Exception ex)
            {
                Common.LogHandler.WriteLog("机器注册错误", ex);
                ShowErrorMessage("机器注册错误，原因是" + ex.Message, "机器注册错误");
            }
        }


        /// <summary>
        /// 切换当前工单（显示）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectJobOrder_click(object sender, System.EventArgs e)
        {
            try
            {
                //将触发此事件的对象转换为该Button对象
                Button b1 = (Button)sender;

                int jobOrderIndex = 0;
                int.TryParse(b1.Tag.ToString(), out jobOrderIndex);
                //切换当前工单（显示）
                mc_MachineStatusHander.mc_MachineProduceStatusHandler.ChangeCurrentProcessJobOrder(jobOrderIndex);
            }
            catch (Exception ex)
            {
                Common.LogHandler.WriteLog("切换工单错误", ex);
                ShowErrorMessage("切换工单错误，原因是：" + ex.Message, "切换工单错误");
            }
        }


        /*检测机器注册*/
        /*---------------------------------------------------------------------------------------*/

        /// <summary>
        /// 检测机器是否注册
        /// </summary>
        private void CheckMachineRegister()
        {
            try
            {
                //获取数据库机器注册信息
                Common.MachineRegisterInfoHelper machineRegisterInfoHelperClass = new Common.MachineRegisterInfoHelper();
                DataModel.Machine machineInfoEntity = machineRegisterInfoHelperClass.GetMachineRegisterInfo();

                if (machineInfoEntity != null)
                {
                    //将机器注册信息保存至变量中
                    mc_MachineStatusHander.mc_MachineProduceStatusHandler.MC_machine = machineInfoEntity;

                    //机器注册信息保存在界面中，方便对比使用
                    MC_Machine = machineInfoEntity;

                    //如果存在机器注册信息，则显示机器名，也不可再注册
                    btn_MachineName.Text = machineInfoEntity.MachineCode;
                    //btn_MachineName.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Common.LogHandler.WriteLog("检查机器注册错误", ex);
            }
        }

       
        //判断是否升级
        /// <summary>
        /// 判断是否升级
        /// </summary>
        private void btn_Version_Click(object sender, EventArgs e)
        {
            
            if (click > 0)
            {
                click = 0;
                TimeSpan span = DateTime.Now - clickTime;
                if (span.Milliseconds < SystemInformation.DoubleClickTime)
                {
                    //检查机器状态
                    if (mc_MachineStatusHander.MachineStatusCode== "Produce")
                    {
                        MessageBox.Show("正在生产中，请先更改状态", "提示");
                        return;
                    }
                    //输入密码
                    string response = Microsoft.VisualBasic.Interaction.InputBox("请输入密码", "用户输入");
                    if (response.Trim() == "")
                    {
                        return;
                    }
                    if (response.Trim() == password)
                    {
                        ThreadStart threadStart_Update = new ThreadStart(start_Update);//通过ThreadStart委托告诉子线程执行什么方法　　
                        Thread thread_Update = new Thread(threadStart_Update);
                        thread_Update.Start();//启动新线程
                        this.lab_log.Text = "正在检测";
                        this.lab_log.ForeColor= Color.White;
                    }
                    else
                    {
                        MessageBox.Show("密码错误");
                    }           
                }      
            }
            else
            {
                click++;
                clickTime = DateTime.Now;
            }
        }

        /// <summary>
        /// 关闭系统进程
        /// </summary>
        public void KillProgram()
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
        /// 显示可升级状态，新线程运行方法
        /// </summary>
        /// 开始升级
        private void start_Update()
        {
           
            try
            {
                jsonString = Common.HttpHelper.HttpGetWithToken(Common.ConfigFileHandler.GetAppConfig("UpdatePath"));
            }
            catch
            {
                // MessageBox.Show("获取不到版本信息");
                this.Invoke(new Action(() =>
                {
                    this.lab_log.Text = "检测失败";
                    this.lab_log.ForeColor = Color.White;
                }));

                return;
            }
            //委托
            this.Invoke(new Action(() => { 
                
                string newVersion = GetJsonDate("ClientVersionCode");
                if (newVersion != "" && !(newVersion is null))
                {
                    if (oldVersion == newVersion)
                    {
                        this.lab_log.Text = "最新版本";
                        this.lab_log.ForeColor = Color.White;
                        MessageBox.Show("当前版本为最新版本", "提示");
                    }
                    else
                    {
                        this.lab_log.Text = "可升级";
                        this.lab_log.ForeColor = Color.Green;
                        if (MessageBox.Show("检测到新版本" + newVersion + "，确认是否继续升级？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                        {
                            
                            
                            
                                if (File.Exists(update_Path))
                                {
                                    Process process = new Process();
                                    process.StartInfo.FileName = update_Path;
                                    string basicHttpUrl = Common.CommonFunction.GenerateBackendUri();
                                    string url = "\"" + basicHttpUrl + GetJsonDate("FilePath") + "\"";
                                    string path = "\"" + new DirectoryInfo(Application.StartupPath).Parent.FullName + "\"";
                                    string ClientVersionCode = "\"" + newVersion + "\"";
                                    string ClientVersionName = "\"" + GetJsonDate("ClientVersionName") + "\"";
                                    string ClientVersionDesc = "\"" + GetJsonDate("ClientVersionDesc") + "\"";
                                    string Remark = "\"" + GetJsonDate("Remark") + "\"";
                                    string CreateAt = GetJsonDate("CreateAt");
                                    if (CreateAt != "" && !(CreateAt is null))
                                    {
                                        CreateAt = "\"" + DateTime.Parse(CreateAt).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") + "\"";
                                    }
                                    string LastUpdateAt = GetJsonDate("LastUpdateAt");
                                    if (LastUpdateAt != "" && !(LastUpdateAt is null))
                                    {
                                        LastUpdateAt = "\"" + DateTime.Parse(LastUpdateAt).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") + "\"";
                                    }
                                    process.StartInfo.Arguments = string.Format("{0} {1} {2} {3} {4} {5} {6} {7}", url, path, ClientVersionCode, ClientVersionName, ClientVersionDesc, Remark, CreateAt, LastUpdateAt);
                                    process.Start();
                                    KillProgram();
                                }
                                else
                                {
                                    MessageBox.Show("不存在更新程序" + update_Path);
                                    this.lab_log.Text = "可升级";
                                    this.lab_log.ForeColor = Color.Green;
                                }
                            
                        }
                        else
                        {
                            this.lab_log.Text = "可升级";
                            this.lab_log.ForeColor = Color.Green;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("获取不到版本信息");
                    this.lab_log.Text = "获取不到版本信息";
                }
            }));
        }
        /// <summary>
        /// 显示可升级状态
        /// </summary>
        private void getJson()
        {
            this.Invoke(new Action(() =>
            {
                this.lab_log.Text = "正在检测";
            }));
            try
            {
                jsonString = Common.HttpHelper.HttpGetWithToken(Common.ConfigFileHandler.GetAppConfig("UpdatePath"));
            }
            catch
            {
               // MessageBox.Show("获取不到版本信息");
                this.Invoke(new Action(() => 
                { 
                    this.lab_log.Text = "检测失败";
                }));

            return;
            }
            this.Invoke(new Action(() =>
            {
               string newVersion = GetJsonDate("ClientVersionCode");
               if (newVersion != "" && !(newVersion is null))
               {
                if (oldVersion == newVersion)
                {
                    // this.lab_Version.Text = "版本：" + Application.ProductVersion.ToString();
                    this.lab_log.Text = "最新版本";
                }
                else
                {
                    // this.lab_Version.Text = "当前版本：" + Application.ProductVersion.ToString();
                    this.lab_log.Text = "可升级";
                    this.lab_log.ForeColor = Color.Green;
                }

               }
               else
               {
                this.lab_log.Text = "无信息";
               }
            }));

        }
        /// <summary>
        /// 获取json详细数据
        /// </summary>
        private string GetJsonDate(string pro)
        {
            if (jsonString != null)
            {
                try
                {
                    string ClientVersionCode = "";

                    var jobj = JArray.Parse(jsonString);
                    foreach (var ss in jobj)
                    {
                        ClientVersionCode = ((JObject)ss)[pro].ToString();
                    }
                    return ClientVersionCode;
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        private void btn_ClockIn_Click(object sender, EventArgs e)
        {
            frmAttend newfrmAttend = new frmAttend();
            newfrmAttend.ShowDialog();
            if (!newfrmAttend.MC_IsManualCancel)
            {
                showClockIn();
            }
        }
        private void btn_refresh_Click(object sender, EventArgs e)
        {

        }
        ///<summary>
        ///显示QC和员工
        ///</summary>
        private void showClockIn()
        {
            Common.ClockInRecordHandler clockInRecordHandler = new Common.ClockInRecordHandler();
            List<DataModel.ClockInRecord> displayClockInRecords = clockInRecordHandler.GetClockInRecordList();
            haveEmployee = false;
            haveQC = false;
            foreach (var item in displayClockInRecords)
            {
                DataModel.Employee employee = Common.EmployeeHelper.QueryEmployeeByEmployeeID(item.EmployeeID);
                if (employee != null)
                {
                    DataModel.JobPositon jobPositon = Common.JobPositionHelper.GetJobPositon(employee.JobPostionID);
                    if (jobPositon != null)
                    {
                        string jobPositionCode = jobPositon.JobPositionCode;
                        if (jobPositionCode == frmAttend.JobPositionCode.Employee.ToString())
                        {
                            haveEmployee = true;
                            this.label21.Text = employee.EmployeeName;
                            if (System.IO.File.Exists(Application.StartupPath + imagePath+"\\" + employee.LocalFileName))
                            {
                                pictureBoxEmp.Image = Image.FromFile(Application.StartupPath + imagePath+"\\" + employee.LocalFileName);
                            }
                            else
                            {
                                this.pictureBoxEmp.Image = ((System.Drawing.Image)(resources.GetObject("noimage.Image")));
                            }

                        }
                        else if (jobPositionCode == frmAttend.JobPositionCode.QC.ToString())
                        {
                            haveQC = true;
                            this.label20.Text = employee.EmployeeName;
                            if (System.IO.File.Exists(Application.StartupPath + imagePath+"\\" + employee.LocalFileName))
                            {
                                pictureBoxQC.Image = Image.FromFile(Application.StartupPath + imagePath+"\\" + employee.LocalFileName);
                            }
                            else
                            {
                                this.pictureBoxQC.Image = ((System.Drawing.Image)(resources.GetObject("noimage.Image")));
                            }

                        }
                    }
                }
            }
            //处理无员工情况
            showNoEmployee();
        }

        ///<summary>
        ///处理不在当前班次的打卡记录
        /// </summary>
        private void autoSignOut()
        {
            Common.ClockInRecordHandler clockInRecordHandler = new Common.ClockInRecordHandler();
            List<DataModel.ClockInRecord> displayClockInRecords = clockInRecordHandler.GetClockInRecordList();
            if (nowStartTime != null)
            {
                foreach (var item in displayClockInRecords)
                {
                    if (item.StartDate.ToLocalTime().AddHours(1) < nowStartTime)
                    {
                        DataModel.Employee employee = Common.EmployeeHelper.QueryEmployeeByEmployeeID(item.EmployeeID);
                        if (employee != null)
                        {
                            DataModel.JobPositon jobPositon = Common.JobPositionHelper.GetJobPositon(employee.JobPostionID);
                            //QC不处理
                            if (jobPositon.JobPositionCode != frmAttend.JobPositionCode.QC.ToString())
                            {
                                clockInRecordHandler.UpdateClockInRecord(item, true);
                            }
                        }
                        else
                        {
                            clockInRecordHandler.UpdateClockInRecord(item, true);
                        }
                    }
                }
            }
        }
        ///<summary>
        ///处理无员工情况
        /// </summary>
        private void showNoEmployee()
        {
            if (!haveEmployee)
            {
                this.label21.Text = "";
                this.pictureBoxEmp.Image = null;
                this.label27.ForeColor = System.Drawing.Color.Yellow;
            }
            else
            {
                this.label27.ForeColor = System.Drawing.Color.Chartreuse;
            }
            if (!haveQC)
            {
                this.label20.Text = "";
                this.pictureBoxQC.Image = null;
                this.label26.ForeColor = System.Drawing.Color.Yellow;
            }
            else
            {
                this.label26.ForeColor = System.Drawing.Color.Chartreuse;
            }
            if (!haveCharge)
            {
                charge = null;
                this.label19.Text = "";
                this.pictureBoxCharge.Image = null;
                this.label25.ForeColor = System.Drawing.Color.Yellow;
            }
            else
            {
                this.label25.ForeColor = System.Drawing.Color.Chartreuse;
            }
            if (!haveHead)
            {
                head = null;
                this.label18.Text = "";
                this.pictureBoxHead.Image = null;
                this.label24.ForeColor = System.Drawing.Color.Yellow;
            }
            else
            {
                this.label24.ForeColor = System.Drawing.Color.Chartreuse;
            }
        }
        ///<summary>
        ///显示领班和组长
        /// </summary>
        private delegate void showHeadDelegate();
        private void showHeadAndCharge()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    try
                    {
                        this.Invoke(new SetDateTimeDelegate(showHeadAndCharge));
                    }
                    catch (Exception ex)
                    {
                        //响铃并显示异常给用户
                        System.Media.SystemSounds.Beep.Play();
                    }
                }
                else
                {
                    //找到本机器负责员工
                    List<DataModel.WorkshopResponsiblePerson> headAreas = Common.WorkshopResponsiblePersonHandler.findEmployeeByWorkshopID(MC_Machine.WorkshopID);
                    //找到组长
                    List<DataModel.MachineResponsiblePerson> chargeAreas = Common.MachineResponsiblePersonHandler.findChargeAreaByMachineID(MC_Machine._id);
                    DateTime now = DateTime.Now;
                    string today = now.ToString("yyyy-MM-dd");
                    string lastday = now.AddDays(-1).ToString("yyyy-MM-dd");
                    string maxTime = today + "T23:59:59Z";
                    string minTime = lastday + "T00:00:00Z";
                    haveHead = false;
                    haveCharge = false;
                    nowStartTime = null;
                    nowEndTime = null;
                    foreach (var item in headAreas)
                    {
                        foreach (var employeeID in item.ResponsiblePersonID)
                        {
                            //通过班次找到适合该时间的该员工
                            //通过（员工，时间）找到今天和昨天的班次
                            List<DataModel.EmployeeWorkSchedule> employeeSchedulings = Common.EmployeeWorkScheduleHandler.findRecordByIDAndTime(maxTime, minTime, employeeID);
                            foreach (var employeeScheduling in employeeSchedulings)
                            {
                                //找到班次对应时间
                                DataModel.WorkShift workShift = Common.WorkShiftHandler.QueryWorkShiftByid(employeeScheduling.WorkShiftID);
                                DateTime dt;
                                DateTime.TryParse(employeeScheduling.ScheduleDate,out dt);
                                if (string.Compare(workShift.WorkShiftStartTime, workShift.WorkShiftEndTime, true) == -1)
                                {
                                    //同一天
                                    employeeScheduling.startTime = Convert.ToDateTime(dt.ToString("yyyy-MM-dd") + " " + workShift.WorkShiftStartTime + ":00");
                                    employeeScheduling.endTime = Convert.ToDateTime(dt.ToString("yyyy-MM-dd") + " " + workShift.WorkShiftEndTime + ":00");
                                }
                                else
                                {

                                    employeeScheduling.startTime = Convert.ToDateTime(dt.ToString("yyyy-MM-dd") + " " + workShift.WorkShiftStartTime + ":00");
                                    employeeScheduling.endTime = Convert.ToDateTime(dt.AddDays(1).ToString("yyyy-MM-dd") + " " + workShift.WorkShiftEndTime + ":00");
                                }
                                //当前时间谁值班
                                if (employeeScheduling.startTime <= now && employeeScheduling.endTime >= now)
                                {
                                    DataModel.Employee employee = Common.EmployeeHelper.QueryEmployeeByEmployeeID(employeeScheduling.EmployeeID);
                                    if (employee != null)
                                    {
                                        nowStartTime = employeeScheduling.startTime;
                                        nowEndTime = employeeScheduling.endTime;
                                        head = employee;
                                        haveHead = true;
                                        this.label18.Text = employee.EmployeeName;
                                        if (System.IO.File.Exists(Application.StartupPath +imagePath+ "\\" + employee.LocalFileName))
                                        {
                                            pictureBoxHead.Image = Image.FromFile(Application.StartupPath + imagePath+"\\" + employee.LocalFileName);
                                        }
                                        else
                                        {
                                            this.pictureBoxHead.Image = ((System.Drawing.Image)(resources.GetObject("noimage.Image")));
                                        }
                                        break;
                                    }
                                }

                            }
                        }


                    }

                    foreach (var item in chargeAreas)
                    {
                        //通过班次找到适合该时间的该员工
                        //通过（员工，时间）找到今天和昨天的班次
                        foreach (var employeeID in item.ResponsiblePersonID)
                        {
                            List<DataModel.EmployeeWorkSchedule> employeeSchedulings = Common.EmployeeWorkScheduleHandler.findRecordByIDAndTime(maxTime, minTime, employeeID);
                            foreach (var employeeScheduling in employeeSchedulings)
                            {

                                //找到班次对应时间
                                DataModel.WorkShift workShift = Common.WorkShiftHandler.QueryWorkShiftByid(employeeScheduling.WorkShiftID);
                                DateTime dt;
                                DateTime.TryParse(employeeScheduling.ScheduleDate, out dt);
                                if (string.Compare(workShift.WorkShiftStartTime, workShift.WorkShiftEndTime, true) == -1)
                                {
                                    //同一天
                                    employeeScheduling.startTime = Convert.ToDateTime(dt.ToString("yyyy-MM-dd") + " " + workShift.WorkShiftStartTime + ":00");
                                    employeeScheduling.endTime = Convert.ToDateTime(dt.ToString("yyyy-MM-dd") + " " + workShift.WorkShiftEndTime + ":00");
                                }
                                else
                                {

                                    employeeScheduling.startTime = Convert.ToDateTime(dt.ToString("yyyy-MM-dd") + " " + workShift.WorkShiftStartTime + ":00");
                                    employeeScheduling.endTime = Convert.ToDateTime(dt.AddDays(1).ToString("yyyy-MM-dd") + " " + workShift.WorkShiftEndTime + ":00");
                                }
                                //当前时间谁值班
                                if (employeeScheduling.startTime <= now && employeeScheduling.endTime >= now)
                                {
                                    DataModel.Employee employee = Common.EmployeeHelper.QueryEmployeeByEmployeeID(employeeScheduling.EmployeeID);
                                    if (employee != null)
                                    {
                                        nowStartTime = employeeScheduling.startTime;
                                        nowEndTime = employeeScheduling.endTime;
                                        charge = employee;
                                        haveCharge = true;
                                        this.label19.Text = employee.EmployeeName;
                                        if (System.IO.File.Exists(Application.StartupPath + imagePath+"\\" + employee.LocalFileName))
                                        {
                                            pictureBoxCharge.Image = Image.FromFile(Application.StartupPath + imagePath+ "\\" + employee.LocalFileName);
                                        }
                                        else
                                        {
                                            this.pictureBoxCharge.Image = ((System.Drawing.Image)(resources.GetObject("noimage.Image")));
                                        }
                                        break;
                                    }
                                }
                            }

                        }

                    }
                    autoSignOut();
                    showClockIn();
                    showNoEmployee();
                }
            }
            catch(Exception ex)
            {
                Common.LogHandler.WriteLog("显示领班和组长错误", ex);
            }
           
        }
    }
}
