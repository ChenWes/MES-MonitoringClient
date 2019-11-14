using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace MES_Service_Defend
{
    class Program
    {
        static void Main(string[] args)
        {
            //服务的默认值
            string defaultServiceName = ConfigurationManager.AppSettings["DefaultServiceName"].ToString();
            string defaultServiceDisplayName = ConfigurationManager.AppSettings["DefaultServiceDisplayName"].ToString();
            string defaultServiceDescription = ConfigurationManager.AppSettings["DefaultServiceDescription"].ToString();

            //后台测试方法
            //BackendServiceHandler BackendServiceHandlerClass = new BackendServiceHandler();
            //BackendServiceHandlerClass.ProcessMachineStatusLog();
            //BackendServiceHandlerClass.ProcessSyncDataAction();
            //BackendServiceHandlerClass.CheckMachineRegister();
            //BackendServiceHandlerClass.ProcessEmployeeImage();
            //BackendServiceHandlerClass.ProcessJobOrder();

            var exitCode = HostFactory.Run(x =>
            {
                //配置服务
                x.Service<BackendServiceHandler>(s =>
                {
                    //配置服务运行方法
                    s.ConstructUsing(uploadstatus => new BackendServiceHandler());

                    //配置服务开始与结束
                    s.WhenStarted(uploadstatus => uploadstatus.Start());
                    s.WhenStopped(uploadstatus => uploadstatus.Stop());
                });

                //运行在本地系统
                x.RunAsLocalSystem();

                //服务设置属性
                x.SetServiceName(defaultServiceName);
                x.SetDisplayName(defaultServiceDisplayName);
                x.SetDescription(defaultServiceDescription);
            });

            //转换并返回退出编码
            int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;
        }
    }
}
