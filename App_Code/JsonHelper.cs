using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace QQHelper
{
    /// <summary>
    /// 封装了对json的操作 add by liuqiang
    /// </summary>
    public class JsonHelper
    {
        /// <summary>
        /// JSON字符串序列化为T
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="jsonString">JSON字符串</param>
        /// <returns>对象T</returns>
        public static T DeserializeToObj<T>(string jsonString)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(jsonString);
            }
            catch
            {
                return default(T);
            }
        }
        /// <summary>
        /// 对象序列化为JSON字符串
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>JSON字符串</returns>
        public static string SerializeObject(object obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj);
            }
            catch
            {
                return "";
            }
        }
    }
}
