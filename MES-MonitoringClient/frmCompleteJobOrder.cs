using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MES_MonitoringClient
{
    public partial class frmCompleteJobOrder : Form
    {
        //工单列表
        public List<DataModel.JobOrder> JobOrderList = null;
        //选择的工单列表
        public List<DataModel.JobOrder> selectJobOrderList = new List<DataModel.JobOrder>();
 
        //显示工单列表
        TableLayoutPanel  tableLayoutPanel;
     

        public byte[] RevDataBuffer = new byte[30];
        public UInt32 RevDataBufferCount;

        //是否接收数据
        private bool MC_CanLoadData = true;      
      
        //是否主动取消刷卡
        public bool MC_IsManualCancel = false;

        //刷卡员工信息
        public DataModel.Employee MC_EmployeeInfo = null;

        //修改机器状态参数
        public DataModel.formParameter.frmChangeMachineStatusPara MC_frmChangeMachineStatusPara = null;

    



        /*主窗口方法*/
        /*---------------------------------------------------------------------------------------*/

        public frmCompleteJobOrder()
        {
            InitializeComponent();
        }

        private void frmCompleteJobOrder_Load(object sender, EventArgs e)
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
                showEmployeeImage(JobOrderList);



            }
            catch (Exception ex)
            {
                Common.LogHandler.WriteLog("RFID界面初始化失败", ex);
                ShowErrorMessage(ex.Message, "RFID界面初始化");
            }
        }

        private void frmCompleteJobOrder_FormClosing(object sender, FormClosingEventArgs e)
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


                OnStopJobOrder(employee.JobPostionID);


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


                if (selectJobOrderList.Count == JobOrderList.Count)
                {
                    //弹出可操作的界面
                    frmChangeStatus frmChangeStatus = new frmChangeStatus();
                    frmChangeStatus.mc_machineStatuses = machineStatuses;
                    frmChangeStatus.ShowDialog();
                    SettingButtonEnable(false);//按钮不可用


                    //完成参数传递（机器状态窗口至刷卡窗口）
                    MC_frmChangeMachineStatusPara = frmChangeStatus.MC_frmChangeMachineStatusPara;
                    SettingButtonEnable(true);//按钮可用
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message, "选择机器状态错误");
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

                if (selectJobOrderList.Count == JobOrderList.Count)
                {
                    //弹出可操作的界面
                    frmChangeStatus frmChangeStatus = new frmChangeStatus();
                    frmChangeStatus.mc_machineStatuses = machineStatuses;
                    frmChangeStatus.ShowDialog();
                    SettingButtonEnable(false);//按钮不可用
                    //完成参数传递（机器状态窗口至刷卡窗口）
                    MC_frmChangeMachineStatusPara = frmChangeStatus.MC_frmChangeMachineStatusPara;
                    SettingButtonEnable(true);//按钮可用
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message, "工单暂停错误");
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
               
                if (selectJobOrderList.Count == JobOrderList.Count)
                {
                    if (MC_frmChangeMachineStatusPara == null)
                    {
                        OnStopJobOrder(MC_EmployeeInfo.JobPostionID);
                        if (MC_frmChangeMachineStatusPara == null)
                        {
                            MC_IsManualCancel = true;
                            MC_EmployeeInfo = null;
                        }
                    }
                }
                if (MC_frmChangeMachineStatusPara != null)
                {
                    if (selectJobOrderList.Count != JobOrderList.Count)
                    {
                        MC_frmChangeMachineStatusPara = null;
                    }
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
        /// 员工工单显示
        /// </summary>
        private void showEmployeeImage(List<DataModel.JobOrder> jobOrders)
        {
          
            tableLayoutPanel = new TableLayoutPanel();

            tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel.RowCount = 1;
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel.ColumnCount = jobOrders.Count;
            
            for (int i = 0; i < jobOrders.Count; i++)
            {
                tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
                Button button = new Button();
              
                button.Anchor = System.Windows.Forms.AnchorStyles.Top;
                button.Name = jobOrders[i].JobOrderID;
                button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                button.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                button.ForeColor = System.Drawing.Color.White;
                button.Size = new System.Drawing.Size(150, 80);
                var sumProductCount = jobOrders[i].MachineProcessLog.Sum(t => t.ProduceCount);
                var sumErrorCount = jobOrders[i].MachineProcessLog.Sum(t => t.ErrorCount);
                //够数
                if(jobOrders[i].OrderCount<=(sumProductCount - sumErrorCount))
                {
                    selectJobOrderList.Add(jobOrders[i]);
                    button.Tag = true;
                    button.BackColor = Color.Red;
                    if (selectJobOrderList.Count == 1)
                    {
                        this.lab_select.Text = "已选择工单：" + jobOrders[i].JobOrderID;
                    }
                    else
                    {
                        this.lab_select.Text = this.lab_select.Text + "," + jobOrders[i].JobOrderID;
                    }
                   
                }
                else
                {
                    button.Tag = false;
                    button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
                }
                button.Text = jobOrders[i].JobOrderID+ "\r\n(" + Math.Round((double)(sumProductCount - sumErrorCount) * 100 / jobOrders[i].OrderCount, 2) + "%" + ")";
                button.UseVisualStyleBackColor = false;
                button.Click += new System.EventHandler(this.button1_Click);
                tableLayoutPanel.Controls.Add(button, i, 0);
            }
      
            
            tlp_mian.Controls.Add(tableLayoutPanel, 0, 3);

        }




     
        
        private void button1_Click(object sender, EventArgs e)
        {
            Button b1 = (Button)sender;
            if (!bool.Parse(b1.Tag.ToString()))
            {
                foreach(var item in JobOrderList.ToArray())
                {
                    if (item.JobOrderID == b1.Name)
                    {
                        selectJobOrderList.Add(item);
                        break;
                    }
                }
                for(int i=0;i<selectJobOrderList.Count;i++)
                {
                    if (i == 0)
                    {
                        this.lab_select.Text = "已选择工单：" + selectJobOrderList[i].JobOrderID;
                    }
                    else
                    {
                        this.lab_select.Text = this.lab_select.Text +","+ selectJobOrderList[i].JobOrderID;
                    }
                 
                }

                b1.BackColor = Color.Red;
                b1.Tag = true;
            }
            else
            {
                foreach (var item in selectJobOrderList.ToArray())
                {
                    if (item.JobOrderID == b1.Name)
                    {
                        selectJobOrderList.Remove(item);
                        break;
                    }
                }
                if (selectJobOrderList.Count > 0)
                {
                    for (int i = 0; i < selectJobOrderList.Count; i++)
                    {
                        if (i == 0)
                        {
                            this.lab_select.Text = "已选择工单：" + selectJobOrderList[i].JobOrderID;
                        }
                        else
                        {
                            this.lab_select.Text = this.lab_select.Text +","+ selectJobOrderList[i].JobOrderID;
                        }

                    }
                }
                else
                {
                    this.lab_select.Text = "请选择需要完成的工单";
                }
               
                b1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
                b1.Tag = false;
            }

        }

      
    }
}
