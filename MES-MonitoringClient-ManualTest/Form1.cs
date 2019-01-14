using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Newtonsoft.Json;

namespace MES_MonitoringClient_ManualTest
{
    public partial class Form1 : Form
    {
        private static string defaultMachineStatusQueueName = Common.ConfigFileHandler.GetAppConfig("MachineStatusLogQueueName");
        public int statusInt = 0;
        public string[] statusList = new string[] { "运行", "停机", "故障" };

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Model.MachineStatusLog_JSON newMachineStatus_JSON = new Model.MachineStatusLog_JSON();
            newMachineStatus_JSON.Id = new Guid().ToString();//ObjectID转换成string
            newMachineStatus_JSON.Status = statusList[statusInt];

            newMachineStatus_JSON.UseTotalSeconds = 100;//使用秒数

            newMachineStatus_JSON.StartDateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffffffK");//Date转换成string
            newMachineStatus_JSON.EndDateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffffffK");//Date转换成string

            newMachineStatus_JSON.IsStopFlag = true;
            newMachineStatus_JSON.LocalMacAddress = "";

            Common.RabbitMQClientHandler.GetInstance().publishMessageToServer(defaultMachineStatusQueueName, JsonConvert.SerializeObject(newMachineStatus_JSON));

            statusInt++;
            if (statusInt >= 3) statusInt = 0;
        }
    }
}
