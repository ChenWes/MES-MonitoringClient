using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;

namespace MES_MonitoringClient.Common
{
    /// <summary>
    /// 多线程任务
    /// </summary>
    public class ThreadHandler
    {
        /*-------------------------------------------------------------------------------------*/

        //
        public Thread _TThread = null;

        //默认后台运行
        private const bool default_isBackground = true;

        //默认不自动运行
        private const bool default_autoRun = false;


        /*-------------------------------------------------------------------------------------*/

        public ThreadHandler(System.Threading.ThreadStart startFunction) : this(startFunction, default_isBackground, default_autoRun)
        { }

        public ThreadHandler(System.Threading.ThreadStart startFunction, bool isBackground) : this(startFunction, isBackground, default_autoRun)
        { }        

        /// <summary>
        /// 创建线程
        /// </summary>
        /// <param name="startFunction">运行方法</param>
        /// <param name="isBackground">是否后台运行</param>
        /// <param name="autoRun">是否自动运行</param>
        public ThreadHandler(System.Threading.ThreadStart startFunction, bool isBackground, bool autoRun)
        {
            _TThread = new Thread(startFunction);

            //是否后台运行
            _TThread.IsBackground = isBackground;

            //自动运行线程
            if (autoRun)
            {
                _TThread.Start();
            }
        }


        /*-------------------------------------------------------------------------------------*/

        /// <summary>
        /// 开始线程
        /// </summary>
        public void ThreadStart()
        {
            _TThread.Start();
        }

        /// <summary>
        /// 查看线程执行状态
        /// </summary>
        /// <returns></returns>
        public bool ThreadIsAlive()
        {
            return _TThread.IsAlive;
        }

        /// <summary>
        /// 查看线程
        /// 前后台执行
        /// </summary>
        /// <returns></returns>
        public string ThreadState()
        {
            return _TThread.ThreadState.ToString();
        }

        /// <summary>
        /// 当前线程唯一托管标识符
        /// </summary>
        /// <returns></returns>
        public int ThreadManagedThreadId()
        {
            return _TThread.ManagedThreadId;
        }

        /// <summary>
        /// 等待线程结束
        /// </summary>
        public void ThreadJoin()
        {
            _TThread.Join();
        }
    }
}
