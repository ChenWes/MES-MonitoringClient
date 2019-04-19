using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MES_MonitoringClient
{
    public partial class frmScanRFID : Form
    {
        public enum OperationType
        {
            ChangeMachineType,

            OnDuty,
            OffDuty,

            StartJobOrder,
            StopJobOrder,
            ResumeJobOrder
        }
        //操作类型（修改状态、上班、下班等）
        public OperationType MC_OperationType;

        //是否主动取消刷卡
        public bool MC_IsManualCancel = false;

        //刷卡员工信息
        public DataModel.Employee MC_EmployeeInfo = null;

        //修改机器状态参数
        public DataModel.formParameter.frmChangeMachineStatusPara MC_frmChangeMachineStatusPara = null;        

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
                //接收的时候，将用户处理掉
                MC_EmployeeInfo = null;

                //获取串口
                System.IO.Ports.SerialPort COM = (System.IO.Ports.SerialPort)sender;

                //获取到串口1刷卡的信息
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


                //更新界面
                this.Invoke((EventHandler)(delegate
                {
                    lab_ScanStatus.Text = "刷卡成功";
                    lab_CardID.Text = "卡号:" + stringBuilder.ToString();                    
                    
                    //IC卡检测（能否匹配员工信息及有效性）
                    CheckCardID(stringBuilder.ToString());
                }
                    )
                );                
            }
            catch (Exception ex)
            {
                ShowErrorMessage( "RFID串口获取数据时出错","serialPort1_DataReceived");
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
                        OnStartJobOrder();
                        break;
                    case OperationType.StopJobOrder:
                        OnStopJobOrder(employee.JobPostionID);
                        break;
                    case OperationType.ResumeJobOrder:
                        OnResumeJobOrder();
                        break;
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


                System.Threading.Thread.Sleep(1000);

                //弹出可操作的界面
                frmChangeStatus frmChangeStatus = new frmChangeStatus();
                frmChangeStatus.mc_machineStatuses = machineStatuses;
                frmChangeStatus.ShowDialog();

                //完成参数传递（机器状态窗口至刷卡窗口）
                MC_frmChangeMachineStatusPara = frmChangeStatus.MC_frmChangeMachineStatusPara;
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
                frmSelectJobOrder newfrmSelectJobOrder = new frmSelectJobOrder();
                newfrmSelectJobOrder.MC_JobOrderFilter = frmSelectJobOrder.FilterOrderType.NoStart;
                newfrmSelectJobOrder.ShowDialog();
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


                System.Threading.Thread.Sleep(1000);

                //弹出可操作的界面
                frmChangeStatus frmChangeStatus = new frmChangeStatus();
                frmChangeStatus.mc_machineStatuses = machineStatuses;
                frmChangeStatus.ShowDialog();

                //完成参数传递（机器状态窗口至刷卡窗口）
                MC_frmChangeMachineStatusPara = frmChangeStatus.MC_frmChangeMachineStatusPara;
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

                if (MC_OperationType == OperationType.ChangeMachineType && MC_frmChangeMachineStatusPara == null)
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
    }
}
