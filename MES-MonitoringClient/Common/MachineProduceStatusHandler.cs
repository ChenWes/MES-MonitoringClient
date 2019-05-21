using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;
using System.Diagnostics;

using MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MES_MonitoringClient.Common
{
    /// <summary>
    /// 机器生产状态
    /// </summary>
    public class MachineProduceStatusHandler
    {

        /// <summary>
        /// 信号类型
        /// </summary>
        public enum SignalType
        {
            Unknow,

            X01,
            X02,
            X03,

            X01_X02,
            X01_X03,

            X02_X03,

            X01_X02_X03
        }


        /// <summary>
        /// 回复信号前缀
        /// </summary>
        private static string singnalDefaultStart = Common.ConfigFileHandler.GetAppConfig("GetSerialPortDataDefaultSignal_StartPrefix");
        /// <summary>
        /// 回复信号后缀
        /// </summary>
        private static string singnalDefaultEnd = Common.ConfigFileHandler.GetAppConfig("GetSerialPortDataDefaultSignal_EndPrefix");

        /// <summary>
        /// 本机的机器注册信息，方便写入DB时使用
        /// </summary>
        public DataModel.Machine MC_machine = null;

        /// <summary>
        /// 机器生命周期时间
        /// </summary>
        private IMongoCollection<DataModel.MachineProduceLifeCycle> machineProcuceLifeCycleCollection;

        /// <summary>
        /// 机器生产状态记录默认Mongodb集合名
        /// </summary>
        private static string defaultMachineProduceLifeCycleMongodbCollectionName = "MachineLifeCycle";
        

        /// <summary>
        /// 产品生命周期（计算次数）
        /// </summary>
        private List<MachineProcedure> _MachineProcedureListForCount=null;

        /// <summary>
        /// 订单数量
        /// </summary>
        public int OrderCount = 0;

        /// <summary>
        /// 订单未完成数量
        /// </summary>
        public int OrderNoCompleteCount = 0;

        /// <summary>
        /// 产品周期计数（生产数量）
        /// </summary>
        public int ProductCount = 0;

        /// <summary>
        /// 空产品周期计数（不完整[空啤]生产数量）
        /// </summary>
        public int ProductErrorCount = 0;

        /// <summary>
        /// 单次产品周期秒数
        /// </summary>
        public long LastProductUseMilliseconds = 0;

        /// <summary>
        /// 
        /// </summary>
        public Nullable<DateTime> LastX03SignalGetTime = null;

        /// <summary>
        /// 上一个信号
        /// </summary>
        public SignalType LastSignal;


        /// <summary>
        /// 起工时间（开始工作的时间）
        /// </summary>
        public DateTime? StartWorkTime { get; set; }

        /// <summary>
        /// 预计完成时间（回写到界面中）
        /// </summary>
        public DateTime? PlanCompleteDateTime { get; set; }


        //当前操作的工单
        public DataModel.JobOrder ProcessJobOrder = null;
        //关联的产品及模具信息（显示到界面中）
        public DataModel.Material ProcessMaterial = null;
        public DataModel.Mould ProcessMould = null;


        /// <summary>
        /// 更新机器信号后更新界面
        /// </summary>
        /// <param name="signalType"></param>
        public delegate void UpdateMachineSignal(SignalType signalType);
        public UpdateMachineSignal UpdateMachineSignalDelegate;

        /// <summary>
        /// 更新机器生命周期
        /// </summary>
        public delegate void UpdateMachineLifeCycleTime();
        public UpdateMachineLifeCycleTime UpdateMachineLifeCycleTimeDelegate;

        /// <summary>
        /// 更新良品数量
        /// </summary>
        public delegate void UpdateMachineNondefectiveCount();
        public UpdateMachineNondefectiveCount UpdateMachineNondefectiveCountDelegate;

        /// <summary>
        /// 更新未完成数量
        /// </summary>
        public delegate void UpdateMachineNoCompleteCount();
        public UpdateMachineNoCompleteCount UpdateMachineNoCompleteCountDelegate;


        /// <summary>
        /// 显示工单基本信息
        /// </summary>
        public delegate void ShowJobOrderBasicInfo();
        public ShowJobOrderBasicInfo ShowJobOrderBasicInfoDelegate;

        /// <summary>
        /// 更新机器使用时间返回至界面
        /// </summary>
        public delegate void UpdateMachineCompleteDateTime();
        public UpdateMachineCompleteDateTime UpdateMachineCompleteDateTimeDelegate;


        /// <summary>
        /// 构造函数，处理初始化的参数
        /// </summary>
        public MachineProduceStatusHandler()
        {
            //产品生命周期（计算时间）
            //_MachineProcedureListForTime = new List<MachineProcedure>();

            //产品生命周期（计算次数）
            _MachineProcedureListForCount = new List<MachineProcedure>();

            machineProcuceLifeCycleCollection = MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.MachineProduceLifeCycle>(defaultMachineProduceLifeCycleMongodbCollectionName);
        }


        
        /// <summary>
        /// 界面输入不良品数量
        /// </summary>
        /// <param name="rejectProductCount"></param>
        public bool SettingProductErrorCount(int intProductErrorCount)
        {
            if (ProcessJobOrder != null)
            {
                ProductErrorCount = intProductErrorCount;
                Common.JobOrderProcessHelper.SetJobOrderProcessErrorCount(ProductErrorCount, ProcessJobOrder._id);
                SettingMachineNondefectiveCount();

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 机器良品数量设置
        /// </summary>
        public void SettingMachineNondefectiveCount()
        {
            //未完成数量与不良品有关系
            if (ProductErrorCount > ProductCount)
            {
                OrderNoCompleteCount = OrderCount - ProductCount;
            }
            else
            {
                OrderNoCompleteCount = OrderCount - ProductCount + ProductErrorCount;
            }

            //更新界面
            UpdateMachineNondefectiveCountDelegate();
        }


        /// <summary>
        /// 机器未完成数量设置
        /// </summary>
        public void SettingMachineNoCompleteCount()
        {
            SettingMachineNondefectiveCount();

            UpdateMachineNoCompleteCountDelegate();
        }

        /// <summary>
        /// 更新机器生命周期
        /// </summary>
        public void SettingMachineLifeCycleTime()
        {
            SettingMachineNoCompleteCount();

            UpdateMachineLifeCycleTimeDelegate();
        }



        /// <summary>
        /// 设置工单
        /// 1.开始工单
        /// 2.恢复工单
        /// </summary>
        /// <param name="jobOrder">选择的工单实体</param>
        public void SetJobOrder(DataModel.JobOrder jobOrder)
        {
            try
            {
                //传入的工单
                ProcessJobOrder = jobOrder;
                ProcessMaterial = Common.MaterialHelper.GetMaterialByID(ProcessJobOrder.MaterialID);
                ProcessMould = Common.MouldHelper.GetMouldByID(ProcessJobOrder.MouldID);


                //设置工单处理记录(不存在就新增)
                Common.JobOrderProcessHelper.SetJobOrderProcessStatus(JobOrderProcessHelper.JobOrderProcessStatus.Process, ProcessJobOrder._id);


                //查询工单处理记录
                DataModel.JobOrderProcess jobOrderProcess = Common.JobOrderProcessHelper.GetJobOrderProcessByJobOrderID(ProcessJobOrder._id);
                //DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                StartWorkTime = jobOrderProcess.StartDateTime.ToLocalTime();

                //已生产的和有问题的
                ProductCount = jobOrderProcess.ProductCount;
                ProductErrorCount = jobOrderProcess.ProductErrorCount;

                //工单数量
                OrderCount = ProcessJobOrder.OrderCount;

                //界面显示基本消息
                SettingJobOrderBasicInfo();

                //计算预计完成时间
                SettingMachineCompleteDateTime();

                //计算未完成数量
                SettingMachineNondefectiveCount();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 更新信号方法
        /// </summary>
        /// <param name="newSingnal">新信号</param>
        public void ChangeSignal(string newSingnal)
        {
            //转换信号
            string convertSingnalString = ConvertSingnalString(newSingnal);

            //判断是正常的信号
            if (!string.IsNullOrEmpty(convertSingnalString))
            {
                //判断X信号
                SignalType convertSingnalStatusType = ConvertSingnalStatus(convertSingnalString);

                if (convertSingnalStatusType != LastSignal)
                {

                    if (Common.ConfigFileHandler.GetAppConfig("SaveSignalToLog") == "1")
                    {
                        Common.LogHandler.WriteLog("机器收集到信号=>" + convertSingnalString + " =>" + convertSingnalStatusType.ToString());
                    }

                    #region 与上一次信号不同

                    if (convertSingnalStatusType == SignalType.X03)
                    {
                        #region 自动信号（区分上一个信号）


                        _MachineProcedureListForCount.Add(new MachineProcedure()
                        {
                            ProcedureID = convertSingnalString,
                            ProcedureCode = convertSingnalStatusType.ToString(),
                            ProcedureName = "自动",
                        });


                        if (LastSignal == SignalType.X01_X03)
                        {
                            //结束产品周期并计时
                            if (LastX03SignalGetTime.HasValue)
                            {
                                //使用timespan计算时间
                                TimeSpan ts = new TimeSpan();
                                ts = System.DateTime.Now.Subtract(LastX03SignalGetTime.Value);
                                //上一个产品的用时
                                LastProductUseMilliseconds= (long)ts.TotalMilliseconds;

                                //更新界面
                                SettingMachineLifeCycleTime();                                
                            }
                            LastX03SignalGetTime = System.DateTime.Now;
                        }
                        else if (LastSignal == SignalType.X02_X03)
                        {
                            //必须包含完整的生命周期才计数
                            if (CheckHaveRealProduceProcess(_MachineProcedureListForCount))
                            {
                                //计数
                                ProductCount++;
                                //这里应该处理应用数量，根据逻辑来处理1*1和1+1的区别

                                if (LastX03SignalGetTime.HasValue)
                                {
                                    //处理生命周期
                                    DataModel.MachineProduceLifeCycle produceLifeCycle = new DataModel.MachineProduceLifeCycle();
                                    produceLifeCycle.LocalMacAddress = Common.CommonFunction.getMacAddress();
                                    produceLifeCycle.StartDateTime = LastX03SignalGetTime.HasValue ? LastX03SignalGetTime.Value : System.DateTime.Now;
                                    produceLifeCycle.EndDateTime = System.DateTime.Now;

                                    TimeSpan timeSpan = produceLifeCycle.EndDateTime - produceLifeCycle.StartDateTime;
                                    if (LastProductUseMilliseconds > 0)
                                    {
                                        produceLifeCycle.UseTotalSeconds = (decimal)LastProductUseMilliseconds / 1000;
                                    }
                                    else
                                    {
                                        produceLifeCycle.UseTotalSeconds = (decimal)timeSpan.TotalSeconds;
                                    }


                                    produceLifeCycle.IsUpdateToServer = false;
                                    produceLifeCycle.IsUpdateToServer = false;

                                    //保存每一个生命周期数据至数据库
                                    SaveMachineProduceLifeCycle(produceLifeCycle);
                                    //每次处理
                                    //2019-05-21 该代码导致数量变多的bug
                                    //Common.JobOrderProcessHelper.SetJobOrderProcessCount(ProductCount, ProcessJobOrder._id);
                                }

                                //订单未完成数量，等于订单数量减去已完成数量
                                OrderNoCompleteCount = OrderCount - ProductCount - ProductErrorCount;
                                //立刻更新界面
                                SettingMachineNoCompleteCount();

                                _MachineProcedureListForCount.Clear();
                                _MachineProcedureListForCount.Add(new MachineProcedure()
                                {
                                    ProcedureID = convertSingnalString,
                                    ProcedureCode = convertSingnalStatusType.ToString(),
                                    ProcedureName = "自动",
                                });
                            }
                        }

                        #endregion
                    }
                    else if (convertSingnalStatusType == SignalType.X01_X03 || convertSingnalStatusType == SignalType.X02_X03)
                    {
                        #region 开模完成==射胶完成（不区分上一个信号）
                        //产品生命周期（计算数量）
                        if (_MachineProcedureListForCount != null && _MachineProcedureListForCount.Count > 0)
                        {
                            //信号
                            string procedureNameString = string.Empty;
                            if (convertSingnalStatusType == SignalType.X01_X03) procedureNameString = "开模完成";
                            else if (convertSingnalStatusType == SignalType.X02_X03) procedureNameString = "自动射胶";

                            _MachineProcedureListForCount.Add(new MachineProcedure()
                            {
                                ProcedureID = convertSingnalString,
                                ProcedureCode = convertSingnalStatusType.ToString(),
                                ProcedureName = procedureNameString,
                            });
                        }

                        #endregion
                    }

                    #endregion

                    //上一个信号
                    LastSignal = convertSingnalStatusType;

                    //根据信号更新界面（三个信号灯的闪烁）
                    UpdateMachineSignalDelegate(LastSignal);
                }
            }
        }

        //取数数字
        private int[] ProcessProductCount(string strMouldSpecification)
        {            
            var matchMiddle = Regex.Matches(strMouldSpecification, "[0-9]+");
            List<int> getNumber = new List<int>();
            foreach (var march in matchMiddle)
            {
                int number = 0;
                int.TryParse(march.ToString(), out number);

                if (number > 0)
                {
                    getNumber.Add(number);
                }
            }

            return getNumber.ToArray();
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

            if (match.Success)
            {
                Regex regexMiddle = new Regex("(?<=(" + singnalDefaultStart + "))[.\\s\\S]*?(?=(" + singnalDefaultEnd + "))", RegexOptions.Multiline | RegexOptions.Singleline);
                Match matchMiddle = regexMiddle.Match(inputSingnal);

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
        private SignalType ConvertSingnalStatus(string inputSingnal)
        {
            if (inputSingnal == "0800") return SignalType.X01; //开模终止信号
            else if (inputSingnal == "0400") return SignalType.X02;//射胶信号
            else if (inputSingnal == "0200") return SignalType.X03;//自动运行模式信号

            else if (inputSingnal == "0C00") return SignalType.X01_X02;
            else if (inputSingnal == "0A00") return SignalType.X01_X03;
            else if (inputSingnal == "0600") return SignalType.X02_X03;

            else if (inputSingnal == "0E00") return SignalType.X01_X02_X03;

            else return SignalType.Unknow;
        }

        /// <summary>
        /// 判断是否是真实的生产流程
        /// </summary>
        /// <param name="oldMachineProcedureList"></param>
        /// <returns></returns>
        private bool CheckHaveRealProduceProcess(List<MachineProcedure> oldMachineProcedureList)
        {
            bool resultFlag = false;

            //各种信号标识
            bool isX01_X03 = false;
            bool isX02_X03 = false;
            bool isX03 = false;

            //判断是否有完整的信号
            foreach (var processItem in oldMachineProcedureList)
            {
                if (processItem.ProcedureCode == SignalType.X01_X03.ToString()) isX01_X03 = true;
                if (processItem.ProcedureCode == SignalType.X02_X03.ToString()) isX02_X03 = true;
                if (processItem.ProcedureCode == SignalType.X03.ToString()) isX03 = true;
            }

            //完整的信号则算正常生产流程
            if (isX01_X03 && isX02_X03 && isX03) return true;

            return resultFlag;
        }


        /// <summary>
        /// 机器生产生命周期保存
        /// </summary>
        /// <param name="produceLifeCycle">单个生命周期数据</param>
        private void SaveMachineProduceLifeCycle(DataModel.MachineProduceLifeCycle produceLifeCycle)
        {
            machineProcuceLifeCycleCollection.InsertOne(produceLifeCycle);
        }
        

        /// <summary>
        /// 机器预计完成时间设置
        /// </summary>
        private void SettingMachineCompleteDateTime()
        {
            UpdateMachineCompleteDateTimeDelegate();
        }

        private void SettingJobOrderBasicInfo()
        {
            ShowJobOrderBasicInfoDelegate();
        }
    }
}
