using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

using System.Threading;

namespace MES_MonitoringService.Common
{
    public class RabbitMQClientHandler
    {
        private static string defaultRabbitMQHostName = Common.ConfigFileHandler.GetAppConfig("RabbitMQServerHostName");
        private static string defaultRabbitMQPort = Common.ConfigFileHandler.GetAppConfig("RabbitMQServerPort");
        private static string defaultRabbitMQUserName = Common.ConfigFileHandler.GetAppConfig("RabbitMQUserName");
        private static string defaultRabbitMQPassword = Common.ConfigFileHandler.GetAppConfig("RabbitMQPassword");
        private static string defaultRabbitVirtualHost = Common.ConfigFileHandler.GetAppConfig("RabbitMQVirtualHost");

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
            try
            {
                Common.LogHandler.Log("获取RabbitMQ服务器参数：" + defaultRabbitMQHostName + ":" + defaultRabbitMQPort + " (" + defaultRabbitMQUserName + "/" + defaultRabbitMQPassword + ")");
                //连接工厂
                mc_ConnectionFactory = new ConnectionFactory();

                //连接工厂信息
                mc_ConnectionFactory.HostName = defaultRabbitMQHostName;// "localhost";

                int rabbitmq_port = 5672;// 默认是5672端口
                int.TryParse(defaultRabbitMQPort, out rabbitmq_port);
                mc_ConnectionFactory.Port = rabbitmq_port;// "5672"

                mc_ConnectionFactory.UserName = defaultRabbitMQUserName;// "guest";
                mc_ConnectionFactory.Password = defaultRabbitMQPassword;// "guest";
                mc_ConnectionFactory.VirtualHost = defaultRabbitVirtualHost;// "/"

                mc_ConnectionFactory.RequestedHeartbeat = 10;//心跳包
                mc_ConnectionFactory.AutomaticRecoveryEnabled = true;//自动重连
                mc_ConnectionFactory.TopologyRecoveryEnabled = true;//技术重连
                mc_ConnectionFactory.NetworkRecoveryInterval = TimeSpan.FromSeconds(1);

                //创建连接
                Connection = mc_ConnectionFactory.CreateConnection();
                //创建频道
                Channel = Connection.CreateModel();

                //确认模式，发送了消息后，可以收到回应
                Channel.ConfirmSelect();

                Common.LogHandler.Log("尝试连接至RabbitMQ服务器：" + defaultRabbitMQHostName);
            }
            catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException e)
            {
                //DisposeAllConnectionObjects();

                throw e;
                //Thread.Sleep(5000);
                // apply retry logic
            }
            catch (Exception)
            {
                throw;
            }
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

        /// <summary>
        /// 遇到断开连接时，需要将原有的连接信息、频道等全部删除一次，便于下次重连
        /// </summary>
        private void DisposeAllConnectionObjects()
        {
            //连接工厂
            if (mc_ConnectionFactory != null)
            {
                mc_ConnectionFactory = null;
            }

            //连接
            if (Connection != null)
            {
                Connection.Dispose();
                Connection = null;
            }

            //频道
            if (Channel != null)
            {
                Channel.Dispose();
                Channel = null;
            }
        }


        /*-------------------------------------------------------------------------------------*/


        /// <summary>
        /// Direct路由，发送消息至服务端
        /// </summary>
        /// <param name="exchangeName">交换机名称</param>
        /// <param name="routingKey">RoutingKey</param>
        /// <param name="queueName">队列名称</param>
        /// <param name="message">消息内容</param>
        /// <returns></returns>
        public bool DirectExchangePublishMessageToServerAndWaitConfirm(string exchangeName, string routingKey, string queueName, string message)
        {
            try
            {
                if (Connection == null) throw new Exception("连接为空");
                if (Channel == null) throw new Exception("通送为空");

                //创建一个持久化的队列
                bool queueDurable = true;

                string QueueName = queueName;
                string ExchangeName = exchangeName;
                string RoutingKey = routingKey;

                //声明交换机
                Channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
                //声明队列
                Channel.QueueDeclare(QueueName, queueDurable, false, false, null);
                //路由绑定队列
                Channel.QueueBind(QueueName, ExchangeName, RoutingKey, null);

                //设置消息持久性
                IBasicProperties props = Channel.CreateBasicProperties();
                props.ContentType = "text/plain";
                props.DeliveryMode = 2;//持久性

                //消息内容转码，并发送至服务器
                var messageBody = System.Text.Encoding.UTF8.GetBytes(message);
                Channel.BasicPublish(ExchangeName, RoutingKey, null, messageBody);

                //等待确认
                return Channel.WaitForConfirms();
            }
            catch (Exception ex)
            {
                LogHandler.Log("RabbitMQ出现通用问题" + ex.Message);

                return false;
            }
        }

