using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace MES_MonitoringService.Common
{
    public static class LogHandler
    {

        public static void Log(string logMessage)
        {
            //是否写入参数日志
            string UploadDataLogFile = ConfigFileHandler.GetAppConfig("UploadDataLogFile");

            if (UploadDataLogFile.ToLower() == "true")
            {
                //日志内容
                string[] logString = new string[] { System.DateTime.Now.ToString() + " " + logMessage };

                //日志文件夹
                string FolderPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
                //日志文件
                string FilePath = FolderPath + @"\log.log";
                //写入日志
                File.AppendAllLines(FilePath, logString, Encoding.UTF8);
            }
        }
    }
}
