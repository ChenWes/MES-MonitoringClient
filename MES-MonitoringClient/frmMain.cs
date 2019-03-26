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

namespace MES_MonitoringClient
{
    public partial class frmMain : Form
    {
        //数字为发送及接收数据状态的灯
        private int SendDataSuccessColor = 0;
        private int ReceiveDataSuccessColor = 0;

        //发送串口数据信号时间间隔
        private long sendDataTimeInterval = 0;

        //机器名称
        private string defaultMachineName = Common.ConfigFileHandler.GetAppConfig("MachineName");
        //发送数据时间间隔，以毫秒计
        private string defaultSendDataIntervalMilliseconds = Common.ConfigFileHandler.GetAppConfig("SendDataIntervalMilliseconds");
        //数据上传服务名称
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

        //发送数据线程方法
        Thread MachineTemperatureThreadClass = null;
        ThreadStart MachineTemperatureThreadFunction = null;
        Common.TimmerHandler MachineTemperatureTimerClass = null;

        //状态灯
        Thread StatusLightThreadClass = null;
        ThreadStart StatusLightThreadFunction = null;
        Common.TimmerHandler StatusLightTimerClass = null;

        /*---------------------------------------------------------------------------------------*/

        //状态操作类
        static Common.MachineStatusHandler mc_MachineStatusHander = null;

        //温度状态表
        public ChartValues<DataModel.MachineTemperature> MachineTemperature1ChartValues { get; set; }

        /*---------------------------------------------------------------------------------------*/
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
                //显示机器名称
                btn_MachineName.Text = defaultMachineName;

                //最大化窗口
                this.WindowState = FormWindowState.Maximized;

                //检测端口
                CheckSerialPort(mc_DefaultRequiredSerialPortName);

                //发送数据串口默认配置
                sendDataSerialPortGetDefaultSetting();

                //设置机器默认状态
                mc_MachineStatusHander = new Common.MachineStatusHandler();
                mc_MachineStatusHander.UpdateMachineUseTimeDelegate += UpdateMachineUseTime;//状态更新方法（更新饼图）
                mc_MachineStatusHander.UpdateMachineCompleteDateTimeDelegate += UpdateMachineCompleteDateTime;//预计完成时间更新方法（预计完成时间）
                mc_MachineStatusHander.UpdateMachineStatusTotalDateTimeDelegate += UpdateMachineStatusTotalDateTime;//状态总时间更新方法（状态总时间）

                //设置机器生命周期信号状态
                mc_MachineStatusHander.mc_MachineProduceStatusHandler.UpdateMachineSignalDelegate += UpdateMachineSignalStatus;//信号更新方法（更新信号灯）
                mc_MachineStatusHander.mc_MachineProduceStatusHandler.UpdateMachineLifeCycleTimeDelegate += UpdateMachineLifeCycleTime;//更新产品生命周期（实际生产时间）
                mc_MachineStatusHander.mc_MachineProduceStatusHandler.UpdateMachineNondefectiveCountDelegate += UpdateMachineNondefectiveCount;//良品更新方法（良品数量）
                mc_MachineStatusHander.mc_MachineProduceStatusHandler.UpdateMachineNoCompleteCountDelegate += UpdateMachineNoCompletedCount;//未完成产品数量更新方法（未完成产品数量）
                
                //检测MongoDB服务
                if (!Common.CommonFunction.ServiceRunning(Common.MongodbHandler.MongodbServiceName))
                {
                    if (MessageBox.Show("MongoDB服务未安装或未运行，是否继续运行应用？", "MongoDB服务", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        this.Close();
                    }
                }
                else
                {
                    mc_MachineStatusHander.ShowStatusPieChart();//获取饼图
                }

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


                #region 开机后设置默认参数，直接运行，该功能只作为收集机器信号稳定性测试，正式功能需要删除该代码

                txt_WorkOrderCount.Text = "999999";
                txt_PlanWorkTime.Text = "50";
                
                btn_Start_Click(null, null);

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
                if (MessageBox.Show("退出系统后，暂时不会收集到机器的数据，请知悉", "系统退出提醒", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
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
                    if(StatusLightThreadClass !=null && StatusLightTimerClass != null)
                    {
                        StatusLightTimerClass.StopTimmer();
                        StatusLightThreadClass.Abort();
                    }

                    //串口关闭
                    if (serialPort6.IsOpen)
                    {
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
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message, "退出系统错误");
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
                TTimerClass = null;
            }
        }

        /// <summary>
        /// 声明显示状态灯委托
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
                ShowErrorMessage(ex.Message, "设置当前时间错误");
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
                MachineTemperatureTimerClass = null;
            }
        }



