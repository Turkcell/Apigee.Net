using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace Apigee.Net.Networking
{
    public class HttpTools
    {
        public enum RequestTypes { Get, Post, Put, Delete }

        //Converts an object to a name value collection (for posts)
        public static Dictionary<string, string> ObjectToNameValueCollection<T>(T obj)
        {
            var results = new Dictionary<string, string>();

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

        //Approved Types for serialization
        public static List<Type> NativeTypes
        {
            get
            {
                var approvedTypes = new List<Type>();

                approvedTypes.Add(typeof(int));
                approvedTypes.Add(typeof(Int32));
                approvedTypes.Add(typeof(Int64));
                approvedTypes.Add(typeof(string));
                approvedTypes.Add(typeof(DateTime));
                approvedTypes.Add(typeof(double));
                approvedTypes.Add(typeof(decimal));
                approvedTypes.Add(typeof(float));
                approvedTypes.Add(typeof(List<>));
                approvedTypes.Add(typeof(bool));
                approvedTypes.Add(typeof(Boolean));

                approvedTypes.Add(typeof(int?));
                approvedTypes.Add(typeof(Int32?));
                approvedTypes.Add(typeof(Int64?));
                approvedTypes.Add(typeof(DateTime?));
                approvedTypes.Add(typeof(double?));
                approvedTypes.Add(typeof(decimal?));
                approvedTypes.Add(typeof(float?));
                approvedTypes.Add(typeof(bool?));
                approvedTypes.Add(typeof(Boolean?));

                return approvedTypes;
            }
        }
    }
}
