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
        public DataModel.JobOrder currentJobOrder = null;
        public List<DataModel.QCCheckCount> QCCheckCounts = new List<DataModel.QCCheckCount>();
        public int errorCount = 0;
        List<DataModel.QCCheck> QCChecks = null;
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
            int.TryParse(this.lab_ErrorCount.Text, out errorCount);
            //保存大于0的项
            foreach (var item in textBoxes)
            {
                int value = 0;
                int.TryParse(item.Text, out value);
                if (value > 0)
                {
                    DataModel.QCCheckCount QCCheckCount = new DataModel.QCCheckCount();
                    QCCheckCount.QCCheckID = QCChecks[textBoxes.IndexOf(item)]._id;
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
            this.lab_JobOrder.Text = currentJobOrder.JobOrderID;
            var machineProcessLog = currentJobOrder.MachineProcessLog.Find(t => t.ProduceStartDate == t.ProduceEndDate);
            //良品  
            if (machineProcessLog != null)
            {
                this.lab_Count.Text = (machineProcessLog.ProduceCount - machineProcessLog.ErrorCount).ToString();
            }
            //获取所有QC项
            QCChecks = EntityHelper.QCCheckHelper.GetAllQCCheck();
            if (QCChecks != null)
            {
                foreach (var item in QCChecks)
                {
                    Label label = new Label();
                    label.Anchor = System.Windows.Forms.AnchorStyles.Right;
                    label.AutoSize = true;
                    label.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                    label.ForeColor = System.Drawing.Color.White;
                    label.Text = item.code + "(" + item.desc + ")";
                    tlp_QCCheck.Controls.Add(label);
                    //文本框
                    TextBox textBox = new TextBox();
                    textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
                    textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
                    textBox.Anchor = System.Windows.Forms.AnchorStyles.None;
                    textBox.Font = new System.Drawing.Font("宋体", 13.5F);
                    textBox.Size = new System.Drawing.Size(200, 35);
                    textBox.Margin = new System.Windows.Forms.Padding(0);
                    textBox.Text = "0";
                    textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
                    textBox.TextChanged += new System.EventHandler(this.textBox_TextChanged);
                    textBoxes.Add(textBox);
                    //按钮
                    Button subButton10 = addButton("-10");
                    subButtons10.Add(subButton10);
                    subButton10.Click += new System.EventHandler(btnAdd_click);
                    Button subButton1 = addButton("-1");
                    subButtons1.Add(subButton1);
                    subButton1.Click += new System.EventHandler(btnAdd_click);
                    Button addButton1 = addButton("+1");
                    addButtons1.Add(addButton1);
                    addButton1.Click += new System.EventHandler(btnAdd_click);
                    Button addButton10 = addButton("+10");
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
                    tlp_QCCheck.Controls.Add(tableLayoutPanel);
                }

            }

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
                textBoxes[addIndex1].Text =(value+1).ToString();
            }
            //+10
            else if (addIndex10 >= 0)
            {
                int value = 0;
                int.TryParse(textBoxes[addIndex10].Text, out value);
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
        private void textBox_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            int nowValue = 0;
            int.TryParse(textBox.Text, out nowValue);
            if (nowValue > 0)
            {
                textBox.ForeColor = Color.Red;
            }
            else
            {
                textBox.ForeColor = Color.Black;
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
        }
        private Button addButton(string text)
        {
            Button button = new Button();
            button.Size = new System.Drawing.Size(40, 26);
            button.Anchor = System.Windows.Forms.AnchorStyles.None;
            button.UseVisualStyleBackColor = true;
            button.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            button.Margin = new System.Windows.Forms.Padding(0);
            button.Text = text;
            return button;
        }
    }
}
     