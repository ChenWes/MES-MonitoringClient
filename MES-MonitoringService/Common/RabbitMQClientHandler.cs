using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

using System.Threading;
using RabbitMQ.Client.Events;
using System.IO;

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
        private static IConnection Connection;

        //发送频道及接收频道分开，避免互相影响，导致整个服务不可用
        //Send Channel
        private static IModel SendChannel;
        //Listen Channel
        private static IModel ListenChannel;

        //数据监控队列
        private static string MC_SyncDataConsume = string.Empty;

        //
        //private SyncDataHandler syncDataHandlerClass;

        /*-------------------------------------------------------------------------------------*/

        /// <summary>
        /// 定义私有构造函数，使外界不能创建该类实例
        /// </summary>
        public RabbitMQClientHandler()
        {
            Reconnect();            
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

            //发送频道
            if (SendChannel != null)
            {
                SendChannel.Dispose();
                SendChannel = null;
            }
            //接收频道
            if (ListenChannel != null)
            {
                ListenChannel.Dispose();
                ListenChannel = null;
            }
        }



        static void Connect()
        {
            try
            {
                Common.LogHandler.WriteLog("获取RabbitMQ服务器参数：" + defaultRabbitMQHostName + ":" + defaultRabbitMQPort + " (" + defaultRabbitMQUserName + "/" + defaultRabbitMQPassword + ")");
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

                mc_ConnectionFactory.RequestedHeartbeat = 30;//心跳包
                mc_ConnectionFactory.AutomaticRecoveryEnabled = true;//自动重连
                mc_ConnectionFactory.TopologyRecoveryEnabled = true;//拓扑重连
                mc_ConnectionFactory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);

                //创建连接
                Connection = mc_ConnectionFactory.CreateConnection();

                //断开连接时，调用方法自动重连
                Connection.ConnectionShutdown += Connection_ConnectionShutdown;

                //创建发送频道
                SendChannel = Connection.CreateModel();
                //创建接收频道
                ListenChannel = Connection.CreateModel();

                //发送频道确认模式，发送了消息后，可以收到回应
                SendChannel.ConfirmSelect();

                if(!string.IsNullOrEmpty(MC_SyncDataConsume))
                {
                    //重新监控消息
                    RabbitmqMessageConsume(MC_SyncDataConsume);
                }

                Common.LogHandler.WriteLog("尝试连接至RabbitMQ服务器：" + defaultRabbitMQHostName);
            }
            catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException e)
            {
                throw e;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static void Cleanup()
        {
            try
            {

                if (SendChannel != null && SendChannel.IsOpen)
                {
                    try
                    {
                        SendChannel.Close();
                    }
                    catch (Exception ex)
                    {
                        LogHandler.WriteLog("RabbitMQ重新连接，正在尝试关闭之前的Channel[发送]，但遇到错误", ex);
                    }
                    SendChannel = null;
                }

                if (ListenChannel != null && ListenChannel.IsOpen)
                {
                    try
                    {
                        ListenChannel.Close();
                    }
                    catch (Exception ex)
                    {
                        LogHandler.WriteLog("RabbitMQ重新连接，正在尝试关闭之前的Channel[接收]，但遇到错误", ex);
                    }
                    ListenChannel = null;
                }

                if (Connection != null && Connection.IsOpen)
                {
                    try
                    {
                        Connection.Close();
                    }
                    catch (Exception ex)
                    {
                        LogHandler.WriteLog("RabbitMQ重新连接，正在尝试关闭之前的连接，但遇到错误", ex);
                    }
                    Connection = null;
                }
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }

        private static void Connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            LogHandler.WriteLog("):  RabbitMQ已经断开连接，正在尝试重新连接至RabbitMQ服务器");

            Reconnect();
        }

        private static void Reconnect()
        {
            try
            {
                //清除连接及频道
                Cleanup();

                var mres = new ManualResetEventSlim(false); // state is initially false
                while (!mres.Wait(3000)) // loop until state is true, checking every 3s
                {
                    try
                    {
                        //连接
                        Connect();
                        
                        mres.Set(); // state set to true - breaks out of loop
                    }
                    catch (Exception ex)
                    {
                        LogHandler.WriteLog("RabbitMQ尝试连接RabbitMQ服务器出现错误：" + ex.Message, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHandler.WriteLog("RabbitMQ尝试重新连接RabbitMQ服务器出现错误：" + ex.Message, ex);
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
                if (Connection == null || !Connection.IsOpen) throw new Exception("连接为空或连接已经关闭");
                if (SendChannel == null || !SendChannel.IsOpen) throw new Exception("通道为空或通道已经关闭");

                //创建一个持久化的队列
                bool queueDurable = true;

                string QueueName = queueName;
                string ExchangeName = exchangeName;
                string RoutingKey = routingKey;

                //声明交换机
                SendChannel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
                //声明队列
                SendChannel.QueueDeclare(QueueName, queueDurable, false, false, null);
                //路由绑定队列
                SendChannel.QueueBind(QueueName, ExchangeName, RoutingKey, null);

                //设置消息持久性
                IBasicProperties props = SendChannel.CreateBasicProperties();
                props.ContentType = "text/plain";
                props.DeliveryMode = 2;//持久性

                //消息内容转码，并发送至服务器
                var messageBody = System.Text.Encoding.UTF8.GetBytes(message);
                SendChannel.BasicPublish(ExchangeName, RoutingKey, props, messageBody);

                //等待确认
                return SendChannel.WaitForConfirms();

            }
            catch (Exception ex)
            {
                LogHandler.WriteLog("RabbitMQ出现通用问题" + ex.Message, ex);

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
                if (Connection == null || !Connection.IsOpen) throw new Exception("连接为空或连接已经关闭");
                if (SendChannel == null || !SendChannel.IsOpen) throw new Exception("通道为空或通道已经关闭");

                //创建一个持久化的频道
                bool queueDurable = true;

                string QueueName = queueName;
                string ExchangeName = exchangeName;
                string RoutingKey = routingKey;

                //声明交换机
                SendChannel.ExchangeDeclare(ExchangeName, ExchangeType.Fanout);
                //声明队列
                SendChannel.QueueDeclare(QueueName, queueDurable, false, false, null);
                //路由绑定队列
                SendChannel.QueueBind(QueueName, ExchangeName, RoutingKey, null);

                //设置消息持久性
                IBasicProperties props = SendChannel.CreateBasicProperties();
                props.ContentType = "text/plain";
                props.DeliveryMode = 2;//持久性

                //消息内容转码，并发送至服务器
                var messageBody = System.Text.Encoding.UTF8.GetBytes(message);
                SendChannel.BasicPublish(ExchangeName, RoutingKey, props, messageBody);

                //等待确认
                return SendChannel.WaitForConfirms();

            }
            catch (Exception ex)
            {
                LogHandler.WriteLog("RabbitMQ出现通用问题" + ex.Message, ex);

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
                if (Connection == null || !Connection.IsOpen) throw new Exception("连接为空或连接已经关闭");
                if (SendChannel == null || !SendChannel.IsOpen) throw new Exception("通道为空或通道已经关闭");

                //创建一个持久化的频道
                bool queueDurable = true;

                string QueueName = queueName;
                string ExchangeName = exchangeName;
                string RoutingKey = routingKey;

                //声明交换机
                SendChannel.ExchangeDeclare(ExchangeName, ExchangeType.Topic);
                //声明队列
                SendChannel.QueueDeclare(QueueName, queueDurable, false, false, null);
                //路由绑定队列
                SendChannel.QueueBind(QueueName, ExchangeName, RoutingKey, null);

                //设置消息持久性
                IBasicProperties props = SendChannel.CreateBasicProperties();
                props.ContentType = "text/plain";
                props.DeliveryMode = 2;//持久性

                //消息内容转码，并发送至服务器
                var messageBody = System.Text.Encoding.UTF8.GetBytes(message);
                SendChannel.BasicPublish(ExchangeName, RoutingKey, props, messageBody);

                //等待确认
                return SendChannel.WaitForConfirms();

            }
            catch (Exception ex)
            {
                LogHandler.WriteLog("RabbitMQ出现通用问题" + ex.Message, ex);

                return false;
            }
        }


        /// <summary>
        /// Topic路由，接收同步消息
        /// </summary>
        /// <param name="queueName">监听的队列</param>
        public void SyncDataFromServer(string queueName)
        {
            try
            {
                MC_SyncDataConsume = queueName;
                RabbitmqMessageConsume(MC_SyncDataConsume);
            }
            catch (Exception ex)
            {
                LogHandler.WriteLog("TopicExchangeConsumeMessageFromServer运行错误：" + ex.Message, ex);
                throw ex;
            }
            //try
            //{
            //    if (Connection == null || !Connection.IsOpen) throw new Exception("连接为空或连接已经关闭");
            //    if (ListenChannel == null || !ListenChannel.IsOpen) throw new Exception("通道为空或通道已经关闭");

            //    //设定参数，方便重启RabbitMQ服务器时处理
            //    MC_SyncDataConsume = queueName;


            //    bool queueDurable = true;
            //    string QueueName = queueName;

            //    //在MQ上定义一个持久化队列，如果名称相同不会重复创建
            //    ListenChannel.QueueDeclare(QueueName, queueDurable, false, false, null);
            //    //输入1，那如果接收一个消息，但是没有应答，则客户端不会收到下一个消息
            //    ListenChannel.BasicQos(0, 1, false);

            //    //创建基于该队列的消费者，绑定事件
            //    var consumer = new EventingBasicConsumer(ListenChannel);

            //    //回应消息监控
            //    consumer.Received += SyncData_Received;

            //    //绑定消费者
            //    ListenChannel.BasicConsume(QueueName, //队列名
            //                          false,    //false：手动应答；true：自动应答
            //                          consumer);

            //    Common.LogHandler.WriteLog("开始监控RabbitMQ服务器，队列" + QueueName);

            //}
            //catch (Exception ex)
            //{
            //    LogHandler.WriteLog("TopicExchangeConsumeMessageFromServer运行错误：" + ex.Message, ex);
            //    throw ex;
            //}
        }

        private static void RabbitmqMessageConsume(string queueName)
        {
            try
            {
                if (Connection == null || !Connection.IsOpen) throw new Exception("连接为空或连接已经关闭");
                if (ListenChannel == null || !ListenChannel.IsOpen) throw new Exception("通道为空或通道已经关闭");

                //设定参数，方便重启RabbitMQ服务器时处理
                MC_SyncDataConsume = queueName;


                bool queueDurable = true;
                string QueueName = queueName;

                //在MQ上定义一个持久化队列，如果名称相同不会重复创建
                ListenChannel.QueueDeclare(QueueName, queueDurable, false, false, null);
                //输入1，那如果接收一个消息，但是没有应答，则客户端不会收到下一个消息
                ListenChannel.BasicQos(0, 1, false);

                //创建基于该队列的消费者，绑定事件
                var consumer = new EventingBasicConsumer(ListenChannel);

                //回应消息监控
                consumer.Received += SyncData_Received;

                //绑定消费者
                ListenChannel.BasicConsume(QueueName, //队列名
                                      false,    //false：手动应答；true：自动应答
                                      consumer);

                Common.LogHandler.WriteLog("开始监控RabbitMQ服务器，队列" + QueueName);

            }
            catch (Exception ex)
            {                
                throw ex;
            }
        }


        private static void SyncData_Received(object sender, BasicDeliverEventArgs e)
        {
            try
            {
                //TOOD 验证程序退出后消费者是否退出去了
                var body = e.Body; //消息主体
                var message = Encoding.UTF8.GetString(body);

                LogHandler.WriteLog("[x] 队列接收到消息：" + message.ToString());

                //处理数据
                bool processSuccessFlag = new SyncDataHandler().ProcessSyncData(message);
                if (processSuccessFlag)
                {
                    //回复确认
                    ListenChannel.BasicAck(e.DeliveryTag, false);
                }
                else
                {
                    //未正常处理的消息，重新放回队列
                    ListenChannel.BasicReject(e.DeliveryTag, true);
                }
            }
            catch (RabbitMQ.Client.Exceptions.OperationInterruptedException ex1)
            {
                Thread.Sleep(5000);
                ListenChannel.BasicNack(e.DeliveryTag, false, true);
            }
            catch (Exception ex)
            {
                Thread.Sleep(5000);
                ListenChannel.BasicNack(e.DeliveryTag, false, true);
            }
        }
    }
}
