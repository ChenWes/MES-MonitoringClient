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
    public partial class frmAttend : Form
    {

        public byte GetDataMode;

        public byte[] RevDataBuffer = new byte[30];
        public UInt32 RevDataBufferCount;

        //是否接收数据
        private bool MC_CanLoadData = true;

        public enum JobPositionCode
        {
            Employee,
            QC
        }


        //是否主动取消刷卡
        public bool MC_IsManualCancel = false;

        //刷卡员工信息
        public DataModel.Employee MC_EmployeeInfo = null;

   

        //开始订单参数
        public List<DataModel.JobOrder> MC_frmChangeJobOrderPara = null;
        //图片
        TableLayoutPanel imageTableLayoutPanel;
        //显示员工列表
        TableLayoutPanel  tableLayoutPanel;
        //签到的打卡记录
        List<DataModel.ClockInRecord> inClockInRecords=new List<DataModel.ClockInRecord>();

        
        //用于显示员工
        List<DataModel.displayClockInRecord> displayEmployeeClockInRecords = new List<DataModel.displayClockInRecord>();
        //保存QC
        DataModel.ClockInRecord QCClockInRecord = null;
        //用于显示QC
        DataModel.displayClockInRecord displayQCClockInRecord = null;
        //本机
        private DataModel.Machine MC_Machine = null;
        private Common.ClockInRecordHandler clockInRecordHandler = new Common.ClockInRecordHandler();
        //用于更新的记录
        private DataModel.ClockInRecord clockInRecord = null;
        //显示QC
        TableLayoutPanel tableLayoutPanel_QC;
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAttend));
        DateTime startTime ;
        ThreadStart DateTimeThreadFunction=null;
        Thread DateTimeThreadClass=null;
        Common.TimmerHandler TTimerClass=null;
        private bool listening = false;//是否没有执行完invoke相关操作
        private bool closing = false;//是否正在关闭串口，执行Application.DoEvents，并阻止再次invoke
        //判断数据是否有更新
        private bool isUpdate = false;
        //当前显示页数
        private  int page = 1;
        //每页显示员工数量
        private int pagecount = 5;

        /*---------------------------------------------------------------------------------------*/
        //private long COM1_ReceiveDataCount = 0;
        //private StringBuilder COM1_DataStringBuilder = new StringBuilder();
        /*---------------------------------------------------------------------------------------*/
        //public delegate void AddDataDelegate(String myString);
        //public AddDataDelegate myDelegate;


        /*主窗口方法*/
        /*---------------------------------------------------------------------------------------*/

        public frmAttend()
        {
            InitializeComponent();


        }
        private void frmAttend_Load(object sender, EventArgs e)
        {
            try
            {
                //窗口最大化
                this.WindowState = FormWindowState.Maximized;
                this.lab_DateTime.Text = string.Format(DateTime.Now.ToString("MM-dd HH:mm:ss"));
                startTime = DateTime.Now;
                //RFID配置端口默认配置
                RFIDSerialPortGetDefaultSetting();
                //如果串口1未开启，则开启串口1
                if (!serialPort1.IsOpen)
                {
                    serialPort1.Open();
                }
                Common.MachineRegisterInfoHelper machineRegisterInfoHelperClass = new Common.MachineRegisterInfoHelper();
                DataModel.Machine machineInfoEntity = machineRegisterInfoHelperClass.GetMachineRegisterInfo();
                if (machineInfoEntity != null)
                {
                    MC_Machine = machineInfoEntity;
                }
                else
                {
                    throw new Exception("请先注册机器");
                }
                //RFID配置端口默认配置
                // RFIDSerialPortGetDefaultSetting();
                //
                if(frmMain.head!=null)
                {
                    AddImage(frmMain.head.LocalFileName, frmMain.head.EmployeeName, frmMain.head.EmployeeCode,null);
                    tlp_manage.Controls.Add(imageTableLayoutPanel, 0, 1);
                }
                if (frmMain.charge != null)
                {
                    AddImage(frmMain.charge.LocalFileName,frmMain.charge.EmployeeName, frmMain.charge.EmployeeCode,null);
                    tlp_manage.Controls.Add(imageTableLayoutPanel, 1, 1);
                }
                //加载QC和员工
                List<DataModel.ClockInRecord> displayClockInRecords = clockInRecordHandler.GetClockInRecordList();
                //处理QC和员工
                classify(displayClockInRecords);
                showQCImage(displayQCClockInRecord);
                showEmployeeImage(displayEmployeeClockInRecords, page);
                //开始后台进程（更新时间）                
                DateTimeThreadFunction = new ThreadStart(DateTimeTimer);
                DateTimeThreadClass = new Thread(DateTimeThreadFunction);
                DateTimeThreadClass.Start();
            }
            catch (Exception ex)
            {
                Common.LogHandler.WriteLog("打卡界面初始化失败", ex);
                ShowErrorMessage(ex.Message, "打卡界面初始化");
            }
        }

        private void frmAttend_FormClosing(object sender, FormClosingEventArgs e)
        {
            //定时器
            if (DateTimeThreadClass != null && TTimerClass != null)
            {
                TTimerClass.StopTimmer();
                DateTimeThreadClass.Abort();
            }
            if (serialPort1.IsOpen)
            {
                closing = true;
                while (listening) Application.DoEvents();
                serialPort1.Close();
            }
            Thread.Sleep(50);
            
        }

        /// <summary>
        /// 显示时间定时器
        /// </summary>
        private void DateTimeTimer()
        {
            TTimerClass = new Common.TimmerHandler(1000, true, (o, a) =>
            {
                SetDateTime();
            }, true);

        }
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
                    this.Invoke(new SetDateTimeDelegate(SetDateTime));
                }
                else
                {
                    lab_DateTime.Text = string.Format(DateTime.Now.ToString("MM-dd HH:mm:ss"));
                    //2分钟自动关闭
                    if ((DateTime.Now - startTime) >= new TimeSpan(0, 2, 0))
                    {
                        //取消操作
                        MC_IsManualCancel = true;
                        MC_EmployeeInfo = null;
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Common.LogHandler.WriteLog("设置当前时间错误", ex);
            }  
        }
       
        ///<summary>
        ///处理QC和员工
        /// </summary>
        private void classify(List<DataModel.ClockInRecord> clockInRecords)
        {

           
            foreach(var item in clockInRecords)
            {
                DataModel.Employee employee = Common.EmployeeHelper.QueryEmployeeByEmployeeID(item.EmployeeID);
                if (employee != null)
                {
                    DataModel.JobPositon jobPositon = Common.JobPositionHelper.GetJobPositon(employee.JobPostionID);
                    if (jobPositon != null)
                    {
                        if (jobPositon.JobPositionCode == JobPositionCode.Employee.ToString())
                        {
                            DataModel.displayClockInRecord displayClockInRecord = new DataModel.displayClockInRecord();
                            displayClockInRecord._id = item._id;
                            displayClockInRecord.EmployeeCode = employee.EmployeeCode;
                            displayClockInRecord.EmployeeName = employee.EmployeeName;
                            displayClockInRecord.LocalFileName = employee.LocalFileName;
                            displayClockInRecord.StartDate = item.StartDate;
                            displayEmployeeClockInRecords.Add(displayClockInRecord);
                        }
                        else if (jobPositon.JobPositionCode == JobPositionCode.QC.ToString())
                        {
                            DataModel.displayClockInRecord displayClockInRecord = new DataModel.displayClockInRecord();
                            QCClockInRecord = item;
                            displayClockInRecord.EmployeeCode = employee.EmployeeCode;
                            displayClockInRecord.EmployeeName = employee.EmployeeName;
                            displayClockInRecord.LocalFileName = employee.LocalFileName;
                            displayClockInRecord.StartDate = item.StartDate;
                            displayQCClockInRecord=displayClockInRecord;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 加入图，姓名及工号
        /// </summary>
        private void AddImage(string imagePath, string name, string num,string time)
        {
            imageTableLayoutPanel = new TableLayoutPanel();
            //图片
            PictureBox pictureBox = new PictureBox();
            pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            pictureBox.Name = "pictureBox1";
            pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pictureBox.TabStop = false;
            if (System.IO.File.Exists(Application.StartupPath + "\\image\\" + imagePath))
            {
               
                pictureBox.Image = Image.FromFile(Application.StartupPath + "\\image\\" + imagePath);
              
            }
            else
            {
                //无照片
                pictureBox.Image = ((System.Drawing.Image)(resources.GetObject("noimage.Image")));
            }
            imageTableLayoutPanel.Controls.Add(pictureBox, 0, 0);
            //工号
            Label lab_num = new Label();
            lab_num.AutoSize = true;
            lab_num.Dock = System.Windows.Forms.DockStyle.Fill;
            lab_num.Text = "工号：" + num;
            lab_num.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lab_num.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            lab_num.ForeColor = System.Drawing.Color.White;
            //姓名
            Label lab_name = new Label();
            lab_name.AutoSize = true;
            lab_name.Dock = System.Windows.Forms.DockStyle.Fill;
            lab_name.Text = "姓名：" + name;
            lab_name.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lab_name.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            lab_name.ForeColor = System.Drawing.Color.White;
            //签到时间
            Label lab_time = new Label();
            lab_time.AutoSize = true;
            lab_time.Dock = System.Windows.Forms.DockStyle.Fill;
         
            lab_time.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lab_time.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            lab_time.ForeColor = System.Drawing.Color.White;



            imageTableLayoutPanel.ColumnCount = 1;
            imageTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            imageTableLayoutPanel.Controls.Add(lab_num, 0, 1);
            imageTableLayoutPanel.Controls.Add(lab_name, 0, 2);
            
            imageTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            imageTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            imageTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            imageTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            if (time != null)
            {
                lab_time.Text = "签到：" + time;
                lab_num.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                lab_name.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                imageTableLayoutPanel.Controls.Add(lab_time, 0, 3);
                imageTableLayoutPanel.RowCount = 4;
                imageTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            }
            else
            {
                lab_time.Text = "";
                imageTableLayoutPanel.RowCount = 3;
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
            if (closing) return;//如果正在关闭，忽略操作，直接返回，尽快的完成串口监听线程的一次循环
            try
            {
                listening = true;//设置标记，已经开始处理数据。
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
                byte[] tempbuf_2 = new byte[4];
                if (GetValidateValueLength(RevDataBuffer, 4))
                {
                    //淳元电脑，只有四位有效数据

                    byte[] tempbuf_1 = new byte[4];
                   

                    for (int i = 0; i < 4; i++)
                    {
                        tempbuf_1[i] = RevDataBuffer[i]; //获取卡号，16进制，卡号保存在数组的7-10字节,正向在数组中排序
                        tempbuf_2[3 - i] = RevDataBuffer[i]; //获取卡号，16进制，卡号保存在数组的7-10字节，反向向在数组中排序
                    }
                  


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
                            

                            for (int i = 0; i < 4; i++)
                            {
                                tempbuf_1[i] = RevDataBuffer[i + 7]; //获取卡号，16进制，卡号保存在数组的7-10字节,正向在数组中排序
                                tempbuf_2[3 - i] = RevDataBuffer[i + 7]; //获取卡号，16进制，卡号保存在数组的7-10字节，反向向在数组中排序
                            }

                        }
                    }
                }
                
                updateImage(tempbuf_2);

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
            finally
            {
                listening = false;//用完了，ui可以关闭串口
            }
        }

        ///<summary>
        ///QC根据记录显示照片
        /// </summary>
        private void showQCImage(DataModel.displayClockInRecord clockInRecord)
        {
            tlp_manage.Controls.Remove(tableLayoutPanel_QC);
            if (clockInRecord != null)
            {
                tableLayoutPanel_QC = new TableLayoutPanel();
                tableLayoutPanel_QC.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
                tableLayoutPanel_QC.Dock = System.Windows.Forms.DockStyle.Fill;
                int count_QC = 0;
                tableLayoutPanel_QC.RowCount = 1;
                tableLayoutPanel_QC.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
                AddImage(clockInRecord.LocalFileName, clockInRecord.EmployeeName, clockInRecord.EmployeeCode, clockInRecord.StartDate.ToLocalTime().ToString("MM-dd HH:mm"));
                tableLayoutPanel_QC.Controls.Add(imageTableLayoutPanel, 0, 0);
                tableLayoutPanel_QC.ColumnCount = count_QC;
                tlp_manage.Controls.Add(tableLayoutPanel_QC, 2, 1);
            }
        }
        /// <summary>
        /// 员工根据记录显示照片
        /// </summary>
        /// <param name="clockInRecords"></param>
        private void showEmployeeImage(List<DataModel.displayClockInRecord> clockInRecords, int nowpage)
        {
            tlp_employee.Controls.Remove(tableLayoutPanel);
            //员工
            tableLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            int count = 0;
            int nowpagecount = 0;
            tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel.RowCount = 1;
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            foreach (var item in clockInRecords)
            {
                tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
                AddImage(item.LocalFileName, item.EmployeeName, item.EmployeeCode, item.StartDate.ToLocalTime().ToString("MM-dd HH:mm"));
                count++;
                if (nowpage > 0 && count > (nowpage - 1) * pagecount && nowpagecount < pagecount)
                {
                    tableLayoutPanel.Controls.Add(imageTableLayoutPanel, nowpagecount, 0);
                    nowpagecount++;
                 }
            }
            if (count == 0)
            {
                this.label2.Text = "员工";
            }
            else if (nowpagecount != 0)
            {
                this.label2.Text = count + "位员工" + "<" + page + ">";
                tableLayoutPanel.ColumnCount = nowpagecount;
                tlp_employee.Controls.Add(tableLayoutPanel, 0, 1);
            }
            else if (nowpage > 0)
            {
                page = 1;
                showEmployeeImage(clockInRecords, page);
            }
            else
            {
                if (count % 5 == 0)
                {
                    page = count / 5;
                }
                else
                {
                    page = count / 5 + 1;
                }
                showEmployeeImage(clockInRecords, page);
            }
        }
       
        private void updateImage(byte[] tempbuf_2)
        {
            this.Invoke((EventHandler)(delegate
            {
                //IC卡检测（能否匹配员工信息及有效性）
                CheckCardID(GetCardID(tempbuf_2));
                if (MC_EmployeeInfo != null)
                {
                    //更新界面
                    //卡号处理
                    DataModel.JobPositon jobPositon = Common.JobPositionHelper.GetJobPositon(MC_EmployeeInfo.JobPostionID);
                    if (jobPositon != null)
                    {
                        
                        if(jobPositon.JobPositionCode== JobPositionCode.Employee.ToString())
                        {
                            //需要更新数据库
                            isUpdate = true;
                            //检测是否存在未结束记录
                            if (CheckIfExist(MC_EmployeeInfo._id))
                            {
                                this.lab_type.ForeColor = System.Drawing.Color.Yellow;
                                this.lab_type.Text = "签退";
                                foreach (var item in displayEmployeeClockInRecords.ToArray())
                                {
                                    if (item._id == clockInRecord._id)
                                    {
                                        displayEmployeeClockInRecords.Remove(item);
                                        break;
                                    }
                                }
                                //显示在打卡区
                                AddImage(MC_EmployeeInfo.LocalFileName, MC_EmployeeInfo.EmployeeName, MC_EmployeeInfo.EmployeeCode, null);
                            }
                            else
                            {
                                this.lab_type.ForeColor = System.Drawing.Color.LawnGreen;
                                this.lab_type.Text = "签到";
                                DataModel.displayClockInRecord displayClockInRecord = new DataModel.displayClockInRecord();
                                DataModel.ClockInRecord newClockInRecord = new DataModel.ClockInRecord();
                                newClockInRecord.EmployeeID = MC_EmployeeInfo._id;
                                newClockInRecord.MachineID = MC_Machine._id;
                                DateTime Now = DateTime.Now.ToLocalTime();
                                newClockInRecord.StartDate = Now;
                                newClockInRecord.EndDate = Now;
                                newClockInRecord.IsUploadToServer = false;
                                newClockInRecord.IsAuto = false;
                                displayClockInRecord.EmployeeCode = MC_EmployeeInfo.EmployeeCode;
                                displayClockInRecord.EmployeeName = MC_EmployeeInfo.EmployeeName;
                                displayClockInRecord.LocalFileName = MC_EmployeeInfo.LocalFileName;
                                displayClockInRecord.StartDate = newClockInRecord.StartDate;
                                //用于更新数据库
                                inClockInRecords.Add(newClockInRecord);
                                //用于显示
                                displayEmployeeClockInRecords.Add(displayClockInRecord);
                                //跳转到最后一页
                                if ((displayEmployeeClockInRecords.Count) % pagecount == 0)
                                {
                                    page = displayEmployeeClockInRecords.Count / pagecount;
                                }
                                else
                                {
                                    page = displayEmployeeClockInRecords.Count  / pagecount + 1;
                                }
                                //显示在打卡区
                                AddImage(MC_EmployeeInfo.LocalFileName, MC_EmployeeInfo.EmployeeName, MC_EmployeeInfo.EmployeeCode, DateTime.Now.ToString("MM-dd HH:mm"));
                            }
                            tableLayoutPanel1.Controls.Remove(lab_wait);
                            //显示在打卡区
                            tableLayoutPanel1.Controls.Add(imageTableLayoutPanel, 0, 2);
                            showEmployeeImage(displayEmployeeClockInRecords, page);
                        }
                        else if(jobPositon.JobPositionCode == JobPositionCode.QC.ToString())
                        {
                            isUpdate = true;
                            //检测是否存在未结束记录
                            if (CheckIfExist(MC_EmployeeInfo._id))
                            {
                                this.lab_type.ForeColor = System.Drawing.Color.Yellow;
                                this.lab_type.Text = "签退";
                                displayQCClockInRecord = null;
                                AddImage(MC_EmployeeInfo.LocalFileName, MC_EmployeeInfo.EmployeeName, MC_EmployeeInfo.EmployeeCode, null);
                            }
                            else
                            {
                                this.lab_type.ForeColor = System.Drawing.Color.LawnGreen;
                                this.lab_type.Text = "签到";
                                DataModel.displayClockInRecord displayClockInRecord = new DataModel.displayClockInRecord();
                                DataModel.ClockInRecord newClockInRecord = new DataModel.ClockInRecord();
                                newClockInRecord.EmployeeID = MC_EmployeeInfo._id;
                                newClockInRecord.MachineID = MC_Machine._id;
                                DateTime Now = DateTime.Now.ToLocalTime();
                                newClockInRecord.StartDate = Now;
                                newClockInRecord.EndDate = Now;
                                newClockInRecord.IsUploadToServer = false;
                                newClockInRecord.IsAuto = false;
                                displayClockInRecord.EmployeeCode = MC_EmployeeInfo.EmployeeCode;
                                displayClockInRecord.EmployeeName = MC_EmployeeInfo.EmployeeName;
                                displayClockInRecord.LocalFileName = MC_EmployeeInfo.LocalFileName;
                                displayClockInRecord.StartDate = newClockInRecord.StartDate;

                                //用于更新数据库
                                clockInRecord = QCClockInRecord;
                                if (clockInRecord != null)
                                {
                                    clockInRecord.EndDate = Now;
                                }
                                inClockInRecords.Add(newClockInRecord);
                                //用于显示
                                displayQCClockInRecord = displayClockInRecord;
                                //显示在打卡区
                                AddImage(MC_EmployeeInfo.LocalFileName, MC_EmployeeInfo.EmployeeName, MC_EmployeeInfo.EmployeeCode, DateTime.Now.ToString("MM-dd HH:mm"));
                            }
                            tableLayoutPanel1.Controls.Remove(lab_wait);
                            //显示在打卡区
                            tableLayoutPanel1.Controls.Add(imageTableLayoutPanel, 0, 2);
                            showQCImage(displayQCClockInRecord);
                        }
                        else
                        {
                            this.lab_type.Text = "职位不符:"+jobPositon.JobPositionCode;
                            this.lab_type.Font = new System.Drawing.Font("宋体",16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                            this.lab_type.ForeColor = System.Drawing.Color.Red;
                            AddImage(MC_EmployeeInfo.LocalFileName, MC_EmployeeInfo.EmployeeName, MC_EmployeeInfo.EmployeeCode,null);
                            tableLayoutPanel1.Controls.Remove(lab_wait);
                            tableLayoutPanel1.Controls.Add(imageTableLayoutPanel, 0, 2);
                        }
                    }
                    else
                    {
                        this.lab_type.Text = "不存在该职位";
                        this.lab_type.ForeColor = System.Drawing.Color.Red;
                        AddImage(MC_EmployeeInfo.LocalFileName, MC_EmployeeInfo.EmployeeName, MC_EmployeeInfo.EmployeeCode,null);
                        tableLayoutPanel1.Controls.Remove(lab_wait);
                        tableLayoutPanel1.Controls.Add(imageTableLayoutPanel, 0, 2);
                    }
                }
            }
             )
            );
        }
        ///<summary>
        ///检测是否存在未结束记录
        ///</summary>
        private bool CheckIfExist(string employeeID)
        {
            try
            {
                clockInRecord = clockInRecordHandler.QueryClockInRecordByEmployeeID(employeeID);
                if (clockInRecord != null)
                {
                    clockInRecord.EndDate = DateTime.Now;
                    return true;
                }
                else
                {
                    return false;
                }
            }
             catch (Exception ex)
            {
                throw ex;
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

            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message, "权限认证");
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
            this.Close();
        }

        private void btn_Confirm_Click(object sender, EventArgs e)
        {
            if (isUpdate)
            {
                if (clockInRecord!=null)
                {
                    clockInRecordHandler.UpdateClockInRecord(clockInRecord,false);
                }
                foreach (var item in inClockInRecords)
                {
                    clockInRecordHandler.SaveClockInRecord(item);
                }
            }
            else
            {
                MC_IsManualCancel = true;
                MC_EmployeeInfo = null;
            }
            this.Close();

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

        private void button1_Click(object sender, EventArgs e)
        {
            
            page = page - 1;
            showEmployeeImage(displayEmployeeClockInRecords, page);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            page = page + 1;
            showEmployeeImage(displayEmployeeClockInRecords, page);
        }
    }
}
