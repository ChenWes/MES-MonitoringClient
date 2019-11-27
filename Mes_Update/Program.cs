using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mes_Update
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(String[] arg)
        {
            bool createNew;
            // 只能运行一次程序
            using (Mutex m = new Mutex(true, Application.ProductName, out createNew))
            {
                if (createNew)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new frmUpdate(arg));
                }
                else
                {
                    MessageBox.Show("程序已经运行");
                }
            }
        }

    }
}