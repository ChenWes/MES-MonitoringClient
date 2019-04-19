namespace MES_MonitoringClient
{
    partial class frmScanRFID
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmScanRFID));
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lab_CardID = new System.Windows.Forms.Label();
            this.lab_ScanStatus = new System.Windows.Forms.Label();
            this.circularButton1 = new MES_MonitoringClient.Common.Component.CircularButton();
            this.circularButton2 = new MES_MonitoringClient.Common.Component.CircularButton();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // serialPort1
            // 
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.00079F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.9988F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.0008F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.9988F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.0008F));
            this.tableLayoutPanel1.Controls.Add(this.lab_CardID, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.lab_ScanStatus, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.circularButton1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.circularButton2, 4, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.00001F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1048, 685);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // lab_CardID
            // 
            this.lab_CardID.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lab_CardID.AutoSize = true;
            this.lab_CardID.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_CardID.ForeColor = System.Drawing.Color.White;
            this.lab_CardID.Location = new System.Drawing.Point(503, 353);
            this.lab_CardID.Name = "lab_CardID";
            this.lab_CardID.Size = new System.Drawing.Size(39, 20);
            this.lab_CardID.TabIndex = 5;
            this.lab_CardID.Text = "...";
            // 
            // lab_ScanStatus
            // 
            this.lab_ScanStatus.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lab_ScanStatus.AutoSize = true;
            this.lab_ScanStatus.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_ScanStatus.ForeColor = System.Drawing.Color.White;
            this.lab_ScanStatus.Location = new System.Drawing.Point(478, 111);
            this.lab_ScanStatus.Name = "lab_ScanStatus";
            this.lab_ScanStatus.Size = new System.Drawing.Size(89, 20);
            this.lab_ScanStatus.TabIndex = 0;
            this.lab_ScanStatus.Text = "等待刷卡";
            // 
            // circularButton1
            // 
            this.circularButton1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.circularButton1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("circularButton1.BackgroundImage")));
            this.circularButton1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.circularButton1.Location = new System.Drawing.Point(67, 547);
            this.circularButton1.Name = "circularButton1";
            this.circularButton1.Size = new System.Drawing.Size(75, 75);
            this.circularButton1.TabIndex = 7;
            this.circularButton1.UseVisualStyleBackColor = true;
            this.circularButton1.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // circularButton2
            // 
            this.circularButton2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.circularButton2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("circularButton2.BackgroundImage")));
            this.circularButton2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.circularButton2.Location = new System.Drawing.Point(904, 547);
            this.circularButton2.Name = "circularButton2";
            this.circularButton2.Size = new System.Drawing.Size(75, 75);
            this.circularButton2.TabIndex = 7;
            this.circularButton2.UseVisualStyleBackColor = true;
            this.circularButton2.Click += new System.EventHandler(this.btn_Confirm_Click);
            // 
            // frmScanRFID
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.ClientSize = new System.Drawing.Size(1048, 685);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmScanRFID";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RFID";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmScanRFID_FormClosing);
            this.Load += new System.EventHandler(this.frmScanRFID_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lab_ScanStatus;
        private System.Windows.Forms.Label lab_CardID;
        private Common.Component.CircularButton circularButton1;
        private Common.Component.CircularButton circularButton2;
    }
}