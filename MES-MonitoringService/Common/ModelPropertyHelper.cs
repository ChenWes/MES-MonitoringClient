using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;

namespace MES_MonitoringService.Common
{
    public class ModelPropertyHelper<T>
    {
        /// <summary>
        /// 将类属性设置为MongoDB属性
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public UpdateDefinition<BsonDocument>[] GetModelClassProperty(T t)
        {
            if (t == null)
            {
                return null;
            }

            System.Reflection.PropertyInfo[] properties = t.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            List<UpdateDefinition<BsonDocument>> updates = new List<UpdateDefinition<BsonDocument>>();

            foreach (System.Reflection.PropertyInfo item in properties)
            {
                string name = item.Name;
                object value = item.GetValue(t, null);

                updates.Add(Builders<BsonDocument>.Update.Set(name, value));
            }

            return updates.ToArray();
        }
    }
}
