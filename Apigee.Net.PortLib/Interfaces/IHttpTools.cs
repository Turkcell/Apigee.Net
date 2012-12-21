using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apigee.Net.Networking;

namespace Apigee.Net.PortLib
{
    public interface IHttpTools
    {
        string PerformGet(string url);
        
        ReturnT PerformPost<ReturnT>(string url);

        ReturnT PerformPost<PostT, ReturnT>(string url, PostT postData);

        ReturnT PerformPost<PostT, ReturnT>(string url, PostT postData, Dictionary<string, string> files);
     
        ReturnT PerformJsonRequest<ReturnT>(string url,  Apigee.Net.Networking.HttpTools.RequestTypes method, object postData);
            
    }
}
