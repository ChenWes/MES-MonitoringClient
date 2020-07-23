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

        ///<summary>
        ///当前时间及班次ID
        /// </summary>
        public DataModel.NowWorkShift nowWorkShift = new DataModel.NowWorkShift();

        private MachineProductionHandler machineProductionHandler = new MachineProductionHandler();

        /// <summary>
        /// 机器生命周期时间
        /// </summary>
        private IMongoCollection<DataModel.MachineProduceLifeCycle> machineProcuceLifeCycleCollection;

        /// <summary>
        /// 机器生产状态记录默认Mongodb集合名
        /// </summary>
        private static string defaultMachineProduceLifeCycleMongodbCollectionName = "MachineLifeCycle";

        /// <summary>
        ///工单第一次生产记录默认Mongodb集合名
        /// </summary>
        private static string defaultJobOrderFirstProduceLogCollectionName = Common.ConfigFileHandler.GetAppConfig("JobOrderFirstProduceLogCollectionName");

        /// <summary>
        /// 工单第一次生产记录
        /// </summary>
        private IMongoCollection<DataModel.JobOrderFirstProduceLog> jobOrderFirstProduceLogCollection;

        /// <summary>
        /// 产品生命周期（计算次数）
        /// </summary>
        private List<MachineProcedure> _MachineProcedureListForCount = null;

        /// <summary>
        /// 订单数量
        /// </summary>
        //public int OrderCount = 0;

        /// <summary>
        /// 订单未完成数量
        /// </summary>
        //public int OrderNoCompleteCount = 0;

        /// <summary>
        /// 本机生产的生产数量
        /// </summary>
        //public int ProductCount = 0;

        /// <summary>
        /// 所有机器生产的产品数量
        /// </summary>
        //public int AllMachineProductCount = 0;

        /// <summary>
        /// 本机不良品数量
        /// </summary>
        //public int ProductErrorCount = 0;

        /// <summary>
        /// 所有机器不良品数量
        /// </summary>
        //public int AllMachineErrorCount = 0;

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


        //选定的工单列表及当前操作的工单
        public List<DataModel.JobOrder> ProcessJobOrderList = null;
        public DataModel.JobOrder CurrentProcessJobOrder = null;

        //当前模具对应产品出数
        public DataModel.MouldProduct CurrentMouldProduct = null;



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


        private static readonly object obj = new object();

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

            jobOrderFirstProduceLogCollection = MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.JobOrderFirstProduceLog>(defaultJobOrderFirstProduceLogCollectionName);
        }



        /*工单相关********************************************************************************************************************************/
        public void StopJobOrder()
        {
            try
            {
				
                List<DataModel.JobOrder> newJobOrderList = new List<DataModel.JobOrder>();

				if (ProcessJobOrderList != null && ProcessJobOrderList.Count > 0)
				{
                    DateTime now = DateTime.Now;
					//更新至数据库
					foreach (DataModel.JobOrder jobOrderItem in ProcessJobOrderList)
					{
                        //找到没结束的处理记录
                        var findMachineProcessLog = jobOrderItem.MachineProcessLog.Find(t => t.MachineID == MC_machine._id && t.ProduceStartDate==t.ProduceEndDate);
                        //结束时间为当前时间
                        findMachineProcessLog.ProduceEndDate = now;

                        jobOrderItem.Status = Common.JobOrderStatus.eumJobOrderStatus.Suspend.ToString();

						//需要返回值，并更新回class
						DataModel.JobOrder jobOrder = JobOrderHelper.UpdateJobOrder(jobOrderItem, true);
						newJobOrderList.Add(jobOrder);
                        //停止每小时计数
                        stopAdd(jobOrder, now);
                    }

					//更新完的class                
					ProcessJobOrderList = null;
					CurrentProcessJobOrder = null;

					//界面显示基本消息
					SettingJobOrderBasicInfo();

					//计算预计完成时间
					SettingMachineCompleteDateTime();

					//计算未完成数量
					SettingMachineNondefectiveCount();
                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CompleteJobOrder(string operaterID)
        {
            try
            {
                List<DataModel.JobOrder> newJobOrderList = new List<DataModel.JobOrder>();
                DateTime now = DateTime.Now;
                //更新至数据库
                foreach (DataModel.JobOrder jobOrderItem in ProcessJobOrderList)
                {

                    //找到没结束的处理记录
                    var findMachineProcessLog = jobOrderItem.MachineProcessLog.Find(t => t.MachineID == MC_machine._id && t.ProduceStartDate == t.ProduceEndDate);
                    //结束时间为当前时间
                    findMachineProcessLog.ProduceEndDate = now;

                    jobOrderItem.Status = Common.JobOrderStatus.eumJobOrderStatus.Completed.ToString();
                    //完成时间及操作人
                    jobOrderItem.CompletedDate = now;
                    jobOrderItem.CompletedOperaterID = operaterID;

                    //需要返回值，并更新回class
                    DataModel.JobOrder jobOrder = JobOrderHelper.UpdateJobOrder(jobOrderItem, true);
                    newJobOrderList.Add(jobOrder);
                    //停止每小时计数
                    stopAdd(jobOrder, now);
                }

                //更新完的class
                ProcessJobOrderList = null;
                CurrentProcessJobOrder = null;

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
        /// 清空工单
        /// </summary>
        public void ClearJobOrder()
        {

        }

        /// <summary>
        /// 设置工单
        /// 1.开始工单
        /// 2.恢复工单
        /// </summary>
        /// <param name="jobOrder">选择的工单实体</param>
        public void SetJobOrder(List<DataModel.JobOrder> jobOrderList, string operaterID)
        {
            try
            {


                //处理生产中的工单并传入的工单参数
                ProcessJobOrderList = StopOtherJobOrder(jobOrderList);

                //设置工单处理记录（在工单基础之上直接修改）                
                List<DataModel.JobOrder> newJobOrderList = new List<DataModel.JobOrder>();
                foreach (DataModel.JobOrder jobOrderItem in jobOrderList)
                {
                    //找到本机生产记录
                    //var findMachineProcessLog = jobOrderItem.MachineProcessLog.Find(t => t.MachineID == MC_machine._id);

                    //如果没有，则新增本机生产记录
                    //if (jobOrderItem.MachineProcessLog == null || jobOrderItem.MachineProcessLog.Count == 0 || findMachineProcessLog == null)
                    //{

                    //解决思路，一次生产，按一次计算


                    DataModel.JobOrder_MachineProcessLog newJobOrder_MachineProcessLog = new DataModel.JobOrder_MachineProcessLog();
                    newJobOrder_MachineProcessLog._id = ObjectId.GenerateNewId().ToString();

                    newJobOrder_MachineProcessLog.MachineID = MC_machine._id;

                    //开始及结束取同一个值
                    DateTime orderStartDate = System.DateTime.Now;
                    newJobOrder_MachineProcessLog.ProduceStartDate = orderStartDate;
                    newJobOrder_MachineProcessLog.ProduceEndDate = orderStartDate;

                    newJobOrder_MachineProcessLog.ProduceCount = 0;
                    newJobOrder_MachineProcessLog.ErrorCount = 0;

                    newJobOrder_MachineProcessLog.EmployeeID = operaterID;

                    //机器处理记录
                    jobOrderItem.MachineProcessLog.Add(newJobOrder_MachineProcessLog);
                    //}
                }
                //更新周期
                LastProductUseMilliseconds = 0;
                //更新至数据库
                foreach (DataModel.JobOrder jobOrderItem in jobOrderList)
                {
                    //获取工单状态
                    string status = jobOrderItem.Status;
                    jobOrderItem.Status = Common.JobOrderStatus.eumJobOrderStatus.Producing.ToString();

                    //需要返回值，并更新回class
                    DataModel.JobOrder jobOrder = JobOrderHelper.UpdateJobOrder(jobOrderItem, true);
                    newJobOrderList.Add(jobOrder);
                    //添加新记录
                    ProcessMachineProduction(jobOrder, 0);
                    //保存第一次生产记录
                    if (status== Common.JobOrderStatus.eumJobOrderStatus.Assigned.ToString())
                    {
                        DataModel.JobOrderFirstProduceLog jobOrderFirstProduceLog = new DataModel.JobOrderFirstProduceLog();
                        jobOrderFirstProduceLog.JobOrderID = jobOrderItem._id;
                        jobOrderFirstProduceLog.MachineID= MC_machine._id;
                        jobOrderFirstProduceLog.StartDateTime = DateTime.Now;
                        jobOrderFirstProduceLog.IsSyncToServer = false;
                        jobOrderFirstProduceLogCollection.InsertOne(jobOrderFirstProduceLog);
                    }

                }

                //更新完的class
                ProcessJobOrderList = newJobOrderList;
                
                //当前工单
                ChangeCurrentProcessJobOrder(0);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 暂停所有生产中的工单
        /// </summary>
        public List<DataModel.JobOrder> StopOtherJobOrder(List<DataModel.JobOrder> jobOrderList)
        {
            try
            {
                //传入最新工单
                List<DataModel.JobOrder> newJobOrderList = jobOrderList;
                //处理生产中的工单

                List<DataModel.JobOrder> startedJobOrders = ((List<DataModel.JobOrder>)Common.JobOrderHelper.GetAllJobOrder()).FindAll(x => x.Status == Common.JobOrderStatus.eumJobOrderStatus.Producing.ToString());


                DateTime now = DateTime.Now;
                //更新至数据库
                foreach (DataModel.JobOrder jobOrderItem in startedJobOrders)
                {
                    //找到没结束的处理记录
                    var findMachineProcessLog = jobOrderItem.MachineProcessLog.Find(t => t.MachineID == MC_machine._id && t.ProduceStartDate == t.ProduceEndDate);
                    //结束时间为当前时间
                    findMachineProcessLog.ProduceEndDate = now;
                    jobOrderItem.Status = Common.JobOrderStatus.eumJobOrderStatus.Suspend.ToString();

                    //需要返回值，并更新回class
                    DataModel.JobOrder jobOrder = JobOrderHelper.UpdateJobOrder(jobOrderItem, true);
                    //停止每小时计数
                    stopAdd(jobOrder, now);
                    //查找传入的工单是否有修改
                    foreach (DataModel.JobOrder oldJobOrder in jobOrderList.ToArray())
                    {
                        if (oldJobOrder._id == jobOrderItem._id)
                        {
                            newJobOrderList.Remove(oldJobOrder);
                            newJobOrderList.Add(jobOrder);
                        }
                    }
                }
                return newJobOrderList;
            }
            catch (Exception ex)
            {
                throw;
                
            }
        }
        /// <summary>
        /// 切换当前工单
        /// </summary>
        /// <param name="jobOrderIndex"></param>
        public void ChangeCurrentProcessJobOrder(int jobOrderIndex)
        {
            try
            {
                //当前工单
                CurrentProcessJobOrder = ProcessJobOrderList[jobOrderIndex];

                //通过当前单据找到模具对应产品出数数据
                CurrentMouldProduct = Common.MouldProductHelper.GetMmouldProductByMouldCode(CurrentProcessJobOrder.MouldCode);

                //显示当前工单
                ShowCurrentJobOrder();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 显示当前工单信息
        /// </summary>
        private void ShowCurrentJobOrder()
        {
            try
            {
                if (CurrentProcessJobOrder == null) throw new Exception("无法显示工单信息，因为当前没有工单");

                //界面显示基本消息
                SettingJobOrderBasicInfo();
                //更新实际周期
                UpdateMachineLifeCycleTimeDelegate();
                //计算未完成数量
                SettingMachineNondefectiveCount();
                //计算预计完成时间
                SettingMachineCompleteDateTime();


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /*信号相关********************************************************************************************************************************/


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
                                LastProductUseMilliseconds = (long)ts.TotalMilliseconds;

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
                                //ProductCount++;
                                //这里应该处理应用数量，根据逻辑来处理1*1和1+1的区别

                                if (LastX03SignalGetTime.HasValue)
                                {

                                    #region 保存生命周期 
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

                                    #endregion

                                    #region 处理产品出数

                                    ProcessMouldLifeCycle();

                                    #endregion
                                }

                                //订单未完成数量，等于订单数量减去已完成数量
                                //OrderNoCompleteCount = OrderCount - ProductCount - ProductErrorCount;
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




        /*输入相关********************************************************************************************************************************/


        /// <summary>
        /// 界面输入不良品数量
        /// </summary>
        /// <param name="rejectProductCount"></param>
        public bool SettingProductErrorCount(int intProductErrorCount)
        {
            if (CurrentProcessJobOrder != null)
            {
                //找出机器处理记录
                var findMachineProcessLog = CurrentProcessJobOrder.MachineProcessLog.Find(t => t.MachineID == MC_machine._id && t.ProduceStartDate == t.ProduceEndDate);

                if (findMachineProcessLog != null)
                {
                    findMachineProcessLog.ErrorCount = intProductErrorCount;

                    //更新到工单列表中
                    int currentIndex = ProcessJobOrderList.FindIndex(i => i._id == CurrentProcessJobOrder._id);
                    ProcessJobOrderList[currentIndex] = CurrentProcessJobOrder;

                    //保存至数据库中
                    JobOrderHelper.UpdateJobOrder(CurrentProcessJobOrder, false);
                    //处理不良品
                    UpdateErrorCount(findMachineProcessLog, intProductErrorCount);
                    //更新未完成数量（一起更新良品数量）
                    SettingMachineNoCompleteCount();
                }


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
            //if (ProductErrorCount > ProductCount)
            //{
            //    OrderNoCompleteCount = OrderCount - ProductCount;
            //}
            //else
            //{
            //    OrderNoCompleteCount = OrderCount - ProductCount + ProductErrorCount;
            //}

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
            UpdateMachineCompleteDateTimeDelegate();
            UpdateMachineLifeCycleTimeDelegate();
        }



        /*界面相关********************************************************************************************************************************/


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

        /// <summary>
        /// 显示工单基础消息
        /// </summary>
        private void SettingJobOrderBasicInfo()
        {
            ShowJobOrderBasicInfoDelegate();
        }


        private void ProcessJobOrderMachineProduceCount(DataModel.JobOrder jobOrder, int addCount)
        {
            try
            {
                //采集次数加1
                jobOrder.MouldLifecycle = jobOrder.MouldLifecycle + 1;
                //加数时，处理的是同一机器且未结束的记录==》同一机器生产多次，加数在最后一次（即未完成的那一次）
                var findMachineProcessLog = jobOrder.MachineProcessLog.Find(t => t.MachineID == MC_machine._id && t.ProduceStartDate == t.ProduceEndDate);

                if (findMachineProcessLog != null)
                {
                    if (addCount > 0)
                    {
                        findMachineProcessLog.ProduceCount = findMachineProcessLog.ProduceCount + addCount;
                    }
                }
                //不需要返回最新的数据
                JobOrderHelper.UpdateJobOrder(jobOrder, false);
                //按时段记录生产数
                ProcessMachineProduction(jobOrder, addCount);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //按时段记录生产数
        public void ProcessMachineProduction(DataModel.JobOrder jobOrder, int addCount)
        {
            //查找该工单有没有未stop的记录
            try
            {
                lock (obj)
                {
                    
                    DateTime now = DateTime.Now;
                    //是否已经增加
                    bool isAdd = false;
                    List<DataModel.MachineProduction> newMachineProductions = machineProductionHandler.findNewRecordByID(jobOrder._id);
                    if (newMachineProductions.Count > 0)
                    {
                        foreach (var item in newMachineProductions)
                        {
                            if (now >= item.StartDateTime.ToLocalTime() && now <= item.EndDateTime.ToLocalTime())
                            {
                                //周期
                                item.ProduceSecond = LastProductUseMilliseconds * 1.000 / 1000;
                                //员工工时
                                int i = 0;
                                foreach (var employeeProductionTimeList in item.EmployeeProductionTimeList.ToArray())
                                {
                                    if (employeeProductionTimeList.StartTime == employeeProductionTimeList.EndTime)
                                    {
                                        item.EmployeeProductionTimeList[i].WorkHour = Math.Round((now - item.EmployeeProductionTimeList[i].StartTime.ToLocalTime()).TotalHours,3);
                                    }
                                    i++;
                                }
                                //生产工时
                                double jobOrderTime = 0;
                                foreach (var log in item.JobOrderProductionLog.ToArray())
                                {
                                    if (log.ProduceStartDate == log.ProduceEndDate)
                                    {
                                        jobOrderTime = jobOrderTime + Math.Round((now - log.ProduceStartDate.ToLocalTime()).TotalHours,3);
                                    }
                                    else
                                    {
                                        jobOrderTime = jobOrderTime + Math.Round((log.ProduceEndDate.ToLocalTime() - log.ProduceStartDate.ToLocalTime()).TotalHours,3);
                                    }
                                }
                                item.JobOrderProductionTime = jobOrderTime;
                                //判断生产记录id是否一致
                                var findMachineProcessLog = jobOrder.MachineProcessLog.Find(t => t.MachineID == MC_machine._id && t.ProduceStartDate == t.ProduceEndDate);
                                if (findMachineProcessLog != null)
                                {
                                    if (findMachineProcessLog._id != item.MachineProcessLogID)
                                    {
                                        //修改最近生产记录
                                        item.MachineProcessLogID = findMachineProcessLog._id;
                                        //增加工单生产记录
                                        DataModel.JobOrderProductionLog jobOrderProductionLog = new DataModel.JobOrderProductionLog();
                                        jobOrderProductionLog.MachineProcessLogID = findMachineProcessLog._id;
                                        jobOrderProductionLog.ProduceCount = 0;
                                        jobOrderProductionLog.ErrorCount = 0;
                                        jobOrderProductionLog.ProduceStartDate = now;
                                        jobOrderProductionLog.ProduceEndDate = now;
                                        item.JobOrderProductionLog.Add(jobOrderProductionLog);
                                        //增加员工记录
                                        foreach (var employeeProductionTime in getEmployeeProductionTimeList(now))
                                        {
                                            item.EmployeeProductionTimeList.Add(employeeProductionTime);
                                        }
                                    }
                                }
                                //生产数
                                int count = item.ProduceCount + addCount;
                                //修改停止状态为false,并加数
                                machineProductionHandler.StartMachineProduction(item, count);
                                isAdd = true;
                                break;
                            }
                        }
                        if (!isAdd)
                        {
                            //新增记录
                            addMachineProduction(jobOrder, now, addCount);
                        }
                    }
                    else
                    {
                        //新增记录
                        addMachineProduction(jobOrder, now, addCount);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //新增记录
        private void addMachineProduction(DataModel.JobOrder jobOrder,DateTime now,int addCount)
        {
            DataModel.MachineProduction machineProduction = new DataModel.MachineProduction();
            DataModel.NowWorkShift nowWorkShift = findWorkShiftByNow(now);
            if (nowWorkShift != null)
            {
                machineProduction.Date = nowWorkShift.Date;
                machineProduction.WorkShiftID = nowWorkShift.WorkShiftID;
            }
            else
            {
                if (now.Hour >= 7 && now.Minute >= 30)
                {
                    machineProduction.Date = DateTime.Parse(now.ToString("yyyy-MM-dd"));
                }
                else
                {
                    machineProduction.Date = DateTime.Parse(now.AddDays(-1).ToString("yyyy-MM-dd"));
                }
               
            }
            if (now.Minute >= 30)
            {
                machineProduction.StartDateTime = DateTime.Parse(now.ToString("yyyy-MM-dd HH")+":30:00");
                machineProduction.EndDateTime = DateTime.Parse(now.AddHours(1).ToString("yyyy-MM-dd HH") + ":00:00");
            }
            else
            {
                machineProduction.StartDateTime = DateTime.Parse(now.ToString("yyyy-MM-dd HH") + ":00:00");
                machineProduction.EndDateTime = DateTime.Parse(now.ToString("yyyy-MM-dd HH") + ":30:00");
            }
           
            machineProduction.MachineID = MC_machine._id;
            machineProduction.JobOrderID = jobOrder._id;
            machineProduction.IsStopFlag = false;
            machineProduction.IsSyncToServer = false;
            machineProduction.ProduceCount = addCount;
            if (addCount == 0)
            {
                machineProduction.ProduceSecond = 0;
            }
            else
            {
                machineProduction.ProduceSecond = LastProductUseMilliseconds * 1.00 / 1000;
            }
            machineProduction.ErrorCount = 0;
            var findMachineProcessLog = jobOrder.MachineProcessLog.Find(t => t.MachineID == MC_machine._id && t.ProduceStartDate == t.ProduceEndDate);
            if (findMachineProcessLog != null)
            {
                machineProduction.MachineProcessLogID = findMachineProcessLog._id;
            }
            DataModel.JobOrderProductionLog jobOrderProductionLog = new DataModel.JobOrderProductionLog();
            //连续生产
            if (machineProductionHandler.findRecordByProcessID(findMachineProcessLog._id,jobOrder._id).Count>0)
            {
             
                if (findMachineProcessLog != null)
                {
                    jobOrderProductionLog.MachineProcessLogID = findMachineProcessLog._id;
                }
                jobOrderProductionLog.ProduceCount = 0;
                jobOrderProductionLog.ErrorCount = 0;
                jobOrderProductionLog.ProduceStartDate = machineProduction.StartDateTime;
                jobOrderProductionLog.ProduceEndDate = machineProduction.StartDateTime;
                machineProduction.JobOrderProductionLog = new List<DataModel.JobOrderProductionLog>();
                machineProduction.JobOrderProductionLog.Add(jobOrderProductionLog);

            }
            //换批生产
            else{
               
                if (findMachineProcessLog != null)
                {
                    jobOrderProductionLog.MachineProcessLogID = findMachineProcessLog._id;
                }
                jobOrderProductionLog.ProduceCount = 0;
                jobOrderProductionLog.ErrorCount = 0;
                jobOrderProductionLog.ProduceStartDate = now;
                jobOrderProductionLog.ProduceEndDate = now;
                machineProduction.JobOrderProductionLog = new List<DataModel.JobOrderProductionLog>();
                machineProduction.JobOrderProductionLog.Add(jobOrderProductionLog);
            }
            machineProduction.JobOrderProductionTime = 0;
            machineProduction.EmployeeProductionTimeList = getEmployeeProductionTimeList(jobOrderProductionLog.ProduceStartDate);
            machineProductionHandler.SaveMachineProduction(machineProduction);
        }
        //刷卡新增员工记录
        public void addEmployee(string employeeID,DateTime startTime)
        {
            try
            {
                //有工单在生产
                if (ProcessJobOrderList != null && ProcessJobOrderList.Count > 0)
                {
                    foreach (var processJobOrder in ProcessJobOrderList.ToArray())
                    {
                        List<DataModel.MachineProduction> machineProductions = machineProductionHandler.findRecordByID(processJobOrder._id);
                        if (machineProductions.Count > 0)
                        {
                            //增加数量
                            foreach (var item in machineProductions)
                            {
                                if (startTime >= item.StartDateTime.ToLocalTime() && startTime <= item.EndDateTime.ToLocalTime())
                                {
                                    //只更新工时
                                    BsonDocument bsons = item.ToBsonDocument();
                                    DataModel.EmployeeProductionTimeList employeeProductionTimeList = new DataModel.EmployeeProductionTimeList();
                                    employeeProductionTimeList.EmployeeID = employeeID;
                                    employeeProductionTimeList.StartTime = startTime;
                                    employeeProductionTimeList.EndTime = startTime;
                                    employeeProductionTimeList.WorkHour = 0;
                                    item.EmployeeProductionTimeList.Add(employeeProductionTimeList);
                                    if (machineProductionHandler.AddEmployee(item, bsons) == null)
                                    {
                                        addEmployee(employeeID, startTime);
                                        break;
                                    }

                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //刷卡结束员工记录
        public void endEmployee(string employeeID, DateTime endTime)
        {
            try
            {
                //有工单在生产
                if (ProcessJobOrderList != null && ProcessJobOrderList.Count > 0)
                {
                    foreach (var processJobOrder in ProcessJobOrderList.ToArray())
                    {
                        List<DataModel.MachineProduction> machineProductions = machineProductionHandler.findRecordByID(processJobOrder._id);
                        if (machineProductions.Count > 0)
                        {
                            //增加数量
                            foreach (var item in machineProductions)
                            {
                                BsonDocument bsons = item.ToBsonDocument();
                                if (endTime >= item.StartDateTime.ToLocalTime() && endTime <= item.EndDateTime.ToLocalTime())
                                {
                                    int i = 0;
                                    foreach (var employeeProductionTime in item.EmployeeProductionTimeList.ToArray())
                                    {
                                        if (employeeProductionTime.EmployeeID == employeeID && employeeProductionTime.StartTime == employeeProductionTime.EndTime)
                                        {
                                            //只更新工时
                                            item.EmployeeProductionTimeList[i].EndTime = endTime;
                                            item.EmployeeProductionTimeList[i].WorkHour = Math.Round((endTime - item.EmployeeProductionTimeList[i].StartTime.ToLocalTime()).TotalHours,3);
                                            if (machineProductionHandler.AddEmployee(item, bsons) == null)
                                            {
                                                endEmployee(employeeID, endTime);
                                                break;
                                            }
                                        }
                                        i++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //
        //停止记录
        private void stopAdd(DataModel.JobOrder jobOrder,DateTime now)
        {
            List<DataModel.MachineProduction> machineProductions = machineProductionHandler.findRecordByID(jobOrder._id);
            foreach (var item in machineProductions)
            {
                //结束员工记录
                int i = 0;
                foreach (var employeeProductionTimeList in item.EmployeeProductionTimeList.ToArray())
                {

                    if (employeeProductionTimeList.StartTime == employeeProductionTimeList.EndTime)
                    {
                        if (item.EndDateTime.ToLocalTime() > now)
                        {
                            item.EmployeeProductionTimeList[i].EndTime = now;
                        }
                        else
                        {
                            item.EmployeeProductionTimeList[i].EndTime = item.EndDateTime.ToLocalTime();
                        }
                        //计算员工工时
                        item.EmployeeProductionTimeList[i].WorkHour = Math.Round((item.EmployeeProductionTimeList[i].EndTime.ToLocalTime() - item.EmployeeProductionTimeList[i].StartTime.ToLocalTime()).TotalHours,3);

                    }
                    i++;
                }
                //结束工单记录
                int j = 0;
                double jobOrderTime = 0;
                foreach (var jobOrderProductionLog in item.JobOrderProductionLog.ToArray())
                {

                    if (jobOrderProductionLog.ProduceStartDate == jobOrderProductionLog.ProduceEndDate)
                    {
                        if (item.EndDateTime.ToLocalTime() > now)
                        {
                            item.JobOrderProductionLog[j].ProduceEndDate = now;
                            //计算工单时间
                            
                        }
                        else
                        {
                            item.JobOrderProductionLog[j].ProduceEndDate = item.EndDateTime.ToLocalTime();
                            //计算工单时间
                           
                        }

                    }
                    jobOrderTime = jobOrderTime + Math.Round((item.JobOrderProductionLog[j].ProduceEndDate.ToLocalTime() - jobOrderProductionLog.ProduceStartDate.ToLocalTime()).TotalHours,3);
                    j++;
                }
            item.JobOrderProductionTime = jobOrderTime;
            machineProductionHandler.StopMachineProduction(item);
            }
        }
        //获取当前班次和日期
        private DataModel.NowWorkShift findWorkShiftByNow(DateTime now)
        {
            //通过领班查找
            List<DataModel.WorkshopResponsiblePerson> headAreas = Common.WorkshopResponsiblePersonHandler.findEmployeeByWorkshopID(MC_machine.WorkshopID);
            string today = now.ToString("yyyy-MM-dd");
            string lastday = now.AddDays(-1).ToString("yyyy-MM-dd");
            string maxTime = today + "T23:59:59Z";
            string minTime = lastday + "T00:00:00Z";
            foreach (var item in headAreas)
            {
                foreach (var employeeID in item.ResponsiblePersonID)
                {
                    //通过班次找到适合该时间的该员工
                    //通过（员工，时间）找到今天和昨天的班次
                    List<DataModel.EmployeeWorkSchedule> employeeSchedulings = Common.EmployeeWorkScheduleHandler.findRecordByIDAndTime(maxTime, minTime, employeeID);
                    foreach (var employeeScheduling in employeeSchedulings)
                    {
                        //找到班次对应时间
                        DataModel.WorkShift workShift = Common.WorkShiftHandler.QueryWorkShiftByid(employeeScheduling.WorkShiftID);
                        if (workShift != null)
                        {
                            DateTime dt;
                            DateTime.TryParse(employeeScheduling.ScheduleDate, out dt);
                            if (string.Compare(workShift.WorkShiftStartTime, workShift.WorkShiftEndTime, true) == -1)
                            {
                                //同一天
                                employeeScheduling.startTime = Convert.ToDateTime(dt.ToString("yyyy-MM-dd") + " " + workShift.WorkShiftStartTime + ":00");
                                employeeScheduling.endTime = Convert.ToDateTime(dt.ToString("yyyy-MM-dd") + " " + workShift.WorkShiftEndTime + ":00");
                            }
                            else
                            {

                                employeeScheduling.startTime = Convert.ToDateTime(dt.ToString("yyyy-MM-dd") + " " + workShift.WorkShiftStartTime + ":00");
                                employeeScheduling.endTime = Convert.ToDateTime(dt.AddDays(1).ToString("yyyy-MM-dd") + " " + workShift.WorkShiftEndTime + ":00");
                            }
                            //当前时间谁值班
                            if (employeeScheduling.startTime <= now && employeeScheduling.endTime > now)
                            {
                                DataModel.Employee employee = Common.EmployeeHelper.QueryEmployeeByEmployeeID(employeeScheduling.EmployeeID);
                                if (employee != null)
                                {
                                    nowWorkShift.Date = Convert.ToDateTime(dt.ToString("yyyy-MM-dd"));
                                    nowWorkShift.WorkShiftID = employeeScheduling.WorkShiftID;
                                    return nowWorkShift;
                                }
                            }
                        }
                    }
                }
            }
            //通过组长查找
            List<DataModel.MachineResponsiblePerson> chargeAreas = Common.MachineResponsiblePersonHandler.findChargeAreaByMachineID(MC_machine._id);
            foreach (var item in chargeAreas)
            {
                //通过班次找到适合该时间的该员工
                //通过（员工，时间）找到今天和昨天的班次
                foreach (var employeeID in item.ResponsiblePersonID)
                {
                    List<DataModel.EmployeeWorkSchedule> employeeSchedulings = Common.EmployeeWorkScheduleHandler.findRecordByIDAndTime(maxTime, minTime, employeeID);
                    foreach (var employeeScheduling in employeeSchedulings)
                    {

                        //找到班次对应时间
                        DataModel.WorkShift workShift = Common.WorkShiftHandler.QueryWorkShiftByid(employeeScheduling.WorkShiftID);
                        if (workShift != null)
                        {
                            DateTime dt;
                            DateTime.TryParse(employeeScheduling.ScheduleDate, out dt);
                            if (string.Compare(workShift.WorkShiftStartTime, workShift.WorkShiftEndTime, true) == -1)
                            {
                                //同一天
                                employeeScheduling.startTime = Convert.ToDateTime(dt.ToString("yyyy-MM-dd") + " " + workShift.WorkShiftStartTime + ":00");
                                employeeScheduling.endTime = Convert.ToDateTime(dt.ToString("yyyy-MM-dd") + " " + workShift.WorkShiftEndTime + ":00");
                            }
                            else
                            {

                                employeeScheduling.startTime = Convert.ToDateTime(dt.ToString("yyyy-MM-dd") + " " + workShift.WorkShiftStartTime + ":00");
                                employeeScheduling.endTime = Convert.ToDateTime(dt.AddDays(1).ToString("yyyy-MM-dd") + " " + workShift.WorkShiftEndTime + ":00");
                            }
                            //当前时间谁值班
                            if (employeeScheduling.startTime <= now && employeeScheduling.endTime > now)
                            {
                                DataModel.Employee employee = Common.EmployeeHelper.QueryEmployeeByEmployeeID(employeeScheduling.EmployeeID);
                                if (employee != null)
                                {
                                    nowWorkShift.Date = Convert.ToDateTime(dt.ToString("yyyy-MM-dd"));
                                    nowWorkShift.WorkShiftID = employeeScheduling.WorkShiftID;
                                    return nowWorkShift;
                                }
                            }
                        }
                    }

                }

            }
            //通过排班查找
            //通过班次找到适合该时间的该员工
            //通过（员工，时间）找到今天和昨天的班次
            List<DataModel.EmployeeWorkSchedule> allemployeeSchedulings = Common.EmployeeWorkScheduleHandler.findRecordByTime(maxTime, minTime);
            foreach (var employeeScheduling in allemployeeSchedulings)
            {
                //找到班次对应时间
                DataModel.WorkShift workShift = Common.WorkShiftHandler.QueryWorkShiftByid(employeeScheduling.WorkShiftID);
                if (workShift != null)
                {
                    DateTime dt;
                    DateTime.TryParse(employeeScheduling.ScheduleDate, out dt);
                    if (string.Compare(workShift.WorkShiftStartTime, workShift.WorkShiftEndTime, true) == -1)
                    {
                        //同一天
                        employeeScheduling.startTime = Convert.ToDateTime(dt.ToString("yyyy-MM-dd") + " " + workShift.WorkShiftStartTime + ":00");
                        employeeScheduling.endTime = Convert.ToDateTime(dt.ToString("yyyy-MM-dd") + " " + workShift.WorkShiftEndTime + ":00");
                    }
                    else
                    {

                        employeeScheduling.startTime = Convert.ToDateTime(dt.ToString("yyyy-MM-dd") + " " + workShift.WorkShiftStartTime + ":00");
                        employeeScheduling.endTime = Convert.ToDateTime(dt.AddDays(1).ToString("yyyy-MM-dd") + " " + workShift.WorkShiftEndTime + ":00");
                    }
                    //当前时间谁值班
                    if (employeeScheduling.startTime <= now && employeeScheduling.endTime > now)
                    {
                        DataModel.Employee employee = Common.EmployeeHelper.QueryEmployeeByEmployeeID(employeeScheduling.EmployeeID);
                        if (employee != null)
                        {
                            nowWorkShift.Date = Convert.ToDateTime(dt.ToString("yyyy-MM-dd"));
                            nowWorkShift.WorkShiftID = employeeScheduling.WorkShiftID;
                            return nowWorkShift;
                        }
                    }
                }
            }
            //都没有的情况，默认用A1,A2
            DataModel.WorkShift a1 = Common.WorkShiftHandler.QueryWorkShiftByCode("A1");
            DataModel.WorkShift a2 = Common.WorkShiftHandler.QueryWorkShiftByCode("A2");
            DateTime startTime=new DateTime();
            DateTime endTime= new DateTime();
            //同一天
            if (a1 != null)
            {
                //比较开始结束时间
                if (string.Compare(a1.WorkShiftStartTime, a1.WorkShiftEndTime, true) == -1)
                {
                    //a1白班，a2夜班
                    //同一天
                    startTime = Convert.ToDateTime(now.ToString("yyyy-MM-dd") + " " + a1.WorkShiftStartTime + ":00");
                    endTime = Convert.ToDateTime(now.ToString("yyyy-MM-dd") + " " + a1.WorkShiftEndTime + ":00");
                    if (a2 != null)
                    {
                        //前一天夜班
                        if (now < startTime)
                        {
                            nowWorkShift.Date = Convert.ToDateTime(now.AddDays(-1).ToString("yyyy-MM-dd"));
                            nowWorkShift.WorkShiftID = a2._id;
                            return nowWorkShift;
                        }
                        //今天夜班
                        else if (now >= endTime)
                        {
                            nowWorkShift.Date = Convert.ToDateTime(now.ToString("yyyy-MM-dd"));
                            nowWorkShift.WorkShiftID = a2._id;
                            return nowWorkShift;
                        }
                        //今天白班
                        else
                        {
                            nowWorkShift.Date = Convert.ToDateTime(now.ToString("yyyy-MM-dd"));
                            nowWorkShift.WorkShiftID = a1._id;
                            return nowWorkShift;
                        }
                    }
                   
                }
                else if (a2 != null)
                {
                    //a1夜班，a2白班
                    if (string.Compare(a2.WorkShiftStartTime, a2.WorkShiftEndTime, true) == -1)
                    {
                        startTime = Convert.ToDateTime(now.ToString("yyyy-MM-dd") + " " + a2.WorkShiftStartTime + ":00");
                        endTime = Convert.ToDateTime(now.ToString("yyyy-MM-dd") + " " + a2.WorkShiftEndTime + ":00");
                    }
                    //前一天夜班
                    if (now < startTime)
                    {
                        nowWorkShift.Date = Convert.ToDateTime(now.AddDays(-1).ToString("yyyy-MM-dd"));
                        nowWorkShift.WorkShiftID = a1._id;
                        return nowWorkShift;
                    }
                    //今天夜班
                    else if (now >= endTime)
                    {
                        nowWorkShift.Date = Convert.ToDateTime(now.ToString("yyyy-MM-dd"));
                        nowWorkShift.WorkShiftID = a1._id;
                        return nowWorkShift;
                    }
                    //今天白班
                    else
                    {
                        nowWorkShift.Date = Convert.ToDateTime(now.ToString("yyyy-MM-dd"));
                        nowWorkShift.WorkShiftID = a2._id;
                        return nowWorkShift;
                    }
                }



            }

            return null;
           

        }
        //新增记录获取员工
        public List<DataModel.EmployeeProductionTimeList> getEmployeeProductionTimeList(DateTime startTime)
        {
            Common.ClockInRecordHandler clockInRecordHandler = new Common.ClockInRecordHandler();
            List<DataModel.ClockInRecord> displayClockInRecords = clockInRecordHandler.GetClockInRecordList();
            List<DataModel.EmployeeProductionTimeList> employeeProductionTimeLists = new List<DataModel.EmployeeProductionTimeList>();
          
            foreach (var item in displayClockInRecords)
            {
                DataModel.Employee employee = Common.EmployeeHelper.QueryEmployeeByEmployeeID(item.EmployeeID);
                if (employee != null)
                {
                    DataModel.JobPositon jobPositon = Common.JobPositionHelper.GetJobPositon(employee.JobPostionID);
                    if (jobPositon != null)
                    {
                        if (jobPositon.JobPositionCode == frmAttend.JobPositionCode.Employee.ToString())
                        {
                            DataModel.EmployeeProductionTimeList employeeProductionTimeList = new DataModel.EmployeeProductionTimeList();
                            employeeProductionTimeList.EmployeeID = employee._id;
                            employeeProductionTimeList.StartTime = startTime;
                            employeeProductionTimeList.EndTime = startTime;
                            employeeProductionTimeList.WorkHour = 0;
                            employeeProductionTimeLists.Add(employeeProductionTimeList);
                        }
                    }
                   
                }
            }
            return employeeProductionTimeLists;
        }
        //更新不良品
        private void UpdateErrorCount(DataModel.JobOrder_MachineProcessLog jobOrder_MachineProcessLog, int count)
        {
            try
            {
                //找到当前工单生产记录Id
                List<DataModel.MachineProduction> machineProductions = machineProductionHandler.findRecordByProcessID(jobOrder_MachineProcessLog._id, CurrentProcessJobOrder._id);
                int errorCount = 0;
                //最后记录不算
                for (int i = 0; i < machineProductions.Count - 1; i++)
                {
                    errorCount = errorCount + machineProductions[i].JobOrderProductionLog[machineProductions[i].JobOrderProductionLog.Count - 1].ErrorCount;
                }
                errorCount = count - errorCount;
                //更新最后一条记录
                if (machineProductions.Count > 0)
                {
                    DataModel.MachineProduction machineProduction = machineProductions[machineProductions.Count - 1];
                    machineProduction.JobOrderProductionLog[machineProduction.JobOrderProductionLog.Count - 1].ErrorCount = errorCount;
                    machineProduction.ErrorCount = 0;
                    foreach (var item in machineProduction.JobOrderProductionLog)
                    {
                        machineProduction.ErrorCount = machineProduction.ErrorCount+item.ErrorCount;
                    }
                    machineProductionHandler.UpdateError(machineProduction);
                }
                else
                {
                    ProcessMachineProduction(CurrentProcessJobOrder, 0);
                }
            }
              catch (Exception ex)
            {
                throw ex;
            }

        }
        //同摸具同产品逐一处理，同摸具不同产品一起处理
        private void ProcessMouldLifeCycle()
        {
            try
            {

                CurrentMouldProduct = Common.MouldProductHelper.GetMmouldProductByMouldCode(CurrentProcessJobOrder.MouldCode);
                if (CurrentMouldProduct == null)
                {
                    //未处理进行加1处理
                    OneProcessMouldLifeCycle(ProcessJobOrderList);
                }
                else
                {
                    //记录未处理工单
                    List<DataModel.JobOrder> unProcessJobOrderList = new List<DataModel.JobOrder>();
                    foreach (var jobOrderItem in ProcessJobOrderList)
                    {
                        unProcessJobOrderList.Add(jobOrderItem);
                    }
                    foreach (var MouldProductItem in CurrentMouldProduct.ProductList)
                    {
                        //查询出对应的工单先，查看工单有多少个
                        List<DataModel.JobOrder> findJobOrderListByProductCode = ProcessJobOrderList.FindAll(t => t.ProductCode.ToUpper() == MouldProductItem.ProductCode.ToUpper());


                        if (findJobOrderListByProductCode.Count > 0)
                        {
                            //是不是找到没有生产完的工单？？
                            bool isFindNoOverJobOrder = false;

                            //循环工单，看看是不是已经满数
                            for (int i = 0; i < findJobOrderListByProductCode.Count; i++)
                            {
                                //找到工单
                                var getFilterJobOrder = findJobOrderListByProductCode[i];

                                //移除已处理工单
                                unProcessJobOrderList.Remove(getFilterJobOrder);

                                //找到它本身的生产数量、不良品数量（所有机器都算进来）
                                var sumProductCount = getFilterJobOrder.MachineProcessLog.Sum(t => t.ProduceCount);
                                var sumErrorCount = getFilterJobOrder.MachineProcessLog.Sum(t => t.ErrorCount);

                                if (getFilterJobOrder.OrderCount > (sumProductCount - sumErrorCount))
                                {
                                    //还没有生产完
                                    isFindNoOverJobOrder = true;

                                    //处理加1
                                    ProcessJobOrderMachineProduceCount(getFilterJobOrder, MouldProductItem.ProductCount);
                                    //同产品不同工单，不加1处理
                                    for (int j = i + 1; j < findJobOrderListByProductCode.Count; j++)
                                    {
                                        unProcessJobOrderList.Remove(findJobOrderListByProductCode[j]);
                                    }
                                    break;
                                }
                            }


                            //全部都够数了？？（即从未加数）
                            if (isFindNoOverJobOrder == false)
                            {
                                AutoChangeJobOrder(findJobOrderListByProductCode, MouldProductItem.ProductCount);
                            }
                        }
                    }
                    //处理找不到产品出数
                    ProcessNoMouldProduc(unProcessJobOrderList, CurrentMouldProduct.ProductList);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //处理找不到产品出数
        private void ProcessNoMouldProduc(List<DataModel.JobOrder> unProcessJobOrderList,List<DataModel.MouldProductList> mouldProductList)
        {
            if (unProcessJobOrderList.Count > 0)
            {
                foreach (var item in mouldProductList)
                {

                    List<DataModel.JobOrder> findJobOrderListByProductCode = unProcessJobOrderList.FindAll(t => getPrefix(t.ProductCode.ToUpper()) == getPrefix(item.ProductCode.ToUpper()));
                    if (findJobOrderListByProductCode.Count > 0)
                    {
                        //是不是找到没有生产完的工单？？
                        bool isFindNoOverJobOrder = false;

                        //循环工单，看看是不是已经满数
                        for (int i = 0; i < findJobOrderListByProductCode.Count; i++)
                        {
                            //找到工单
                            var getFilterJobOrder = findJobOrderListByProductCode[i];

                            //移除已处理工单
                            unProcessJobOrderList.Remove(getFilterJobOrder);

                            //找到它本身的生产数量、不良品数量（所有机器都算进来）
                            var sumProductCount = getFilterJobOrder.MachineProcessLog.Sum(t => t.ProduceCount);
                            var sumErrorCount = getFilterJobOrder.MachineProcessLog.Sum(t => t.ErrorCount);

                            if (getFilterJobOrder.OrderCount > (sumProductCount - sumErrorCount))
                            {
                                //还没有生产完
                                isFindNoOverJobOrder = true;

                                //处理加1
                                ProcessJobOrderMachineProduceCount(getFilterJobOrder, item.ProductCount);
                                //同产品不同工单，不加1处理
                                for (int j = i + 1; j < findJobOrderListByProductCode.Count; j++)
                                {
                                    unProcessJobOrderList.Remove(findJobOrderListByProductCode[j]);
                                }
                                break;
                            }
                        }

                        //全部都够数了？？（即从未加数）
                        if (isFindNoOverJobOrder == false)
                        {
                            AutoChangeJobOrder(findJobOrderListByProductCode, item.ProductCount);
                        }
                    }
                }
            }
            //未处理进行加1处理
            OneProcessMouldLifeCycle(unProcessJobOrderList);
        }
        //获取产品前缀
        public string getPrefix(string productCode)
        {
            //能找到且位置不为0
            if (productCode.LastIndexOf('-') > 0)
            {
                return productCode.Substring(0, productCode.LastIndexOf('-') - 0);
            }
            else
            {
                return productCode;
            }
        }
        //加1处理
        private void OneProcessMouldLifeCycle(List<DataModel.JobOrder> OneProcessJobOrderList)
        {
            //不同产品工单
            List<DataModel.JobOrder> distinctJobOrderList = new List<DataModel.JobOrder>();
            foreach (var jobOrderItem in OneProcessJobOrderList)
            {

                if (!distinctJobOrderList.Exists(t => t.ProductCode == jobOrderItem.ProductCode))
                {
                    distinctJobOrderList.Add(jobOrderItem);
                }
            }
            foreach (var jobOrderItem in distinctJobOrderList)
            {
                //查询出对应的工单先，查看工单有多少个
                List<DataModel.JobOrder> findJobOrderListByProductCode = OneProcessJobOrderList.FindAll(t => t.ProductCode.ToUpper() == jobOrderItem.ProductCode.ToUpper());
                if (findJobOrderListByProductCode.Count > 0)
                {
                    //是不是找到没有生产完的工单？？
                    bool isFindNoOverJobOrder = false;

                    //循环工单，看看是不是已经满数
                    for (int i = 0; i < findJobOrderListByProductCode.Count; i++)
                    {
                        //找到工单
                        var getFilterJobOrder = findJobOrderListByProductCode[i];
                        //找到它本身的生产数量、不良品数量（所有机器都算进来）
                        var sumProductCount = getFilterJobOrder.MachineProcessLog.Sum(t => t.ProduceCount);
                        var sumErrorCount = getFilterJobOrder.MachineProcessLog.Sum(t => t.ErrorCount);

                        if (getFilterJobOrder.OrderCount > (sumProductCount - sumErrorCount))
                        {
                            //还没有生产完
                            isFindNoOverJobOrder = true;

                            //处理加1
                            ProcessJobOrderMachineProduceCount(getFilterJobOrder, 1);
                            break;
                        }
                    }
                    //全部都够数了？？（即从未加数）
                    if (isFindNoOverJobOrder == false)
                    {
                        AutoChangeJobOrder(findJobOrderListByProductCode,1);
                    }
                }
            }
        }

        //自动切换工单
        private void AutoChangeJobOrder(List<DataModel.JobOrder> jobOrderList, int productCount)
        {
            //判断是否存在同产品同模具并且还有未完成数量并且处于未开始或者暂停中，否，找到最后一个工单，强行加数
            //是，加入新工单
            DateTime now = DateTime.Now;
            List<DataModel.JobOrder> findJobOrderList = JobOrderHelper.GetJobOrderByMouldCodeAndProductCode(jobOrderList[0].MouldCode, jobOrderList[0].ProductCode);
            if (findJobOrderList.Count() > 0)
            {

                //完成工单
                //更新至数据库

                //保存员工
                string lastOperaterID = jobOrderList[0].MachineProcessLog.Find(t => t.MachineID == MC_machine._id && t.ProduceStartDate == t.ProduceEndDate).EmployeeID;
                foreach (DataModel.JobOrder jobOrderItem in jobOrderList)
                {

                    //找到没结束的处理记录
                    var findMachineProcessLog = jobOrderItem.MachineProcessLog.Find(t => t.MachineID == MC_machine._id && t.ProduceStartDate == t.ProduceEndDate);
                    //结束时间为当前时间
                    findMachineProcessLog.ProduceEndDate = now;

                    jobOrderItem.Status = Common.JobOrderStatus.eumJobOrderStatus.Completed.ToString();
                    //完成时间及操作人
                    jobOrderItem.CompletedDate = now;
                    //取上一张工单选择人为完成员工
                    jobOrderItem.CompletedOperaterID = lastOperaterID;

                    //不需要返回值，并更新回class
                    JobOrderHelper.UpdateJobOrder(jobOrderItem, false);
                    //停止每小时计数
                    stopAdd(jobOrderItem, now);
                    ProcessJobOrderList.Remove(jobOrderItem);

                }
                //开始新工单
                DataModel.JobOrder_MachineProcessLog newJobOrder_MachineProcessLog = new DataModel.JobOrder_MachineProcessLog();
                newJobOrder_MachineProcessLog._id = ObjectId.GenerateNewId().ToString();

                newJobOrder_MachineProcessLog.MachineID = MC_machine._id;

                //开始及结束取同一个值
                DateTime orderStartDate = now;
                newJobOrder_MachineProcessLog.ProduceStartDate = orderStartDate;
                newJobOrder_MachineProcessLog.ProduceEndDate = orderStartDate;

                newJobOrder_MachineProcessLog.ProduceCount = 0;
                newJobOrder_MachineProcessLog.ErrorCount = 0;
                //取上一张工单选择人为完成员工
                newJobOrder_MachineProcessLog.EmployeeID = lastOperaterID;

                //机器处理记录
                findJobOrderList[0].MachineProcessLog.Add(newJobOrder_MachineProcessLog);



                //更新至数据库

                findJobOrderList[0].Status = Common.JobOrderStatus.eumJobOrderStatus.Producing.ToString();

                //需要返回值，并更新回class
                DataModel.JobOrder jobOrder = JobOrderHelper.UpdateJobOrder(findJobOrderList[0], true);
               

                ProcessJobOrderList.Add(findJobOrderList[0]);
                CurrentProcessJobOrder = findJobOrderList[0];

                //添加新记录
                ProcessMachineProduction(jobOrder, 0);
                //界面显示基本消息
                SettingJobOrderBasicInfo();

                //计算未完成数量
                SettingMachineNondefectiveCount();

                //计算预计完成时间
                SettingMachineCompleteDateTime();



                //找到它本身的生产数量、不良品数量（所有机器都算进来）
                var sumProductCount = findJobOrderList[0].MachineProcessLog.Sum(t => t.ProduceCount);
                var sumErrorCount = findJobOrderList[0].MachineProcessLog.Sum(t => t.ErrorCount);
                //不够数，则加，够数则切换到下一张工单
                if (findJobOrderList[0].OrderCount > (sumProductCount - sumErrorCount))
                {
                    ProcessJobOrderMachineProduceCount(findJobOrderList[0], productCount);
                }
                else
                {
                    List<DataModel.JobOrder> jobOrders = new List<DataModel.JobOrder>();
                    jobOrders.Add(findJobOrderList[0]);
                    AutoChangeJobOrder(jobOrders, productCount);
                }
            }

            else
            {
                //找到最后一个工单，强行加数
                var getFilterJobOrder = jobOrderList[jobOrderList.Count - 1];
                //处理加1
                ProcessJobOrderMachineProduceCount(getFilterJobOrder, productCount);
            }

        }
    }
}
