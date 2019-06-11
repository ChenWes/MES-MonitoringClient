using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MES_MonitoringClient
{
    public partial class frmChangeStatus : Form
    {
        //修改的状态
        public DataModel.formParameter.frmChangeMachineStatusPara MC_frmChangeMachineStatusPara = null;

        public List<DataModel.MachineStatus> mc_machineStatuses;
        private char splitMachineStatusPara = '&';

        /*窗口公共方法*/
        /*---------------------------------------------------------------------------------------*/

        public frmChangeStatus()
        {
            InitializeComponent();
        }

        private void frmChangeStatus_Load(object sender, EventArgs e)
        {
            try
            {
                //窗口最大化
                this.WindowState = FormWindowState.Maximized;

                //未选择机器状态时，确认按钮不可用
                btn_Confirm.Enabled = false;

                //获取所有可用的机器状态               
                foreach (var item in mc_machineStatuses)
                {
                    //声明一个按钮
                    Button circularButton = new Button
                    {
                        Text = item.MachineStatusName,
                        Anchor = AnchorStyles.None,
                        Size = new Size() { Height = 80, Width = 150 },
                        BackColor = Common.CommonFunction.colorHx16toRGB(item.StatusColor),
                        FlatStyle = FlatStyle.Flat,
                        ForeColor = Color.White,
                        Tag = item._id + splitMachineStatusPara + item.MachineStatusCode + splitMachineStatusPara + item.MachineStatusName + splitMachineStatusPara + item.MachineStatusDesc + splitMachineStatusPara + item.StatusColor

                    };
                    circularButton.Click += new System.EventHandler(btn_click);

                    //将可用的机器状态动态用按钮的形式加载到界面中
                    this.tableLayoutPanel1.Controls.Add(circularButton, 0, 1);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //解决界面闪烁的问题
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }


        /*窗口公共方法*/
        /*---------------------------------------------------------------------------------------*/

        /// <summary>
        /// 显示系统错误信息
        /// </summary>
        /// <param name="errorTitle">错误标题</param>
        /// <param name="errorMessage">错误</param>
        private void ShowErrorMessage(string errorMessage, string errorTitle)
        {
            MessageBox.Show(errorMessage, errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 机器状态动太按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_click(object sender, System.EventArgs e)
        {
            try
            {
                //将触发此事件的对象转换为该Button对象
                Button b1 = (Button)sender;

                //分隔
                string propsString = b1.Tag.ToString();
                string[] props = propsString.Split(splitMachineStatusPara);

                //将机器状态变成一个公共访问的参数
                MC_frmChangeMachineStatusPara = new DataModel.formParameter.frmChangeMachineStatusPara()
                {
                    machineStatusID = props[0],
                    machineStatusCode=props[1],
                    machineStatusName = props[2],
                    machineStatusDesc = props[3],

                    machineStatusColor = props[4],
                };        

                lab_SelectStatus.Text= "已选机器状态："+ b1.Text;

                //选择机器状态后，确认按钮可用
                btn_Confirm.Enabled = true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /*按钮方法*/
        /*---------------------------------------------------------------------------------------*/
        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            MC_frmChangeMachineStatusPara = null;

            this.Close();
        }

        private void btn_Confirm_Click(object sender, EventArgs e)
        {
            if (MC_frmChangeMachineStatusPara == null)
            {
                ShowErrorMessage("请选择要操作的状态", "改变机器状态");
            }
            else
            {
                this.Close();
            }
        }

    }
}
