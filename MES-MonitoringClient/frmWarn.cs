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
    public partial class frmWarn : Form
    {
        int num = 0;
        public frmWarn()
        {

            InitializeComponent();
            this.btn_light.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
            this.btn_light.BackColor = Color.Red;

        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void btn_light_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void timer_light_Tick(object sender, EventArgs e)
        {
            TimeSpan timeSpan = DateTime.Now - frmMain.mc_MachineStatusHander.mc_MachineProduceStatusHandler.addCountTime;
            if(timeSpan<new TimeSpan(TimeSpan.TicksPerMinute * 5))
            {
                this.Close();
            }
            this.btn_light.Text = string.Format("{0:00}:{1:00}:{2:00}",timeSpan.Days*24+timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

            if (num == 0)
            {
                this.btn_light.BackColor = Color.Red;
                this.tableLayoutPanel1.BackColor = Color.Black;
                num = 1;
            }
            else
            {
                this.btn_light.BackColor = Color.Black;
                this.tableLayoutPanel1.BackColor = Color.Red;
                num = 0;
            }
        }
       
    }
}
