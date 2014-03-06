using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Collections;
using System.Collections.Specialized;
using System.IO.Compression;

namespace QQHelper
{
    /// <summary>
    /// 封装了对http请求的操作
    /// </summary>
    public class HttpHelper
    {
        public static int TimeOut = 5000;
        public static CookieContainer CookieContainers = new CookieContainer();
        private static string FireFoxAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.9.2.23) Gecko/20110920 Firefox/3.6.23";
        private static string IE = "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.97 Safari/537.11";
        private static string IE7 = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; InfoPath.2; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022; .NET4.0C; .NET4.0E)";

        public static string Post(string url, string data)
        {
            return GetResponse(url, "post", data, "", "", "");
        }
        public static string Post(string url, string data, string host, string orign, string refer)
        {
            return GetResponse(url, "post", data, host, orign, refer);
        }
        public static string Get(string url)
        {
            return GetResponse(url, "get", "", "", "", "");
        }
        public static string Get(string url, string host, string orign, string refer)
        {
            return GetResponse(url, "get", "", host, orign, refer);
        }
        /// <summary>
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method">"POST" or "GET"</param>
        /// <param name="data">when the method is "POST", the data will send to web server, if the method is "GET", the data should be string.empty</param>
        /// <returns></returns>
        public static string GetResponse(string url, string method, string data, string host, string orign, string refer)
        {
            HttpWebResponse res = null;
            Encoding myEncoding = Encoding.UTF8;
            Stream st = null;
            StreamReader sr = null;
            HttpWebRequest req = null;
            try
            {
                req = (HttpWebRequest)WebRequest.Create(url);
                req.KeepAlive = true;
                req.Method = method.ToUpper();
                req.AllowAutoRedirect = true;
                req.CookieContainer = CookieContainers;
                req.ContentType = "application/x-www-form-urlencoded";
                req.UserAgent = IE7;
                req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                req.Timeout = 20000;
                req.Referer = refer;
                if (!string.IsNullOrEmpty(host))
                {
                    req.Host = host;
                }
                req.Headers.Add("X-Requested-With", "XMLHttpRequest");
                if (!string.IsNullOrEmpty(orign))
                {
                    req.Headers.Add("Origin", orign);
                }
                if (method.ToUpper() == "POST" && data != null)
                {
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    byte[] postBytes = encoding.GetBytes(data); ;
                    req.ContentLength = postBytes.Length;
                    st = req.GetRequestStream();
                    st.Write(postBytes, 0, postBytes.Length);
                    st.Close();
                }
                System.Net.ServicePointManager.ServerCertificateValidationCallback += (se, cert, chain, sslerror) =>
                {
                    return true;
                };
                if (req.GetResponse() != null)
                {
                    res = (HttpWebResponse)req.GetResponse();
                    st = res.GetResponseStream();
                    if (res.ContentEncoding.ToLower().Contains("gzip"))
                    {
                        st = new GZipStream(st, CompressionMode.Decompress);
                    }
                    sr = new StreamReader(st, myEncoding);
                    if (res.Cookies.Count > 0)
                    {
                        CookieContainers.Add(res.Cookies);
                    }
                    if (req.CookieContainer != null)
                    {
                        string host1 = req.Address.Host;
                        if (host1 == "d.web2.qq.com")
                        {
                            List<Cookie> cookieList = GetAllCookies();
                            foreach (Cookie cookie in cookieList)
                            {
                                cookie.Domain = "d.web2.qq.com";
                                CookieContainers.Add(cookie);
                            }

                        }
                    }
                    
                    string str = sr.ReadToEnd();
                    return str;
                }
                else
                    return string.Empty;
            }
            catch (WebException ex)
            {
                string error = string.Empty;
                if (ex.Response != null)
                {
                    res = (HttpWebResponse)ex.Response;
                    st = res.GetResponseStream();
                    sr = new StreamReader(st, myEncoding);
                    error = sr.ReadToEnd();
                }
                return error;
            }

        }
        public static List<Cookie> GetAllCookies()
        {
            CookieContainer cc = CookieContainers;
            List<Cookie> lstCookies = new List<Cookie>();

            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.Instance, null, cc, new object[] { });

            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c in colCookies) lstCookies.Add(c);
            }

            return lstCookies;
        }
        /// <summary>
        /// 修复httpwebrequest发送cookier的bug
        /// </summary>
        /// <param name="cookieContainer"></param>
        public static void BugFix_CookieDomain(CookieContainer cookieContainer)
        {
            System.Type _ContainerType = typeof(CookieContainer);
            Hashtable table = (Hashtable)_ContainerType.InvokeMember("m_domainTable",
                                       System.Reflection.BindingFlags.NonPublic |
                                       System.Reflection.BindingFlags.GetField |
                                       System.Reflection.BindingFlags.Instance,
                                       null,
                                       cookieContainer,
                                       new object[] { });
            ArrayList keys = new ArrayList(table.Keys);
            foreach (string keyObj in keys)
            {
                string key = (keyObj as string);
                if (key[0] == '.')
                {
                    string newKey = key.Remove(0, 1);
                    table[newKey] = table[keyObj];
                }
            }
        }
        /// <summary>
        /// 获得图片流
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="host">host</param>
        /// <param name="orign">orign</param>
        /// <param name="refer">refer</param>
        /// <returns></returns>
        public static Stream GetResponseImage(string url, string host, string orign, string refer)
        {
            Stream resst = null;
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.KeepAlive = true;
                req.Method = "GET";
                req.AllowAutoRedirect = true;
                req.CookieContainer = CookieContainers;
                req.ContentType = "application/x-www-form-urlencoded";
                req.UserAgent = IE7;
                req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                req.Timeout = 50000;
                if (!string.IsNullOrEmpty(refer))
                {
                    req.Referer = refer;
                }
                if (!string.IsNullOrEmpty(orign))
                {
                    req.Headers.Add("Origin", orign);
                }
                if (!string.IsNullOrEmpty(host))
                {
                    req.Host = host;
                }
                System.Net.ServicePointManager.ServerCertificateValidationCallback += (se, cert, chain, sslerror) =>
                {
                    return true;
                };
                Encoding myEncoding = Encoding.GetEncoding("UTF-8");
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                if (res.Cookies.Count > 0)
                {
                    CookieContainers.Add(res.Cookies);
                }
                BugFix_CookieDomain(CookieContainers);
                resst = res.GetResponseStream();
                return resst;
            }
            catch
            {
                return null;
            }
        }
        public static string GetCookie(string url,string name)
        {
            string cookieStr=string.Empty;
            foreach (Cookie cookie in HttpHelper.CookieContainers.GetCookies(new Uri(url)))
            {
                if (cookie.Name == name)
                {
                    cookieStr = cookie.Value;
                    break;
                }
            }
            return cookieStr;
        }
        public static void SetCookie(string url, string name, string value,string domain)
        {
            HttpHelper.CookieContainers.Add(new Cookie() { Name=name, Value=value, Domain=domain});
        }
        public static string HttpPostData(string url, int timeOut, string fileKeyName, string filePath, NameValueCollection stringDict)
        {
            string responseContent;
            var memStream = new MemoryStream();
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.CookieContainer = CookieContainers;
            // 边界符
            var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
            // 边界符
            var beginBoundary = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            // 最后的结束符
            var endBoundary = Encoding.ASCII.GetBytes("--" + boundary + "--\r\n");

            // 设置属性
            webRequest.Method = "POST";
            webRequest.Timeout = timeOut;
            webRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            FileInfo fileInfo = new FileInfo(filePath);
            // 写入文件
            const string filePartHeader =
                "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
                 "Content-Type: image/jpeg\r\n\r\n";
            var header = string.Format(filePartHeader, fileKeyName, fileInfo.Name);
            var headerbytes = Encoding.UTF8.GetBytes(header);

            memStream.Write(beginBoundary, 0, beginBoundary.Length);
            memStream.Write(headerbytes, 0, headerbytes.Length);

            var buffer = new byte[1024];
            int bytesRead; // =0

            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                memStream.Write(buffer, 0, bytesRead);
            }

            // 写入字符串的Key
            var stringKeyHeader = "\r\n--" + boundary +
                                   "\r\nContent-Disposition: form-data; name=\"{0}\"" +
                                   "\r\n\r\n{1}\r\n";

            foreach (byte[] formitembytes in from string key in stringDict.Keys
                                             select string.Format(stringKeyHeader, key, stringDict[key])
                                                 into formitem
                                                 select Encoding.UTF8.GetBytes(formitem))
            {
                memStream.Write(formitembytes, 0, formitembytes.Length);
            }

            // 写入最后的结束边界符
            memStream.Write(endBoundary, 0, endBoundary.Length);

            webRequest.ContentLength = memStream.Length;

            var requestStream = webRequest.GetRequestStream();

            memStream.Position = 0;
            var tempBuffer = new byte[memStream.Length];
            memStream.Read(tempBuffer, 0, tempBuffer.Length);
            memStream.Close();

            requestStream.Write(tempBuffer, 0, tempBuffer.Length);
            requestStream.Close();

            var httpWebResponse = (HttpWebResponse)webRequest.GetResponse();

            using (var httpStreamReader = new StreamReader(httpWebResponse.GetResponseStream(),
                                                            Encoding.GetEncoding("utf-8")))
            {
                responseContent = httpStreamReader.ReadToEnd();
            }

            fileStream.Close();
            httpWebResponse.Close();
            webRequest.Abort();

            return responseContent;
        }
    }
}
