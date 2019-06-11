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
            this.lab_SelectStatus = new System.Windows.Forms.Label();
            this.btn_Confirm = new MES_MonitoringClient.Common.Component.CircularButton();
            this.btn_Cancel = new MES_MonitoringClient.Common.Component.CircularButton();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Controls.Add(this.lab_SelectStatus, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btn_Confirm, 4, 4);
            this.tableLayoutPanel1.Controls.Add(this.btn_Cancel, 0, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(936, 703);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // lab_SelectStatus
            // 
            this.lab_SelectStatus.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lab_SelectStatus.AutoSize = true;
            this.lab_SelectStatus.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_SelectStatus.ForeColor = System.Drawing.Color.White;
            this.lab_SelectStatus.Location = new System.Drawing.Point(29, 40);
            this.lab_SelectStatus.Name = "lab_SelectStatus";
            this.lab_SelectStatus.Size = new System.Drawing.Size(129, 20);
            this.lab_SelectStatus.TabIndex = 15;
            this.lab_SelectStatus.Text = "已选机器状态";
            // 
            // btn_Confirm
            // 
            this.btn_Confirm.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btn_Confirm.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_Confirm.BackgroundImage")));
            this.btn_Confirm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_Confirm.Location = new System.Drawing.Point(804, 565);
            this.btn_Confirm.Name = "btn_Confirm";
            this.btn_Confirm.Size = new System.Drawing.Size(75, 75);
            this.btn_Confirm.TabIndex = 14;
            this.btn_Confirm.UseVisualStyleBackColor = true;
            this.btn_Confirm.Click += new System.EventHandler(this.btn_Confirm_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btn_Cancel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_Cancel.BackgroundImage")));
            this.btn_Cancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_Cancel.Location = new System.Drawing.Point(56, 565);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(75, 75);
            this.btn_Cancel.TabIndex = 14;
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
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
        private Common.Component.CircularButton btn_Cancel;
        private Common.Component.CircularButton btn_Confirm;
        private System.Windows.Forms.Label lab_SelectStatus;
    }
}