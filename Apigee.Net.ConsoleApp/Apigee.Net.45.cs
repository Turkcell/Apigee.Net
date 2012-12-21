using Apigee.Net.Networking;
using Apigee.Net.PortLib;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Apigee.Net.ConsoleApp
{
    class ApigeeNET45 : IHttpTools
    {
        #region Get

        public string PerformGet(string url)
        {
            WebRequest req = WebRequest.Create(url);
            WebResponse resp = req.GetResponse();
            StreamReader sr = new StreamReader(resp.GetResponseStream());
            return sr.ReadToEnd().Trim();
        }

        #endregion

        #region Post

        public ReturnT PerformPost<ReturnT>(string url)
        {
            return PerformPost<object, ReturnT>(url, new object());
        }
        public ReturnT PerformPost<PostT, ReturnT>(string url, PostT postData)
        {
            return PerformPost<PostT, ReturnT>(url, postData, new NameValueCollection());
        }

        ReturnT PerformPost<PostT, ReturnT>(string url, PostT postData, Dictionary<string, string> filesDic);
        public ReturnT PerformPost<PostT, ReturnT>(string url, PostT postData, Dictionary<string, string> files)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
			NameValueCollection files 
            NameValueCollection nvpPost;
            if (typeof(PostT) == typeof(NameValueCollection))
            {
                nvpPost = postData as NameValueCollection;
            }
            else
            {
                nvpPost = ObjectToNameValueCollection<PostT>(postData);
            }

            List<UploadFile> postFiles = new List<UploadFile>();
            foreach (var fKey in files.AllKeys)
            {
                FileStream fs = File.OpenRead(files[fKey]);
                postFiles.Add(new UploadFile(fs, fKey, files[fKey], "application/octet-stream"));
            }

            var response = HttpUploadHelper.Upload(req, postFiles.ToArray(), nvpPost);

            using (Stream s = response.GetResponseStream())
            using (StreamReader sr = new StreamReader(s))
            {
                var responseJson = sr.ReadToEnd();
                if (typeof(ReturnT) == typeof(string))
                {
                    return (ReturnT)Convert.ChangeType(responseJson, typeof(ReturnT));
                }

                return fastJSON.JSON.Instance.ToObject<ReturnT>(responseJson);
            }
        }

        //Converts an object to a name value collection (for posts)
        private NameValueCollection ObjectToNameValueCollection<T>(T obj)
        {
            NameValueCollection results = new NameValueCollection();

            var oType = typeof(T);
            foreach (var prop in oType.GetProperties())
            {
                string pVal = "";
                try
                {
                    pVal = oType.GetProperty(prop.Name).GetValue(obj, null).ToString();
                }
                catch { }
                results[prop.Name] = pVal;
            }

            return results;
        }

        #endregion

        #region JSON Request

        public ReturnT PerformJsonRequest<ReturnT>(string url, HttpTools.RequestTypes method, object postData)
        {
            //Initilize the http request
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.ContentType = "application/json";
            req.Method = Enum.GetName(typeof(HttpTools.RequestTypes), method);


            //If posting data - serialize it to a json object
            if (method != HttpTools.RequestTypes.Get)
            {
                StringBuilder sbJsonRequest = new StringBuilder();
                var T = postData.GetType();
                foreach (var prop in T.GetProperties())
                {
                    if (HttpTools.NativeTypes.Contains(prop.PropertyType))
                    {
                        sbJsonRequest.AppendFormat("\"{0}\":\"{1}\",", prop.Name.ToLower(), prop.GetValue(postData, null));
                    }
                }

                using (var sWriter = new StreamWriter(req.GetRequestStream()))
                {
                    sWriter.Write("{" + sbJsonRequest.ToString().TrimEnd(',') + "}");
                }
            }

            //Submit the Http Request
            string responseJson = "";
            try
            {
                using (var wResponse = req.GetResponse())
                {
                    StreamReader sReader = new StreamReader(wResponse.GetResponseStream());
                    responseJson = sReader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                using (WebResponse response = ex.Response)
                {
                    StreamReader sReader = new StreamReader(response.GetResponseStream());
                    responseJson = sReader.ReadToEnd();
                }
            }

            if (typeof(ReturnT) == typeof(string))
            {
                return (ReturnT)Convert.ChangeType(responseJson, typeof(ReturnT));
            }

            return fastJSON.JSON.Instance.ToObject<ReturnT>(responseJson);
        }
 
        #endregion

    }
}
