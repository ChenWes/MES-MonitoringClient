using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_MonitoringService.Model
{
    public abstract class SyncData
    {

        /// <summary>
        /// 获取当前实体对应的Collection数据集名称
        /// </summary>
        /// <returns></returns>
        public abstract string getCollectionName();
    }
}
