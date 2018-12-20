using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace MES_MonitoringClient.Common
{
    /// <summary>
    /// 定时器帮助类
    /// 创建定时器=》开始执行任务
    /// </summary>
    public class TimmerHandler
    {
        //定时器
        System.Timers.Timer _TTimer = null;

        //定时器默认执行一次
        private const bool default_autoReset = false;

        //定时器默认一秒执行一次
        private const int default_interval = 1000;

        //定时器创建后即刻运行
        private const bool default_autoRun = false;



        public TimmerHandler(System.Timers.ElapsedEventHandler elapseEvent) : this(default_interval, elapseEvent, default_autoRun)
        { }

        public TimmerHandler(System.Timers.ElapsedEventHandler elapseEvent, bool autoRun) : this(default_interval, elapseEvent, autoRun)
        { }

        public TimmerHandler(int interval, System.Timers.ElapsedEventHandler elapseEvent, bool autoRun) : this(interval, default_autoReset, elapseEvent, autoRun)
        { }

        /// <summary>
        /// 创建定时器
        /// </summary>
        /// <param name="interval">间隔时间（ms）</param>
        /// <param name="autoReset">是否重复执行</param>
        /// <param name="elapseEvent">定时事件</param>
        /// <param name="autoRun">是否自动运行</param>
        public TimmerHandler(int interval, bool autoReset, System.Timers.ElapsedEventHandler elapseEvent, bool autoRun)
        {
            _TTimer = new System.Timers.Timer();

            //时间间隔            
            _TTimer.Interval = interval;

            //是否重复执行
            _TTimer.AutoReset = autoReset;

            //定时器处理事件
            _TTimer.Elapsed += elapseEvent;

            //定时器自动运行
            if (autoRun)
            {
                _TTimer.Start();
            }
        }




        /// <summary>
        /// 定时器是否可用
        /// </summary>
        /// <returns></returns>
        public bool GetTimmerEnable()
        {
            return _TTimer.Enabled;
        }

        /// <summary>
        /// 开始定时器任务
        /// </summary>
        public void StartTimmer()
        {
            _TTimer.Start();
        }

        /// <summary>
        /// 关闭定时器任务
        /// </summary>
        public void StopTimmer()
        {
            _TTimer.Stop();
        }

        /// <summary>
        /// 删除定时器
        /// </summary>
        public void RemoveTimmer()
        {
            _TTimer.Stop();
            _TTimer.Dispose();
        }
    }
}
