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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Deserializers;

namespace Apigee.Net.WP7_TestApp
{
    /// <summary>
    /// This is an Async Implementation wrapped as a Sync one.
    /// Because the Interface from Apigee is Syncronized, but Windows Phone almost DMEAND an Async one..
    /// </summary>
    class ApigeeWP7Implementation : IHttpTools
    {
        /* This is still an ALPHA Implementation.
         * Might be better utilizing Background Workers inside the Imlementation rather than outside in the Main Page...
         * (zaxy78)
         */

        #region Generic Async Mechanism

        public class AsyncState
        {
            // This class stores the State of the request.
            public HttpWebRequest request;
            public object responseObject;
            
            
            public ManualResetEvent syncPoint;
            
            public bool success = false;

            public AsyncState()
            {
                request = null;
                syncPoint = new ManualResetEvent(false);
            }
        }


        private AsyncState BeginAsyncGET(string url)
        {
            //Preforme Async Call
            AsyncState asycState = new AsyncState();
            var client = new RestClient(url);

            var request = new RestRequest();
            request.Method = Method.GET;

            client.ExecuteAsync(request, response =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    asycState.responseObject = response.Content;
                    asycState.success = true;
                    //OK
                }
                else
                {
                    asycState.responseObject = response.Content;
                    asycState.success = false;
                    //NOT_OK
                }

                asycState.syncPoint.Set();
            });

            return asycState;
        }
        private AsyncState BeginAsyncPOST<ReturnT>(string url, object postData)
        {
            var asycState = new AsyncState();
            var client = new RestClient(url);
                                                    // client.Authenticator = new HttpBasicAuthenticator(username, password);
            var request = new RestRequest();
            request.Method = Method.POST;

            //Json to post 
            string jsonToSend = ObjectToJson(postData);

            request.AddParameter("application/json; charset=utf-8", jsonToSend, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;
            // used to be try/catchar here

            client.ExecuteAsync(request, response =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    asycState.responseObject = response.Content;
                    asycState.success = true;
                    //OK
                }
                else
                {
                    asycState.responseObject = response.Content;
                    asycState.success = false;
                    //NOT_OK
                }

                asycState.syncPoint.Set();
            });
        
            return asycState;
        }      
  
        #endregion

        #region Get

        public string PerformGet(string url)
        {
            //HttpWebRequest req = (HttpWebRequest) HttpWebRequest.Create(url);

            //preform an Async Call
            var asyncState = BeginAsyncGET(url);
            
            //wait for result
            asyncState.syncPoint.WaitOne(); 
            
            // get the response
            var response = asyncState.responseObject;

            return response.ToString();
        }

        #endregion

        #region JSON Request

        public ReturnT PerformJsonRequest<ReturnT>(string url, HttpTools.RequestTypes method, object postData)
        {
            AsyncState asyncState;

            if (!typeof(ReturnT).Equals(typeof(string)))
                throw new NotImplementedException("PerformJsonRequest accepts only string types for ReturnT");

            if (method.Equals(HttpTools.RequestTypes.Get))
            {
                object stringGetResponse = Convert.ChangeType(PerformGet(url), typeof(ReturnT), null);
                return (ReturnT)stringGetResponse;
            }

            
            //preform an Async Call
            asyncState = BeginAsyncPOST<ReturnT>(url, postData);
        
            //wait for result
            asyncState.syncPoint.WaitOne();

            // get the response
            return (ReturnT)asyncState.responseObject;
            
        }
        #endregion        

        #region Post  *** NOT IN USE ***
        public ReturnT PerformPost<ReturnT>(string url)
        {
            return PerformPost<object, ReturnT>(url, new object());
        }
        public ReturnT PerformPost<PostT, ReturnT>(string url, PostT postData)
        {
            return PerformPost<PostT, ReturnT>(url, postData, new Dictionary<string, string>());
        }
        public ReturnT PerformPost<PostT, ReturnT>(string url, PostT postData, Dictionary<string, string> files)
        {
            throw new NotImplementedException();
        }
        #endregion


        #region
        public static string ObjectToJson(object postData)
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
            return "{" + sbJsonRequest.ToString().TrimEnd(',') + "}";
        }
        #endregion
    }
}