        /// <summary>
        /// 获取机器信号，并更新界面
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
        /// 更新机器生命周期
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
        delegate void UpdateMachineUserTimeDelegate(List<DataModel.MachineStatusUseTime> useTimeList);
        private void UpdateMachineUseTime(List<DataModel.MachineStatusUseTime> useTimeList)
        {
            if (InvokeRequired)
            {
                this.Invoke(new UpdateMachineUserTimeDelegate(delegate (List<DataModel.MachineStatusUseTime> s)
                {
                    

                }), useTimeList);
            }
            else
            {
                //显示标签
                Func<ChartPoint, string> labelPoint = chartPoint => string.Format("{0}", string.Format("{0:D2}时:{1:D2}分:{2:D2}秒", TimeSpan.FromSeconds(chartPoint.Y).Hours, TimeSpan.FromSeconds(chartPoint.Y).Minutes, TimeSpan.FromSeconds(chartPoint.Y).Seconds, TimeSpan.FromSeconds(chartPoint.Y).Milliseconds));
                SeriesCollection seriesViews = new SeriesCollection();

                var converter = new System.Windows.Media.BrushConverter();
                foreach (var item in useTimeList)
                {
                    System.Windows.Media.Brush brush = (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF");

                    if (item.Status == "运行")
                    {
                        brush = (System.Windows.Media.Brush)converter.ConvertFromString("#00E676");
                    }
                    else if (item.Status == "故障")
                    {
                        brush = (System.Windows.Media.Brush)converter.ConvertFromString("#FF3D00");
                    }
                    else if (item.Status == "停机")
                    {
                        brush = (System.Windows.Media.Brush)converter.ConvertFromString("#DDDD00");
                    }

                    seriesViews.Add(new PieSeries
                    {
                        Title = item.Status,
                        Values = new ChartValues<double> { item.UseTotalSeconds },                        
                        PushOut = 5,
                        Fill = brush,
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
                int int_WorkOrderCount = 0;
                int int_PlanWorkTime = 0;

                int.TryParse(txt_WorkOrderCount.Text.Trim(), out int_WorkOrderCount);
                int.TryParse(txt_PlanWorkTime.Text.Trim(), out int_PlanWorkTime);

                //订单总数
                mc_MachineStatusHander.mc_MachineProduceStatusHandler.OrderCount = int_WorkOrderCount;                

                if (mc_MachineStatusHander.StartWorkTime.HasValue && int_WorkOrderCount > 0 && int_PlanWorkTime > 0)
                {
                    txt_StartDateTime.BackColor = System.Drawing.Color.FromArgb(208, 208, 208);
                    txt_StartDateTime.Text = mc_MachineStatusHander.StartWorkTime.Value.ToString("yyyy-MM-dd HH:mm:ss");

                    //预计完成工
                    mc_MachineStatusHander.PlanCompleteDateTime = mc_MachineStatusHander.StartWorkTime.Value.AddSeconds(int_WorkOrderCount * int_PlanWorkTime);
                    txt_PlanCompleteDateTime.Text = mc_MachineStatusHander.PlanCompleteDateTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
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
                if (mc_MachineStatusHander.mc_MachineProduceStatusHandler.ProductCount > mc_MachineStatusHander.mc_MachineProduceStatusHandler.ProductErrorCount)
                {
                    //良品                    
                    txt_NondefectiveCount.Text = (mc_MachineStatusHander.mc_MachineProduceStatusHandler.ProductCount - mc_MachineStatusHander.mc_MachineProduceStatusHandler.ProductErrorCount).ToString();
                }
                else
                {
                    txt_NondefectiveCount.Text = "0";
                }

                txt_NoCompletedCount.Text = mc_MachineStatusHander.mc_MachineProduceStatusHandler.OrderNoCompleteCount.ToString();
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
                txt_ProductTotalCount.Text = mc_MachineStatusHander.mc_MachineProduceStatusHandler.ProductCount.ToString();
                txt_NoCompletedCount.Text = mc_MachineStatusHander.mc_MachineProduceStatusHandler.OrderNoCompleteCount.ToString();
            }
        }

        //private void pieChart_MachineStatusOnDataClick(object sender, ChartPoint chartPoint)
        //{
        //    MessageBox.Show("You clicked (" + chartPoint.X + "," + chartPoint.Y + ")");
        //}


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

                //如果系统还未开始处理，则不会更改信号
                if (!string.IsNullOrEmpty(mc_MachineStatusHander.StatusCode) && !string.IsNullOrEmpty(mc_MachineStatusHander.StatusDescription))
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
                //Common.LogHandler.Log(ex.ToString());
            }
        }

        /// <summary>
        /// 串口6获取数据错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serialPort6_ErrorReceived(object sender, System.IO.Ports.SerialErrorReceivedEventArgs e)
        {
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
                    SettingMachineStatusLight();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message, "更新机器状态错误");
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
        /// 工单数输入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txt_WorkOrderCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;

            if(!Char.IsDigit(ch) && ch!=8 && ch != 46)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// 预计周期
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txt_PlanWorkTime_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;

            if (!Char.IsDigit(ch) && ch != 8 && ch != 46)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// 不良品
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
        /// 工单数==》修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txt_WorkOrderCount_TextChanged(object sender, EventArgs e)
        {
            int int_WorkOrderCount = 0;
            int int_PlanWorkTime = 0;

            int.TryParse(txt_WorkOrderCount.Text.Trim(), out int_WorkOrderCount);
            int.TryParse(txt_PlanWorkTime.Text.Trim(), out int_PlanWorkTime);

            if (int_WorkOrderCount > 0 && int_PlanWorkTime > 0)
            {
                mc_MachineStatusHander.SettingMachineCompleteDateTime();
            }
        }

        /// <summary>
        /// 预计周期==》修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txt_PlanWorkTime_TextChanged(object sender, EventArgs e)
        {
            int int_WorkOrderCount = 0;
            int int_PlanWorkTime = 0;

            int.TryParse(txt_WorkOrderCount.Text.Trim(), out int_WorkOrderCount);
            int.TryParse(txt_PlanWorkTime.Text.Trim(), out int_PlanWorkTime);

            if (int_WorkOrderCount > 0 && int_PlanWorkTime > 0)
            {
                mc_MachineStatusHander.SettingMachineCompleteDateTime();
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

            if (int_RejectsCount > 0)
            {
                //不良品数量
                mc_MachineStatusHander.mc_MachineProduceStatusHandler.ProductErrorCount = int_RejectsCount;

                mc_MachineStatusHander.mc_MachineProduceStatusHandler.SettingMachineNondefectiveCount();
            }
            else
            {
                //不良品数量
                mc_MachineStatusHander.mc_MachineProduceStatusHandler.ProductErrorCount = 0;

                mc_MachineStatusHander.mc_MachineProduceStatusHandler.SettingMachineNondefectiveCount();
            }
        }


        /*菜单按钮事件*/
        /*---------------------------------------------------------------------------------------*/

        /// <summary>
        /// 开始按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Start_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txt_WorkOrderCount.Text.Trim()))
            {
                ShowErrorMessage("请输入[工单数]", "运行参数检测");
                txt_WorkOrderCount.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txt_PlanWorkTime.Text.Trim()))
            {
                ShowErrorMessage("请输入[预计周期]", "运行参数检测");
                txt_PlanWorkTime.Focus();
                return;
            }

            if (!string.IsNullOrEmpty(mc_MachineStatusHander.StatusCode) && !string.IsNullOrEmpty(mc_MachineStatusHander.StatusDescription))
            {
                btn_StatusLight_Click(sender, null);
            }
            else
            {
                //开工时间
                mc_MachineStatusHander.StartWorkTime = System.DateTime.Now;

                //设置机器完成时间
                mc_MachineStatusHander.SettingMachineCompleteDateTime();

                //状态
                mc_MachineStatusHander.ChangeStatus("StartProduce", "运行", "WesChen", "001A");
                SettingMachineStatusLight();
            }
        }

        /// <summary>
        /// 暂停按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Stop_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(mc_MachineStatusHander.StatusCode) && !string.IsNullOrEmpty(mc_MachineStatusHander.StatusDescription))
            {
                btn_StatusLight_Click(sender, null);
            }
            else
            {
                ShowErrorMessage("请先开始工单", "工单停止失败");
            }
        }

        /// <summary>
        /// 恢复按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Recovery_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(mc_MachineStatusHander.StatusCode) && !string.IsNullOrEmpty(mc_MachineStatusHander.StatusDescription))
            {
                btn_StatusLight_Click(sender, null);
            }
            else
            {
                ShowErrorMessage("没有暂停的工单", "工单恢复失败");
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
        }
    }
}
