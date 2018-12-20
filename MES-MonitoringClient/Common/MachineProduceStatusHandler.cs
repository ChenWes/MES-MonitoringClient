using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;
using System.Diagnostics;

namespace MES_MonitoringClient.Common
{
    /// <summary>
    /// 机器生产状态
    /// </summary>
    public class MachineProduceStatusHandler
    {
        private static string singnalDefaultStart = "AA";
        private static string singnalDefaultEnd = "ZZ";

        /// <summary>
        /// 计时（类内部使用）
        /// </summary>
        private Stopwatch _lifeCycleTime = null;

        /// <summary>
        /// 产品周期列表
        /// </summary>
        private List<MachineProcedure> _MachineProcedureList;

        /// <summary>
        /// 产品周期计数（生产数量）
        /// </summary>
        public int LifeCycleCount = 0;

        /// <summary>
        /// 空产品周期计数（不完整[空啤]生产数量）
        /// </summary>
        public int LiftCycleEmptyCount = 0;

        /// <summary>
        /// 单次产品周期秒数
        /// </summary>
        public int LastLifeCycleSecond = 0;


        /// <summary>
        /// 构造函数，处理初始化的参数
        /// </summary>
        public MachineProduceStatusHandler()
        {
            //产品周期
            _MachineProcedureList = new List<MachineProcedure>();
        }


        /// <summary>
        /// 更新信号方法
        /// </summary>
        /// <param name="newSingnal">新信号</param>
        public void ChangeSignal(string newSingnal)
        {
            string l_convertSingnalString = ConvertSingnalString(newSingnal);
            //判断是正常的信号
            if (l_convertSingnalString != null)
            {
                //判断X[]信号
                string l_convertSingnalStatus = ConvertSingnalStatus(l_convertSingnalString);
                if (l_convertSingnalStatus != null)
                {
                    if (l_convertSingnalStatus == "X01+X03")
                    {
                        #region X01+X03信号
                        //判断是否存在产品周期
                        if (_MachineProcedureList != null && _MachineProcedureList.Count > 0)
                        {
                            _MachineProcedureList[_MachineProcedureList.Count - 1].UseMilliseconds = (System.DateTime.Now - _MachineProcedureList[_MachineProcedureList.Count - 1].StartDateTime).Milliseconds;
                            _MachineProcedureList[_MachineProcedureList.Count - 1].EndDateTime = _MachineProcedureList[_MachineProcedureList.Count - 1].StartDateTime.AddMilliseconds(_MachineProcedureList[_MachineProcedureList.Count - 1].UseMilliseconds);
                            _lifeCycleTime.Stop();


                            LastLifeCycleSecond = 0;
                            //结束产品周期
                            foreach (MachineProcedure getMachineProcedure in _MachineProcedureList)
                            {
                                //最后一次产品周期的秒数
                                LastLifeCycleSecond += ((Int32)getMachineProcedure.UseMilliseconds / 1000);
                            }

                            //计数
                            LifeCycleCount++;

                            //清空产品周期列表，进入下一次产品周期
                            _MachineProcedureList.Clear();

                            /************************************************************/
                            _lifeCycleTime.Start();
                            
                            //清空后增加第一个工序
                            //开始产品周期
                            //开模信号                        
                            _MachineProcedureList.Add(new MachineProcedure()
                            {
                                ProcedureID = l_convertSingnalString,
                                ProcedureCode = l_convertSingnalStatus,
                                ProcedureName = "开模完成",
                                StartDateTime = System.DateTime.Now
                            });
                        }
                        else
                        {
                            _lifeCycleTime = Stopwatch.StartNew();
                            _lifeCycleTime.Start();                            

                            //开始产品周期
                            //开模信号                        
                            _MachineProcedureList.Add(new MachineProcedure()
                            {
                                ProcedureID = l_convertSingnalString,
                                ProcedureCode = l_convertSingnalStatus,
                                ProcedureName = "开模完成",
                                StartDateTime = System.DateTime.Now
                            });
                        }
                        #endregion
                    }
                    else if (l_convertSingnalStatus == "X02+X03")
                    {
                        #region X02+X03
                        //判断是否存在产品周期
                        if (_MachineProcedureList != null && _MachineProcedureList.Count > 0)
                        {
                            _MachineProcedureList[_MachineProcedureList.Count - 1].UseMilliseconds = (System.DateTime.Now - _MachineProcedureList[_MachineProcedureList.Count - 1].StartDateTime).Milliseconds;
                            _MachineProcedureList[_MachineProcedureList.Count - 1].EndDateTime = _MachineProcedureList[_MachineProcedureList.Count - 1].StartDateTime.AddMilliseconds(_MachineProcedureList[_MachineProcedureList.Count - 1].UseMilliseconds);
                            _lifeCycleTime.Stop();
                            _lifeCycleTime.Start();


                            //射胶信号
                            _MachineProcedureList.Add(new MachineProcedure()
                            {
                                ProcedureID = l_convertSingnalString,
                                ProcedureCode = l_convertSingnalStatus,
                                ProcedureName = "自动射胶",
                                StartDateTime = System.DateTime.Now
                            });
                        }
                        #endregion
                    }
                    else if (l_convertSingnalStatus == "X03")
                    {
                        #region X03
                        //判断是否存在产品周期
                        if (_MachineProcedureList != null && _MachineProcedureList.Count > 0)
                        {
                            _MachineProcedureList[_MachineProcedureList.Count - 1].UseMilliseconds = (System.DateTime.Now - _MachineProcedureList[_MachineProcedureList.Count - 1].StartDateTime).Milliseconds;
                            _MachineProcedureList[_MachineProcedureList.Count - 1].EndDateTime = _MachineProcedureList[_MachineProcedureList.Count - 1].StartDateTime.AddMilliseconds(_MachineProcedureList[_MachineProcedureList.Count - 1].UseMilliseconds);
                            _lifeCycleTime.Stop();
                            _lifeCycleTime.Start();

                            //自动信号
                            _MachineProcedureList.Add(new MachineProcedure()
                            {
                                ProcedureID = l_convertSingnalString,
                                ProcedureCode = l_convertSingnalStatus,
                                ProcedureName = _MachineProcedureList[_MachineProcedureList.Count - 1].ProcedureCode == "X02+X03" ? "射胶完成" : "自动信号",
                                StartDateTime = System.DateTime.Now
                            });
                        }
                        #endregion
                    }
                }
            }
        }

