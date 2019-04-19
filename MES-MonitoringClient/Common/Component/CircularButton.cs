using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MES_MonitoringClient.Common.Component
{
    /// <summary>
    /// 圆形控件
    /// </summary>
    public class CircularButton:Button
    {
        protected override void OnPaint(PaintEventArgs pevent)
        {
            GraphicsPath grpath = new GraphicsPath();
            grpath.AddEllipse(0, 0, ClientSize.Width, ClientSize.Height);
            this.Region = new System.Drawing.Region(grpath);

            //this.FlatStyle = FlatStyle.Flat;            

            //不设边框
            //this.FlatAppearance.BorderSize = 0;            


            base.OnPaint(pevent);
        }

        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        CreateParams cp = base.CreateParams;
        //        cp.ExStyle |= 0x02000000;
        //        return cp;
        //    }
        //}
    }
}
