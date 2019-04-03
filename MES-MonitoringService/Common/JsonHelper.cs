using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MES_MonitoringService.Common
{
    public static class JsonHelper
    {
        public static JObject GetJsonObject(string pi_jsonString)
        {
            return (JObject)JsonConvert.DeserializeObject(pi_jsonString);
        }

        public static string GetJsonValue(string jsonString,string key)
        {
            JObject newObject= (JObject)JsonConvert.DeserializeObject(jsonString);

            return newObject[key].ToString();
        }
    }
}
