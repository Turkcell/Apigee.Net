using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Apigee.Net.Networking;
using Apigee.Net.PortLib;

namespace Apigee.Net.WP7_TestApp
{
    /// <summary>
    /// This is an Async Implementation wrapped as a Sync one.
    /// Because the Interface from Apigee is Syncronized, but Windows Phone almost DMEAND an Async one..
    /// </summary>
    class ApigeeWP7Implementation //: IHttpTools
    {
        #region Generic Async Mechanism
        private Dictionary<HttpWebRequest, ManualResetEvent> RequestsSyncPointList = new Dictionary<HttpWebRequest, ManualResetEvent>();
        private Dictionary<ManualResetEvent, HttpWebResponse> ResponseSyncPointList = new Dictionary<ManualResetEvent, HttpWebResponse>();

        
        private ManualResetEvent MakeAsyncRequest(HttpWebRequest request)
        {
            //Register this Async Call
            var syncPoint = new ManualResetEvent(false);
            RequestsSyncPointList.Add(request, syncPoint);

            //Preforme Async Call
            request.BeginGetResponse(GenericAsyncCallback, request);

            //return SyncPoint to caller
            return syncPoint;
        }

        private void GenericAsyncCallback(IAsyncResult ar)
        {
            HttpWebRequest request = (HttpWebRequest)ar.AsyncState;
            var syncPoint = RequestsSyncPointList[request];

            //register response
            ResponseSyncPointList.Add(syncPoint, (HttpWebResponse)request.EndGetResponse(ar));
            //notify caller - response is here
            syncPoint.Set();
        }

        private HttpWebResponse GetAsyncResponse(ManualResetEvent syncPoint)
        {
            return ResponseSyncPointList[syncPoint];
        }
        #endregion

        #region Get

        public string PerformGet(string url)
        {
            HttpWebRequest req = (HttpWebRequest) HttpWebRequest.Create(url);

            //preform an Async Call
            var syncPoint = MakeAsyncRequest(req);
            
            //wait for result
            syncPoint.WaitOne(); 
            
            // get the response
            var response = GetAsyncResponse(syncPoint);

            StreamReader sr = new StreamReader(response.GetResponseStream());          
            return sr.ReadToEnd().Trim();
        }

        #endregion


        /*

        #region Post
        public  ReturnT PerformPost<ReturnT>(string url)
        {
            return PerformPost<object, ReturnT>(url, new object());
        }

        public  ReturnT PerformPost<PostT, ReturnT>(string url, PostT postData)
        {
            return PerformPost<PostT, ReturnT>(url, postData, new Dictionary<string,string>());
        }

        
        public ReturnT PerformPost<PostT, ReturnT>(string url, PostT postData, Dictionary<string,string> files)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            Dictionary<string,string> dicPost;
            if (typeof(PostT) == typeof(Dictionary<string, string>))
            {
                dicPost = postData as Dictionary<string, string>;
            }
            else
            {
                dicPost = HttpTools.ObjectToNameValueCollection<PostT>(postData);
            }

            List<UploadFile> postFiles = new List<UploadFile>();
            foreach (var fKey in files.Keys)
            {
                
                FileStream fs = File.OpenRead(files[fKey]);
                postFiles.Add(new UploadFile(fs, fKey, files[fKey], "application/octet-stream"));
            }

            //convert to nameValue
            var nvcPost = new NameValueCollection();
            foreach (KeyValuePair<string, string> pair in dicPost)
            {
                nvcPost[pair.Key] = pair.Value;
            }

            var response = HttpUploadHelper.Upload(req, postFiles.ToArray(), nvcPost );

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


        //private NameValueCollection ObjectToNameValueCollection<T>(T obj)  --> Stayed in HttpTools (PortLib) with Dic<s,s>
        
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
        */
    }
}
