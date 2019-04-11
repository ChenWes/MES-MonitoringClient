using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Newtonsoft.Json;

namespace MES_MonitoringService.Common
{
    public static class HttpHelper
    {
        private enum HttpResultType
        {
            SUCCESS,
            ERROR
        }

        //基础服务器API地址
        private static string basicHttpUrl = Common.CommonFunction.GenerateBackendUri();

        /// <summary>
        /// Http Post请求
        /// </summary>
        /// <param name="UrlPath">除主机地址以外的逻辑地址(URL可以增加参数条件)</param>
        /// <returns></returns>
        public static string HttpGet(string UrlPath)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    string basicHttpUrl = Common.CommonFunction.GenerateBackendUri();
                    //post
                    var url = new Uri(basicHttpUrl + UrlPath);

                    // response
                    var response = httpClient.GetAsync(url).Result;

                    //接口调用成功数据
                    var data = response.Content.ReadAsStringAsync().Result;

                    //检测返回JSON数据
                    string resultJson = CheckHttpPostResult(data);

                    return resultJson;
                }
            }
            catch (Exception ex)
            {
                Common.LogHandler.WriteLog("HTTP请求出错，原因是：" + ex.Message, ex);
                throw ex;
            }
        }

        /// <summary>
        /// 加入Token的Http Get请求
        /// </summary>
        /// <param name="UrlPath"></param>
        /// <returns></returns>
        public static string HttpGetWithToken(string UrlPath)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    string basicHttpUrl = Common.CommonFunction.GenerateBackendUri();

                    //token
                    httpClient.DefaultRequestHeaders.Add("Authorization", Common.ConfigFileHandler.GetAppConfig("BackendServerToken"));

                    //post
                    var url = new Uri(basicHttpUrl + UrlPath);

                    // response
                    var response = httpClient.GetAsync(url).Result;

                    //接口调用成功数据
                    var data = response.Content.ReadAsStringAsync().Result;

                    //检测返回JSON数据
                    string resultJson = CheckHttpPostResult(data);

                    return resultJson;
                }
            }
            catch (Exception ex)
            {
                Common.LogHandler.WriteLog("HTTP请求出错，原因是：" + ex.Message, ex);
                throw ex;
            }
        }

        public static bool HttpGetFileWithToken(string UrlPath, string filename, string filePath)
        {
            try
            {
                System.IO.FileStream fs;

                using (var httpClient = new HttpClient())
                {
                    string basicHttpUrl = Common.CommonFunction.GenerateBackendUri();

                    //token
                    httpClient.DefaultRequestHeaders.Add("Authorization", Common.ConfigFileHandler.GetAppConfig("BackendServerToken"));

                    //post
                    var url = new Uri(basicHttpUrl + UrlPath);

                    var bytes = httpClient.GetByteArrayAsync(url).Result;

                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }

                    if (File.Exists(filePath + "//" + filename))
                    {
                        File.Delete(filePath + "//" + filename);
                    }
                    fs = new System.IO.FileStream(filePath + "//" + filename, System.IO.FileMode.CreateNew);
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Close();


                    return true;
                }
            }
            catch (Exception ex)
            {
                Common.LogHandler.WriteLog("HTTP请求出错，原因是：" + ex.Message, ex);
                throw ex;
            }
        }

        /// <summary>
        /// Http Post请求
        /// </summary>
        /// <param name="UrlPath">除主机地址以外的逻辑地址</param>
        /// <param name="body">发送的Body数据</param>
        /// <returns></returns>
        public static string HttpPost(string UrlPath, FormUrlEncodedContent body)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    string basicHttpUrl = Common.CommonFunction.GenerateBackendUri();

                    //post
                    var url = new Uri(basicHttpUrl + UrlPath);

                    // response
                    var response = httpClient.PostAsync(url, body).Result;

                    //接口调用成功数据
                    var data = response.Content.ReadAsStringAsync().Result;

                    //检测返回JSON数据
                    string resultJson = CheckHttpPostResult(data);

                    return resultJson;
                }
            }
            catch (Exception ex)
            {
                Common.LogHandler.WriteLog("HTTP请求出错，原因是：" + ex.Message, ex);
                throw ex;
            }
        }

        /// <summary>
        /// 加入Token的Http Post请求
        /// </summary>
        /// <param name="UrlPath"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static string HttpPostWithToken(string UrlPath, FormUrlEncodedContent body)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    string basicHttpUrl = Common.CommonFunction.GenerateBackendUri();

                    //token
                    httpClient.DefaultRequestHeaders.Add("Authorization", Common.ConfigFileHandler.GetAppConfig("BackendServerToken"));

                    //post
                    var url = new Uri(basicHttpUrl + UrlPath);

                    // response
                    var response = httpClient.PostAsync(url, body).Result;

                    //接口调用成功数据
                    var data = response.Content.ReadAsStringAsync().Result;

                    //检测返回JSON数据
                    string resultJson = CheckHttpPostResult(data);

                    return resultJson;
                }
            }
            catch (Exception ex)
            {
                Common.LogHandler.WriteLog("HTTP请求出错，原因是：" + ex.Message, ex);
                throw ex;
            }
        }

        /// <summary>
        /// 检测返回的数据实体
        /// 
        /// {
        /// "resultType": "ERROR"/"SUCCESS",
        /// "results": null/数据实体,
        /// "resultMsg": "错误信息",
        /// "exceptionDetail": {}
        /// }
        /// </summary>
        /// <param name="jsonString">返回的Json完整数据</param>
        /// <returns></returns>
        public static string CheckHttpPostResult(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString)) return string.Empty;

            //检查标识
            string l_resultType = Common.JsonHelper.GetJsonValue(jsonString, "resultType");
            string l_resultMsg = Common.JsonHelper.GetJsonValue(jsonString, "resultMsg");

            if (l_resultType == HttpResultType.SUCCESS.ToString())
            {
                //正常返回了，处理返回的数据（JSON格式）
                if (!string.IsNullOrEmpty(Common.JsonHelper.GetJsonValue(jsonString, "results"))) return Common.JsonHelper.GetJsonValue(jsonString, "results");
                else return string.Empty;
            }
            else
            {
                //错误返回，返回一个错误，并将错误信息返回
                throw new Exception(l_resultMsg);
            }
        }
    }
}
