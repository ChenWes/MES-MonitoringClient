namespace MES_MonitoringClient_ManualTest
{
    partial class Form1
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
            this.btn_ScanCard = new System.Windows.Forms.Button();
            this.serialPort2 = new System.IO.Ports.SerialPort(this.components);
            this.txt_CardID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel_Test = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.txt_LoopOrder = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_GSignal = new System.Windows.Forms.TextBox();
            this.txt_FSignal = new System.Windows.Forms.TextBox();
            this.txt_ESignal = new System.Windows.Forms.TextBox();
            this.txt_CSignal = new System.Windows.Forms.TextBox();
            this.txt_BSignal = new System.Windows.Forms.TextBox();
            this.txt_GSecond = new System.Windows.Forms.TextBox();
            this.txt_FSecond = new System.Windows.Forms.TextBox();
            this.txt_ESecond = new System.Windows.Forms.TextBox();
            this.txt_CSecond = new System.Windows.Forms.TextBox();
            this.txt_DSecond = new System.Windows.Forms.TextBox();
            this.txt_BSecond = new System.Windows.Forms.TextBox();
            this.txt_DSignal = new System.Windows.Forms.TextBox();
            this.txt_ASecond = new System.Windows.Forms.TextBox();
            this.txt_ASignal = new System.Windows.Forms.TextBox();
            this.btn_Operation = new System.Windows.Forms.Button();
            this.serialPort4 = new System.IO.Ports.SerialPort(this.components);
            this.txt_log = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel_Test.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_ScanCard
            // 
            this.btn_ScanCard.Location = new System.Drawing.Point(163, 10);
            this.btn_ScanCard.Name = "btn_ScanCard";
            this.btn_ScanCard.Size = new System.Drawing.Size(75, 23);
            this.btn_ScanCard.TabIndex = 0;
            this.btn_ScanCard.Text = "刷卡";
            this.btn_ScanCard.UseVisualStyleBackColor = true;
            this.btn_ScanCard.Click += new System.EventHandler(this.btn_ScanCard_Click);
            // 
            // serialPort2
            // 
            this.serialPort2.PortName = "COM2";
            // 
            // txt_CardID
            // 
            this.txt_CardID.Location = new System.Drawing.Point(57, 12);
            this.txt_CardID.Name = "txt_CardID";
            this.txt_CardID.Size = new System.Drawing.Size(100, 21);
            this.txt_CardID.TabIndex = 1;
            this.txt_CardID.Text = "E20F1AA2";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "卡号";
            // 
            // panel_Test
            // 
            this.panel_Test.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_Test.Controls.Add(this.label19);
            this.panel_Test.Controls.Add(this.textBox2);
            this.panel_Test.Controls.Add(this.label9);
            this.panel_Test.Controls.Add(this.textBox1);
            this.panel_Test.Controls.Add(this.label5);
            this.panel_Test.Controls.Add(this.txt_LoopOrder);
            this.panel_Test.Controls.Add(this.label10);
            this.panel_Test.Controls.Add(this.label18);
            this.panel_Test.Controls.Add(this.label16);
            this.panel_Test.Controls.Add(this.label4);
            this.panel_Test.Controls.Add(this.label15);
            this.panel_Test.Controls.Add(this.label3);
            this.panel_Test.Controls.Add(this.label17);
            this.panel_Test.Controls.Add(this.label14);
            this.panel_Test.Controls.Add(this.label8);
            this.panel_Test.Controls.Add(this.label13);
            this.panel_Test.Controls.Add(this.label7);
            this.panel_Test.Controls.Add(this.label12);
            this.panel_Test.Controls.Add(this.label6);
            this.panel_Test.Controls.Add(this.label11);
            this.panel_Test.Controls.Add(this.label2);
            this.panel_Test.Controls.Add(this.txt_GSignal);
            this.panel_Test.Controls.Add(this.txt_FSignal);
            this.panel_Test.Controls.Add(this.txt_ESignal);
            this.panel_Test.Controls.Add(this.txt_CSignal);
            this.panel_Test.Controls.Add(this.txt_BSignal);
            this.panel_Test.Controls.Add(this.txt_GSecond);
            this.panel_Test.Controls.Add(this.txt_FSecond);
            this.panel_Test.Controls.Add(this.txt_ESecond);
            this.panel_Test.Controls.Add(this.txt_CSecond);
            this.panel_Test.Controls.Add(this.txt_DSecond);
            this.panel_Test.Controls.Add(this.txt_BSecond);
            this.panel_Test.Controls.Add(this.txt_DSignal);
            this.panel_Test.Controls.Add(this.txt_ASecond);
            this.panel_Test.Controls.Add(this.txt_ASignal);
            this.panel_Test.Location = new System.Drawing.Point(14, 58);
            this.panel_Test.Name = "panel_Test";
            this.panel_Test.Size = new System.Drawing.Size(774, 228);
            this.panel_Test.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 175);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 4;
            this.label5.Text = "持续发送";
            // 
            // txt_LoopOrder
            // 
            this.txt_LoopOrder.Location = new System.Drawing.Point(65, 145);
            this.txt_LoopOrder.Name = "txt_LoopOrder";
            this.txt_LoopOrder.Size = new System.Drawing.Size(680, 21);
            this.txt_LoopOrder.TabIndex = 3;
            this.txt_LoopOrder.Text = "CFCECFCEECCEECCECFCECFCECFCEECCEECCECFCECFCCFCEECCEECCECFCECFCECFCEECCEECCECFCECF" +
    "CECFCEECCEECCECFCECFCEC";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(31, 148);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(29, 12);
            this.label10.TabIndex = 2;
            this.label10.Text = "顺序";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(511, 66);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(71, 12);
            this.label18.TabIndex = 2;
            this.label18.Text = "G:1+2+3信号";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(260, 110);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(59, 12);
            this.label16.TabIndex = 2;
            this.label16.Text = "F:2+3信号";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 108);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "C:3信号";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(260, 66);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(59, 12);
            this.label15.TabIndex = 2;
            this.label15.Text = "E:1+3信号";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "B:2信号";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(728, 66);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(17, 12);
            this.label17.TabIndex = 2;
            this.label17.Text = "秒";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(459, 110);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(17, 12);
            this.label14.TabIndex = 2;
            this.label14.Text = "秒";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(206, 108);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(17, 12);
            this.label8.TabIndex = 2;
            this.label8.Text = "秒";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(459, 66);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(17, 12);
            this.label13.TabIndex = 2;
            this.label13.Text = "秒";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(206, 67);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(17, 12);
            this.label7.TabIndex = 2;
            this.label7.Text = "秒";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(459, 20);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(17, 12);
            this.label12.TabIndex = 2;
            this.label12.Text = "秒";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(206, 21);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 12);
            this.label6.TabIndex = 2;
            this.label6.Text = "秒";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(260, 20);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(59, 12);
            this.label11.TabIndex = 2;
            this.label11.Text = "D:1+2信号";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "A:1信号";
            // 
            // txt_GSignal
            // 
            this.txt_GSignal.BackColor = System.Drawing.SystemColors.Info;
            this.txt_GSignal.Enabled = false;
            this.txt_GSignal.Location = new System.Drawing.Point(588, 63);
            this.txt_GSignal.Name = "txt_GSignal";
            this.txt_GSignal.Size = new System.Drawing.Size(100, 21);
            this.txt_GSignal.TabIndex = 0;
            this.txt_GSignal.Text = "0E00";
            // 
            // txt_FSignal
            // 
            this.txt_FSignal.BackColor = System.Drawing.SystemColors.Info;
            this.txt_FSignal.Enabled = false;
            this.txt_FSignal.Location = new System.Drawing.Point(319, 107);
            this.txt_FSignal.Name = "txt_FSignal";
            this.txt_FSignal.Size = new System.Drawing.Size(100, 21);
            this.txt_FSignal.TabIndex = 0;
            this.txt_FSignal.Text = "0600";
            // 
            // txt_ESignal
            // 
            this.txt_ESignal.BackColor = System.Drawing.SystemColors.Info;
            this.txt_ESignal.Enabled = false;
            this.txt_ESignal.Location = new System.Drawing.Point(319, 63);
            this.txt_ESignal.Name = "txt_ESignal";
            this.txt_ESignal.Size = new System.Drawing.Size(100, 21);
            this.txt_ESignal.TabIndex = 0;
            this.txt_ESignal.Text = "0A00";
            // 
            // txt_CSignal
            // 
            this.txt_CSignal.BackColor = System.Drawing.SystemColors.Info;
            this.txt_CSignal.Enabled = false;
            this.txt_CSignal.Location = new System.Drawing.Point(66, 105);
            this.txt_CSignal.Name = "txt_CSignal";
            this.txt_CSignal.Size = new System.Drawing.Size(100, 21);
            this.txt_CSignal.TabIndex = 0;
            this.txt_CSignal.Text = "0200";
            // 
            // txt_BSignal
            // 
            this.txt_BSignal.BackColor = System.Drawing.SystemColors.Info;
            this.txt_BSignal.Enabled = false;
            this.txt_BSignal.Location = new System.Drawing.Point(66, 64);
            this.txt_BSignal.Name = "txt_BSignal";
            this.txt_BSignal.Size = new System.Drawing.Size(100, 21);
            this.txt_BSignal.TabIndex = 0;
            this.txt_BSignal.Text = "0400";
            // 
            // txt_GSecond
            // 
            this.txt_GSecond.Location = new System.Drawing.Point(694, 63);
            this.txt_GSecond.Name = "txt_GSecond";
            this.txt_GSecond.Size = new System.Drawing.Size(28, 21);
            this.txt_GSecond.TabIndex = 0;
            this.txt_GSecond.Text = "3";
            // 
            // txt_FSecond
            // 
            this.txt_FSecond.Location = new System.Drawing.Point(425, 107);
            this.txt_FSecond.Name = "txt_FSecond";
            this.txt_FSecond.Size = new System.Drawing.Size(28, 21);
            this.txt_FSecond.TabIndex = 0;
            this.txt_FSecond.Text = "5";
            // 
            // txt_ESecond
            // 
            this.txt_ESecond.Location = new System.Drawing.Point(425, 63);
            this.txt_ESecond.Name = "txt_ESecond";
            this.txt_ESecond.Size = new System.Drawing.Size(28, 21);
            this.txt_ESecond.TabIndex = 0;
            this.txt_ESecond.Text = "5";
            // 
            // txt_CSecond
            // 
            this.txt_CSecond.Location = new System.Drawing.Point(172, 105);
            this.txt_CSecond.Name = "txt_CSecond";
            this.txt_CSecond.Size = new System.Drawing.Size(28, 21);
            this.txt_CSecond.TabIndex = 0;
            this.txt_CSecond.Text = "10";
            // 
            // txt_DSecond
            // 
            this.txt_DSecond.Location = new System.Drawing.Point(425, 17);
            this.txt_DSecond.Name = "txt_DSecond";
            this.txt_DSecond.Size = new System.Drawing.Size(28, 21);
            this.txt_DSecond.TabIndex = 0;
            this.txt_DSecond.Text = "5";
            // 
            // txt_BSecond
            // 
            this.txt_BSecond.Location = new System.Drawing.Point(172, 64);
            this.txt_BSecond.Name = "txt_BSecond";
            this.txt_BSecond.Size = new System.Drawing.Size(28, 21);
            this.txt_BSecond.TabIndex = 0;
            this.txt_BSecond.Text = "10";
            // 
            // txt_DSignal
            // 
            this.txt_DSignal.BackColor = System.Drawing.SystemColors.Info;
            this.txt_DSignal.Enabled = false;
            this.txt_DSignal.Location = new System.Drawing.Point(319, 17);
            this.txt_DSignal.Name = "txt_DSignal";
            this.txt_DSignal.Size = new System.Drawing.Size(100, 21);
            this.txt_DSignal.TabIndex = 0;
            this.txt_DSignal.Text = "0C00";
            // 
            // txt_ASecond
            // 
            this.txt_ASecond.Location = new System.Drawing.Point(172, 18);
            this.txt_ASecond.Name = "txt_ASecond";
            this.txt_ASecond.Size = new System.Drawing.Size(28, 21);
            this.txt_ASecond.TabIndex = 0;
            this.txt_ASecond.Text = "10";
            // 
            // txt_ASignal
            // 
            this.txt_ASignal.BackColor = System.Drawing.SystemColors.Info;
            this.txt_ASignal.Enabled = false;
            this.txt_ASignal.Location = new System.Drawing.Point(66, 18);
            this.txt_ASignal.Name = "txt_ASignal";
            this.txt_ASignal.Size = new System.Drawing.Size(100, 21);
            this.txt_ASignal.TabIndex = 0;
            this.txt_ASignal.Text = "0800";
            // 
            // btn_Operation
            // 
            this.btn_Operation.Location = new System.Drawing.Point(27, 306);
            this.btn_Operation.Name = "btn_Operation";
            this.btn_Operation.Size = new System.Drawing.Size(75, 23);
            this.btn_Operation.TabIndex = 0;
            this.btn_Operation.Text = "发送信号";
            this.btn_Operation.UseVisualStyleBackColor = true;
            this.btn_Operation.Click += new System.EventHandler(this.btn_Operation_Click);
            // 
            // serialPort4
            // 
            this.serialPort4.PortName = "COM4";
            // 
            // txt_log
            // 
            this.txt_log.Location = new System.Drawing.Point(794, 58);
            this.txt_log.Name = "txt_log";
            this.txt_log.Size = new System.Drawing.Size(205, 188);
            this.txt_log.TabIndex = 4;
            this.txt_log.Text = "";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(393, 306);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "持续发送";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(65, 172);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(680, 21);
            this.textBox1.TabIndex = 5;
            this.textBox1.Text = "CFCECFCEECCEECCECFCECFCECFCEECCEECCECFCECFCCFCEECCEECCECFCECFCECFCEECCEECCECFCECF" +
    "CECFCEECCEECCECFCECFCEC";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(10, 204);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 12);
            this.label9.TabIndex = 6;
            this.label9.Text = "时间间隔";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(69, 201);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(28, 21);
            this.textBox2.TabIndex = 7;
            this.textBox2.Text = "1";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(103, 204);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(17, 12);
            this.label19.TabIndex = 8;
            this.label19.Text = "秒";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1011, 381);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txt_log);
            this.Controls.Add(this.panel_Test);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_CardID);
            this.Controls.Add(this.btn_ScanCard);
            this.Controls.Add(this.btn_Operation);
            this.Name = "Form1";
            this.Text = "MES手动测试工具";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.panel_Test.ResumeLayout(false);
            this.panel_Test.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_ScanCard;
        private System.IO.Ports.SerialPort serialPort2;
        private System.Windows.Forms.TextBox txt_CardID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel_Test;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_CSignal;
        private System.Windows.Forms.TextBox txt_BSignal;
        private System.Windows.Forms.TextBox txt_ASignal;
        private System.Windows.Forms.TextBox txt_CSecond;
        private System.Windows.Forms.TextBox txt_BSecond;
        private System.Windows.Forms.TextBox txt_ASecond;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txt_LoopOrder;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btn_Operation;
        private System.IO.Ports.SerialPort serialPort4;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txt_FSignal;
        private System.Windows.Forms.TextBox txt_ESignal;
        private System.Windows.Forms.TextBox txt_FSecond;
        private System.Windows.Forms.TextBox txt_ESecond;
        private System.Windows.Forms.TextBox txt_DSecond;
        private System.Windows.Forms.TextBox txt_DSignal;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txt_GSignal;
        private System.Windows.Forms.TextBox txt_GSecond;
        private System.Windows.Forms.RichTextBox txt_log;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Timer timer1;
    }
}

