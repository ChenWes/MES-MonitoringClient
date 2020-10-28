using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_MonitoringClient.Common
{
    public class QCRecordHandler
    {
        /*MongoDB数据库*/
        /*-------------------------------------------------------------------------------------*/
        /// <summary>
        ///  QC记录Mongodb集合
        /// </summary>
        private static IMongoCollection<DataModel.QCRecord> QCRecordCollection;

        /// <summary>
        /// QC记录默认Mongodb集合名
        /// </summary>
        private static string defaultQCRecordMongodbCollectionName = "QCRecord";

        /// <summary>
        /// 构造函数，处理初始化的参数
        /// </summary>
        public QCRecordHandler()
        {
            QCRecordCollection = MongodbHandler.GetInstance().mc_MongoDatabase.GetCollection<DataModel.QCRecord>(defaultQCRecordMongodbCollectionName);
        }


        /// <summary>
        /// QC记录保存
        /// </summary>
        /// <param name="clockInRecord">QC记录保存</param>
        public  void SaveClockInRecord(DataModel.QCRecord QCRecord)
        {
            QCRecordCollection.InsertOne(QCRecord);

        }
    }
}
