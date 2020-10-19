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
        List<DataModel.DefectiveType> DefectiveType = null;
        Panel panel;
        /*主窗口方法*/
        /*---------------------------------------------------------------------------------------*/

        public frmQC()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {
          
           
        }

        private void btn_Confirm_Click(object sender, EventArgs e)
        {
            //总数
          //  int.TryParse(this.lab_ErrorCount.Text, out errorCount);
            //保存大于0的项
            foreach (var item in textBoxes)
            {
                int value = 0;
                int.TryParse(item.Text, out value);
                if (value > 0)
                {
                    DataModel.QCCheckCount QCCheckCount = new DataModel.QCCheckCount();
                    QCCheckCount.DefectiveTypeID = DefectiveType[textBoxes.IndexOf(item)]._id;
                    QCCheckCount.Count = value;
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
            //获取所有QC项
            DefectiveType = EntityHelper.DefectiveTypeHelper.GetAllQCCheck().OrderBy(t => t.DefectiveTypeCode).ToList();

            for(int i = 0; i < DefectiveType.Count; i++)
            {
                DefectiveType[i].sort = i + 1;
            }
            this.dgv_DefectiveType.DataSource = DefectiveType;
            /* if (DefectiveType != null)
             {
                 foreach (var item in DefectiveType)
                 {
                     panel = new Panel();
                     panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                     panel.Dock = System.Windows.Forms.DockStyle.Fill;
                    // tlp_QCCheck.Controls.Add(panel);

                     Label labelCode = new Label();
                     labelCode.BackColor = Color.Green;
                     labelCode.Anchor = System.Windows.Forms.AnchorStyles.Left;
                     labelCode.AutoSize = true;
                     labelCode.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                     labelCode.ForeColor = System.Drawing.Color.White;
                     labelCode.Text = item.DefectiveTypeCode ;
                     labelCodeList.Add(labelCode);
                     Label labelName = new Label();
                     labelName.Anchor = (System.Windows.Forms.AnchorStyles.Right| System.Windows.Forms.AnchorStyles.Top);
                     labelName.AutoSize = true;
                     labelName.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                     labelName.ForeColor = System.Drawing.Color.LightGray;
                     labelName.Text =  item.DefectiveTypeName ;
                     TableLayoutPanel labelTableLayoutPanel = new TableLayoutPanel();
                     //outTableLayoutPanel.BackColor = Color.Gray;
                     labelTableLayoutPanel.ColumnCount = 2;
                     labelTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
                     labelTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
                     labelTableLayoutPanel.Controls.Add(labelCode, 0, 0);
                     labelTableLayoutPanel.Controls.Add(labelName, 1, 0);
                     labelTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
                     labelTableLayoutPanel.RowCount = 1;
                     labelTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));



                     //文本框
                     TextBox textBox = new TextBox();
                     textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
                     textBox.BackColor = Color.LightGray;
                     textBox.MaxLength = 7;
                     textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
                     textBox.Anchor = System.Windows.Forms.AnchorStyles.None;
                     textBox.Font = new System.Drawing.Font("宋体", 13.5F);
                     textBox.Size = new System.Drawing.Size(200, 35);
                     textBox.Margin = new System.Windows.Forms.Padding(0);
                     textBox.Text = "0";
                     textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
                    // textBox.TextChanged += new System.EventHandler(this.textBox_TextChanged);
                     textBoxes.Add(textBox);
                     //按钮
                     Button subButton10 = addButton("<<");
                     subButtons10.Add(subButton10);
                     subButton10.Click += new System.EventHandler(btnAdd_click);
                     Button subButton1 = addButton("<");
                     subButtons1.Add(subButton1);
                     subButton1.Click += new System.EventHandler(btnAdd_click);
                     Button addButton1 = addButton(">");
                     addButtons1.Add(addButton1);
                     addButton1.Click += new System.EventHandler(btnAdd_click);
                     Button addButton10 = addButton(">>");
                     addButtons10.Add(addButton10);
                     addButton10.Click += new System.EventHandler(btnAdd_click);
                     TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
                     tableLayoutPanel.ColumnCount = 5;
                     tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 36F));
                     tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 36F));
                     tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
                     tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 36F));
                     tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 36F));
                     tableLayoutPanel.Controls.Add(subButton10, 0, 0);
                     tableLayoutPanel.Controls.Add(subButton1, 1, 0);
                     tableLayoutPanel.Controls.Add(textBox, 2, 0);
                     tableLayoutPanel.Controls.Add(addButton1, 3, 0);
                     tableLayoutPanel.Controls.Add(addButton10, 4, 0);
                     tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
                     tableLayoutPanel.RowCount = 1;
                     tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));

                     TableLayoutPanel outTableLayoutPanel = new TableLayoutPanel();
                     outTableLayoutPanel.ColumnCount = 1;
                     outTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
                     outTableLayoutPanel.Controls.Add(labelTableLayoutPanel, 0, 0);
                     outTableLayoutPanel.Controls.Add(tableLayoutPanel, 0, 1);
                     outTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
                     outTableLayoutPanel.RowCount = 2;
                     outTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
                     outTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
                     panel.Controls.Add(outTableLayoutPanel);
                 }
             }*/

        }
        private void btnAdd_click(object sender, System.EventArgs e)
        {

            //将触发此事件的对象转换为该Button对象
            Button b1 = (Button)sender;
            int subIndex10 = subButtons10.IndexOf(b1);
            int subIndex1 = subButtons1.IndexOf(b1);
            int addIndex1 = addButtons1.IndexOf(b1);
            int addIndex10= addButtons10.IndexOf(b1);
            
            //+1
            if (addIndex1 >= 0)
            {
                int value = 0;
                int.TryParse(textBoxes[addIndex1].Text, out value);
                if (value + 1 > 9999999)
                {
                    return;
                }
                textBoxes[addIndex1].Text =(value+1).ToString();
            }
            //+10
            else if (addIndex10 >= 0)
            {
                int value = 0;
                int.TryParse(textBoxes[addIndex10].Text, out value);
                if (value + 10 > 9999999)
                {
                    return;
                }
                textBoxes[addIndex10].Text = (value + 10).ToString();
            }
            //-1
            else if (subIndex1 >= 0)
            {
                int value = 0;
                int.TryParse(textBoxes[subIndex1].Text, out value);
                textBoxes[subIndex1].Text = (value-1).ToString();
                int.TryParse(textBoxes[subIndex1].Text, out value);
                if (value < 0)
                {
                    textBoxes[subIndex1].Text = "0";
                }
            }
            //-10
            else
            {
                int value = 0;
                int.TryParse(textBoxes[subIndex10].Text, out value);
                textBoxes[subIndex10].Text = (value - 10).ToString();
                int.TryParse(textBoxes[subIndex10].Text, out value);
                if (value < 0)
                {
                    textBoxes[subIndex10].Text = "0";
                }

            }
            
        }

        /// <summary>
        /// 只能输入数字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;

            if (!Char.IsDigit(ch) && ch != 8)
            {
                e.Handled = true;
            }
        }
        /// <summary>
        /// 计数总数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
      /*  private void textBox_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            int index = textBoxes.IndexOf(textBox);
            
            int nowValue = 0;
            int.TryParse(textBox.Text, out nowValue);
            if (nowValue > 0)
            {
                labelCodeList[index].BackColor = Color.Red;
                textBox.ForeColor = Color.OrangeRed ;
                textBox.BackColor = Color.White;
            }
            else
            {
                labelCodeList[index].BackColor = Color.Green;
                textBox.ForeColor = Color.Black;
                textBox.BackColor = Color.LightGray;
            }
            //计数总数
            foreach (var item in textBoxes)
            {
                int value = 0;
                int.TryParse(item.Text, out value);
                errorCount = 0;
                int.TryParse(lab_ErrorCount.Text, out errorCount);
                if (textBoxes.IndexOf(item) == 0)
                {
                    this.lab_ErrorCount.Text =  value.ToString();
                }
                else{
                    this.lab_ErrorCount.Text = (errorCount+value).ToString();
                }
            }
            errorCount = 0;
            int.TryParse(lab_ErrorCount.Text, out errorCount);
            //良品数
            int count = 0;
            int.TryParse(lab_Count.Text, out count);
            if (errorCount > count)
            {
                this.lab_ErrorCount.ForeColor = Color.Red;
                this.btn_Confirm.Enabled = false;
            }
            else if (errorCount == 0)
            {
                this.lab_ErrorCount.ForeColor = System.Drawing.Color.White;
                this.btn_Confirm.Enabled = false;
            }
            else
            {
                this.lab_ErrorCount.ForeColor = System.Drawing.Color.White;
                this.btn_Confirm.Enabled = true;
            }
        }*/
        //添加按钮
        private Button addButton(string text)
        {
            Button button = new Button();
            button.Dock = System.Windows.Forms.DockStyle.Fill;
            button.FlatAppearance.BorderSize = 0;
            button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button.ForeColor = System.Drawing.Color.Gray;
            button.Text = text;
            button.Margin = new System.Windows.Forms.Padding(0);
           // button.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            button.Size = new System.Drawing.Size(50, 30);
            button.UseVisualStyleBackColor = true;
            return button;
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
            defectiveTypeName_Column.HeaderText = "報廢原因";
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
            //count_Column.Width = 200;
            count_Column.Name = "textBoxe";
            dgv_DefectiveType.Columns.Add(count_Column);


            DataGridViewButtonColumn btn_Add = new DataGridViewButtonColumn();
            btn_Add.Name = "btn_Add";
            btn_Add.HeaderText = "修改";
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
                        dgv_DefectiveType.Rows[e.RowIndex].Cells[5].Value = value - 10;
                    }
                    else
                    {
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
                        dgv_DefectiveType.Rows[e.RowIndex].Cells[5].Value = value - 1;
                    }
                    else
                    {
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
                    dgv_DefectiveType.Rows[e.RowIndex].Cells[5].Value = value + 1;
                }
                else
                {
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
                    dgv_DefectiveType.Rows[e.RowIndex].Cells[5].Value = value + 10;
                }
                else
                {
                    dgv_DefectiveType.Rows[e.RowIndex].Cells[5].Value = 10;
                }
            }
        }
    }
}
     