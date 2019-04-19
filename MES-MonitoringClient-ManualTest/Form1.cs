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
                    //不转码直接发送
                    serialPort2.Write(txt_CardID.Text.Trim());
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
                        txt_log.AppendText(txt_ASignal.Text.Trim()+"\r");
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

        private void SendTestSignal(string SendSignal,int SleepSecond)
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
    }


}
