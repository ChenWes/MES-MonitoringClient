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
		//是否有过滤工单
		private bool MC_FilterJobOrder = false;

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
                //设置工单页面样式
                SettingJobOrderView();

                List<DataModel.JobOrderDisplay> jobOrders = new List<DataModel.JobOrderDisplay>();

				//显示出订单列表，供用户选择
				if (MC_JobOrderFilter == FilterOrderType.NoStart)
				{
                    //未开始的工单
                    jobOrders = Common.JobOrderHelper.GetJobOrderByAssigned();
				}
				else if (MC_JobOrderFilter == FilterOrderType.NoCompleted)
				{
                    //已开始，但未结束的工单
                    jobOrders = Common.JobOrderHelper.GetJobOrderBySuspend();
				}

                dgv_JobOrder.DataSource = FormatNeedSecond(jobOrders);



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


        /// <summary>
        /// 转换时间
        /// </summary>
        /// <param name="pi_jobOrders"></param>
        /// <returns></returns>
        private List<DataModel.JobOrderDisplay> FormatNeedSecond(List<DataModel.JobOrderDisplay> pi_jobOrders)
        {
            foreach (var item in pi_jobOrders)
            {
                item.sumNeedSecondDesc = Common.CommonFunction.FormatSeconds_D(item.sumNeedSecond);
            }

            return pi_jobOrders;
        }

		/*按钮方法*/
		/*---------------------------------------------------------------------------------------*/

		private void dgv_JobOrder_CellClick(object sender, DataGridViewCellEventArgs e)
		{

			try
			{

                int rowID = dgv_JobOrder.ColumnCount-1;

				
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
							sb.Append(JobOrderItem.JobOrderID + " ");
						});

						lab_SelectJobOrder.Text = "选择的工单ID：" + sb.ToString();
					}
					else
					{
						lab_SelectJobOrder.Text = "选择的工单ID：";
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
			try
			{

				if (MC_frmChangeJobOrderPara == null)
				{
					throw new Exception("请选择要操作的工单");
				}
				else
				{
					for (int i = 0; i < MC_frmChangeJobOrderPara.Count; i++)
					{
						for (int j = i + 1; j < MC_frmChangeJobOrderPara.Count; j++)
						{
							if (MC_frmChangeJobOrderPara[i].MouldCode != MC_frmChangeJobOrderPara[j].MouldCode)
							{
								throw new Exception("不同模具的工单不能一起合并生产！");
							}
						}
					}

					this.Close();
				}
			}
			catch (Exception ex)
			{
				ShowErrorMessage("选择工单错误，原因是：" + ex.Message, "选择工单");
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender">1：未开始工单 2：未完成工单</param>
		/// <param name="e"></param>
		private void btn_SearchByMouldCode_Click(object sender, EventArgs e)
		{
			try
			{

				if (string.IsNullOrEmpty(txt_MouldCode.Text.Trim())) throw new Exception("模具编号为空");

				string type = "";

				if (MC_JobOrderFilter == FilterOrderType.NoStart)
				{
					type = "1";
				}
				else if (MC_JobOrderFilter == FilterOrderType.NoCompleted)
				{
					type = "2";
				}


                List<DataModel.JobOrderDisplay> jobOrders = Common.JobOrderHelper.GetJobOrderByMouldCode(txt_MouldCode.Text.Trim(), type);

                dgv_JobOrder.DataSource = FormatNeedSecond(jobOrders);

                //清空已经选择的工单
                MC_frmChangeJobOrderPara.Clear();				
				
			}
			catch (Exception ex)
			{
				ShowErrorMessage("查询工单错误，原因是：" + ex.Message, "查询工单错误");

			}
		}

        /// <summary>
        /// 设置工单页面样式
        /// </summary>
        public void SettingJobOrderView()
        {
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
            dgv_JobOrder.ColumnHeadersDefaultCellStyle.Font = new Font("宋体", 10, FontStyle.Bold);
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
            dgv_JobOrder.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#C7C9CC");
            // 选中行前景色
            dgv_JobOrder.DefaultCellStyle.SelectionForeColor = Color.BlueViolet;
            // 隔行背景色
            dgv_JobOrder.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#F6F6F6");
            // 行高自适应
            //dgv_JobOrder.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedHeaders;                


            //选择模式为整行选择
            dgv_JobOrder.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            //不允许选择多行
            dgv_JobOrder.MultiSelect = true;

            //不允许自动添加列
            dgv_JobOrder.AutoGenerateColumns = false;


            DataGridViewCheckBoxColumn checkBoxColumn = new DataGridViewCheckBoxColumn();
            checkBoxColumn.HeaderText = "选择";
            checkBoxColumn.Name = "CheckFlag";
            dgv_JobOrder.Columns.Add(checkBoxColumn);


            DataGridViewTextBoxColumn JobOrderID_Column = new DataGridViewTextBoxColumn();
            JobOrderID_Column.HeaderText = "工单ID";
            JobOrderID_Column.DataPropertyName = "JobOrderID";
            dgv_JobOrder.Columns.Add(JobOrderID_Column);


            DataGridViewTextBoxColumn JobOrderNumber_Column = new DataGridViewTextBoxColumn();
            JobOrderNumber_Column.HeaderText = "工单Number";
            JobOrderNumber_Column.DataPropertyName = "JobOrderNumber";
            dgv_JobOrder.Columns.Add(JobOrderNumber_Column);

            DataGridViewTextBoxColumn ProductCode_Column = new DataGridViewTextBoxColumn();
            ProductCode_Column.HeaderText = "产品编号";
            ProductCode_Column.DataPropertyName = "ProductCode";
            dgv_JobOrder.Columns.Add(ProductCode_Column);

            DataGridViewTextBoxColumn ProductCategory_Column = new DataGridViewTextBoxColumn();
            ProductCategory_Column.HeaderText = "产品组";
            ProductCategory_Column.DataPropertyName = "ProductCategory";
            dgv_JobOrder.Columns.Add(ProductCategory_Column);

            DataGridViewTextBoxColumn OrderCount_Column = new DataGridViewTextBoxColumn();
            OrderCount_Column.HeaderText = "订单数量";
            OrderCount_Column.DataPropertyName = "OrderCount";
            dgv_JobOrder.Columns.Add(OrderCount_Column);

            DataGridViewTextBoxColumn MaterialCode_Column = new DataGridViewTextBoxColumn();
            MaterialCode_Column.HeaderText = "原料编号";
            MaterialCode_Column.DataPropertyName = "MaterialCode";
            dgv_JobOrder.Columns.Add(MaterialCode_Column);

            DataGridViewTextBoxColumn DeliveryDate_Column = new DataGridViewTextBoxColumn();
            DeliveryDate_Column.HeaderText = "到期日期";
            DeliveryDate_Column.DataPropertyName = "DeliveryDate";
            dgv_JobOrder.Columns.Add(DeliveryDate_Column);

            DataGridViewTextBoxColumn MachineTonnage_Column = new DataGridViewTextBoxColumn();
            MachineTonnage_Column.HeaderText = "机器吨位";
            MachineTonnage_Column.DataPropertyName = "MachineTonnage";
            dgv_JobOrder.Columns.Add(MachineTonnage_Column);

            DataGridViewTextBoxColumn MouldID_Column = new DataGridViewTextBoxColumn();
            MouldID_Column.HeaderText = "模具";
            MouldID_Column.DataPropertyName = "MouldID";
            dgv_JobOrder.Columns.Add(MouldID_Column);

            DataGridViewTextBoxColumn MouldStandardProduceSecond_Column = new DataGridViewTextBoxColumn();
            MouldStandardProduceSecond_Column.HeaderText = "模具标准生命周期";
            MouldStandardProduceSecond_Column.DataPropertyName = "MouldStandardProduceSecond";
            dgv_JobOrder.Columns.Add(MouldStandardProduceSecond_Column);

            DataGridViewTextBoxColumn Status_Column = new DataGridViewTextBoxColumn();
            Status_Column.HeaderText = "工单状态";
            Status_Column.DataPropertyName = "Status";
            dgv_JobOrder.Columns.Add(Status_Column);

            DataGridViewTextBoxColumn sumProduceCount_Column = new DataGridViewTextBoxColumn();
            sumProduceCount_Column.HeaderText = "已生产总数";
            sumProduceCount_Column.DataPropertyName = "sumProduceCount";
            dgv_JobOrder.Columns.Add(sumProduceCount_Column);

            DataGridViewTextBoxColumn sumErrorCount_Column = new DataGridViewTextBoxColumn();
            sumErrorCount_Column.HeaderText = "不良品总数";
            sumErrorCount_Column.DataPropertyName = "sumErrorCount";
            dgv_JobOrder.Columns.Add(sumErrorCount_Column);

            DataGridViewTextBoxColumn sumNoCompleted_Column = new DataGridViewTextBoxColumn();
            sumNoCompleted_Column.HeaderText = "未完成数量";
            sumNoCompleted_Column.DataPropertyName = "sumNoCompleted";
            dgv_JobOrder.Columns.Add(sumNoCompleted_Column);

            DataGridViewTextBoxColumn sumNeedSecondDesc_Column = new DataGridViewTextBoxColumn();
            sumNeedSecondDesc_Column.HeaderText = "预计生产时间";
            sumNeedSecondDesc_Column.DataPropertyName = "sumNeedSecondDesc";
            dgv_JobOrder.Columns.Add(sumNeedSecondDesc_Column);

            DataGridViewTextBoxColumn ServiceDepartment_Column = new DataGridViewTextBoxColumn();
            ServiceDepartment_Column.HeaderText = "送达部门";
            ServiceDepartment_Column.DataPropertyName = "ServiceDepartment";
            dgv_JobOrder.Columns.Add(ServiceDepartment_Column);

            DataGridViewTextBoxColumn id_Column = new DataGridViewTextBoxColumn();
            id_Column.HeaderText = "ID";
            id_Column.DataPropertyName = "ID";
            id_Column.Visible = false;
            dgv_JobOrder.Columns.Add(id_Column);
        }


		private void dgv_JobOrder_DataError(object sender, DataGridViewDataErrorEventArgs e)
		{

		}

    }
}
