using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_MonitoringClient.Common
{
    public static class ImageHelp
    {
        public static Image GetImage(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            Image result = Image.FromStream(fs);
            fs.Close();
            return result;
        }
    }
}
