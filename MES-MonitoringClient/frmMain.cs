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

        /*---------------------------------------------------------------------------------------*/

        //向串口7发送的默认信号
        static string mc_DefaultSignal = "AA1086";

        //必须的串口端口
        static string[] mc_DefaultRequiredSerialPortName = new string[] { "COM1", "COM7" };

        //三个后台线程
        static Common.ThreadHandler DateTimeThreadHandler = null;
        static Common.ThreadHandler SerialPort1ThreadHandler = null;
        static Common.ThreadHandler SerialPort7ThreadHandler = null;

        static Common.MachineStatusHandler mc_MachineStatusHander = null;

        /*---------------------------------------------------------------------------------------*/
        //后台线程变量
        Thread timerThread = null;

        //定时器变量
        System.Timers.Timer TTimer;


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
                this.WindowState = FormWindowState.Maximized;

                var sw = Stopwatch.StartNew();
                //检测端口


                //打开端口
                serialPort7.Open();                

                //开始后台进程
                DateTimeThreadHandler = new Common.ThreadHandler(new ThreadStart(DateTimeTimer), true, true);
                //SerialPort1ThreadHandler = new Common.ThreadHandler(new ThreadStart(GetRFIDDataTimer), true, true);

                SerialPort7ThreadHandler = new Common.ThreadHandler(new ThreadStart(SendDataToSerialPortTimer), true, true);

                //label2.Text = DateTimeThreadHandler.ThreadState();

                //MessageBox.Show("初始化共使用" + sw.ElapsedMilliseconds.ToString() + "毫秒");

                //状态
                mc_MachineStatusHander = new Common.MachineStatusHandler();
                mc_MachineStatusHander.ChangeStatus("Online", "运行", "WesChen", "001A");

                sw.Stop();
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
            if (DateTimeThreadHandler != null)
            {
                DateTimeThreadHandler.ThreadJoin();
            }
            //SerialPort1ThreadHandler.ThreadJoin();
            if (SerialPort1ThreadHandler != null)
            {
                SerialPort7ThreadHandler.ThreadJoin();
            }

            if (serialPort7.IsOpen)
            {
                serialPort7.Close();
            }
        }

        /*定时器方法*/
        /*---------------------------------------------------------------------------------------*/

        /// <summary>
        /// 显示时间定时器
        /// </summary>
        private void DateTimeTimer()
        {            
            Common.TimmerHandler TTimerClass = new Common.TimmerHandler(1000, true, (o, a) => {
                SetDateTime();
            }, true);
        }

        /// <summary>
        /// 发送AA1086至串口7定时器
        /// </summary>
        private void SendDataToSerialPortTimer()
        {
            Common.TimmerHandler TTimerClass = new Common.TimmerHandler(1000, true, (o, a) => {
                SendDataToSerialPort(mc_DefaultSignal);
            }, true);
        }

        /*定时器委托*/
        /*---------------------------------------------------------------------------------------*/

        /// <summary>
        /// 声明显示当前时间委托
        /// </summary>
        private delegate void SetDateTimeDelegate();
        private void SetDateTime()
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
                    label7.Text = "当前状态:["+ mc_MachineStatusHander.StatusDescription + "][" + Common.CommonFunction.FormatMilliseconds(mc_MachineStatusHander.HoldStatusTotalMilliseconds) + "]";
                }
                catch (Exception ex)
                {
                    //响铃并显示异常给用户
                    System.Media.SystemSounds.Beep.Play();
                }
            }
        }

        /// <summary>
        /// 声明发送数据至串口委托
        /// </summary>
        /// <param name="defaultSignal"></param>
        private delegate void SendDataToSerialPortDelegate(string defaultSignal);
        private void SendDataToSerialPort(string defaultSignal)
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
                    if (serialPort7.IsOpen)
                    {
                        //转码后再发送
                        //byte[] byteArray = System.Text.Encoding.Default.GetBytes(defaultSignal);
                        //serialPort7.Write(byteArray, 0, byteArray.Length);

                        //不转码直接发送
                        serialPort7.Write(defaultSignal);

                        //发送的大小
                        COM7_SendDataCount += 1;
                        lab_SendSuccessCount.Text = "发送成功：" + COM7_SendDataCount;
                    }
                }
                catch (Exception ex)
                {
                    COM7_SendDataErrorCount += 1;
                    lab_SendErrorCount.Text = "发送失败：" + COM7_SendDataErrorCount;

                    //响铃并显示异常给用户
                    System.Media.SystemSounds.Beep.Play();
                }
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

        /*获取串口数据事件*/
        /*---------------------------------------------------------------------------------------*/

        /// <summary>
        /// 串口7获取数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serialPort7_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
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
                    richTextBox1.AppendText(stringBuilder.ToString() + "\r");

                    COM7_ReceiveDataCount += 1;
                    lab_ReceviedDataCount.Text = "接收成功：" + COM7_ReceiveDataCount;


                    lab_ProductCount.Text = "累计生产数量：" + mc_MachineStatusHander.mc_MachineProduceStatusHandler.LifeCycleCount;
                    lab_LastLifeCycleTime.Text = "最后一次生产用时：" + Common.CommonFunction.FormatMilliseconds(mc_MachineStatusHander.mc_MachineProduceStatusHandler.LastLifeCycleSecond);
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


        private void button1_Click(object sender, EventArgs e)
        {
            mc_MachineStatusHander = new Common.MachineStatusHandler();

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

        private void button2_Click(object sender, EventArgs e)
        {
            mc_MachineStatusHander.ChangeStatus("Error", "故障", "Wes", "A02");
        }

        private void btn_CloseWindow_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_ChangeStatus_Click(object sender, EventArgs e)
        {
            frmChangeStatus newfrmChangeStatus = new frmChangeStatus();
            newfrmChangeStatus.ShowDialog();

            string newOperatePersonCardID = newfrmChangeStatus.OperatePersonCardID;
            string newOperatePersonName = newfrmChangeStatus.OperatePersonName;

            string strNewStatusCode = newfrmChangeStatus.NewStatusCode;
            string strNewStatusString = newfrmChangeStatus.NewStatusString;

            if (!string.IsNullOrEmpty(strNewStatusString) && strNewStatusString != mc_MachineStatusHander.StatusDescription)
            {
                mc_MachineStatusHander.ChangeStatus(strNewStatusCode, strNewStatusString, newOperatePersonName, newOperatePersonCardID);
            }
        }
    }
}
