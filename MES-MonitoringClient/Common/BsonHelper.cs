using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace MES_MonitoringClient.Common
{
    public class BsonHelper
    {
        /// <summary>
        /// 需要转换成BsonObjectId类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static BsonObjectId[] ConvertType(string[] id)
        {
            try
            {
                List<BsonObjectId> bsonObjectIds = new List<BsonObjectId>();

                if (id == null && id.Length <= 0) return bsonObjectIds.ToArray();

                for (int i = 0; i < id.Length; i++)
                {
                    bsonObjectIds.Add(new BsonObjectId(id[i]));
                }

                return bsonObjectIds.ToArray();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static BsonObjectId ConvertType(string id)
        {
            try
            {
                return new BsonObjectId(id);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
