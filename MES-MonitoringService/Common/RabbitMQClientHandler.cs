using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RabbitMQ.Client;

namespace MES_MonitoringService.Common
{
    public class RabbitMQClientHandler
    {
        private static string defaultRabbitMQHostName = Common.ConfigFileHandler.GetAppConfig("RabbitMQServerHostName");
        private static string defaultRabbitMQUserName = Common.ConfigFileHandler.GetAppConfig("RabbitMQUserName");
        private static string defaultRabbitMQPassword = Common.ConfigFileHandler.GetAppConfig("RabbitMQPassword");

        // 定义一个静态变量来保存类的实例
        private static RabbitMQClientHandler uniqueInstance;
        //定义一个标识确保线程同步 
        private static readonly object locker = new object();

        /*-------------------------------------------------------------------------------------*/

        //ConnectionFactory
        private static ConnectionFactory mc_ConnectionFactory = null;
        //Connection
        public IConnection Connection;
        //Channel
        public IModel Channel;


        /*-------------------------------------------------------------------------------------*/

        /// <summary>
        /// 定义私有构造函数，使外界不能创建该类实例
        /// </summary>
        public RabbitMQClientHandler()
        {
            //连接工厂
            mc_ConnectionFactory = new ConnectionFactory();

            //连接工厂信息
            mc_ConnectionFactory.HostName = defaultRabbitMQHostName;// "localhost";
            mc_ConnectionFactory.UserName = defaultRabbitMQUserName;// "guest";
            mc_ConnectionFactory.Password = defaultRabbitMQPassword;// "guest";

            //创建连接
            Connection = mc_ConnectionFactory.CreateConnection();
            //创建频道
            Channel = Connection.CreateModel();
        }

        /// <summary>
        /// 定义公有方法提供一个全局访问点,同时你也可以定义公有属性来提供全局访问点
        /// </summary>
        /// <returns></returns>
        public static RabbitMQClientHandler GetInstance()
        {
            // 当第一个线程运行到这里时，此时会对locker对象 "加锁"，
            // 当第二个线程运行该方法时，首先检测到locker对象为"加锁"状态，该线程就会挂起等待第一个线程解锁
            // lock语句运行完之后（即线程运行完之后）会对该对象"解锁"
            // 双重锁定只需要一句判断就可以了
            if (uniqueInstance == null)
            {
                lock (locker)
                {
                    // 如果类的实例不存在则创建，否则直接返回
                    if (uniqueInstance == null)
                    {
                        uniqueInstance = new RabbitMQClientHandler();
                    }
                }
            }
            return uniqueInstance;
        }


        /*-------------------------------------------------------------------------------------*/

        /// <summary>
        /// 发送消息至服务端
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool publishMessageToServer(string queueName, string message)
        {
            try
            {
                //创建一个持久化的频道
                bool durable = true;
                Channel.QueueDeclare(queueName, durable, false, false, null);


                //设置消息持久性
                //var properties = Channel.CreateBasicProperties();
                //properties.SetPersistent(true);

                //消息内容转码，并发送至服务器
                var messageBody = Encoding.UTF8.GetBytes(message);
                Channel.BasicPublish("", queueName, null, messageBody);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
