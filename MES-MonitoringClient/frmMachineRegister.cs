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
    public partial class frmMachineRegister : Form
    {

        //修改的状态
        public DataModel.MachineInfo mc_MachineInfo;

        public frmMachineRegister()
        {
            InitializeComponent();
        }
        private void frmMachineRegister_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
        }


        private void ShowErrorMessage(string errorMessage, string errorTitle)
        {
            MessageBox.Show(errorMessage, errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /**-------------------------------------------------------------*/
        private void btn_Query_Click(object sender, EventArgs e)
        {
            try
            {
                //查询方法，调用HTTP，注册至DB
                Common.HttpHelper httpHelperClass = new Common.HttpHelper();
                string l_machineRegisterUrlPath = Common.ConfigFileHandler.GetAppConfig("MachineRegisterUrlPath");

                //拿到返回的JSON数据（无论是正常的还是不正常的）
                string getJsonString = httpHelperClass.HttpPost(l_machineRegisterUrlPath, txt_MachineID.Text.Trim());

                if (!string.IsNullOrEmpty(getJsonString))
                {
                    //转换成Class，显示至界面上
                    mc_MachineInfo = new DataModel.MachineInfo();

                    mc_MachineInfo.MachineID = Common.JsonHelper.GetJsonValue(getJsonString, "_id");
                    mc_MachineInfo.MachineCode = Common.JsonHelper.GetJsonValue(getJsonString, "MachineCode");
                    mc_MachineInfo.MachineName = Common.JsonHelper.GetJsonValue(getJsonString, "MachineName");
                    mc_MachineInfo.MachineDesc = Common.JsonHelper.GetJsonValue(getJsonString, "MachineDesc");


                    txt_MachineCode.Text = mc_MachineInfo.MachineCode;
                    txt_MachineName.Text = mc_MachineInfo.MachineName;
                    txt_MachineDesc.Text = mc_MachineInfo.MachineDesc;                    
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message, "查询注册码出错");
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_Confirm_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}
