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
        public DataModel.Machine machine = null;
        public string MouldCode = "";




        public frmCheckMouldForm()
        {
            InitializeComponent();
        }
        private void frmCheckMouldForm_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.txt_MouldCode.Text = MouldCode;
            if (Employee != null)
            {
                this.txt_Employee.Text = Employee.EmployeeName + "(" + Employee.EmployeeCode + ")";

            }
            if (machine != null)
            {
                this.txt_Machine.Text = machine.MachineCode + "(" + machine.Tonnage + ")";
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
                int num = 0;
                int.TryParse(this.txt_PlanCount.Text.Trim(), out num);
                if (num <= 0)
                {
                    ShowErrorMessage("预计啤数请输入整数", "保存失败");
                    return;
                }
                if (this.txt_MouldOutput.Text.Trim() == "")
                {
                    ShowErrorMessage("请输入型腔穴数", "保存失败");
                    return;
                }
                if (this.txt_PlanCycle.Text.Trim() == "")
                {
                    ShowErrorMessage("请输入报价周期", "保存失败");
                    return;
                }
                decimal PlanCycle = 0;
                decimal.TryParse(this.txt_PlanCycle.Text.Trim(), out PlanCycle);
                if (PlanCycle <= 0)
                {
                    ShowErrorMessage("请输入正确的报价周期", "保存失败");
                    return;
                }
                if (this.txt_Version.Text.Trim() == "")
                {
                    ShowErrorMessage("请输入版本", "保存失败");
                    return;
                }
                //保存首产记录
                CheckMouldRecord = new DataModel.CheckMouldRecord();
                CheckMouldRecord.MouldCode = this.txt_MouldCode.Text.Trim();
                CheckMouldRecord.ProductCode = this.txt_ProductCode.Text.Trim();
                CheckMouldRecord.PlanCount = num;

                if (Employee != null)
                {
                    CheckMouldRecord.EmployeeName = Employee.EmployeeName;
                    CheckMouldRecord.EmployeeID = Employee.EmployeeCode;
                }
                if (machine != null)
                {
                    CheckMouldRecord.MachineCode = machine.MachineCode;
                    CheckMouldRecord.MachineTonnage = machine.Tonnage;
                }
                CheckMouldRecord.MouldOutput = this.txt_MouldOutput.Text.Trim();
                CheckMouldRecord.PlanCycle = PlanCycle;
                CheckMouldRecord.Version = this.txt_Version.Text.Trim();

                this.Close();

            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message, "录入资料出错");
            }

        }

        private void txt_PlanCycle_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;
            if (!Char.IsDigit(ch) && ch != 8 && ch != 46)
            {
                e.Handled = true;
            }
        }

        private void txt_PlanCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;
            if (!Char.IsDigit(ch) && ch != 8)
            {
                e.Handled = true;
            }
        }
    }
}
