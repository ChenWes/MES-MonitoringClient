namespace MES_MonitoringClient
{
    partial class frmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.serialPort6 = new System.IO.Ports.SerialPort(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lab_Title = new System.Windows.Forms.Label();
            this.lab_DateTime = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.lab_CurrentStatusTotalTime = new System.Windows.Forms.Label();
            this.lab_ProductCount = new System.Windows.Forms.Label();
            this.lab_LastLifeCycleTime = new System.Windows.Forms.Label();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.lab_ReceviedDataCount = new System.Windows.Forms.Label();
            this.lab_SendErrorCount = new System.Windows.Forms.Label();
            this.lab_SendSuccessCount = new System.Windows.Forms.Label();
            this.lab_ReceviedDataErrorCount = new System.Windows.Forms.Label();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.lab_UploadDataServiceStatus = new System.Windows.Forms.Label();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.cartesianChart_temperature = new LiveCharts.WinForms.CartesianChart();
            this.btn_CloseWindow = new System.Windows.Forms.Button();
            this.tableLayoutPanel9 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btn_StatusLight = new MES_MonitoringClient.Common.Component.CircularButton();
            this.btn_SignalX03 = new MES_MonitoringClient.Common.Component.CircularButton();
            this.btn_SignalX02 = new MES_MonitoringClient.Common.Component.CircularButton();
            this.btn_SignalX01 = new MES_MonitoringClient.Common.Component.CircularButton();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel5.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel8.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel9.SuspendLayout();
            this.SuspendLayout();
            // 
            // serialPort6
            // 
            this.serialPort6.BaudRate = 115200;
            this.serialPort6.PortName = "COM6";
            this.serialPort6.ErrorReceived += new System.IO.Ports.SerialErrorReceivedEventHandler(this.serialPort6_ErrorReceived);
            this.serialPort6.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort6_DataReceived);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(98)))), ((int)(((byte)(215)))));
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(991, 50);
            this.panel1.TabIndex = 12;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.Controls.Add(this.lab_Title, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lab_DateTime, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel4, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel5, 3, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(991, 50);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // lab_Title
            // 
            this.lab_Title.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lab_Title.AutoSize = true;
            this.lab_Title.Font = new System.Drawing.Font("宋体", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_Title.ForeColor = System.Drawing.Color.White;
            this.lab_Title.Location = new System.Drawing.Point(202, 11);
            this.lab_Title.Name = "lab_Title";
            this.lab_Title.Size = new System.Drawing.Size(189, 27);
            this.lab_Title.TabIndex = 7;
            this.lab_Title.Text = "MES信息收集端";
            // 
            // lab_DateTime
            // 
            this.lab_DateTime.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lab_DateTime.AutoSize = true;
            this.lab_DateTime.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_DateTime.ForeColor = System.Drawing.Color.White;
            this.lab_DateTime.Location = new System.Drawing.Point(498, 15);
            this.lab_DateTime.Name = "lab_DateTime";
            this.lab_DateTime.Size = new System.Drawing.Size(89, 20);
            this.lab_DateTime.TabIndex = 9;
            this.lab_DateTime.Text = "当前时间";
            // 
            // panel4
            // 
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(3, 3);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(93, 44);
            this.panel4.TabIndex = 2;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.btn_CloseWindow);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(894, 3);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(94, 44);
            this.panel5.TabIndex = 3;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel9, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel8, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel6, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel5, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 50);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(991, 662);
            this.tableLayoutPanel2.TabIndex = 13;
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.tableLayoutPanel4);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel8.Location = new System.Drawing.Point(3, 3);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(489, 310);
            this.panel8.TabIndex = 2;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.lab_CurrentStatusTotalTime, 0, 3);
            this.tableLayoutPanel4.Controls.Add(this.lab_ProductCount, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.lab_LastLifeCycleTime, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel7, 0, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 4;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(489, 310);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // lab_CurrentStatusTotalTime
            // 
            this.lab_CurrentStatusTotalTime.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lab_CurrentStatusTotalTime.AutoSize = true;
            this.lab_CurrentStatusTotalTime.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_CurrentStatusTotalTime.ForeColor = System.Drawing.Color.White;
            this.lab_CurrentStatusTotalTime.Location = new System.Drawing.Point(4, 268);
            this.lab_CurrentStatusTotalTime.Name = "lab_CurrentStatusTotalTime";
            this.lab_CurrentStatusTotalTime.Size = new System.Drawing.Size(129, 20);
            this.lab_CurrentStatusTotalTime.TabIndex = 13;
            this.lab_CurrentStatusTotalTime.Text = "状态保持时间";
            // 
            // lab_ProductCount
            // 
            this.lab_ProductCount.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lab_ProductCount.AutoSize = true;
            this.lab_ProductCount.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_ProductCount.ForeColor = System.Drawing.Color.White;
            this.lab_ProductCount.Location = new System.Drawing.Point(4, 144);
            this.lab_ProductCount.Name = "lab_ProductCount";
            this.lab_ProductCount.Size = new System.Drawing.Size(129, 20);
            this.lab_ProductCount.TabIndex = 12;
            this.lab_ProductCount.Text = "累计生产数量";
            // 
            // lab_LastLifeCycleTime
            // 
            this.lab_LastLifeCycleTime.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lab_LastLifeCycleTime.AutoSize = true;
            this.lab_LastLifeCycleTime.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_LastLifeCycleTime.ForeColor = System.Drawing.Color.White;
            this.lab_LastLifeCycleTime.Location = new System.Drawing.Point(4, 206);
            this.lab_LastLifeCycleTime.Name = "lab_LastLifeCycleTime";
            this.lab_LastLifeCycleTime.Size = new System.Drawing.Size(129, 20);
            this.lab_LastLifeCycleTime.TabIndex = 13;
            this.lab_LastLifeCycleTime.Text = "生产周期用时";
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 2;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.Controls.Add(this.btn_StatusLight, 1, 0);
            this.tableLayoutPanel7.Controls.Add(this.tableLayoutPanel8, 0, 0);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(4, 4);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 1;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 116F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(481, 116);
            this.tableLayoutPanel7.TabIndex = 14;
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.ColumnCount = 3;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel8.Controls.Add(this.btn_SignalX03, 2, 0);
            this.tableLayoutPanel8.Controls.Add(this.btn_SignalX02, 1, 0);
            this.tableLayoutPanel8.Controls.Add(this.btn_SignalX01, 0, 0);
            this.tableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel8.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 1;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tableLayoutPanel8.Size = new System.Drawing.Size(234, 110);
            this.tableLayoutPanel8.TabIndex = 1;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel3.ColumnCount = 4;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel3.Controls.Add(this.lab_ReceviedDataCount, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.lab_SendErrorCount, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.lab_SendSuccessCount, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.lab_ReceviedDataErrorCount, 3, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 635);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(489, 24);
            this.tableLayoutPanel3.TabIndex = 4;
            // 
            // lab_ReceviedDataCount
            // 
            this.lab_ReceviedDataCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lab_ReceviedDataCount.AutoSize = true;
            this.lab_ReceviedDataCount.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_ReceviedDataCount.ForeColor = System.Drawing.Color.White;
            this.lab_ReceviedDataCount.Location = new System.Drawing.Point(248, 5);
            this.lab_ReceviedDataCount.Name = "lab_ReceviedDataCount";
            this.lab_ReceviedDataCount.Size = new System.Drawing.Size(115, 14);
            this.lab_ReceviedDataCount.TabIndex = 16;
            this.lab_ReceviedDataCount.Text = "接收成功";
            // 
            // lab_SendErrorCount
            // 
            this.lab_SendErrorCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lab_SendErrorCount.AutoSize = true;
            this.lab_SendErrorCount.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_SendErrorCount.ForeColor = System.Drawing.Color.White;
            this.lab_SendErrorCount.Location = new System.Drawing.Point(126, 5);
            this.lab_SendErrorCount.Name = "lab_SendErrorCount";
            this.lab_SendErrorCount.Size = new System.Drawing.Size(115, 14);
            this.lab_SendErrorCount.TabIndex = 15;
            this.lab_SendErrorCount.Text = "发送失败";
            // 
            // lab_SendSuccessCount
            // 
            this.lab_SendSuccessCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lab_SendSuccessCount.AutoSize = true;
            this.lab_SendSuccessCount.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_SendSuccessCount.ForeColor = System.Drawing.Color.White;
            this.lab_SendSuccessCount.Location = new System.Drawing.Point(4, 5);
            this.lab_SendSuccessCount.Name = "lab_SendSuccessCount";
            this.lab_SendSuccessCount.Size = new System.Drawing.Size(115, 14);
            this.lab_SendSuccessCount.TabIndex = 13;
            this.lab_SendSuccessCount.Text = "发送成功";
            // 
            // lab_ReceviedDataErrorCount
            // 
            this.lab_ReceviedDataErrorCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lab_ReceviedDataErrorCount.AutoSize = true;
            this.lab_ReceviedDataErrorCount.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_ReceviedDataErrorCount.ForeColor = System.Drawing.Color.White;
            this.lab_ReceviedDataErrorCount.Location = new System.Drawing.Point(370, 5);
            this.lab_ReceviedDataErrorCount.Name = "lab_ReceviedDataErrorCount";
            this.lab_ReceviedDataErrorCount.Size = new System.Drawing.Size(115, 14);
            this.lab_ReceviedDataErrorCount.TabIndex = 16;
            this.lab_ReceviedDataErrorCount.Text = "接收失败";
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel6.ColumnCount = 1;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel6.Controls.Add(this.lab_UploadDataServiceStatus, 0, 0);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(498, 635);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 1;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(490, 24);
            this.tableLayoutPanel6.TabIndex = 5;
            // 
            // lab_UploadDataServiceStatus
            // 
            this.lab_UploadDataServiceStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lab_UploadDataServiceStatus.AutoSize = true;
            this.lab_UploadDataServiceStatus.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_UploadDataServiceStatus.ForeColor = System.Drawing.Color.White;
            this.lab_UploadDataServiceStatus.Location = new System.Drawing.Point(4, 5);
            this.lab_UploadDataServiceStatus.Name = "lab_UploadDataServiceStatus";
            this.lab_UploadDataServiceStatus.Size = new System.Drawing.Size(482, 14);
            this.lab_UploadDataServiceStatus.TabIndex = 17;
            this.lab_UploadDataServiceStatus.Text = "正在查找数据上传服务运行状态";
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Controls.Add(this.cartesianChart_temperature, 0, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 319);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(489, 310);
            this.tableLayoutPanel5.TabIndex = 6;
            // 
            // cartesianChart_temperature
            // 
            this.cartesianChart_temperature.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cartesianChart_temperature.Location = new System.Drawing.Point(4, 4);
            this.cartesianChart_temperature.Name = "cartesianChart_temperature";
            this.cartesianChart_temperature.Size = new System.Drawing.Size(481, 302);
            this.cartesianChart_temperature.TabIndex = 0;
            this.cartesianChart_temperature.Text = "cartesianChart1";
            // 
            // btn_CloseWindow
            // 
            this.btn_CloseWindow.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_CloseWindow.BackgroundImage")));
            this.btn_CloseWindow.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_CloseWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_CloseWindow.FlatAppearance.BorderSize = 0;
            this.btn_CloseWindow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_CloseWindow.Location = new System.Drawing.Point(0, 0);
            this.btn_CloseWindow.Name = "btn_CloseWindow";
            this.btn_CloseWindow.Size = new System.Drawing.Size(94, 44);
            this.btn_CloseWindow.TabIndex = 0;
            this.btn_CloseWindow.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btn_CloseWindow.UseVisualStyleBackColor = true;
            this.btn_CloseWindow.Click += new System.EventHandler(this.btn_CloseWindow_Click);
            // 
            // tableLayoutPanel9
            // 
            this.tableLayoutPanel9.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel9.ColumnCount = 1;
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel9.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel9.Controls.Add(this.label3, 0, 3);
            this.tableLayoutPanel9.Controls.Add(this.label1, 0, 2);
            this.tableLayoutPanel9.Controls.Add(this.label4, 0, 1);
            this.tableLayoutPanel9.Controls.Add(this.label5, 0, 4);
            this.tableLayoutPanel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel9.Location = new System.Drawing.Point(498, 3);
            this.tableLayoutPanel9.Name = "tableLayoutPanel9";
            this.tableLayoutPanel9.RowCount = 5;
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel9.Size = new System.Drawing.Size(490, 310);
            this.tableLayoutPanel9.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(4, 143);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(189, 20);
            this.label1.TabIndex = 13;
            this.label1.Text = "预计生产数量：1500";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(4, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(169, 20);
            this.label2.TabIndex = 12;
            this.label2.Text = "工单号码：PO-001";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(4, 204);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(179, 20);
            this.label3.TabIndex = 13;
            this.label3.Text = "实际生产数量：500";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(4, 82);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(209, 20);
            this.label4.TabIndex = 12;
            this.label4.Text = "产品编码：STO-156625";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(4, 267);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(209, 20);
            this.label5.TabIndex = 13;
            this.label5.Text = "生产完成比率：33.33%";
            // 
            // btn_StatusLight
            // 
            this.btn_StatusLight.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btn_StatusLight.FlatAppearance.BorderSize = 0;
            this.btn_StatusLight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_StatusLight.Font = new System.Drawing.Font("宋体", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_StatusLight.Location = new System.Drawing.Point(310, 8);
            this.btn_StatusLight.Name = "btn_StatusLight";
            this.btn_StatusLight.Size = new System.Drawing.Size(100, 100);
            this.btn_StatusLight.TabIndex = 0;
            this.btn_StatusLight.UseVisualStyleBackColor = true;
            this.btn_StatusLight.Click += new System.EventHandler(this.btn_StatusLight_Click);
            // 
            // btn_SignalX03
            // 
            this.btn_SignalX03.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btn_SignalX03.FlatAppearance.BorderSize = 0;
            this.btn_SignalX03.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_SignalX03.Location = new System.Drawing.Point(170, 30);
            this.btn_SignalX03.Name = "btn_SignalX03";
            this.btn_SignalX03.Size = new System.Drawing.Size(50, 50);
            this.btn_SignalX03.TabIndex = 0;
            this.btn_SignalX03.Text = "自动";
            this.btn_SignalX03.UseVisualStyleBackColor = true;
            // 
            // btn_SignalX02
            // 
            this.btn_SignalX02.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btn_SignalX02.FlatAppearance.BorderSize = 0;
            this.btn_SignalX02.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_SignalX02.Location = new System.Drawing.Point(92, 30);
            this.btn_SignalX02.Name = "btn_SignalX02";
            this.btn_SignalX02.Size = new System.Drawing.Size(50, 50);
            this.btn_SignalX02.TabIndex = 0;
            this.btn_SignalX02.Text = "射胶";
            this.btn_SignalX02.UseVisualStyleBackColor = true;
            // 
            // btn_SignalX01
            // 
            this.btn_SignalX01.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btn_SignalX01.FlatAppearance.BorderSize = 0;
            this.btn_SignalX01.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_SignalX01.Location = new System.Drawing.Point(14, 30);
            this.btn_SignalX01.Name = "btn_SignalX01";
            this.btn_SignalX01.Size = new System.Drawing.Size(50, 50);
            this.btn_SignalX01.TabIndex = 0;
            this.btn_SignalX01.Text = "开模";
            this.btn_SignalX01.UseVisualStyleBackColor = true;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.ClientSize = new System.Drawing.Size(991, 712);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMain";
            this.Text = "MES数据收集端";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel9.ResumeLayout(false);
            this.tableLayoutPanel9.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort6;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label lab_CurrentStatusTotalTime;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label lab_ReceviedDataCount;
        private System.Windows.Forms.Label lab_SendErrorCount;
        private System.Windows.Forms.Label lab_SendSuccessCount;
        private System.Windows.Forms.Label lab_ProductCount;
        private System.Windows.Forms.Label lab_LastLifeCycleTime;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private Common.Component.CircularButton circularButton1;
        private Common.Component.CircularButton btn_StatusLight;
        private System.Windows.Forms.Label lab_UploadDataServiceStatus;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Label lab_Title;
        private System.Windows.Forms.Label lab_DateTime;
        private System.Windows.Forms.Label lab_ReceviedDataErrorCount;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
        private Common.Component.CircularButton btn_SignalX03;
        private Common.Component.CircularButton btn_SignalX02;
        private Common.Component.CircularButton btn_SignalX01;
        private LiveCharts.WinForms.CartesianChart cartesianChart_temperature;
        private System.Windows.Forms.Button btn_CloseWindow;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel9;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}

