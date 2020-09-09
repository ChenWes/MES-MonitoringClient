using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QRCoder;

namespace MES_MonitoringClient.Common
{
    public class QRCoder
    {
        /// <summary> 
        /// 生成二维码 
        /// </summary> 
        /// <param name="qrCodeContent">要编码的内容</param> 
        /// <returns>返回二维码位图</returns> 
        public static Bitmap QRCodeEncoderUtil(string qrCodeContent,string txt,int size)
        {
         
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrCodeContent, QRCodeGenerator.ECCLevel.Q);
            QRCode qrcode = new QRCode(qrCodeData);

            // qrcode.GetGraphic 方法可参考最下发“补充说明”

            FontFamily fm = new FontFamily("Arial");
            Font font = new Font(fm, 20, FontStyle.Regular, GraphicsUnit.Pixel);
            Bitmap txtImage = GetImage(txt, 400, font);
            Bitmap qrCodeImage = qrcode.GetGraphic(size, Color.Black, Color.White, txtImage, 50, 1, true);
        
            return qrCodeImage;
        }
        public static Bitmap GetImage(string p_Text, int p_Width, Font p_Font)
        {
            Bitmap _Temp = new Bitmap(p_Width, 1);
            Graphics _Graphics = Graphics.FromImage(_Temp);
            SizeF _Size = _Graphics.MeasureString(p_Text, p_Font, new SizeF(_Temp.Width, 10000));
            _Graphics.Dispose();
            _Temp.Dispose();

            Bitmap _ReturnImage = new Bitmap((int)_Size.Width, (int)_Size.Height);
            Graphics _GraphicsImage = Graphics.FromImage(_ReturnImage);
            _GraphicsImage.DrawString(p_Text, p_Font, Brushes.Black, new RectangleF(0, 0, _ReturnImage.Width, _ReturnImage.Height), new StringFormat());
            _GraphicsImage.Dispose();
            return _ReturnImage;

        }
    }
}
