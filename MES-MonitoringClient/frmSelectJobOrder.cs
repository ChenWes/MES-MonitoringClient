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
    public partial class frmSelectJobOrder : Form
    {
        public enum FilterOrderType
        {
            NoStart,
            NoCompleted
        }

        /// <summary>
        /// 工单过滤条件（未开始、未完成），作为传入参数
        /// 分别对应开始工单及恢复工单
        /// </summary>
        public FilterOrderType MC_JobOrderFilter;

        /// <summary>
        /// 选择的工单列表参数，作为传出参数
        /// </summary>
        public List<DataModel.JobOrder> MC_frmChangeJobOrderPara;

        public frmSelectJobOrder()
        {
            InitializeComponent();

            // 行高（要在窗体初始化的地方InitializeComponent调用才生效）
            dgv_JobOrder.RowTemplate.Height = 80;
        }

        /*窗口加载方法*/
        /*---------------------------------------------------------------------------------------*/

        private void frmSelectJobOrder_Load(object sender, EventArgs e)
        {
            try
            {
                //窗口最大化
                this.WindowState = FormWindowState.Maximized;

                //声明变量
                MC_frmChangeJobOrderPara = new List<DataModel.JobOrder>();


                //显示出订单列表，供用户选择
                if (MC_JobOrderFilter == FilterOrderType.NoStart)
                {
                    //未开始的工单
                    dgv_JobOrder.DataSource = Common.JobOrderHelper.GetJobOrderByAssigned();
                }
                else if (MC_JobOrderFilter == FilterOrderType.NoCompleted)
                {
                    //已开始，但未结束的工单
                    dgv_JobOrder.DataSource = Common.JobOrderHelper.GetJobOrderBySuspend();
                }

                // 表格上下左右自适应
                dgv_JobOrder.Anchor = (AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left);
                dgv_JobOrder.AllowUserToAddRows = false;
                dgv_JobOrder.AllowUserToDeleteRows = false;
                dgv_JobOrder.ReadOnly = true;
                // 列手工排序
                dgv_JobOrder.AllowUserToOrderColumns = true;
                // 列头系统样式，设置为false，自定义才生效
                dgv_JobOrder.EnableHeadersVisualStyles = false;
                // 列头高度大小模式
                dgv_JobOrder.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                // 列头高度大小
                dgv_JobOrder.ColumnHeadersHeight = 80;
                // 列头居中
                dgv_JobOrder.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgv_JobOrder.ColumnHeadersDefaultCellStyle.Font = new Font("宋体", 20, FontStyle.Bold);
                // 列头边框样式
                dgv_JobOrder.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                // 列头背景色
                dgv_JobOrder.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#262D3A");
                // 列头前景色
                dgv_JobOrder.ColumnHeadersDefaultCellStyle.ForeColor = ColorTranslator.FromHtml("#FFFFFF");
                // 列宽自适应
                dgv_JobOrder.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                // 网格线颜色
                dgv_JobOrder.GridColor = ColorTranslator.FromHtml("#000000");
                // 背景色
                dgv_JobOrder.BackgroundColor = ColorTranslator.FromHtml("#FFFFFF");
                // 行头边框样式
                dgv_JobOrder.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                // 行头背景色
                dgv_JobOrder.RowHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#C7C9CC");
                // 行高（要在窗体初始化的地方InitializeComponent调用才生效）
                dgv_JobOrder.RowTemplate.Height = 80;
                // 单元格内容居中
                dgv_JobOrder.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                // 单元格背景色
                dgv_JobOrder.DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FFFFFF");
                // 选中行背景色
                //dgv_JobOrder.DefaultCellStyle.SelectionBackColor= ColorTranslator.FromHtml("#AAD1F6");
                // 隔行背景色
                dgv_JobOrder.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#C7C9CC");
                // 行高自适应
                //dgv_JobOrder.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedHeaders;                


                //选择模式为整行选择
                dgv_JobOrder.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                //不允许选择多行
                dgv_JobOrder.MultiSelect = true;


                DataGridViewCheckBoxColumn checkBoxColumn = new DataGridViewCheckBoxColumn();
                checkBoxColumn.HeaderText = "选择";
                checkBoxColumn.Name = "CheckFlag";
                dgv_JobOrder.Columns.Add(checkBoxColumn);


                //工单
                dgv_JobOrder.Columns[0].DataPropertyName = "JobOrderCode";
                dgv_JobOrder.Columns[0].HeaderText = "工单ID";

                dgv_JobOrder.Columns[1].DataPropertyName = "JobOrderName";
                dgv_JobOrder.Columns[1].HeaderText = "工单Number";

                dgv_JobOrder.Columns[2].DataPropertyName = "OrderCount";
                dgv_JobOrder.Columns[2].HeaderText = "工单数量";

                dgv_JobOrder.Columns[3].DataPropertyName = "JobOrderDesc";
                dgv_JobOrder.Columns[3].HeaderText = "工单描述";

                dgv_JobOrder.Columns[4].DataPropertyName = "Status";
                dgv_JobOrder.Columns[4].HeaderText = "工单状态";

                //客户
                dgv_JobOrder.Columns[5].DataPropertyName = "CustomerCode";
                dgv_JobOrder.Columns[5].HeaderText = "客户编号";

                dgv_JobOrder.Columns[6].DataPropertyName = "CustomerName";
                dgv_JobOrder.Columns[6].HeaderText = "客户名称";

                //产品
                dgv_JobOrder.Columns[7].DataPropertyName = "MaterialCode";
                dgv_JobOrder.Columns[7].HeaderText = "产品编号";

                dgv_JobOrder.Columns[8].DataPropertyName = "MaterialName";
                dgv_JobOrder.Columns[8].HeaderText = "产品名称";

                dgv_JobOrder.Columns[9].DataPropertyName = "MaterialSpecification";
                dgv_JobOrder.Columns[9].HeaderText = "产品规格";

                //模具
                dgv_JobOrder.Columns[10].DataPropertyName = "MouldCode";
                dgv_JobOrder.Columns[10].HeaderText = "模具编号";

                dgv_JobOrder.Columns[11].DataPropertyName = "MouldName";
                dgv_JobOrder.Columns[11].HeaderText = "模具名称";

                dgv_JobOrder.Columns[12].DataPropertyName = "MouldSpecification";
                dgv_JobOrder.Columns[12].HeaderText = "模具规格";

                dgv_JobOrder.Columns[13].DataPropertyName = "StandardProduceSecond";
                dgv_JobOrder.Columns[13].HeaderText = "标准生命周期";

                dgv_JobOrder.Columns[14].DataPropertyName = "ID";
                dgv_JobOrder.Columns[14].HeaderText = "工单ID标识";
                dgv_JobOrder.Columns[14].Visible = false;

            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message, "订单选择窗口初始化失败");                
            }
        }


        //解决界面闪烁的问题
        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        CreateParams cp = base.CreateParams;
        //        cp.ExStyle |= 0x02000000;
        //        return cp;
        //    }
        //}

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


        /*按钮方法*/
        /*---------------------------------------------------------------------------------------*/

        /// <summary>
        /// 工单确认事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgv_JobOrder_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgv_JobOrder_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int rowID = 14;

                if (e.RowIndex > -1)
                {
                    DataGridViewRow selectRow = dgv_JobOrder.Rows[e.RowIndex];

                    if (selectRow.Cells["CheckFlag"].Value == "true")
                    {
                        selectRow.Cells["CheckFlag"].Value = "false";

                        int findIndex = MC_frmChangeJobOrderPara.FindIndex(select => select._id == selectRow.Cells[rowID].Value.ToString());
                        MC_frmChangeJobOrderPara.RemoveAt(findIndex);
                    }
                    else
                    {
                        selectRow.Cells["CheckFlag"].Value = "true";

                        DataModel.JobOrder jobOrder = Common.JobOrderHelper.GetJobOrderByID(selectRow.Cells[rowID].Value.ToString());
                        MC_frmChangeJobOrderPara.Add(jobOrder);
                    }


                    if (MC_frmChangeJobOrderPara.Count > 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        MC_frmChangeJobOrderPara.ForEach(delegate (DataModel.JobOrder JobOrderItem)
                        {
                            sb.Append(JobOrderItem.JobOrderCode + " ");
                        });
                        lab_SelectJobOrder.Text = "选择的工单号：" + sb.ToString();
                    }
                    else
                    {
                        lab_SelectJobOrder.Text = "选择的工单号：";
                    }
                }

            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message, "选择工单出错");
            }
        }

        /*按钮方法*/
        /*---------------------------------------------------------------------------------------*/

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            MC_frmChangeJobOrderPara = null;

            this.Close();
        }

        private void btn_Confirm_Click(object sender, EventArgs e)
        {
            if (MC_frmChangeJobOrderPara == null)
            {
                ShowErrorMessage("请选择要操作的工单", "选择工单");
            }
            else
            {
                this.Close();
            }
        }


    }
}
