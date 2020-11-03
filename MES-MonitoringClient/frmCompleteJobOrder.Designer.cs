namespace MES_MonitoringClient
{
    partial class frmCompleteJobOrder
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCompleteJobOrder));
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.tlp_mian = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_btn = new System.Windows.Forms.TableLayoutPanel();
            this.btn_Cancel = new MES_MonitoringClient.Common.Component.CircularButton();
            this.btn_Confirm = new MES_MonitoringClient.Common.Component.CircularButton();
            this.lab_ScanStatus = new System.Windows.Forms.Label();
            this.lab_CardID = new System.Windows.Forms.Label();
            this.lab_select = new System.Windows.Forms.Label();
            this.tlp_mian.SuspendLayout();
            this.tlp_btn.SuspendLayout();
            this.SuspendLayout();
            // 
            // serialPort1
            // 
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // tlp_mian
            // 
            this.tlp_mian.ColumnCount = 1;
            this.tlp_mian.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_mian.Controls.Add(this.tlp_btn, 0, 4);
            this.tlp_mian.Controls.Add(this.lab_ScanStatus, 0, 0);
            this.tlp_mian.Controls.Add(this.lab_CardID, 0, 1);
            this.tlp_mian.Controls.Add(this.lab_select, 0, 2);
            this.tlp_mian.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_mian.Location = new System.Drawing.Point(0, 0);
            this.tlp_mian.Name = "tlp_mian";
            this.tlp_mian.RowCount = 5;
            this.tlp_mian.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tlp_mian.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tlp_mian.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tlp_mian.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tlp_mian.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tlp_mian.Size = new System.Drawing.Size(1048, 685);
            this.tlp_mian.TabIndex = 1;
            // 
            // tlp_btn
            // 
            this.tlp_btn.ColumnCount = 5;
            this.tlp_btn.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.00079F));
            this.tlp_btn.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.9988F));
            this.tlp_btn.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.0008F));
            this.tlp_btn.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.9988F));
            this.tlp_btn.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.0008F));
            this.tlp_btn.Controls.Add(this.btn_Cancel, 0, 0);
            this.tlp_btn.Controls.Add(this.btn_Confirm, 4, 0);
            this.tlp_btn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_btn.Location = new System.Drawing.Point(3, 486);
            this.tlp_btn.Name = "tlp_btn";
            this.tlp_btn.RowCount = 1;
            this.tlp_btn.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 195F));
            this.tlp_btn.Size = new System.Drawing.Size(1042, 196);
            this.tlp_btn.TabIndex = 1;
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btn_Cancel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_Cancel.BackgroundImage")));
            this.btn_Cancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_Cancel.Location = new System.Drawing.Point(66, 60);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(75, 75);
            this.btn_Cancel.TabIndex = 7;
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_Confirm
            // 
            this.btn_Confirm.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btn_Confirm.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_Confirm.BackgroundImage")));
            this.btn_Confirm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_Confirm.Location = new System.Drawing.Point(899, 60);
            this.btn_Confirm.Name = "btn_Confirm";
            this.btn_Confirm.Size = new System.Drawing.Size(75, 75);
            this.btn_Confirm.TabIndex = 7;
            this.btn_Confirm.UseVisualStyleBackColor = true;
            this.btn_Confirm.Click += new System.EventHandler(this.btn_Confirm_Click);
            // 
            // lab_ScanStatus
            // 
            this.lab_ScanStatus.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lab_ScanStatus.AutoSize = true;
            this.lab_ScanStatus.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_ScanStatus.ForeColor = System.Drawing.Color.Transparent;
            this.lab_ScanStatus.Location = new System.Drawing.Point(339, 60);
            this.lab_ScanStatus.Name = "lab_ScanStatus";
            this.lab_ScanStatus.Size = new System.Drawing.Size(370, 24);
            this.lab_ScanStatus.TabIndex = 4;
            this.lab_ScanStatus.Text = "即将完成工单，请选择工单后刷卡";
            // 
            // lab_CardID
            // 
            this.lab_CardID.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lab_CardID.AutoSize = true;
            this.lab_CardID.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_CardID.ForeColor = System.Drawing.Color.Transparent;
            this.lab_CardID.Location = new System.Drawing.Point(501, 145);
            this.lab_CardID.Name = "label2";
            this.lab_CardID.Size = new System.Drawing.Size(46, 24);
            this.lab_CardID.TabIndex = 5;
            this.lab_CardID.Text = "...";
            // 
            // lab_select
            // 
            this.lab_select.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lab_select.AutoSize = true;
            this.lab_select.Font = new System.Drawing.Font("宋体", 18F);
            this.lab_select.ForeColor = System.Drawing.Color.Yellow;
            this.lab_select.Location = new System.Drawing.Point(399, 229);
            this.lab_select.Name = "lab_select";
            this.lab_select.Size = new System.Drawing.Size(250, 24);
            this.lab_select.TabIndex = 6;
            this.lab_select.Text = "请选择需要完成的工单";
            // 
            // frmCompleteJobOrder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.ClientSize = new System.Drawing.Size(1048, 685);
            this.Controls.Add(this.tlp_mian);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmCompleteJobOrder";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RFID";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmCompleteJobOrder_FormClosing);
            this.Load += new System.EventHandler(this.frmCompleteJobOrder_Load);
            this.tlp_mian.ResumeLayout(false);
            this.tlp_mian.PerformLayout();
            this.tlp_btn.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.TableLayoutPanel tlp_mian;
        private System.Windows.Forms.TableLayoutPanel tlp_btn;
        private Common.Component.CircularButton btn_Cancel;
        private Common.Component.CircularButton btn_Confirm;
        private System.Windows.Forms.Label lab_ScanStatus;
        private System.Windows.Forms.Label lab_CardID;
        private System.Windows.Forms.Label lab_select;
    }
}