using System;
using System.Collections.Generic;
using System.Drawing;
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
        public static Bitmap QRCodeEncoderUtil(string qrCodeContent,int size)
        {
         
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrCodeContent, QRCodeGenerator.ECCLevel.Q);
            QRCode qrcode = new QRCode(qrCodeData);

            // qrcode.GetGraphic 方法可参考最下发“补充说明”
            Bitmap qrCodeImage = qrcode.GetGraphic(size, Color.Black, Color.White, null, 0, 0, true);
            return qrCodeImage;
        }
    }
}
