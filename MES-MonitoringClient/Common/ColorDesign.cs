using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_MonitoringClient.Common
{
    public static class ColorDesign
    {
        public static System.Windows.Media.Brush ChangeColor(string colorCode)
        {
            var converter = new System.Windows.Media.BrushConverter();
            return (System.Windows.Media.Brush)converter.ConvertFromString("#FFFFFF90");
        }
    }
}
