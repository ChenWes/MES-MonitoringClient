using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_MonitoringClient.Common
{
    /// <summary>
    /// 公共方法类
    /// </summary>
    public static class CommonFunction
    {
        /*-------------------------------------------------------------------------------------*/


        /// <summary>
        /// 格式化时间显示
        /// </summary>
        /// <param name="ms">总的毫秒数</param>
        /// <returns></returns>
        public static String FormatMilliseconds(long ms)
        {
            //基数
            int ss = 1000;
            int mi = ss * 60;
            int hh = mi * 60;
            int dd = hh * 24;

            //计算
            long day = ms / dd;
            long hour = (ms - day * dd) / hh;
            long minute = (ms - day * dd - hour * hh) / mi;
            long second = (ms - day * dd - hour * hh - minute * mi) / ss;
            long milliSecond = ms - day * dd - hour * hh - minute * mi - second * ss;

            //对应的文本
            String strDay = day < 10 ? "0" + day : "" + day; //天
            String strHour = hour < 10 ? "0" + hour : "" + hour;//小时
            String strMinute = minute < 10 ? "0" + minute : "" + minute;//分钟
            String strSecond = second < 10 ? "0" + second : "" + second;//秒
            String strMilliSecond = milliSecond < 10 ? "0" + milliSecond : "" + milliSecond;//毫秒

            strMilliSecond = milliSecond < 100 ? "0" + strMilliSecond : "" + strMilliSecond;

            //返回标准化的时间
            StringBuilder strTime = new StringBuilder();
            strTime.Append(day > 0 ? (strDay + "天") : "");
            strTime.Append(hour > 0 ? (strHour + "时") : "");
            strTime.Append(minute > 0 ? (strMinute + "分") : "");
            strTime.Append(second > 0 ? (strSecond + "秒") : "");
            strTime.Append(milliSecond > 0 ? (strMilliSecond + "毫秒") : "");

            return strTime.ToString();
        }
    }
}
