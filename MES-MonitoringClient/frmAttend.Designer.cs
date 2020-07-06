namespace MES_MonitoringClient
{
    partial class frmAttend
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAttend));
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.tlp_mian = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_btn = new System.Windows.Forms.TableLayoutPanel();
            this.btn_Cancel = new MES_MonitoringClient.Common.Component.CircularButton();
            this.btn_Confirm = new MES_MonitoringClient.Common.Component.CircularButton();
            this.tlp_view = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lab_DateTime = new System.Windows.Forms.Label();
            this.lab_wait = new System.Windows.Forms.Label();
            this.lab_type = new System.Windows.Forms.Label();
            this.tlp_imagelist = new System.Windows.Forms.TableLayoutPanel();
            this.tlp_employee = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.tlp_manage = new System.Windows.Forms.TableLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lab_title = new System.Windows.Forms.Label();
            this.tlp_mian.SuspendLayout();
            this.tlp_btn.SuspendLayout();
            this.tlp_view.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tlp_imagelist.SuspendLayout();
            this.tlp_employee.SuspendLayout();
            this.tlp_manage.SuspendLayout();
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
            this.tlp_mian.Controls.Add(this.tlp_btn, 0, 2);
            this.tlp_mian.Controls.Add(this.tlp_view, 0, 1);
            this.tlp_mian.Controls.Add(this.lab_title, 0, 0);
            this.tlp_mian.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_mian.Location = new System.Drawing.Point(0, 0);
            this.tlp_mian.Name = "tlp_mian";
            this.tlp_mian.RowCount = 3;
            this.tlp_mian.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlp_mian.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_mian.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 150F));
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
            this.tlp_btn.Location = new System.Drawing.Point(3, 538);
            this.tlp_btn.Name = "tlp_btn";
            this.tlp_btn.RowCount = 1;
            this.tlp_btn.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tlp_btn.Size = new System.Drawing.Size(1042, 144);
            this.tlp_btn.TabIndex = 1;
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btn_Cancel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_Cancel.BackgroundImage")));
            this.btn_Cancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_Cancel.Location = new System.Drawing.Point(66, 37);
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
            this.btn_Confirm.Location = new System.Drawing.Point(899, 37);
            this.btn_Confirm.Name = "btn_Confirm";
            this.btn_Confirm.Size = new System.Drawing.Size(75, 75);
            this.btn_Confirm.TabIndex = 7;
            this.btn_Confirm.UseVisualStyleBackColor = true;
            this.btn_Confirm.Click += new System.EventHandler(this.btn_Confirm_Click);
            // 
            // tlp_view
            // 
            this.tlp_view.ColumnCount = 2;
            this.tlp_view.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.tlp_view.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlp_view.Controls.Add(this.tableLayoutPanel1, 1, 0);
            this.tlp_view.Controls.Add(this.tlp_imagelist, 0, 0);
            this.tlp_view.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_view.Location = new System.Drawing.Point(3, 103);
            this.tlp_view.Name = "tlp_view";
            this.tlp_view.RowCount = 1;
            this.tlp_view.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_view.Size = new System.Drawing.Size(1042, 429);
            this.tlp_view.TabIndex = 2;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.lab_DateTime, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lab_wait, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.lab_type, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(732, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(307, 423);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // lab_DateTime
            // 
            this.lab_DateTime.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lab_DateTime.AutoSize = true;
            this.lab_DateTime.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_DateTime.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lab_DateTime.Location = new System.Drawing.Point(115, 9);
            this.lab_DateTime.Name = "lab_DateTime";
            this.lab_DateTime.Size = new System.Drawing.Size(76, 21);
            this.lab_DateTime.TabIndex = 0;
            this.lab_DateTime.Text = "label1";
            // 
            // lab_wait
            // 
            this.lab_wait.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lab_wait.AutoSize = true;
            this.lab_wait.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_wait.ForeColor = System.Drawing.Color.Chartreuse;
            this.lab_wait.Location = new System.Drawing.Point(89, 216);
            this.lab_wait.Name = "lab_wait";
            this.lab_wait.Size = new System.Drawing.Size(129, 29);
            this.lab_wait.TabIndex = 1;
            this.lab_wait.Text = "等待刷卡";
            // 
            // lab_type
            // 
            this.lab_type.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lab_type.AutoSize = true;
            this.lab_type.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_type.ForeColor = System.Drawing.Color.LawnGreen;
            this.lab_type.Location = new System.Drawing.Point(153, 73);
            this.lab_type.Name = "lab_type";
            this.lab_type.Size = new System.Drawing.Size(0, 29);
            this.lab_type.TabIndex = 2;
            // 
            // tlp_imagelist
            // 
            this.tlp_imagelist.ColumnCount = 1;
            this.tlp_imagelist.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_imagelist.Controls.Add(this.tlp_employee, 0, 1);
            this.tlp_imagelist.Controls.Add(this.tlp_manage, 0, 0);
            this.tlp_imagelist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_imagelist.Location = new System.Drawing.Point(3, 3);
            this.tlp_imagelist.Name = "tlp_imagelist";
            this.tlp_imagelist.RowCount = 2;
            this.tlp_imagelist.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_imagelist.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_imagelist.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlp_imagelist.Size = new System.Drawing.Size(723, 423);
            this.tlp_imagelist.TabIndex = 1;
            // 
            // tlp_employee
            // 
            this.tlp_employee.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tlp_employee.ColumnCount = 1;
            this.tlp_employee.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_employee.Controls.Add(this.label2, 0, 0);
            this.tlp_employee.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_employee.Location = new System.Drawing.Point(3, 214);
            this.tlp_employee.Name = "tlp_employee";
            this.tlp_employee.RowCount = 2;
            this.tlp_employee.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlp_employee.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_employee.Size = new System.Drawing.Size(717, 206);
            this.tlp_employee.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.Chartreuse;
            this.label2.Location = new System.Drawing.Point(338, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "员工";
            // 
            // tlp_manage
            // 
            this.tlp_manage.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tlp_manage.ColumnCount = 3;
            this.tlp_manage.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33332F));
            this.tlp_manage.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tlp_manage.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tlp_manage.Controls.Add(this.label5, 2, 0);
            this.tlp_manage.Controls.Add(this.label4, 1, 0);
            this.tlp_manage.Controls.Add(this.label3, 0, 0);
            this.tlp_manage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_manage.Location = new System.Drawing.Point(3, 3);
            this.tlp_manage.Name = "tlp_manage";
            this.tlp_manage.RowCount = 2;
            this.tlp_manage.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlp_manage.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_manage.Size = new System.Drawing.Size(717, 205);
            this.tlp_manage.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.ForeColor = System.Drawing.Color.Chartreuse;
            this.label5.Location = new System.Drawing.Point(576, 8);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 16);
            this.label5.TabIndex = 2;
            this.label5.Text = "校验";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.Color.Chartreuse;
            this.label4.Location = new System.Drawing.Point(337, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 16);
            this.label4.TabIndex = 1;
            this.label4.Text = "组长";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.Color.Chartreuse;
            this.label3.Location = new System.Drawing.Point(99, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 16);
            this.label3.TabIndex = 0;
            this.label3.Text = "领班";
            // 
            // lab_title
            // 
            this.lab_title.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lab_title.AutoSize = true;
            this.lab_title.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_title.ForeColor = System.Drawing.Color.White;
            this.lab_title.Location = new System.Drawing.Point(488, 35);
            this.lab_title.Name = "lab_title";
            this.lab_title.Size = new System.Drawing.Size(71, 29);
            this.lab_title.TabIndex = 3;
            this.lab_title.Text = "打卡";
            // 
            // frmAttend
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(45)))), ((int)(((byte)(58)))));
            this.ClientSize = new System.Drawing.Size(1048, 685);
            this.Controls.Add(this.tlp_mian);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmAttend";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RFID";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmAttend_FormClosing);
            this.Load += new System.EventHandler(this.frmAttend_Load);
            this.tlp_mian.ResumeLayout(false);
            this.tlp_mian.PerformLayout();
            this.tlp_btn.ResumeLayout(false);
            this.tlp_view.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tlp_imagelist.ResumeLayout(false);
            this.tlp_employee.ResumeLayout(false);
            this.tlp_employee.PerformLayout();
            this.tlp_manage.ResumeLayout(false);
            this.tlp_manage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.TableLayoutPanel tlp_mian;
        private System.Windows.Forms.TableLayoutPanel tlp_btn;
        private Common.Component.CircularButton btn_Cancel;
        private Common.Component.CircularButton btn_Confirm;
        private System.Windows.Forms.TableLayoutPanel tlp_view;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tlp_imagelist;
        private System.Windows.Forms.Label lab_title;
        private System.Windows.Forms.TableLayoutPanel tlp_manage;
        private System.Windows.Forms.TableLayoutPanel tlp_employee;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lab_DateTime;
        private System.Windows.Forms.Label lab_wait;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lab_type;
    }
}