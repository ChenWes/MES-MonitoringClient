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
    public partial class frmProtect : Form
    {


        public byte[] RevDataBuffer = new byte[30];
        public UInt32 RevDataBufferCount;


     

        //刷卡员工信息
        public DataModel.Employee MC_EmployeeInfo = null;



        /*---------------------------------------------------------------------------------------*/
        //private long COM1_ReceiveDataCount = 0;
        //private StringBuilder COM1_DataStringBuilder = new StringBuilder();
        /*---------------------------------------------------------------------------------------*/
        //public delegate void AddDataDelegate(String myString);
        //public AddDataDelegate myDelegate;


        /*主窗口方法*/
        /*---------------------------------------------------------------------------------------*/

        public frmProtect()
        {
            InitializeComponent();
        }

        private void frmProtect_Load(object sender, EventArgs e)
        {
            try
            {                
                //窗口最大化
                this.WindowState = FormWindowState.Maximized;

               
                this.btn_Confirm.Visible = false;
                this.btn_Cancel.Visible = false;

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

        private void frmProtect_FormClosing(object sender, FormClosingEventArgs e)
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
                int revbuflen;

                byte pkttype;
                byte pktlength = 0x0;
                byte cmd;
                byte err;

                bool revflag;
                bool status;

                //是否接收数据开关


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
                /*else
                {
                    RevDataBuffer = new byte[] { };
                    //如果串口出来的数据为空，则不处理
                }*/

                RevDataBufferCount = 0;

                while (revflag) //判读缓冲区是否有数据
                {
                    byte a = (byte)SerialPort.ReadByte();
                    RevDataBuffer[RevDataBufferCount] = a; //读出串口缓冲区数据到数组中
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
                        //lab_ScanStatus.Text = "刷卡成功";
                        //lab_CardID.Text = "卡号:" + GetCardID(tempbuf_2);

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
                                //lab_ScanStatus.Text = "刷卡成功";
                                //lab_CardID.Text = "卡号:" + GetCardID(tempbuf_2);

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
                 if (employee == null)
                 {
                    //lab_CardID.Text = "...";
                    //lab_ScanStatus.Text = "系统维护中";
                    return;
                 }
                else if (employee.JobPostionResponsibility != null && employee.JobPostionResponsibility.Trim() == "系统管理员")
                {
                    MC_EmployeeInfo = employee;
                    this.btn_Confirm.Visible = true;
                    this.btn_Cancel.Visible = true;
                }
                else
                {
                    MC_EmployeeInfo = null;
                    this.btn_Confirm.Visible = false;
                    this.btn_Cancel.Visible = false;
                }


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
            lab_CardID.Text = "...";
            lab_ScanStatus.Text = "系统维护中";
            MC_EmployeeInfo = null;
            this.btn_Confirm.Visible = false;
            this.btn_Cancel.Visible = false;
            
        }

        private void btn_Confirm_Click(object sender, EventArgs e)
        {
            try
            {
                if (MC_EmployeeInfo == null)
                {
                    throw new Exception("未刷卡");
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

       
    }
}
