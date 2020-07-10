using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MES_MonitoringClient
{
    public partial class frmScanRFID : Form
    {

        public byte GetDataMode;

        public byte[] RevDataBuffer = new byte[30];
        public UInt32 RevDataBufferCount;

        //是否接收数据
        private bool MC_CanLoadData = true;

        public enum OperationType
        {
            ChangeMachineType,

            OnDuty,
            OffDuty,

            StartJobOrder,
            StopJobOrder,
            ResumeJobOrder,
            OffPower,
            Update
        }
        //操作类型（修改状态、上班、下班等）
        public OperationType MC_OperationType;

        public enum OperationType_Prompt
        {
            StartJobOrder,
            StopJobOrder,
            finishJobOrder,
            ResumeJobOrder,
            ChangeMachineType,
            Quit,
            Update
        }
        //类型提示
        public OperationType_Prompt MC_OperationType_Prompt;

        
        //是否主动取消刷卡
        public bool MC_IsManualCancel = false;

        //刷卡员工信息
        public DataModel.Employee MC_EmployeeInfo = null;

        //修改机器状态参数
        public DataModel.formParameter.frmChangeMachineStatusPara MC_frmChangeMachineStatusPara = null;

        //开始订单参数
        public List<DataModel.JobOrder> MC_frmChangeJobOrderPara = null;

        //获取服务端新版本信息
        string jsonString = null;
        //旧版本
        string oldVersion = Common.ConfigFileHandler.GetAppConfig("Version");

        //当前程序名
        private string processName = Process.GetCurrentProcess().ProcessName;
        //更新地址
        private string update_Path = new DirectoryInfo(Application.StartupPath).Parent.FullName + @"Client_Update\Mes_Update.exe";

        //守护服务名
        private string defendServiceName = "MESServiceDefend";
        //守护进程名
        private string defendProgramName = "MES-Service-Defend";
        //MES服务名
        private string MESServiceName = "MESUploadDataService";
        //MES服务进程名
        private string MESProgramName = "MES-MonitoringService";
        //MES系统进程名
        private string MESClientProgramName = "MES-MonitoringClient";
        /*---------------------------------------------------------------------------------------*/
        //private long COM1_ReceiveDataCount = 0;
        //private StringBuilder COM1_DataStringBuilder = new StringBuilder();
        /*---------------------------------------------------------------------------------------*/
        //public delegate void AddDataDelegate(String myString);
        //public AddDataDelegate myDelegate;


        /*主窗口方法*/
        /*---------------------------------------------------------------------------------------*/

        public frmScanRFID()
        {
            InitializeComponent();
        }

        private void frmScanRFID_Load(object sender, EventArgs e)
        {
            try
            {                
                //窗口最大化
                this.WindowState = FormWindowState.Maximized;

                //RFID配置端口默认配置
                RFIDSerialPortGetDefaultSetting();

                //如果串口1未开启，则开启串口1
                if (!serialPort1.IsOpen)
                {
                    serialPort1.Open();
                }
                //判断操作类型
                if( MC_OperationType_Prompt  == OperationType_Prompt.finishJobOrder)
                {
                    this.lab_ScanStatus.Text = "即将完成工单，请确认刷卡";
                }
                else if(MC_OperationType_Prompt == OperationType_Prompt.ResumeJobOrder)
                {
                    this.lab_ScanStatus.Text = "即将恢复工单，请确认刷卡";
                }
                else if(MC_OperationType_Prompt == OperationType_Prompt.StopJobOrder)
                {
                    this.lab_ScanStatus.Text = "即将暂停工单，请确认刷卡";
                }
                else if (MC_OperationType_Prompt == OperationType_Prompt.StartJobOrder)
                {
                    this.lab_ScanStatus.Text = "即将开始工单，请确认刷卡";
                }
                else if (MC_OperationType_Prompt == OperationType_Prompt.ChangeMachineType)
                {
                    this.lab_ScanStatus.Text = "即将更改机器状态，请确认刷卡";
                }
                else if (MC_OperationType_Prompt == OperationType_Prompt.Quit)
                {
                    this.lab_ScanStatus.Text = "即将退出系统，请确认刷卡";
                }
                else if (MC_OperationType_Prompt == OperationType_Prompt.Update)
                {
                    this.lab_ScanStatus.Text = "即将更新系统，请确认刷卡";
                }
                else
                {
                    this.lab_ScanStatus.Text = "等待刷卡";
                }
            }
            catch (Exception ex)
            {
                Common.LogHandler.WriteLog("RFID界面初始化失败", ex);
                ShowErrorMessage(ex.Message, "RFID界面初始化");
            }
        }

        private void frmScanRFID_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
        }

        /*窗口公共方法*/
        /*---------------------------------------------------------------------------------------*/

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
        /// 设置窗口按钮是否可用
        /// </summary>
        /// <param name="EnableFlag"></param>
        private void SettingButtonEnable(bool EnableFlag)
        {
            btn_Cancel.Enabled = EnableFlag;
            btn_Confirm.Enabled = EnableFlag;
        }

        /// <summary>
        /// RFID串口默认配置
        /// </summary>
        private void RFIDSerialPortGetDefaultSetting()
        {
            //端口名称
            serialPort1.PortName = Common.ConfigFileHandler.GetAppConfig("RFIDSerialPortName");

            //波特率
            int defaultBaudRate = 0;
            int.TryParse(Common.ConfigFileHandler.GetAppConfig("RFIDSerialBaudRate"), out defaultBaudRate);
            serialPort1.BaudRate = defaultBaudRate;

            //奇偶性验证
            string defaultParity = Common.ConfigFileHandler.GetAppConfig("RFIDSerialParity");
            if (defaultParity.ToUpper() == System.IO.Ports.Parity.None.ToString().ToUpper())
            {
                serialPort1.Parity = System.IO.Ports.Parity.None;
            }
            else if (defaultParity.ToUpper() == System.IO.Ports.Parity.Odd.ToString().ToUpper())
            {
                serialPort1.Parity = System.IO.Ports.Parity.Odd;
            }
            else if (defaultParity.ToUpper() == System.IO.Ports.Parity.Even.ToString().ToUpper())
            {
                serialPort1.Parity = System.IO.Ports.Parity.Even;
            }
            else if (defaultParity.ToUpper() == System.IO.Ports.Parity.Mark.ToString().ToUpper())
            {
                serialPort1.Parity = System.IO.Ports.Parity.Mark;
            }
            else if (defaultParity.ToUpper() == System.IO.Ports.Parity.Space.ToString().ToUpper())
            {
                serialPort1.Parity = System.IO.Ports.Parity.Space;
            }

            //数据位
            int defaultDataBits = 0;
            int.TryParse(Common.ConfigFileHandler.GetAppConfig("RFIDSerialDataBits"), out defaultDataBits);
            serialPort1.DataBits = defaultDataBits;

            //停止位
            string defaultStopBits = Common.ConfigFileHandler.GetAppConfig("RFIDSerialStopBits");
            if (defaultStopBits.ToUpper() == System.IO.Ports.StopBits.None.ToString().ToUpper())
            {
                serialPort1.StopBits = System.IO.Ports.StopBits.None;
            }
            else if (defaultStopBits.ToUpper() == System.IO.Ports.StopBits.One.ToString().ToUpper())
            {
                serialPort1.StopBits = System.IO.Ports.StopBits.One;
            }
            else if (defaultStopBits.ToUpper() == System.IO.Ports.StopBits.OnePointFive.ToString().ToUpper())
            {
                serialPort1.StopBits = System.IO.Ports.StopBits.OnePointFive;
            }
            else if (defaultStopBits.ToUpper() == System.IO.Ports.StopBits.Two.ToString().ToUpper())
            {
                serialPort1.StopBits = System.IO.Ports.StopBits.Two;
            }

        }

        /*获取串口数据事件*/
        /*---------------------------------------------------------------------------------------*/

        /// <summary>
        /// 串口1获取数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                int revbuflen;

                byte pkttype;
                byte pktlength = 0x0;
                byte cmd;
                byte err;

                bool revflag;
                bool status;

                //是否接收数据开关
                if (!MC_CanLoadData) return;

                //获取串口
                System.IO.Ports.SerialPort SerialPort = (System.IO.Ports.SerialPort)sender;                  

                revbuflen = SerialPort.BytesToRead; //读取串口缓冲区中接收字节数
                revflag = false;
                if (revbuflen > 0) //判断串口缓冲区中是否有数据
                {
                    //接收的时候，将用户处理掉
                    MC_EmployeeInfo = null;
                    revflag = true;
                    System.Threading.Thread.Sleep(50); //等待完成数据包接收完成
                }
                else
                {
                    RevDataBuffer = new byte[] { };
                    //如果串口出来的数据为空，则不处理
                }

                RevDataBufferCount = 0;
                while (revflag) //判读缓冲区是否有数据
                {
                    RevDataBuffer[RevDataBufferCount] = (byte)SerialPort.ReadByte(); //读出串口缓冲区数据到数组中
                    RevDataBufferCount = RevDataBufferCount + 1;
                    if (RevDataBufferCount >= 30)//防止缓冲区溢出
                    {
                        RevDataBufferCount = 0;
                    }


                    //是否继续有数据进来
                    System.Threading.Thread.Sleep(2);
                    revbuflen = SerialPort.BytesToRead;

                    if (revbuflen > 0)
                    {
                        revflag = true;
                    }
                    else
                    {
                        revflag = false;
                    }
                }

                if (GetValidateValueLength(RevDataBuffer, 4))
                {
                    //淳元电脑，只有四位有效数据

                    byte[] tempbuf_1 = new byte[4];
                    byte[] tempbuf_2 = new byte[4];

                    for (int i = 0; i < 4; i++)
                    {
                        tempbuf_1[i] = RevDataBuffer[i]; //获取卡号，16进制，卡号保存在数组的7-10字节,正向在数组中排序
                        tempbuf_2[3 - i] = RevDataBuffer[i]; //获取卡号，16进制，卡号保存在数组的7-10字节，反向向在数组中排序
                    }

                    //更新界面
                    //卡号处理
                    this.Invoke((EventHandler)(delegate
                    {
                        lab_ScanStatus.Text = "刷卡成功";
                        lab_CardID.Text = "卡号:" + GetCardID(tempbuf_2);

                        //IC卡检测（能否匹配员工信息及有效性）
                        CheckCardID(GetCardID(tempbuf_2));
                    }
                        )
                    );
                }
                else if (GetValidateValueLength(RevDataBuffer, 12))
                {
                    //华北工控电脑，有十二位数据

                    //if ((RevDataBuffer[1] <= RevDataBufferCount) && (RevDataBufferCount != 0x0))//判断是否接收到一帧完成数据
                    if (RevDataBuffer[1] == 0x0C)//判断是否接收到一帧完成数据
                    {
                        RevDataBufferCount = 0x0;
                        status = CheckSumOut(RevDataBuffer, RevDataBuffer[1]);//计算校验和
                        if (status == false)
                        {
                            return;
                        }

                        pkttype = RevDataBuffer[0];  //获取包类型
                        pktlength = RevDataBuffer[1]; //获取包长度
                        cmd = RevDataBuffer[2]; //获取命令
                        err = RevDataBuffer[4]; //获取包状态，0x00:读卡器成功，包有效

                        if ((pkttype == 0x04) && (cmd == 0x02) && (pktlength == 0x0C) && (err == 0x00)) //开始解析数据包,判断是否为卡号数据包
                        {

                            byte[] tempbuf_1 = new byte[4];
                            byte[] tempbuf_2 = new byte[4];

                            for (int i = 0; i < 4; i++)
                            {
                                tempbuf_1[i] = RevDataBuffer[i + 7]; //获取卡号，16进制，卡号保存在数组的7-10字节,正向在数组中排序
                                tempbuf_2[3 - i] = RevDataBuffer[i + 7]; //获取卡号，16进制，卡号保存在数组的7-10字节，反向向在数组中排序
                            }


                            //更新界面
                            //卡号处理
                            this.Invoke((EventHandler)(delegate
                            {
                                lab_ScanStatus.Text = "刷卡成功";
                                lab_CardID.Text = "卡号:" + GetCardID(tempbuf_2);

                            //IC卡检测（能否匹配员工信息及有效性）
                            CheckCardID(GetCardID(tempbuf_2));
                            }
                                )
                            );

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                //写日志
                Common.LogHandler.WriteLog("RFID串口获取数据时出错", ex);
                //委托主线程报错，防止报错消息被隐藏
                this.Invoke(new Action(() =>
                 {
                     ShowErrorMessage("RFID串口获取数据时出错，请重试", "serialPort1_DataReceived");
                 }));
              
                //响铃并显示异常给用户
                System.Media.SystemSounds.Beep.Play();
            }
        }

        /// <summary>
        /// IC卡检测（能否匹配员工信息及有效性）
        /// 以及根据不同的类型打开窗口
        /// </summary>
        /// <param name="cardID"></param>
        private void CheckCardID(string cardID)
        {
            try
            {
                DataModel.Employee employee;
                try
                {
                    //通过卡号找到员工
                    employee = Common.EmployeeHelper.QueryEmployeeByCardID(cardID.Trim());
                }
                catch (Exception ex)
                {
                    throw new Exception("搜索员工出错，原因是：" + ex.Message);
                }

                //员工检测
                if (employee == null) throw new Exception("未知员工");
                if (!employee.IsActive) throw new Exception("员工已被禁止访问系统");

                //员工信息赋值到公共变量
                MC_EmployeeInfo = employee;


                MC_CanLoadData = false;

                //选定操作
                switch (MC_OperationType)
                {
                    case OperationType.ChangeMachineType:
                        //修改机器状态
                        OnChangeMachineType(employee.JobPostionID);
                        break;
                    case OperationType.OnDuty:
                        break;
                    case OperationType.OffDuty:                    
                        break;
                    case OperationType.StartJobOrder:
                        //开始工单
                        OnStartJobOrder();
                        break;
                    case OperationType.StopJobOrder:
                        //停止工单
                        OnStopJobOrder(employee.JobPostionID);
                        break;
                    case OperationType.ResumeJobOrder:
                        //恢复工单
                        OnResumeJobOrder();
                        break;
                    case OperationType.Update:
                        if (MC_EmployeeInfo.JobPostionResponsibility!=null&&MC_EmployeeInfo.JobPostionResponsibility.Trim()=="系统管理员")
                        {
                            if (File.Exists(update_Path))
                            {
                                ThreadStart threadStart_Update = new ThreadStart(start_Update);//通过ThreadStart委托告诉子线程执行什么方法　　
                                Thread thread_Update = new Thread(threadStart_Update);
                                thread_Update.Start();//启动新线程
                                this.lab_CardID.Text = "正在检测";
                            }
                            else
                            {
                                MessageBox.Show("不存在更新程序" + update_Path);
                            }
                        }
                        else
                        {
                            ShowErrorMessage("对不起，您没有相应权限", "权限认证");
                        }
                        break;
                    case OperationType.OffPower:                                                
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message, "权限认证");
            }
        }
        
        /// <summary>
        /// 选择机器状态
        /// </summary>
        /// <param name="jobpositionID"></param>
        private void OnChangeMachineType(string jobpositionID)
        {
            try
            {
                //通过员工职位ID找到
                DataModel.JobPositon jobPositon = Common.JobPositionHelper.GetJobPositon(jobpositionID);
                if (jobPositon == null) throw new Exception("未知员工职位");


                //找到可操作的机器状态
                List<DataModel.MachineStatus> machineStatuses = Common.MachineStatusHelper.GetMachineStatusByIDArray(jobPositon.MachineStatuss.ToArray());
                if (machineStatuses == null || machineStatuses.Count == 0) throw new Exception("暂时没有机器状态可选");


                //弹出可操作的界面
                frmChangeStatus frmChangeStatus = new frmChangeStatus();
                frmChangeStatus.mc_machineStatuses = machineStatuses;
                frmChangeStatus.ShowDialog();
                SettingButtonEnable(false);//按钮不可用


                //完成参数传递（机器状态窗口至刷卡窗口）
                MC_frmChangeMachineStatusPara = frmChangeStatus.MC_frmChangeMachineStatusPara;
                SettingButtonEnable(true);//按钮可用
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message, "选择机器状态错误");
            }
        }


        private void OnStartJobOrder()
        {
            try
            {
                //弹出可操作的界面
                frmSelectJobOrder newfrmSelectJobOrder = new frmSelectJobOrder();
                newfrmSelectJobOrder.MC_JobOrderFilter = frmSelectJobOrder.FilterOrderType.NoStart;
                newfrmSelectJobOrder.ShowDialog();
                SettingButtonEnable(false);//按钮不可用


                //完成参数传递（开始的工单至刷卡窗口）
                MC_frmChangeJobOrderPara = newfrmSelectJobOrder.MC_frmChangeJobOrderPara;
                SettingButtonEnable(true);//按钮可用
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message, "工单开始错误");
            }
        }

        private void OnStopJobOrder(string jobpositionID)
        {
            try
            {
                //通过员工职位ID找到
                DataModel.JobPositon jobPositon = Common.JobPositionHelper.GetJobPositon(jobpositionID);
                if (jobPositon == null) throw new Exception("未知员工职位");


                //找到可操作的机器状态
                List<DataModel.MachineStatus> machineStatuses = Common.MachineStatusHelper.GetMachineStatusByIDArray(jobPositon.MachineStatuss.ToArray());
                if (machineStatuses == null || machineStatuses.Count == 0) throw new Exception("暂时没有机器状态可选");
                

                //弹出可操作的界面
                frmChangeStatus frmChangeStatus = new frmChangeStatus();
                frmChangeStatus.mc_machineStatuses = machineStatuses;
                frmChangeStatus.ShowDialog();
                SettingButtonEnable(false);//按钮不可用

                //完成参数传递（机器状态窗口至刷卡窗口）
                MC_frmChangeMachineStatusPara = frmChangeStatus.MC_frmChangeMachineStatusPara;
                SettingButtonEnable(true);//按钮可用
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message, "工单暂停错误");
            }
        }

        private void OnResumeJobOrder()
        {
            try
            {
                frmSelectJobOrder newfrmSelectJobOrder = new frmSelectJobOrder();
                newfrmSelectJobOrder.MC_JobOrderFilter = frmSelectJobOrder.FilterOrderType.NoCompleted;
                newfrmSelectJobOrder.ShowDialog();
                SettingButtonEnable(false);//按钮不可用

                //完成参数传递（开始的工单至刷卡窗口）
                MC_frmChangeJobOrderPara = newfrmSelectJobOrder.MC_frmChangeJobOrderPara;
                SettingButtonEnable(true);//按钮可用
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message, "工单恢复错误");
            }
        }

        private void serialPort1_ErrorReceived(object sender, System.IO.Ports.SerialErrorReceivedEventArgs e)
        {
            ShowErrorMessage("RFID串口获取数据出错", "serialPort1_ErrorReceived");
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            //用户主动取消操作
            MC_IsManualCancel = true;

            MC_EmployeeInfo = null;
            MC_frmChangeMachineStatusPara = null;

            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btn_Confirm_Click(object sender, EventArgs e)
        {
            try
            {
                if (MC_EmployeeInfo == null)
                {
                    throw new Exception("未刷卡");
                }

                //修改状态
                if (MC_OperationType == OperationType.ChangeMachineType && MC_frmChangeMachineStatusPara == null)
                {
                    throw new Exception("未选择任何机器状态");
                }

                //开始工单
                if (MC_OperationType == OperationType.StartJobOrder && MC_frmChangeJobOrderPara == null)
                {
                    throw new Exception("未选择任何工单");
                }

                //恢复工单
                if (MC_OperationType == OperationType.ResumeJobOrder && MC_frmChangeJobOrderPara == null)
                {
                    throw new Exception("未选择任何工单");
                }

                //暂停工单
                if (MC_OperationType == OperationType.StopJobOrder && MC_frmChangeMachineStatusPara == null)
                {
                    throw new Exception("未选择任何机器状态");
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("确认失败，原因是：" + ex.Message, "无法确认");
            }
        }

        //---------------------------------------------------

        public void CheckSum(byte[] buf, byte len)
        {
            byte i;
            byte checksum;
            checksum = 0;
            for (i = 0; i < (len - 1); i++)
            {
                checksum ^= buf[i];
            }
            buf[len - 1] = (byte)~checksum;
        }
        public string byteToHexStr(byte[] bytes, int len)  //数组转十六进制字符显示
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < len; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }
        public string byteToHexStrH(byte[] bytes, int len)  //数组转十六进制字符显示
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < len; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                    returnStr += ' ';
                }
            }
            return returnStr;
        }
        private static byte[] strToToHexByte(string hexString) //字符串转16进制
        {
            //hexString = hexString.Replace(" ", " "); 
            if ((hexString.Length % 2) != 0)
                hexString = "0" + hexString;
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
        private static byte[] strToDecByte(string hexString)//字符串转10进制
        {
            //hexString = hexString.Replace(" ", " "); 
            if ((hexString.Length % 2) != 0)
                hexString = "0" + hexString;
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 10);
            return returnBytes;
        }
        public static bool CheckSumOut(byte[] buf, byte len)
        {
            byte i;
            byte checksum;
            checksum = 0;
            for (i = 0; i < (len - 1); i++)
            {
                checksum ^= buf[i];
            }
            if (buf[len - 1] == (byte)~checksum)
            {
                return true;
            }
            return false;
        }

        //转换成文本
        public static string GetCardID(byte[] buf)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < buf.Length; i++)
            {
                sb.Append(buf[i].ToString("X2"));
            }

            return sb.ToString();
        }

        //获取有效数据
        //是否只有某位的数据（淳元出来的只有四位有效数据，华北工控有十二位有效数据，但有头和检验等数据，需要截取）
        public static bool GetValidateValueLength(byte[] buf, int len)
        {
            bool returnFlag = true;
            if (buf.Length == 0) return false;//如果数组为空，那直接为不匹配

            for (int i = len; i < buf.Length; i++)
            {
                if (buf[i].ToString("X2") != "00")
                {
                    returnFlag = false;
                    return returnFlag;
                }
            }

            return returnFlag;
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
                    this.lab_CardID.Text = "检测失败";
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
                        this.lab_CardID.Text = "当前版本为最新版本";
                    }
                    else
                    {
                        this.lab_CardID.Text = "可升级";
                        if (File.Exists(update_Path))
                        {
                            if (CheckSericeStart(defendServiceName, defendProgramName))
                            {
                                StopService(defendServiceName, defendProgramName);
                            }
                            if (CheckSericeStart(MESServiceName, MESProgramName))
                            {
                                StopService(MESServiceName, MESProgramName);
                            }
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
                            this.lab_CardID.Text = "可升级";
                        }

                    }
                }
                else
                {
                    this.lab_CardID.Text = "获取不到版本信息";
                }
            }));
        }
        /// <summary>
        /// 判断服务有无启动
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// /// <param name="programName">进程名</param>
        private bool CheckSericeStart(string serviceName, string programName)
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
    }
}
