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
            this.serialPort7 = new System.IO.Ports.SerialPort(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lab_DateTime = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lab_Title = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.btn_CloseWindow = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.btn_StatusLight = new MES_MonitoringClient.Common.Component.CircularButton();
            this.lab_CurrentStatusTotalTime = new System.Windows.Forms.Label();
            this.lab_ProductCount = new System.Windows.Forms.Label();
            this.lab_LastLifeCycleTime = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.lab_ReceviedDataCount = new System.Windows.Forms.Label();
            this.lab_SendErrorCount = new System.Windows.Forms.Label();
            this.lab_SendSuccessCount = new System.Windows.Forms.Label();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.lab_ConnectionToServer = new System.Windows.Forms.Label();
            this.lab_UploadStatus = new System.Windows.Forms.Label();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel5.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel8.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // serialPort7
            // 
            this.serialPort7.BaudRate = 115200;
            this.serialPort7.PortName = "COM7";
            this.serialPort7.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort7_DataReceived);
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
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 41.66667F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 41.66667F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333333F));
            this.tableLayoutPanel1.Controls.Add(this.panel2, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 1, 0);
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
            // panel2
            // 
            this.panel2.Controls.Add(this.lab_DateTime);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(497, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(406, 44);
            this.panel2.TabIndex = 0;
            // 
            // lab_DateTime
            // 
            this.lab_DateTime.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lab_DateTime.AutoSize = true;
            this.lab_DateTime.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_DateTime.ForeColor = System.Drawing.Color.White;
            this.lab_DateTime.Location = new System.Drawing.Point(124, 10);
            this.lab_DateTime.Name = "lab_DateTime";
            this.lab_DateTime.Size = new System.Drawing.Size(89, 20);
            this.lab_DateTime.TabIndex = 0;
            this.lab_DateTime.Text = "当前时间";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.lab_Title);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(85, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(406, 44);
            this.panel3.TabIndex = 1;
            // 
            // lab_Title
            // 
            this.lab_Title.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lab_Title.AutoSize = true;
            this.lab_Title.Font = new System.Drawing.Font("宋体", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_Title.ForeColor = System.Drawing.Color.White;
            this.lab_Title.Location = new System.Drawing.Point(120, 10);
            this.lab_Title.Name = "lab_Title";
            this.lab_Title.Size = new System.Drawing.Size(189, 27);
            this.lab_Title.TabIndex = 0;
            this.lab_Title.Text = "MES信息收集端";
            // 
            // panel4
            // 
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(3, 3);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(76, 44);
            this.panel4.TabIndex = 2;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.btn_CloseWindow);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(909, 3);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(79, 44);
            this.panel5.TabIndex = 3;
            // 
            // btn_CloseWindow
            // 
            this.btn_CloseWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_CloseWindow.Location = new System.Drawing.Point(0, 0);
            this.btn_CloseWindow.Name = "btn_CloseWindow";
            this.btn_CloseWindow.Size = new System.Drawing.Size(79, 44);
            this.btn_CloseWindow.TabIndex = 0;
            this.btn_CloseWindow.Text = "退出系统";
            this.btn_CloseWindow.UseVisualStyleBackColor = true;
            this.btn_CloseWindow.Click += new System.EventHandler(this.btn_CloseWindow_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
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
            this.tableLayoutPanel4.Controls.Add(this.btn_StatusLight, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.lab_CurrentStatusTotalTime, 0, 3);
            this.tableLayoutPanel4.Controls.Add(this.lab_ProductCount, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.lab_LastLifeCycleTime, 0, 2);
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
            // btn_StatusLight
            // 
            this.btn_StatusLight.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btn_StatusLight.FlatAppearance.BorderSize = 0;
            this.btn_StatusLight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_StatusLight.Location = new System.Drawing.Point(194, 12);
            this.btn_StatusLight.Name = "btn_StatusLight";
            this.btn_StatusLight.Size = new System.Drawing.Size(100, 100);
            this.btn_StatusLight.TabIndex = 0;
            this.btn_StatusLight.UseVisualStyleBackColor = true;
            this.btn_StatusLight.Click += new System.EventHandler(this.btn_StatusLight_Click);
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
            this.lab_LastLifeCycleTime.Size = new System.Drawing.Size(169, 20);
            this.lab_LastLifeCycleTime.TabIndex = 13;
            this.lab_LastLifeCycleTime.Text = "最后一次生产用时";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.Controls.Add(this.lab_ReceviedDataCount, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.lab_SendErrorCount, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.lab_SendSuccessCount, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 635);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(489, 24);
            this.tableLayoutPanel3.TabIndex = 4;
            // 
            // lab_ReceviedDataCount
            // 
            this.lab_ReceviedDataCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lab_ReceviedDataCount.AutoSize = true;
            this.lab_ReceviedDataCount.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_ReceviedDataCount.ForeColor = System.Drawing.Color.White;
            this.lab_ReceviedDataCount.Location = new System.Drawing.Point(328, 2);
            this.lab_ReceviedDataCount.Name = "lab_ReceviedDataCount";
            this.lab_ReceviedDataCount.Size = new System.Drawing.Size(157, 20);
            this.lab_ReceviedDataCount.TabIndex = 16;
            this.lab_ReceviedDataCount.Text = "接收成功";
            // 
            // lab_SendErrorCount
            // 
            this.lab_SendErrorCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lab_SendErrorCount.AutoSize = true;
            this.lab_SendErrorCount.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_SendErrorCount.ForeColor = System.Drawing.Color.White;
            this.lab_SendErrorCount.Location = new System.Drawing.Point(166, 2);
            this.lab_SendErrorCount.Name = "lab_SendErrorCount";
            this.lab_SendErrorCount.Size = new System.Drawing.Size(155, 20);
            this.lab_SendErrorCount.TabIndex = 15;
            this.lab_SendErrorCount.Text = "发送失败";
            // 
            // lab_SendSuccessCount
            // 
            this.lab_SendSuccessCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lab_SendSuccessCount.AutoSize = true;
            this.lab_SendSuccessCount.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_SendSuccessCount.ForeColor = System.Drawing.Color.White;
            this.lab_SendSuccessCount.Location = new System.Drawing.Point(4, 2);
            this.lab_SendSuccessCount.Name = "lab_SendSuccessCount";
            this.lab_SendSuccessCount.Size = new System.Drawing.Size(155, 20);
            this.lab_SendSuccessCount.TabIndex = 13;
            this.lab_SendSuccessCount.Text = "发送成功";
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel6.ColumnCount = 2;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.Controls.Add(this.lab_ConnectionToServer, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.lab_UploadStatus, 1, 0);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(498, 635);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 1;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(490, 24);
            this.tableLayoutPanel6.TabIndex = 5;
            // 
            // lab_ConnectionToServer
            // 
            this.lab_ConnectionToServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lab_ConnectionToServer.AutoSize = true;
            this.lab_ConnectionToServer.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_ConnectionToServer.ForeColor = System.Drawing.Color.White;
            this.lab_ConnectionToServer.Location = new System.Drawing.Point(4, 2);
            this.lab_ConnectionToServer.Name = "lab_ConnectionToServer";
            this.lab_ConnectionToServer.Size = new System.Drawing.Size(237, 20);
            this.lab_ConnectionToServer.TabIndex = 17;
            this.lab_ConnectionToServer.Text = "正在尝试连接至服务器";
            // 
            // lab_UploadStatus
            // 
            this.lab_UploadStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lab_UploadStatus.AutoSize = true;
            this.lab_UploadStatus.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_UploadStatus.ForeColor = System.Drawing.Color.White;
            this.lab_UploadStatus.Location = new System.Drawing.Point(248, 2);
            this.lab_UploadStatus.Name = "lab_UploadStatus";
            this.lab_UploadStatus.Size = new System.Drawing.Size(238, 20);
            this.lab_UploadStatus.TabIndex = 17;
            this.lab_UploadStatus.Text = "数据上传状态";
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Controls.Add(this.richTextBox1, 0, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 319);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(489, 310);
            this.tableLayoutPanel5.TabIndex = 6;
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.ForeColor = System.Drawing.Color.White;
            this.richTextBox1.Location = new System.Drawing.Point(4, 4);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(481, 302);
            this.richTextBox1.TabIndex = 14;
            this.richTextBox1.Text = "";
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
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort7;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lab_DateTime;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label lab_Title;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label lab_CurrentStatusTotalTime;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Button btn_CloseWindow;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label lab_ReceviedDataCount;
        private System.Windows.Forms.Label lab_SendErrorCount;
        private System.Windows.Forms.Label lab_SendSuccessCount;
        private System.Windows.Forms.Label lab_ProductCount;
        private System.Windows.Forms.Label lab_LastLifeCycleTime;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private Common.Component.CircularButton circularButton1;
        private Common.Component.CircularButton btn_StatusLight;
        private System.Windows.Forms.Label lab_ConnectionToServer;
        private System.Windows.Forms.Label lab_UploadStatus;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.RichTextBox richTextBox1;
    }
}

