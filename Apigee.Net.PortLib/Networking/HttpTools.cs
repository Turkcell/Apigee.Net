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

        /// <summary>
        /// Methods to convert Unix time stamp to DateTime
        /// </summary>
        /// <param name="_UnixTimeStamp">Unix time stamp to convert</param>
        /// <returns>Return DateTime</returns>
        public static DateTime UnixToDateTime(long _UnixTimeStamp)
        {
            return (new DateTime(1970, 1, 1, 0, 0, 0)).AddMilliseconds(_UnixTimeStamp);
        }

        /// <summary>
        /// Methods to convert DateTime to Unix time stamp
        /// </summary>
        /// <param name="_UnixTimeStamp">Unix time stamp to convert</param>
        /// <returns>Return Unix time stamp as long type</returns>
        static public long DateTimeToUnix(DateTime _DateTime)
        {
            TimeSpan _UnixTimeSpan = (_DateTime - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)_UnixTimeSpan.TotalMilliseconds;
        }
    }
}
