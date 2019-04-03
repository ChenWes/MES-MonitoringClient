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
                //查询方法，调用HTTP，注册至DB
                Common.HttpHelper httpHelperClass = new Common.HttpHelper();
                string l_machineRegisterUrlPath = Common.ConfigFileHandler.GetAppConfig("MachineRegisterUrlPath");

                //拿到返回的JSON数据（无论是正常的还是不正常的）
                string getJsonString = httpHelperClass.HttpPost(l_machineRegisterUrlPath, txt_MachineID.Text.Trim());

                if (!string.IsNullOrEmpty(getJsonString))
                {
                    //转换成Class，显示至界面上
                    mc_MachineInfo = new DataModel.MachineInfo();

                    mc_MachineInfo.MachineID = Common.JsonHelper.GetJsonValue(getJsonString, "_id");
                    mc_MachineInfo.MachineCode = Common.JsonHelper.GetJsonValue(getJsonString, "MachineCode");
                    mc_MachineInfo.MachineName = Common.JsonHelper.GetJsonValue(getJsonString, "MachineName");
                    mc_MachineInfo.MachineDesc = Common.JsonHelper.GetJsonValue(getJsonString, "MachineDesc");
                    mc_MachineInfo.MACAddress = Common.JsonHelper.GetJsonValue(getJsonString, "MACAddress");


                    string workshopJsonString= Common.JsonHelper.GetJsonValue(getJsonString, "WorkshopID");
                    mc_MachineInfo.WorkshopCode= Common.JsonHelper.GetJsonValue(workshopJsonString, "WorkshopCode");
                    mc_MachineInfo.WorkshopName = Common.JsonHelper.GetJsonValue(workshopJsonString, "WorkshopName");


                    string factoryJsonString= Common.JsonHelper.GetJsonValue(workshopJsonString, "FactoryID");
                    mc_MachineInfo.FactoryCode = Common.JsonHelper.GetJsonValue(factoryJsonString, "FactoryCode");
                    mc_MachineInfo.FactoryName = Common.JsonHelper.GetJsonValue(factoryJsonString, "FactoryName");


                    //显示至界面中
                    txt_MachineCode.Text = mc_MachineInfo.MachineCode;
                    txt_MachineName.Text = mc_MachineInfo.MachineName;
                    txt_MachineDesc.Text = mc_MachineInfo.MachineDesc;
                    txt_MACAddress.Text = mc_MachineInfo.MACAddress;

                    txt_Workshop.Text = mc_MachineInfo.WorkshopName + "("+mc_MachineInfo.WorkshopCode+")";
                    txt_Factory.Text = mc_MachineInfo.FactoryName + "(" + mc_MachineInfo.FactoryCode + ")";
                }
            }
            catch (Exception ex)
            {
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
            if (mc_MachineInfo == null)
            {
                ShowErrorMessage("请输入正确的机器注册码", "机器注册错误");
            }
            else
            {
                machineRegisterCollection = Common.MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.MachineInfo>(defaultMachineRegisterMongodbCollectionName);

                machineRegisterCollection.InsertOne(mc_MachineInfo);

                this.Close();
            }
        }
    }
}
