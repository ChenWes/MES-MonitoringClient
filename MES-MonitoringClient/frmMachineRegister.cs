using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;


namespace MES_MonitoringClient
{
    public partial class frmMachineRegister : Form
    {
        private IMongoCollection<DataModel.MachineInfo> machineRegisterCollection;
        private static string defaultMachineRegisterMongodbCollectionName = Common.ConfigFileHandler.GetAppConfig("MachineRegisterCollectionName");


        //修改的状态
        public DataModel.MachineInfo mc_MachineInfo;

        public frmMachineRegister()
        {
            InitializeComponent();
        }
        private void frmMachineRegister_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;            
        }


        private void ShowErrorMessage(string errorMessage, string errorTitle)
        {
            MessageBox.Show(errorMessage, errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /**-------------------------------------------------------------*/
        private void btn_Query_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txt_MachineID.Text)) throw new Exception("机器注册码为空");


                //机器ID验证注册，拿回Json数据（无论是正常的还是不正常的）
                Common.MachineRegisterInfoHelper machineRegisterInfoHelperClass = new Common.MachineRegisterInfoHelper();
                string getJsonString = machineRegisterInfoHelperClass.MachineIDCheck(txt_MachineID.Text.Trim().Replace("-", ""));


                if (!string.IsNullOrEmpty(getJsonString))
                {
                    //转换成Class，显示至界面上
                    mc_MachineInfo = new DataModel.MachineInfo();

                    //机器本身信息
                    mc_MachineInfo.MachineID = Common.JsonHelper.GetJsonValue(getJsonString, "_id");
                    mc_MachineInfo.MachineCode = Common.JsonHelper.GetJsonValue(getJsonString, "MachineCode");
                    mc_MachineInfo.MachineName = Common.JsonHelper.GetJsonValue(getJsonString, "MachineName");
                    mc_MachineInfo.MachineDesc = Common.JsonHelper.GetJsonValue(getJsonString, "MachineDesc");
                    mc_MachineInfo.Tonnage = Common.JsonHelper.GetJsonValue(getJsonString, "Tonnage");
                    mc_MachineInfo.MACAddress = Common.CommonFunction.getMacAddress();

                    //车间信息
                    string workshopJsonString = Common.JsonHelper.GetJsonValue(getJsonString, "WorkshopID");
                    mc_MachineInfo.WorkshopCode = Common.JsonHelper.GetJsonValue(workshopJsonString, "WorkshopCode");
                    mc_MachineInfo.WorkshopName = Common.JsonHelper.GetJsonValue(workshopJsonString, "WorkshopName");

                    //工厂信息
                    string factoryJsonString = Common.JsonHelper.GetJsonValue(workshopJsonString, "FactoryID");
                    mc_MachineInfo.FactoryCode = Common.JsonHelper.GetJsonValue(factoryJsonString, "FactoryCode");
                    mc_MachineInfo.FactoryName = Common.JsonHelper.GetJsonValue(factoryJsonString, "FactoryName");


                    //显示至界面中
                    txt_MachineCode.Text = mc_MachineInfo.MachineCode;
                    txt_MachineName.Text = mc_MachineInfo.MachineName;
                    txt_MachineDesc.Text = mc_MachineInfo.MachineDesc;
                    txt_Tonnage.Text = mc_MachineInfo.Tonnage;
                    txt_MACAddress.Text = mc_MachineInfo.MACAddress;

                    txt_Workshop.Text = mc_MachineInfo.WorkshopName + "(" + mc_MachineInfo.WorkshopCode + ")";
                    txt_Factory.Text = mc_MachineInfo.FactoryName + "(" + mc_MachineInfo.FactoryCode + ")";

                    //显示绿色
                    txt_MachineID.BackColor = Color.Green;                    
                }
            }
            catch (Exception ex)
            {
                //显示红色
                txt_MachineID.BackColor = Color.Red;                

                ShowErrorMessage(ex.Message, "查询注册码出错");
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            mc_MachineInfo = null;

            this.Close();
        }

        private void btn_Confirm_Click(object sender, EventArgs e)
        {
            try
            {
                //清空数据实体
                mc_MachineInfo = null;

                //机器ID注册，拿回Json数据（无论是正常的还是不正常的）
                Common.MachineRegisterInfoHelper machineRegisterInfoHelperClass = new Common.MachineRegisterInfoHelper();
                string getJsonString = machineRegisterInfoHelperClass.MachineRegister(txt_MachineID.Text.Trim().Replace("-", ""));

                if (!string.IsNullOrEmpty(getJsonString))
                {
                    //转换成Class，显示至界面上
                    mc_MachineInfo = new DataModel.MachineInfo();

                    //机器本身信息
                    mc_MachineInfo.MachineID = Common.JsonHelper.GetJsonValue(getJsonString, "_id");
                    mc_MachineInfo.MachineCode = Common.JsonHelper.GetJsonValue(getJsonString, "MachineCode");
                    mc_MachineInfo.MachineName = Common.JsonHelper.GetJsonValue(getJsonString, "MachineName");
                    mc_MachineInfo.MachineDesc = Common.JsonHelper.GetJsonValue(getJsonString, "MachineDesc");
                    mc_MachineInfo.Tonnage = Common.JsonHelper.GetJsonValue(getJsonString, "Tonnage");
                    mc_MachineInfo.MACAddress = Common.JsonHelper.GetJsonValue(getJsonString, "MACAddress");

                    //车间信息
                    string workshopJsonString = Common.JsonHelper.GetJsonValue(getJsonString, "WorkshopID");
                    mc_MachineInfo.WorkshopCode = Common.JsonHelper.GetJsonValue(workshopJsonString, "WorkshopCode");
                    mc_MachineInfo.WorkshopName = Common.JsonHelper.GetJsonValue(workshopJsonString, "WorkshopName");

                    //工厂信息
                    string factoryJsonString = Common.JsonHelper.GetJsonValue(workshopJsonString, "FactoryID");
                    mc_MachineInfo.FactoryCode = Common.JsonHelper.GetJsonValue(factoryJsonString, "FactoryCode");
                    mc_MachineInfo.FactoryName = Common.JsonHelper.GetJsonValue(factoryJsonString, "FactoryName");
                }

                if (mc_MachineInfo == null)
                {
                    ShowErrorMessage("请输入正确的机器注册码", "机器注册错误");
                }
                else
                {
                    //先同步数据
                    bool syncdata_Flag = Common.SyncDataHelper.SyncData_AllCollection();
                    if (syncdata_Flag)
                    {
                        //后写入注册数据
                        machineRegisterCollection = Common.MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.MachineInfo>(defaultMachineRegisterMongodbCollectionName);
                        machineRegisterCollection.InsertOne(mc_MachineInfo);

                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message, "机器注册出错");
            }

        }

        private void txt_MachineID_KeyPress(object sender, KeyPressEventArgs e)
        {
            txt_MachineID.BackColor = Color.White;

            txt_MachineCode.Text = "";
            txt_MachineName.Text = "";
            txt_MachineDesc.Text = "";

            txt_MACAddress.Text = "";
            txt_Workshop.Text = "";
            txt_Factory.Text = "";
        }

        private void txt_MachineID_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            txt_MachineID.BackColor = Color.White;            
        }
    }
}
