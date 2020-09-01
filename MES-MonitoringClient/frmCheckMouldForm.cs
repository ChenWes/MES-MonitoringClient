using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;


namespace MES_MonitoringClient
{
    public partial class frmCheckMouldForm : Form
    {
        public DataModel.CheckMouldRecord CheckMouldRecord = null;
        public DataModel.Employee Employee = null;

        
        

        public frmCheckMouldForm()
        {
            InitializeComponent();
        }
        private void frmCheckMouldForm_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            if (Employee != null)
            {
                this.txt_EmployeeID.Text = Employee.EmployeeCode;
                this.txt_EmployeeName.Text = Employee.EmployeeName;
            }
            

        }


        private void ShowErrorMessage(string errorMessage, string errorTitle)
        {
            MessageBox.Show(errorMessage, errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

      

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            

            this.Close();
        }

        private void btn_Confirm_Click(object sender, EventArgs e)
        {
            try
            {
            
                if (string.IsNullOrEmpty(txt_MouldCode.Text.Trim())) 
                {
                    throw new Exception("请输入模具编号");
                }
                if (this.txt_ProductCode.Text.Trim() == "")
                {
                    ShowErrorMessage("请输入产品编号", "保存失败");
                    return;
                }
                if (this.txt_PlanCount.Text.Trim() == "")
                {
                    ShowErrorMessage("请输入预计啤数", "保存失败");
                    return;
                }
                int num=0;
                int.TryParse(this.txt_PlanCount.Text.Trim(), out num);
                if (num<=0)
                {
                    ShowErrorMessage("预计啤数请输入整数", "保存失败");
                    return;
                }
                //保存试模记录
                CheckMouldRecord = new DataModel.CheckMouldRecord();
                CheckMouldRecord.MouldCode = this.txt_MouldCode.Text.Trim();
                CheckMouldRecord.ProductCode = this.txt_ProductCode.Text.Trim();
                CheckMouldRecord.PlanCount = num;
                CheckMouldRecord.EmployeeID = this.txt_EmployeeID.Text.Trim();
                CheckMouldRecord.EmployeeName = this.txt_EmployeeName.Text.Trim();
                this.Close();

            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message, "录入资料出错");
            }

        }

      
      
    }
}
