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
    public partial class frmQC : Form
    {
        List<TextBox> textBoxes = new List<TextBox>();
        List<Button> addButtons1 = new List<Button>();
        List<Button> addButtons10 = new List<Button>();
        List<Button> subButtons1 = new List<Button>();
        List<Button> subButtons10 = new List<Button>();
        List<Label> labelCodeList = new List<Label>();
        public DataModel.JobOrder currentJobOrder = null;
        public List<DataModel.QCCheckCount> QCCheckCounts = new List<DataModel.QCCheckCount>();
        public int errorCount = 0;
        //全部
        List<DataModel.DefectiveType> DefectiveType = null;
        //筛选
        List<DataModel.DefectiveType> defectiveTypes = null;
        DataGridViewTextBoxEditingControl CellEdit = null;
        public DataModel.Employee employee = null;
        /*主窗口方法*/
        /*---------------------------------------------------------------------------------------*/

        public frmQC()
        {
            InitializeComponent();
        }

 
        private void btn_Confirm_Click(object sender, EventArgs e)
        {
            //总数
            //  int.TryParse(this.lab_ErrorCount.Text, out errorCount);
            //保存大于0的项
            for (int i = 0; i < DefectiveType.Count; i++)
            {
                if (DefectiveType[i].count>0)
                {
                    DataModel.QCCheckCount QCCheckCount = new DataModel.QCCheckCount();
                    QCCheckCount.DefectiveTypeID = DefectiveType[i]._id;
                    QCCheckCount.Count = DefectiveType[i].count;
                    QCCheckCounts.Add(QCCheckCount);
                }
            }
            this.Close();
           
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmQC_Load(object sender, EventArgs e)
        {
            SettingJobOrderView();
            this.lab_JobOrder.Text = currentJobOrder.JobOrderID;
            var machineProcessLog = currentJobOrder.MachineProcessLog.Find(t => t.ProduceStartDate == t.ProduceEndDate);
            //良品  
            if (machineProcessLog != null)
            {
                this.lab_Count.Text = (machineProcessLog.ProduceCount - machineProcessLog.ErrorCount).ToString();
            }
            if (employee != null)
            {
                this.lab_employee.Text = employee.EmployeeName + "(" + employee.EmployeeCode + ")";
            }
            //获取所有QC项
            DefectiveType = EntityHelper.DefectiveTypeHelper.GetAllQCCheck().OrderBy(t => t.DefectiveTypeCode).ToList();

            for(int i = 0; i < DefectiveType.Count; i++)
            {
                DefectiveType[i].count = 0;
                DefectiveType[i].sort = i + 1;
            }
            defectiveTypes = new List<DataModel.DefectiveType>();
            defectiveTypes = DefectiveType;
            this.dgv_DefectiveType.DataSource = DefectiveType;
        }
        /// <summary>
        /// 设置页面样式
        /// </summary>
        public void SettingJobOrderView()
        {
            // 表格上下左右自适应
            dgv_DefectiveType.Anchor = (AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left);
            dgv_DefectiveType.AllowUserToAddRows = false;
            dgv_DefectiveType.AllowUserToDeleteRows = false;
            dgv_DefectiveType.ReadOnly = true;
            dgv_DefectiveType.AllowUserToResizeColumns = false;
            dgv_DefectiveType.AllowUserToResizeRows = false;
            // 列手工排序
            // dgv_DefectiveType.AllowUserToOrderColumns = true;
            // 列头系统样式，设置为false，自定义才生效
            dgv_DefectiveType.EnableHeadersVisualStyles = false;
            // 列头高度大小模式
            dgv_DefectiveType.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            // 列头高度大小
            dgv_DefectiveType.ColumnHeadersHeight = 40;
            // 列头居中
            dgv_DefectiveType.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv_DefectiveType.ColumnHeadersDefaultCellStyle.Font = new Font("宋体", 15, FontStyle.Regular);
            // 列头边框样式
            dgv_DefectiveType.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            // 列头背景色
            dgv_DefectiveType.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#262D3A");
            // 列头前景色
            dgv_DefectiveType.ColumnHeadersDefaultCellStyle.ForeColor = ColorTranslator.FromHtml("#FFFFFF");
            // 列宽自适应
           // dgv_DefectiveType.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            // 网格线颜色
            dgv_DefectiveType.GridColor = ColorTranslator.FromHtml("#000000");
            // 背景色
            dgv_DefectiveType.BackgroundColor = ColorTranslator.FromHtml("#FFFFFF");
            // 行头边框样式
            dgv_DefectiveType.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            // 行头背景色
            dgv_DefectiveType.RowHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#C7C9CC");
            // 行高（要在窗体初始化的地方InitializeComponent调用才生效）
            dgv_DefectiveType.RowTemplate.Height = 40;
            // 单元格内容居中
            dgv_DefectiveType.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv_DefectiveType.DefaultCellStyle.Font = new Font("宋体", 12, FontStyle.Regular);
            // 单元格背景色
            dgv_DefectiveType.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FFFFFF");
            // 选中行背景色
            dgv_DefectiveType.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#C7C9CC");
            // 选中行前景色
            dgv_DefectiveType.DefaultCellStyle.SelectionForeColor = Color.BlueViolet;
            // 隔行背景色
            dgv_DefectiveType.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#F2F2F2");
            // 行高自适应
            // dgv_DefectiveType.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.;                
           

            //选择模式为整行选择
            dgv_DefectiveType.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            //不允许选择多行
            dgv_DefectiveType.MultiSelect = true;
            dgv_DefectiveType.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            //不允许自动添加列
            dgv_DefectiveType.AutoGenerateColumns = false;

            DataGridViewTextBoxColumn sort_Column = new DataGridViewTextBoxColumn();
            sort_Column.HeaderText = "序号";
            sort_Column.DataPropertyName = "sort";
            dgv_DefectiveType.Columns.Add(sort_Column);

            DataGridViewTextBoxColumn defectiveTypeCode_Column = new DataGridViewTextBoxColumn();
            defectiveTypeCode_Column.HeaderText = "疵品类型编号";
            defectiveTypeCode_Column.DataPropertyName = "DefectiveTypeCode";
            dgv_DefectiveType.Columns.Add(defectiveTypeCode_Column);


            DataGridViewTextBoxColumn defectiveTypeName_Column = new DataGridViewTextBoxColumn();
            defectiveTypeName_Column.HeaderText = "疵品原因";
            defectiveTypeName_Column.ReadOnly = false;
            defectiveTypeName_Column.DataPropertyName = "DefectiveTypeName";
            dgv_DefectiveType.Columns.Add(defectiveTypeName_Column);

            DataGridViewButtonColumn btn_Sub10 = new DataGridViewButtonColumn();
            btn_Sub10.Name = "btn_Sub10";
            btn_Sub10.HeaderText = "-10";
            btn_Sub10.DefaultCellStyle.NullValue = "<<";
            dgv_DefectiveType.Columns.Add(btn_Sub10);

            DataGridViewButtonColumn btn_Sub = new DataGridViewButtonColumn();
            btn_Sub.Name = "btn_Sub";
            btn_Sub.HeaderText = "-1";
            btn_Sub.DefaultCellStyle.NullValue = "<";
            dgv_DefectiveType.Columns.Add(btn_Sub);

            
            DataGridViewTextBoxColumn count_Column = new DataGridViewTextBoxColumn();
            count_Column.HeaderText = "数量";
            //count_Column.DataPropertyName = "count";
            dgv_DefectiveType.Columns.Add(count_Column);


            DataGridViewButtonColumn btn_Add = new DataGridViewButtonColumn();
            btn_Add.Name = "btn_Add";
            btn_Add.HeaderText = "+1";
            btn_Add.DefaultCellStyle.NullValue = ">";
            dgv_DefectiveType.Columns.Add(btn_Add);

            DataGridViewButtonColumn btn_Add10 = new DataGridViewButtonColumn();
            btn_Add10.Name = "btn_Add10";
            btn_Add10.HeaderText = "+10";
            btn_Add10.DefaultCellStyle.NullValue = ">>";
            dgv_DefectiveType.Columns.Add(btn_Add10);

            dgv_DefectiveType.Columns[0].FillWeight = 8;
            dgv_DefectiveType.Columns[1].FillWeight = 18;
            dgv_DefectiveType.Columns[2].FillWeight = 32;
            dgv_DefectiveType.Columns[3].FillWeight = 8;
            dgv_DefectiveType.Columns[4].FillWeight = 8;
            dgv_DefectiveType.Columns[5].FillWeight = 10;
            dgv_DefectiveType.Columns[6].FillWeight = 8;
            dgv_DefectiveType.Columns[7].FillWeight = 8;
        }

        //可编辑文本
        private void dgv_DefectiveType_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                return;
            }
            if (e.ColumnIndex == 5)
            {
                dgv_DefectiveType.ReadOnly = false;
                dgv_DefectiveType.EditMode = DataGridViewEditMode.EditOnEnter;
            }
            else
            {
                dgv_DefectiveType.ReadOnly = true;
            }
        }

        //+1按钮事件
        private void dgv_DefectiveType_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex == -1)
            {
                return;
            }
            //-10
            if (e.ColumnIndex == 3)
            {
                if(dgv_DefectiveType.Rows[e.RowIndex].Cells[5].Value != null)
                {
                    int value = 0;
                    int.TryParse(dgv_DefectiveType.Rows[e.RowIndex].Cells[5].Value.ToString(), out value);
                    if (value - 10 >= 0)
                    {
                        DefectiveType[(int)dgv_DefectiveType.Rows[e.RowIndex].Cells[0].Value - 1].count = value - 10;
                        dgv_DefectiveType.Rows[e.RowIndex].Cells[5].Value = value - 10;
                        
                    }
                    else
                    {
                        DefectiveType[(int)dgv_DefectiveType.Rows[e.RowIndex].Cells[0].Value - 1].count = 0;
                        dgv_DefectiveType.Rows[e.RowIndex].Cells[5].Value = "0";
                       
                    }
                }
            }
            //-1
            else if(e.ColumnIndex == 4)
            {
                if (dgv_DefectiveType.Rows[e.RowIndex].Cells[5].Value != null)
                {
                    int value = 0;
                    int.TryParse(dgv_DefectiveType.Rows[e.RowIndex].Cells[5].Value.ToString(), out value);
                    if (value - 1>= 0)
                    {
                        DefectiveType[(int)dgv_DefectiveType.Rows[e.RowIndex].Cells[0].Value - 1].count = value - 1;
                        dgv_DefectiveType.Rows[e.RowIndex].Cells[5].Value = value - 1;
                       
                    }
                    else
                    {
                        DefectiveType[(int)dgv_DefectiveType.Rows[e.RowIndex].Cells[0].Value - 1].count = 0;
                        dgv_DefectiveType.Rows[e.RowIndex].Cells[5].Value = "0";
                      
                    }
                }
            }
            //+1
            else if(e.ColumnIndex == 6)
            {
                if (dgv_DefectiveType.Rows[e.RowIndex].Cells[5].Value != null)
                {
                    int value = 0;
                    int.TryParse(dgv_DefectiveType.Rows[e.RowIndex].Cells[5].Value.ToString(), out value);
                    if (value >= 999999)
                    {
                        return;
                    }
                    DefectiveType[(int)dgv_DefectiveType.Rows[e.RowIndex].Cells[0].Value - 1].count = value + 1;
                    dgv_DefectiveType.Rows[e.RowIndex].Cells[5].Value = value + 1;
                  
                }
                else
                {
                    DefectiveType[(int)dgv_DefectiveType.Rows[e.RowIndex].Cells[0].Value - 1].count = 1;
                    dgv_DefectiveType.Rows[e.RowIndex].Cells[5].Value =  1;
                    
                }
            }
            //+10
            else if (e.ColumnIndex == 7)
            {
                if (dgv_DefectiveType.Rows[e.RowIndex].Cells[5].Value != null)
                {
                    int value = 0;
                    int.TryParse(dgv_DefectiveType.Rows[e.RowIndex].Cells[5].Value.ToString(), out value);
                    if (value+10 > 999999)
                    {
                        DefectiveType[(int)dgv_DefectiveType.Rows[e.RowIndex].Cells[0].Value - 1].count = 999999;
                        dgv_DefectiveType.Rows[e.RowIndex].Cells[5].Value = 999999;
                       
                        return;
                    }
                    DefectiveType[(int)dgv_DefectiveType.Rows[e.RowIndex].Cells[0].Value - 1].count = value + 10;
                    dgv_DefectiveType.Rows[e.RowIndex].Cells[5].Value = value + 10;
                    
                }
                else
                {
                    DefectiveType[(int)dgv_DefectiveType.Rows[e.RowIndex].Cells[0].Value - 1].count = 10;
                    dgv_DefectiveType.Rows[e.RowIndex].Cells[5].Value = 10;
                    
                }
            }
        }

        //单元格更改事件
        private void dgv_DefectiveType_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            int count = 0;
            foreach (var item in DefectiveType)
            {
                count = count + item.count;
            }
            errorCount = count;
            this.lab_errorCount.Text = count.ToString();
            //验证不良品数量
            checkErrorCount();
        }

        //单元格编辑事件
        private void dgv_DefectiveType_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgv_DefectiveType.CurrentCellAddress.X == 5)//获取当前处于活动状态的单元格索引
            {
                //解绑之前单元格
                if (CellEdit != null)
                {
                    CellEdit.KeyPress -= Cells_KeyPress; //解绑
                    CellEdit.TextChanged -= Cells_TextChanged;
                }
                CellEdit = (DataGridViewTextBoxEditingControl)e.Control;
                //CellEdit.SelectAll();
                CellEdit.KeyPress += Cells_KeyPress; //绑定事件
                CellEdit.TextChanged += Cells_TextChanged;
               
            }
        }

        //自定义键盘事件
        private void Cells_KeyPress(object sender, KeyPressEventArgs e) 
        {
            if (dgv_DefectiveType.CurrentCellAddress.X == 5)//获取当前处于活动状态的单元格索引
            {
                char ch = e.KeyChar;

                if (!Char.IsDigit(ch) && ch != 8)
                {
                  
                    e.Handled = true;
                   
                }
            }
        }

        //自定义更改事件
        private void Cells_TextChanged(object sender, EventArgs e)
        {
            if (dgv_DefectiveType.CurrentCellAddress.X == 5)//获取当前处于活动状态的单元格索引
            {
                TextBox textBox = (TextBox)sender;
                int textBoxValue = 0;
                int.TryParse(textBox.Text, out textBoxValue);
                if (textBoxValue > 999999)
                {
                    textBox.Text = "999999";
                    textBoxValue = 999999;
                }
                DefectiveType[(int)dgv_DefectiveType.Rows[dgv_DefectiveType.CurrentCellAddress.Y].Cells[0].Value - 1].count = textBoxValue;
                int count = 0;
                foreach(var item in DefectiveType)
                {
                    count = count + item.count;
                }
                errorCount = count;
                this.lab_errorCount.Text = count.ToString();

                checkErrorCount();
            }
        }
        //验证不良品数量
        private void checkErrorCount()
        {
            int sum_count = 0;
            int.TryParse(lab_Count.Text, out sum_count);
            if (errorCount > sum_count)
            {
                this.lab_errorCount.ForeColor = Color.Red;
                this.btn_Confirm.Enabled = false;
            }
            else if (errorCount == 0)
            {
                this.lab_errorCount.ForeColor = System.Drawing.Color.White;
                this.btn_Confirm.Enabled = false;
            }
            else
            {
                this.lab_errorCount.ForeColor = System.Drawing.Color.White;
                this.btn_Confirm.Enabled = true;
            }
        }

        private void txt_code_TextChanged(object sender, EventArgs e)
        {
            this.btn_Check.Text = "核对";
            defectiveTypes = new List<DataModel.DefectiveType>();
            foreach (var item in DefectiveType)
            {
                if (item.DefectiveTypeCode.ToUpper().IndexOf(this.txt_code.Text.ToUpper()) != -1)
                {
                    defectiveTypes.Add(item);
                }
            }
            this.dgv_DefectiveType.DataSource = defectiveTypes;
            initCount();
        }
        //给数量赋初始值
        private void initCount()
        {
            for(int i = 0; i < dgv_DefectiveType.RowCount; i++)
            {
                dgv_DefectiveType.Rows[i].Cells[5].Value = DefectiveType[(int)dgv_DefectiveType.Rows[i].Cells[0].Value - 1].count;
            }
        }

        private void btn_Check_Click(object sender, EventArgs e)
        {
           ;
            if (this.btn_Check.Text == "核对")
            {
                this.btn_Check.Text = "返回";
                List<DataModel.DefectiveType> checkDefectiveType = new List<DataModel.DefectiveType>();
                foreach (var item in DefectiveType)
                {
                    if (item.count>0)
                    {
                        checkDefectiveType.Add(item);
                    }
                }
                this.dgv_DefectiveType.DataSource = checkDefectiveType;
                initCount();

            }
            else
            {
                this.btn_Check.Text = "核对";
                this.dgv_DefectiveType.DataSource = defectiveTypes;
                initCount();
            }
        }
    }
}
     