        /// <summary>
        /// Fanout路由，发送消息至服务端
        /// </summary>
        /// <param name="exchangeName">交换机名称</param>
        /// <param name="routingKey">RoutingKey</param>
        /// <param name="queueName">队列名称</param>
        /// <param name="message">消息内容</param>
        /// <returns></returns>
        public bool FanoutExchangePublishMessageToServerAndWaitConfirm(string exchangeName, string routingKey, string queueName, string message)
        {
            try
            {
                if (Connection == null) throw new Exception("连接为空");
                if (Channel == null) throw new Exception("通送为空");

                //创建一个持久化的频道
                bool queueDurable = true;

                string QueueName = queueName;
                string ExchangeName = exchangeName;
                string RoutingKey = routingKey;

                //声明交换机
                Channel.ExchangeDeclare(ExchangeName, ExchangeType.Fanout);
                //声明队列
                Channel.QueueDeclare(QueueName, queueDurable, false, false, null);
                //路由绑定队列
                Channel.QueueBind(QueueName, ExchangeName, RoutingKey, null);

                //设置消息持久性
                IBasicProperties props = Channel.CreateBasicProperties();
                props.ContentType = "text/plain";
                props.DeliveryMode = 2;//持久性

                //消息内容转码，并发送至服务器
                var messageBody = System.Text.Encoding.UTF8.GetBytes(message);
                Channel.BasicPublish(ExchangeName, RoutingKey, null, messageBody);

                //等待确认
                return Channel.WaitForConfirms();
            }
            catch (Exception ex)
            {
                LogHandler.Log("RabbitMQ出现通用问题" + ex.Message);

                return false;
            }
        }

        /// <summary>
        /// Topic路由，发送消息至服务端
        /// </summary>
        /// <param name="exchangeName">交换机名称</param>
        /// <param name="routingKey">RoutingKey</param>
        /// <param name="queueName">队列名称</param>
        /// <param name="message">消息内容</param>
        /// <returns></returns>
        public bool TopicExchangePublishMessageToServerAndWaitConfirm(string exchangeName, string routingKey, string queueName, string message)
        {
            try
            {
                if (Connection == null) throw new Exception("连接为空");
                if (Channel == null) throw new Exception("通送为空");

                //创建一个持久化的频道
                bool queueDurable = true;

                string QueueName = queueName;
                string ExchangeName = exchangeName;
                string RoutingKey = routingKey;

                //声明交换机
                Channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic);
                //声明队列
                Channel.QueueDeclare(QueueName, queueDurable, false, false, null);
                //路由绑定队列
                Channel.QueueBind(QueueName, ExchangeName, RoutingKey, null);

                //设置消息持久性
                IBasicProperties props = Channel.CreateBasicProperties();
                props.ContentType = "text/plain";
                props.DeliveryMode = 2;//持久性

                //消息内容转码，并发送至服务器
                var messageBody = System.Text.Encoding.UTF8.GetBytes(message);
                Channel.BasicPublish(ExchangeName, RoutingKey, null, messageBody);

                //等待确认
                return Channel.WaitForConfirms();
            }
            catch (Exception ex)
            {
                LogHandler.Log("RabbitMQ出现通用问题" + ex.Message);

                return false;
            }
        }
    }
}
