using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

using Newtonsoft.Json;

namespace MES_MonitoringClient_ManualTest
{
    public partial class Form1 : Form
    {
        private bool SendTestSignal_Flag = false;
        private int OperIndex = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void btn_ScanCard_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort2.Open();

                if (serialPort2.IsOpen)
                {
                    //转换成16进制
                    byte[] byteArray = strToToHexByte(txt_CardID.Text.Trim());

                    //直接发送byte[]
                    serialPort2.Write(byteArrayReverse(byteArray), 0, byteArray.Length);
                }

                serialPort2.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void btn_Operation_Click(object sender, EventArgs e)
        {
            try
            {
                Regex reg = new Regex(@"^[A-H]+$");
                Match m = reg.Match(txt_LoopOrder.Text.Trim());
                if (!m.Success)
                {
                    MessageBox.Show("只允许输入[]字符");
                    return;
                }

                //开始及结束                 
                if (!SendTestSignal_Flag) { if (!serialPort4.IsOpen) serialPort4.Open(); }


                int AllLength = txt_LoopOrder.Text.Trim().Length - 1;
                string OperSignal = txt_LoopOrder.Text.Trim().Substring(OperIndex, 1);

                txt_log.Multiline = true;
                txt_log.ScrollBars = RichTextBoxScrollBars.Vertical;
                txt_log.SelectionColor = System.Drawing.Color.Green;


                switch (OperSignal)
                {
                    case "A":
                        SendTestSignal(txt_ASignal.Text.Trim(), int.Parse(txt_ASecond.Text.Trim()));
                        txt_log.AppendText(txt_ASignal.Text.Trim() + "\r");
                        break;
                    case "B":
                        SendTestSignal(txt_BSignal.Text.Trim(), int.Parse(txt_BSecond.Text.Trim()));
                        txt_log.AppendText(txt_BSignal.Text.Trim() + "\r");
                        break;
                    case "C":
                        SendTestSignal(txt_CSignal.Text.Trim(), int.Parse(txt_CSecond.Text.Trim()));
                        txt_log.AppendText(txt_CSignal.Text.Trim() + "\r");
                        break;
                    case "D":
                        SendTestSignal(txt_DSignal.Text.Trim(), int.Parse(txt_DSecond.Text.Trim()));
                        txt_log.AppendText(txt_DSignal.Text.Trim() + "\r");
                        break;
                    case "E":
                        SendTestSignal(txt_ESignal.Text.Trim(), int.Parse(txt_ESecond.Text.Trim()));
                        txt_log.AppendText(txt_ESignal.Text.Trim() + "\r");
                        break;
                    case "F":
                        SendTestSignal(txt_FSignal.Text.Trim(), int.Parse(txt_FSecond.Text.Trim()));
                        txt_log.AppendText(txt_FSignal.Text.Trim() + "\r");
                        break;
                    case "G":
                        SendTestSignal(txt_GSignal.Text.Trim(), int.Parse(txt_GSecond.Text.Trim()));
                        txt_log.AppendText(txt_GSignal.Text.Trim() + "\r");
                        break;
                    default:
                        break;
                }

                OperIndex++;
                if (OperIndex == AllLength) OperIndex = 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //private void SendTestSignal(string SignalLoopOrder)
        //{
        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        private void SendTestSignal(string SendSignal, int SleepSecond)
        {
            try
            {
                serialPort4.WriteLine("AA" + SendSignal + "ZZ");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort2.IsOpen) serialPort2.Close();

            if (serialPort4.IsOpen) serialPort4.Close();
        }


        //--------------------------

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

        private static byte[] byteArrayReverse(byte[] bytea)
        {
            byte[] newArray = new byte[bytea.Length];

            for (int i = 0; i < bytea.Length; i++)
            {
                newArray[bytea.Length - i - 1] = bytea[i];
            }

            return newArray;
        }

        private void button1_Click(object sender, EventArgs e)
        {
             
            double interval = 1;
            double.TryParse(this.textBox2.Text.Trim(), out interval);
            this.timer1.Interval = (int)(interval*1000);
            if (this.timer1.Enabled)
            {
                this.timer1.Enabled = false;
                this.button1.Text = "开始";
            }
            else
            {
                this.timer1.Enabled = true;
                this.button1.Text = "停止";
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.timer1.Stop();
            try
            {
                Regex reg = new Regex(@"^[A-H]+$");
                Match m = reg.Match(this.textBox1.Text.Trim());
                if (!m.Success)
                {
                    MessageBox.Show("只允许输入[]字符");
                    return;
                }

                //开始及结束                 
                if (!SendTestSignal_Flag) { if (!serialPort4.IsOpen) serialPort4.Open(); }


                int AllLength = this.textBox1.Text.Trim().Length - 1;
                string OperSignal = this.textBox1.Text.Trim().Substring(OperIndex, 1);

                txt_log.Multiline = true;
                txt_log.ScrollBars = RichTextBoxScrollBars.Vertical;
                txt_log.SelectionColor = System.Drawing.Color.Green;


                switch (OperSignal)
                {
                    case "A":
                        SendTestSignal(txt_ASignal.Text.Trim(), int.Parse(txt_ASecond.Text.Trim()));
                        txt_log.AppendText(txt_ASignal.Text.Trim() + "\r");
                        break;
                    case "B":
                        SendTestSignal(txt_BSignal.Text.Trim(), int.Parse(txt_BSecond.Text.Trim()));
                        txt_log.AppendText(txt_BSignal.Text.Trim() + "\r");
                        break;
                    case "C":
                        SendTestSignal(txt_CSignal.Text.Trim(), int.Parse(txt_CSecond.Text.Trim()));
                        txt_log.AppendText(txt_CSignal.Text.Trim() + "\r");
                        break;
                    case "D":
                        SendTestSignal(txt_DSignal.Text.Trim(), int.Parse(txt_DSecond.Text.Trim()));
                        txt_log.AppendText(txt_DSignal.Text.Trim() + "\r");
                        break;
                    case "E":
                        SendTestSignal(txt_ESignal.Text.Trim(), int.Parse(txt_ESecond.Text.Trim()));
                        txt_log.AppendText(txt_ESignal.Text.Trim() + "\r");
                        break;
                    case "F":
                        SendTestSignal(txt_FSignal.Text.Trim(), int.Parse(txt_FSecond.Text.Trim()));
                        txt_log.AppendText(txt_FSignal.Text.Trim() + "\r");
                        break;
                    case "G":
                        SendTestSignal(txt_GSignal.Text.Trim(), int.Parse(txt_GSecond.Text.Trim()));
                        txt_log.AppendText(txt_GSignal.Text.Trim() + "\r");
                        break;
                    default:
                        break;
                }

                OperIndex++;
                if (OperIndex == AllLength) OperIndex = 0;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                this.timer1.Start();
            }
        
        }
    }


}
