using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apigee.Net.Networking;

namespace Apigee.Net.PortLib
{
    /// <summary>
    /// Apigee.Net Portable Library contains Interfaces which must be implemeneted per Plaftorm (.Net4.5, Win8, WP7, WP8).
    /// you must supply all required implementations for those interfaces - in order to use the Library on your platform. 
    /// </summary>
    public interface IHttpTools
    {
        string PerformGet(string url);
        
        ReturnT PerformPost<ReturnT>(string url);

        ReturnT PerformPost<PostT, ReturnT>(string url, PostT postData);

        ReturnT PerformPost<PostT, ReturnT>(string url, PostT postData, Dictionary<string, string> files);
     
        ReturnT PerformJsonRequest<ReturnT>(string url,  Apigee.Net.Networking.HttpTools.RequestTypes method, object postData);
            
    }
}
