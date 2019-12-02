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



        /*工单相关********************************************************************************************************************************/
        public void StopJobOrder()
        {
            try
            {
				
                List<DataModel.JobOrder> newJobOrderList = new List<DataModel.JobOrder>();

				if (ProcessJobOrderList != null && ProcessJobOrderList.Count > 0)
				{
					//更新至数据库
					foreach (DataModel.JobOrder jobOrderItem in ProcessJobOrderList)
					{
                        //找到没结束的处理记录
                        var findMachineProcessLog = jobOrderItem.MachineProcessLog.Find(t => t.MachineID == MC_machine._id && t.ProduceStartDate==t.ProduceEndDate);
                        //结束时间为当前时间
                        findMachineProcessLog.ProduceEndDate = System.DateTime.Now;

                        jobOrderItem.Status = Common.JobOrderStatus.eumJobOrderStatus.Suspend.ToString();

						//需要返回值，并更新回class
						DataModel.JobOrder jobOrder = JobOrderHelper.UpdateJobOrder(jobOrderItem, true);
						newJobOrderList.Add(jobOrder);
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
                //更新至数据库
                foreach (DataModel.JobOrder jobOrderItem in ProcessJobOrderList)
                {

                    //找到没结束的处理记录
                    var findMachineProcessLog = jobOrderItem.MachineProcessLog.Find(t => t.MachineID == MC_machine._id && t.ProduceStartDate == t.ProduceEndDate);
                    //结束时间为当前时间
                    findMachineProcessLog.ProduceEndDate = System.DateTime.Now;

                    jobOrderItem.Status = Common.JobOrderStatus.eumJobOrderStatus.Completed.ToString();
                    //完成时间及操作人
                    jobOrderItem.CompletedDate = System.DateTime.Now;
                    jobOrderItem.CompletedOperaterID = operaterID;

                    //需要返回值，并更新回class
                    DataModel.JobOrder jobOrder = JobOrderHelper.UpdateJobOrder(jobOrderItem, true);
                    newJobOrderList.Add(jobOrder);
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
                //处理生产中的工单
                StopOtherJobOrder();
                //传入的工单参数
                ProcessJobOrderList = jobOrderList;

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

                //更新至数据库
                foreach (DataModel.JobOrder jobOrderItem in jobOrderList)
                {
                    jobOrderItem.Status = Common.JobOrderStatus.eumJobOrderStatus.Producing.ToString();

                    //需要返回值，并更新回class
                    DataModel.JobOrder jobOrder = JobOrderHelper.UpdateJobOrder(jobOrderItem, true);
                    newJobOrderList.Add(jobOrder);
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
        public void StopOtherJobOrder()
        {
            try
            {
                //处理生产中的工单
                List<DataModel.JobOrder> startedJobOrders = ((List<DataModel.JobOrder>)Common.JobOrderHelper.GetAllJobOrder()).FindAll(x => x.Status == "Producing");
                //更新至数据库
                foreach (DataModel.JobOrder jobOrderItem in startedJobOrders)
                {
                    //找到没结束的处理记录
                    var findMachineProcessLog = jobOrderItem.MachineProcessLog.Find(t => t.MachineID == MC_machine._id && t.ProduceStartDate == t.ProduceEndDate);
                    //结束时间为当前时间
                    findMachineProcessLog.ProduceEndDate = System.DateTime.Now;
                    jobOrderItem.Status = Common.JobOrderStatus.eumJobOrderStatus.Suspend.ToString();

                    //需要返回值，并更新回class
                    DataModel.JobOrder jobOrder = JobOrderHelper.UpdateJobOrder(jobOrderItem, false);
                }
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
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void ProcessMouldLifeCycle()
        {
            try
            {
                if (CurrentMouldProduct == null)
                {
                    //每次处理数量                                    
                    foreach (var jobOrderItem in ProcessJobOrderList)
                    {
                        //处理加1
                        ProcessJobOrderMachineProduceCount(jobOrderItem, 1);
                    }
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

                                if (getFilterJobOrder.OrderCount >= (sumProductCount - sumErrorCount))
                                {
                                    //还没有生产完
                                    isFindNoOverJobOrder = true;

                                    //处理加1
                                    ProcessJobOrderMachineProduceCount(getFilterJobOrder, MouldProductItem.ProductCount);

                                    break;
                                }
                            }


                            //全部都够数了？？（即从未加数）
                            if (isFindNoOverJobOrder == false)
                            {
                                //找到最后一个工单，强行加数
                                var getFilterJobOrder = findJobOrderListByProductCode[findJobOrderListByProductCode.Count - 1];
                                //处理加1
                                ProcessJobOrderMachineProduceCount(getFilterJobOrder, MouldProductItem.ProductCount);
                            }
                        }
                    }
                    foreach (var jobOrderItem in unProcessJobOrderList)
                    {
                        //处理加1
                        ProcessJobOrderMachineProduceCount(jobOrderItem, 1);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