        /// <summary>
        /// 匹配信号是否正常
        /// 并返回信号中的模式数字
        /// </summary>
        /// <param name="inputSingnal">原信号</param>
        /// <returns></returns>
        private string ConvertSingnalString(string inputSingnal)
        {
            Regex regex = new Regex("^" + singnalDefaultStart + "[a-fA-F0-9]{4}" + singnalDefaultEnd + "$");
            Match match = regex.Match(inputSingnal);

            Regex regexMiddle = new Regex("(?<=(" + singnalDefaultStart + "))[.\\s\\S]*?(?=(" + singnalDefaultEnd + "))", RegexOptions.Multiline | RegexOptions.Singleline);
            Match matchMiddle = regexMiddle.Match(inputSingnal);

            if (match.Success)
            {
                return matchMiddle.Value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 模式数字转换为X[]信号
        /// </summary>
        /// <param name="inputSingnal">模式数字[0800,0400,0200,0C00,0A00,0600,0E00等模式数字]</param>
        /// <returns></returns>
        private string ConvertSingnalStatus(string inputSingnal)
        {
            if (inputSingnal == "0800") return "X01";
            else if (inputSingnal == "0400") return "X02";
            else if (inputSingnal == "0200") return "X03";

            else if (inputSingnal == "0C00") return "X01+X02";
            else if (inputSingnal == "0A00") return "X01+X03";
            else if (inputSingnal == "0600") return "X02+X03";

            else if (inputSingnal == "0E00") return "X01+X02+X03";

            else return null;
        }
    }
}
