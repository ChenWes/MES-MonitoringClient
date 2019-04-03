namespace MES_MonitoringClient
{
    partial class frmChangeStatus
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmChangeStatus));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lab_OperatePersonCardID = new System.Windows.Forms.Label();
            this.btn_Stop = new MES_MonitoringClient.Common.Component.CircularButton();
            this.btn_Run = new MES_MonitoringClient.Common.Component.CircularButton();
            this.btn_Error = new MES_MonitoringClient.Common.Component.CircularButton();
            this.lab_SelectStatus = new System.Windows.Forms.Label();
            this.circularButton1 = new MES_MonitoringClient.Common.Component.CircularButton();
            this.circularButton2 = new MES_MonitoringClient.Common.Component.CircularButton();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33332F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.Controls.Add(this.lab_OperatePersonCardID, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.btn_Stop, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btn_Run, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.btn_Error, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.lab_SelectStatus, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.circularButton1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.circularButton2, 2, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(936, 703);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // lab_OperatePersonCardID
            // 
            this.lab_OperatePersonCardID.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lab_OperatePersonCardID.AutoSize = true;
            this.lab_OperatePersonCardID.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_OperatePersonCardID.ForeColor = System.Drawing.Color.White;
            this.lab_OperatePersonCardID.Location = new System.Drawing.Point(745, 40);
            this.lab_OperatePersonCardID.Name = "lab_OperatePersonCardID";
            this.lab_OperatePersonCardID.Size = new System.Drawing.Size(69, 20);
            this.lab_OperatePersonCardID.TabIndex = 4;
            this.lab_OperatePersonCardID.Text = "未刷卡";
            // 
            // btn_Stop
            // 
            this.btn_Stop.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btn_Stop.FlatAppearance.BorderSize = 0;
            this.btn_Stop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Stop.Location = new System.Drawing.Point(105, 251);
            this.btn_Stop.Name = "btn_Stop";
            this.btn_Stop.Size = new System.Drawing.Size(100, 100);
            this.btn_Stop.TabIndex = 11;
            this.btn_Stop.Text = "停机";
            this.btn_Stop.UseVisualStyleBackColor = true;
            this.btn_Stop.Click += new System.EventHandler(this.btn_Stop_Click);
            // 
            // btn_Run
            // 
            this.btn_Run.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btn_Run.FlatAppearance.BorderSize = 0;
            this.btn_Run.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Run.Location = new System.Drawing.Point(417, 251);
            this.btn_Run.Name = "btn_Run";
            this.btn_Run.Size = new System.Drawing.Size(100, 100);
            this.btn_Run.TabIndex = 12;
            this.btn_Run.Text = "运行";
            this.btn_Run.UseVisualStyleBackColor = true;
            this.btn_Run.Click += new System.EventHandler(this.btn_Run_Click);
            // 
            // btn_Error
            // 
            this.btn_Error.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btn_Error.FlatAppearance.BorderSize = 0;
            this.btn_Error.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Error.Location = new System.Drawing.Point(729, 251);
            this.btn_Error.Name = "btn_Error";
            this.btn_Error.Size = new System.Drawing.Size(100, 100);
            this.btn_Error.TabIndex = 13;
            this.btn_Error.Text = "故障";
            this.btn_Error.UseVisualStyleBackColor = true;
            this.btn_Error.Click += new System.EventHandler(this.btn_Error_Click);
            // 
            // lab_SelectStatus
            // 
            this.lab_SelectStatus.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lab_SelectStatus.AutoSize = true;
            this.lab_SelectStatus.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_SelectStatus.ForeColor = System.Drawing.Color.White;
            this.lab_SelectStatus.Location = new System.Drawing.Point(101, 40);
            this.lab_SelectStatus.Name = "lab_SelectStatus";
            this.lab_SelectStatus.Size = new System.Drawing.Size(109, 20);
            this.lab_SelectStatus.TabIndex = 4;
            this.lab_SelectStatus.Text = "未选择状态";
            // 
            // circularButton1
            // 
            this.circularButton1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.circularButton1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("circularButton1.BackgroundImage")));
            this.circularButton1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.circularButton1.Location = new System.Drawing.Point(118, 565);
            this.circularButton1.Name = "circularButton1";
            this.circularButton1.Size = new System.Drawing.Size(75, 75);
            this.circularButton1.TabIndex = 14;
            this.circularButton1.UseVisualStyleBackColor = true;
            this.circularButton1.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // circularButton2
            // 
            this.circularButton2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.circularButton2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("circularButton2.BackgroundImage")));
            this.circularButton2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.circularButton2.Location = new System.Drawing.Point(742, 565);
            this.circularButton2.Name = "circularButton2";
            this.circularButton2.Size = new System.Drawing.Size(75, 75);
            this.circularButton2.TabIndex = 14;
            this.circularButton2.UseVisualStyleBackColor = true;
            this.circularButton2.Click += new System.EventHandler(this.btn_Confirm_Click);
            // 
            // frmChangeStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.ClientSize = new System.Drawing.Size(936, 703);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmChangeStatus";
            this.Text = "选择机器状态";
            this.Load += new System.EventHandler(this.frmChangeStatus_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lab_OperatePersonCardID;
        private Common.Component.CircularButton btn_Stop;
        private Common.Component.CircularButton btn_Run;
        private Common.Component.CircularButton btn_Error;
        private System.Windows.Forms.Label lab_SelectStatus;
        private Common.Component.CircularButton circularButton1;
        private Common.Component.CircularButton circularButton2;
    }
}