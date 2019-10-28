namespace Mes_Update
{
    partial class frmUpdate
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
            this.lab_downloadPath = new System.Windows.Forms.Label();
            this.txt_Download = new System.Windows.Forms.TextBox();
            this.lab_savePath = new System.Windows.Forms.Label();
            this.txt_SavePath = new System.Windows.Forms.TextBox();
            this.btn_Download = new System.Windows.Forms.Button();
            this.lab_downloadRate = new System.Windows.Forms.Label();
            this.progressBar_Download = new System.Windows.Forms.ProgressBar();
            this.buttonInstall = new System.Windows.Forms.Button();
            this.lab_out = new System.Windows.Forms.Label();
            this.lab_ServiceName = new System.Windows.Forms.Label();
            this.lab_Version = new System.Windows.Forms.Label();
            this.lab_Name = new System.Windows.Forms.Label();
            this.lab_Desc = new System.Windows.Forms.Label();
            this.lab_Remark = new System.Windows.Forms.Label();
            this.lab_CreatAt = new System.Windows.Forms.Label();
            this.lab_UpdateAt = new System.Windows.Forms.Label();
            this.lab_path = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lab_downloadPath
            // 
            this.lab_downloadPath.AutoSize = true;
            this.lab_downloadPath.Location = new System.Drawing.Point(13, 13);
            this.lab_downloadPath.Name = "lab_downloadPath";
            this.lab_downloadPath.Size = new System.Drawing.Size(53, 12);
            this.lab_downloadPath.TabIndex = 0;
            this.lab_downloadPath.Text = "下载链接";
            // 
            // txt_Download
            // 
            this.txt_Download.Location = new System.Drawing.Point(86, 10);
            this.txt_Download.Name = "txt_Download";
            this.txt_Download.Size = new System.Drawing.Size(419, 21);
            this.txt_Download.TabIndex = 1;
            // 
            // lab_savePath
            // 
            this.lab_savePath.AutoSize = true;
            this.lab_savePath.Location = new System.Drawing.Point(12, 220);
            this.lab_savePath.Name = "lab_savePath";
            this.lab_savePath.Size = new System.Drawing.Size(53, 12);
            this.lab_savePath.TabIndex = 2;
            this.lab_savePath.Text = "存储路径";
            // 
            // txt_SavePath
            // 
            this.txt_SavePath.Location = new System.Drawing.Point(71, 217);
            this.txt_SavePath.Name = "txt_SavePath";
            this.txt_SavePath.Size = new System.Drawing.Size(419, 21);
            this.txt_SavePath.TabIndex = 3;
            this.txt_SavePath.Text = "C:\\Program Files\\MES-Install-Pack\\MES-MonitoringClient-Setup.exe";
            // 
            // btn_Download
            // 
            this.btn_Download.Location = new System.Drawing.Point(563, 29);
            this.btn_Download.Name = "btn_Download";
            this.btn_Download.Size = new System.Drawing.Size(75, 43);
            this.btn_Download.TabIndex = 4;
            this.btn_Download.Text = "下载";
            this.btn_Download.UseVisualStyleBackColor = true;
            this.btn_Download.Click += new System.EventHandler(this.btn_Download_Click);
            // 
            // lab_downloadRate
            // 
            this.lab_downloadRate.AutoSize = true;
            this.lab_downloadRate.Location = new System.Drawing.Point(12, 300);
            this.lab_downloadRate.Name = "lab_downloadRate";
            this.lab_downloadRate.Size = new System.Drawing.Size(53, 12);
            this.lab_downloadRate.TabIndex = 5;
            this.lab_downloadRate.Text = "下载进度";
            // 
            // progressBar_Download
            // 
            this.progressBar_Download.Location = new System.Drawing.Point(71, 289);
            this.progressBar_Download.Name = "progressBar_Download";
            this.progressBar_Download.Size = new System.Drawing.Size(552, 23);
            this.progressBar_Download.TabIndex = 6;
            // 
            // buttonInstall
            // 
            this.buttonInstall.Location = new System.Drawing.Point(86, 360);
            this.buttonInstall.Name = "buttonInstall";
            this.buttonInstall.Size = new System.Drawing.Size(75, 23);
            this.buttonInstall.TabIndex = 7;
            this.buttonInstall.Text = "开始安装";
            this.buttonInstall.UseVisualStyleBackColor = true;
            this.buttonInstall.Click += new System.EventHandler(this.buttonInstall_Click);
            // 
            // lab_out
            // 
            this.lab_out.AutoSize = true;
            this.lab_out.Location = new System.Drawing.Point(84, 327);
            this.lab_out.Name = "lab_out";
            this.lab_out.Size = new System.Drawing.Size(53, 12);
            this.lab_out.TabIndex = 8;
            this.lab_out.Text = "等待安装";
            // 
            // lab_ServiceName
            // 
            this.lab_ServiceName.AutoSize = true;
            this.lab_ServiceName.Location = new System.Drawing.Point(13, 86);
            this.lab_ServiceName.Name = "lab_ServiceName";
            this.lab_ServiceName.Size = new System.Drawing.Size(0, 12);
            this.lab_ServiceName.TabIndex = 9;
            // 
            // lab_Version
            // 
            this.lab_Version.AutoSize = true;
            this.lab_Version.Location = new System.Drawing.Point(13, 44);
            this.lab_Version.Name = "lab_Version";
            this.lab_Version.Size = new System.Drawing.Size(71, 12);
            this.lab_Version.TabIndex = 10;
            this.lab_Version.Text = "lab_Version";
            // 
            // lab_Name
            // 
            this.lab_Name.AutoSize = true;
            this.lab_Name.Location = new System.Drawing.Point(13, 74);
            this.lab_Name.Name = "lab_Name";
            this.lab_Name.Size = new System.Drawing.Size(53, 12);
            this.lab_Name.TabIndex = 11;
            this.lab_Name.Text = "lab_Name";
            // 
            // lab_Desc
            // 
            this.lab_Desc.AutoSize = true;
            this.lab_Desc.Location = new System.Drawing.Point(12, 98);
            this.lab_Desc.Name = "lab_Desc";
            this.lab_Desc.Size = new System.Drawing.Size(53, 12);
            this.lab_Desc.TabIndex = 12;
            this.lab_Desc.Text = "lab_Desc";
            // 
            // lab_Remark
            // 
            this.lab_Remark.AutoSize = true;
            this.lab_Remark.Location = new System.Drawing.Point(13, 125);
            this.lab_Remark.Name = "lab_Remark";
            this.lab_Remark.Size = new System.Drawing.Size(65, 12);
            this.lab_Remark.TabIndex = 13;
            this.lab_Remark.Text = "lab_Remark";
            // 
            // lab_CreatAt
            // 
            this.lab_CreatAt.AutoSize = true;
            this.lab_CreatAt.Location = new System.Drawing.Point(12, 151);
            this.lab_CreatAt.Name = "lab_CreatAt";
            this.lab_CreatAt.Size = new System.Drawing.Size(71, 12);
            this.lab_CreatAt.TabIndex = 14;
            this.lab_CreatAt.Text = "lab_CreatAt";
            // 
            // lab_UpdateAt
            // 
            this.lab_UpdateAt.AutoSize = true;
            this.lab_UpdateAt.Location = new System.Drawing.Point(12, 182);
            this.lab_UpdateAt.Name = "lab_UpdateAt";
            this.lab_UpdateAt.Size = new System.Drawing.Size(77, 12);
            this.lab_UpdateAt.TabIndex = 15;
            this.lab_UpdateAt.Text = "lab_UpdateAt";
            // 
            // lab_path
            // 
            this.lab_path.AutoSize = true;
            this.lab_path.Location = new System.Drawing.Point(12, 254);
            this.lab_path.Name = "lab_path";
            this.lab_path.Size = new System.Drawing.Size(101, 12);
            this.lab_path.TabIndex = 16;
            this.lab_path.Text = "卸载程序所在目录";
            // 
            // frmUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(657, 407);
            this.Controls.Add(this.lab_path);
            this.Controls.Add(this.lab_UpdateAt);
            this.Controls.Add(this.lab_CreatAt);
            this.Controls.Add(this.lab_Remark);
            this.Controls.Add(this.lab_Desc);
            this.Controls.Add(this.lab_Name);
            this.Controls.Add(this.lab_Version);
            this.Controls.Add(this.lab_ServiceName);
            this.Controls.Add(this.lab_out);
            this.Controls.Add(this.buttonInstall);
            this.Controls.Add(this.progressBar_Download);
            this.Controls.Add(this.lab_downloadRate);
            this.Controls.Add(this.btn_Download);
            this.Controls.Add(this.txt_SavePath);
            this.Controls.Add(this.lab_savePath);
            this.Controls.Add(this.txt_Download);
            this.Controls.Add(this.lab_downloadPath);
            this.Name = "frmUpdate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MES更新";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmUpdate_FormClosing);
            this.Load += new System.EventHandler(this.frmUpdate_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lab_downloadPath;
        private System.Windows.Forms.TextBox txt_Download;
        private System.Windows.Forms.Label lab_savePath;
        private System.Windows.Forms.TextBox txt_SavePath;
        private System.Windows.Forms.Button btn_Download;
        private System.Windows.Forms.Label lab_downloadRate;
        private System.Windows.Forms.ProgressBar progressBar_Download;
        private System.Windows.Forms.Button buttonInstall;
        private System.Windows.Forms.Label lab_out;
        private System.Windows.Forms.Label lab_ServiceName;
        private System.Windows.Forms.Label lab_Version;
        private System.Windows.Forms.Label lab_Name;
        private System.Windows.Forms.Label lab_Desc;
        private System.Windows.Forms.Label lab_Remark;
        private System.Windows.Forms.Label lab_CreatAt;
        private System.Windows.Forms.Label lab_UpdateAt;
        private System.Windows.Forms.Label lab_path;
    }
}

