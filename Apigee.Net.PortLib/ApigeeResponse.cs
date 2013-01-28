using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apigee.Net.PortLib
{
    public class ApigeeResponse
    {
        public bool success;
        public ApigeeResponseError Error;

        //data
        public object ResponseData;
        public string RawResponse { get; private set; }

        public ApigeeResponse() { }

        public ApigeeResponse(string rawData, bool ExtractEnteties) 
        {
            RawResponse = rawData;
            if (!ExtractEnteties) return; //safty
            var enteties = GetEntitiesFromJson(rawData);
            if (enteties == null)
            {
                this.success = false;
                this.Error = new ApigeeResponseError(JObject.Parse(rawData));
            }
            else 
            {
                this.success = true;
                this.ResponseData = enteties;
            }
        }
 
        public ApigeeResponse(JObject rawData, string wantedKey) {
            RawResponse = rawData.ToString();
            try
            {
                this.ResponseData = rawData.SelectToken(wantedKey, true);
                this.success = true;
            }
            catch (Exception)
            {
                var errorMsg = rawData.SelectToken("error_description").ToString();
                if (String.IsNullOrEmpty(errorMsg))
                    errorMsg = "Unknown Error";
                
                this.Error = new ApigeeResponseError(errorMsg);
                this.success = false;
            }
        }


        public ApigeeResponse(JToken rawData, string wantedKey)
        {
            RawResponse = rawData.ToString();
            try
            {
                this.ResponseData = rawData[wantedKey];
                this.success = true;
            }
            catch (Exception)
            {
                this.Error = new ApigeeResponseError("error");
                this.success = false;
            }
        }


        public override string ToString()
        {
            return ResponseData.ToString();
        }

        private JToken GetEntitiesFromJson(string rawJson)
        {
            if (string.IsNullOrEmpty(rawJson) != true)
            {
                var objResult = JObject.Parse(rawJson);
                return objResult.SelectToken("entities");
            }
            return null;
        }

    }

    public class ApigeeResponseError : Exception
    {
        public string ErrorType;
        public ApigeeResponseError(string msg) : base(msg) { }
        
        public ApigeeResponseError(JObject rawResponse) : base(rawResponse.SelectToken("error_description").ToString() ?? "Unknown Error")
        {
           this.ErrorType = rawResponse.SelectToken("error").ToString() ?? "Unknown Error";
        }
    }

}
