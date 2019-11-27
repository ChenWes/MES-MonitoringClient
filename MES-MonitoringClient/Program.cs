using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MES_MonitoringClient
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
			bool ret = false;
			System.Threading.Mutex running = new System.Threading.Mutex(true, Application.ProductName, out ret);
			if (ret)
			{
			Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
			}
			else
			{
				MessageBox.Show("程序运行中，请不要同时运行本程序！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Information);
				Application.Exit();
			}
		}
    }
}
