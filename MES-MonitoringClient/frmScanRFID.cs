﻿using System;
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
        //修改者信息
        public string OperatePersonCardID = string.Empty;
        public string OperatePersonName = string.Empty;
        /*---------------------------------------------------------------------------------------*/
        private long COM1_ReceiveDataCount = 0;
        private StringBuilder COM1_DataStringBuilder = new StringBuilder();
        /*---------------------------------------------------------------------------------------*/
        public delegate void AddDataDelegate(String myString);
        public AddDataDelegate myDelegate;


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
                this.WindowState = FormWindowState.Maximized;

                //RFID配置端口默认配置
                RFIDSerialPortGetDefaultSetting();

                if (!serialPort1.IsOpen)
                {
                    serialPort1.Open();
                }                
            }
            catch (Exception ex)
            {
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
                System.IO.Ports.SerialPort COM = (System.IO.Ports.SerialPort)sender;

                int num = COM.BytesToRead;      //获取接收缓冲区中的字节数
                byte[] received_buf = new byte[num];    //声明一个大小为num的字节数据用于存放读出的byte型数据

                COM1_ReceiveDataCount += num;                   //接收字节计数变量增加nun
                COM.Read(received_buf, 0, num);   //读取接收缓冲区中num个字节到byte数组中

                COM1_DataStringBuilder.Clear();


                foreach (byte b in received_buf)
                {
                    COM1_DataStringBuilder.Append(b.ToString("X2") + " ");
                }                                

                //更新界面
                this.Invoke((EventHandler)(delegate
                {
                    lab_ScanStatus.Text = "刷卡成功";
                    lab_CardID.Text = "卡号:" + COM1_DataStringBuilder.ToString();
                    OperatePersonCardID = COM1_DataStringBuilder.ToString();

                    richTextBox1.AppendText(COM1_DataStringBuilder.ToString());
                }
                    )
                );

                #region 老代码
                //string getData = "";
                //do
                //{
                //    int count = COM.BytesToRead;
                //    if (count <= 0)
                //        break;
                //    byte[] readBuffer = new byte[count];

                //    Application.DoEvents();
                //    COM.Read(readBuffer, 0, count);
                //    getData += System.Text.Encoding.Default.GetString(readBuffer);

                //    );

                //} while (COM.BytesToRead > 0);

                //    //因为要访问UI资源，所以需要使用invoke方式同步ui
                //    this.Invoke((EventHandler)(delegate
                //    {
                //        receviedDataCount += 1;
                //        //lab_ReceviedDataCount.Text = "接收成功：" + receviedDataCount;

                //        //richTextBox2.AppendText(getData + "\r");
                //    }
                //       )
                #endregion
            }
            catch (Exception ex)
            {
                ShowErrorMessage( "RFID串口获取数据时出错","serialPort1_DataReceived");
                //响铃并显示异常给用户
                System.Media.SystemSounds.Beep.Play();
            }
        }

        private void serialPort1_ErrorReceived(object sender, System.IO.Ports.SerialErrorReceivedEventArgs e)
        {
            ShowErrorMessage("RFID串口获取数据出错", "serialPort1_ErrorReceived");
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            OperatePersonName = string.Empty;
            OperatePersonCardID = string.Empty;

            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btn_Confirm_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
