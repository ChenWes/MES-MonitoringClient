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
    public partial class frmChangeStatus : Form
    {
        //修改的状态
        public string NewStatusCode = string.Empty;
        public string NewStatusString = string.Empty;
        //修改者信息
        public string OperatePersonCardID = string.Empty;
        public string OperatePersonName = string.Empty;

        public frmChangeStatus()
        {
            InitializeComponent();
        }

        private void frmChangeStatus_Load(object sender, EventArgs e)
        {
            try
            {
                this.WindowState = FormWindowState.Maximized;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        private void changeStatus(string pi_newStatusCode,string pi_newStatusString)
        {
            //getRFIDData();
            OperatePersonCardID = string.Empty;
            OperatePersonName = string.Empty;
            lab_OperatePersonCardID.Text = "未刷卡";

            frmScanRFID newfrmScanRFID = new frmScanRFID();
            newfrmScanRFID.ShowDialog();

            //获取弹出窗口的参数
            OperatePersonCardID = newfrmScanRFID.OperatePersonCardID;
            OperatePersonName = newfrmScanRFID.OperatePersonName;

            //状态
            NewStatusCode = pi_newStatusCode;
            NewStatusString = pi_newStatusString;

            //显示参数
            lab_OperatePersonCardID.Text = OperatePersonCardID;
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

        private void btn_Confirm_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(NewStatusCode) && !string.IsNullOrEmpty(NewStatusString))
            {
                if (string.IsNullOrEmpty(OperatePersonCardID))
                {
                    ShowErrorMessage("请刷卡", "更改机器状态");
                    return;
                }
            }
            else
            {
                ShowErrorMessage("请选择机器状态", "更改机器状态");
                return;
            }

            this.Close();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            NewStatusCode = string.Empty;
            NewStatusString = string.Empty;

            OperatePersonCardID = string.Empty;
            OperatePersonName = string.Empty;

            this.Close();
        }

        private void btn_StartProduct_Click(object sender, EventArgs e)
        {
            //getRFIDData();
            OperatePersonCardID = string.Empty;
            OperatePersonName = string.Empty;
            lab_OperatePersonCardID.Text = "未刷卡";

            frmScanRFID newfrmScanRFID = new frmScanRFID();
            newfrmScanRFID.ShowDialog();

            //获取弹出窗口的参数
            OperatePersonCardID = newfrmScanRFID.OperatePersonCardID;
            OperatePersonName = newfrmScanRFID.OperatePersonName;

            //状态
            NewStatusCode = "StartProduce";
            NewStatusString = "运行";

            //显示参数
            lab_OperatePersonCardID.Text = OperatePersonCardID;

            changeStatus("StartProduce", "运行");
        }

        private void btn_Stop_Click(object sender, EventArgs e)
        {
            changeStatus("StopProduce", "停机");
        }

        private void btn_Error_Click(object sender, EventArgs e)
        {
            changeStatus("MachineError", "故障");
        }
    }
}
