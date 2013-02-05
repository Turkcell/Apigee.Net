using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apigee.Net.Models.ApiResponse;
using Apigee.Net.Networking;
using Apigee.Net.Models;
using Newtonsoft.Json.Linq;

namespace Apigee.Net.PortLib
{
    public class ApigeeClient
    {
        #region Private Members
        private string access_token;
        #endregion
        #region Public Members
        public static string SUCCESS = "api_success";
        public string UserGridUrl { get; set; }
        public bool isAuthenticated { get; private set; }

        // All Implementation Interfaces
        public IHttpTools IhttpTools;
        //-------------------------------
        #endregion


        /// <summary>
        /// Create a new Apigee Client
        /// </summary>
        /// <param name="userGridUrl">The Base URL To the UserGrid</param>
        public ApigeeClient(string userGridUrl, IHttpTools currentImplementation)
        {
            this.UserGridUrl = userGridUrl;
            this.IhttpTools = currentImplementation;
            this.isAuthenticated = false;
        }

        #region Core Worker Methods

        /// <summary>
        /// Combines The UserGridUrl and a provided path - checking to ensure proper http formatting
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string BuildPath(string path)
        {
            StringBuilder sbResult = new StringBuilder();
            sbResult.Append(this.UserGridUrl);
            
            if (this.UserGridUrl.EndsWith("/") != true)
            {
                sbResult.Append("/");
            }

            if (path.StartsWith("/"))
            {
                path = path.TrimStart('/');
            }

            sbResult.Append(path);
            
            //Add authentication:
            if (isAuthenticated)
            {
                sbResult.Append("?access_token=" + access_token);
            }

            return sbResult.ToString();
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

        /// <summary>
        /// Performs a ***Get*** agianst the UserGridUrl + provided path
        /// </summary>
        /// <typeparam name="retrunT">Return Type</typeparam>
        /// <param name="path">Sub Path Of the Get Request</param>
        /// <returns>Object of Type T</returns>
        public retrunT PerformRequest<retrunT>(string path)
        {
            return PerformRequest<retrunT>(path, HttpTools.RequestTypes.Get, null);
        }

        /// <summary>
        /// Performs a Request agianst the UserGridUrl + provided path
        /// </summary>
        /// <typeparam name="retrunT">Return Type</typeparam>
        /// <param name="path">Sub Path Of the Get Request</param>
        /// <returns>Object of Type T</returns>
        public retrunT PerformRequest<retrunT>(string path, HttpTools.RequestTypes method, object data)
        {
            string requestPath = BuildPath(path);
            return IhttpTools.PerformJsonRequest<retrunT>(requestPath, method, data);
        }

        #endregion

        #region Account Management

        public ApigeeResponse GetUsers()
        {
            var rawResults = PerformRequest<string>("/users");
            //parse response
            var response = new ApigeeResponse(rawResults, true);
            if (response.success)
            {
                try
                {
                    List<ApigeeUser> results = new List<ApigeeUser>();
                    foreach (var usr in (JToken)response.ResponseData)
                    {
                        results.Add(ApigeeUser.Parse(usr));
                    }
                    // put users List as response data
                    response.ResponseData = results;
                }
                catch (Exception)
                {
                    response.success = false;
                    response.Error = new ApigeeResponseError("Error parsing users entities");
                }
            }
            return response;
        }

        public ApigeeResponse GetUser(string username)
        {
            var rawResults = PerformRequest<string>("/users/"+ username);
            //parse response
            var response = new ApigeeResponse(rawResults, true);
            if (response.success)
            {
                try
                {   
                    var usr = ((JToken)response.ResponseData)[0];
                    
                    // put user as response data
                    response.ResponseData = ApigeeUser.Parse(usr);
                }
                catch (Exception)
                {
                    response.success = false;
                    response.Error = new ApigeeResponseError("Error parsing users entities");
                }
            }
            return response;
        }

        public ApigeeResponse GetGroups()
        {
            var rawResults = PerformRequest<string>("/groups");
            var response = new ApigeeResponse(rawResults, true);
            if (response.success)
            {
                try
                {
                    var results = new List<ApigeeGroup>();
                    foreach (var group in (JToken)response.ResponseData)
                    {
                        results.Add(ApigeeGroup.Parse(group));
                    }
                    // put groups list as response data
                    response.ResponseData = results;
                }
                catch (Exception)
                {
                    response.success = false;
                    response.Error = new ApigeeResponseError("Error parsing Groups entities");
                }
            } 
            return response;
        }

        public ApigeeResponse GetRoles()
        {
            var rawResults = PerformRequest<string>("/roles");
            var response = new ApigeeResponse(rawResults, true);
            if (response.success)
            {
                try
                {
                    var results = new List<ApigeeRole>();
                    foreach (var role in (JToken)response.ResponseData)
                    {
                        results.Add(ApigeeRole.Parse(role));
                    }
                    // put roles list as response data
                    response.ResponseData = results;
                }
                catch (Exception)
                {
                    response.success = false;
                    response.Error = new ApigeeResponseError("Error parsing Roles entities");
                }
            }
            return response;
        }

        /// <summary>
        /// On success, responseData will be a List<T> of all entities 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="TParser"></param>
        /// <returns></returns>
        public ApigeeResponse GetEntities<T>(string collection, Func<JToken,T> TParser)
        {
            var rawResults = PerformRequest<string>("/" + collection);
            
            //parse response
            var response = new ApigeeResponse(rawResults, true);
            if (response.success)
            {
                try
                {
                    List<T> results = new List<T>();
                    foreach (var entity in (JToken)response.ResponseData)
                    {
                        results.Add( TParser(entity) );
                    }
                    // put users List as response data
                    response.ResponseData = results;
                }
                catch (Exception)
                {
                    response.success = false;
                    response.Error = new ApigeeResponseError("Error parsing "+collection+" entities");
                }
            }
            return response;
        }

        /// <summary>
        ///  On success, responseData will be a List<T> of all Entity2 items
        ///  which are connected to Item from Entity1 in the verb connection
        ///  For example GetConnections("users","jhon","like") will return Pictures jhon likes.
        /// </summary>
        /// <typeparam name="Entity2Type">Type of Entity2 in the verb connection</typeparam>
        /// <param name="entity1">Collection of entity1 in the verb connection</param>
        /// <param name="item">The specific item in Entity1</param>
        /// <param name="verb">The connection vers. for example: "like", "attending", etc.</param>
        /// <param name="direction">Which side of the verb you want? True for "Connections", False for "Connecting"</param>
        /// <param name="TParser">Parser for Entity2 Type (Return Type)</param>
        /// <returns></returns>
        public ApigeeResponse GetConnections<Entity2Type>(string entity1, string item, string verb, bool direction, Func<JToken, Entity2Type> TParser)
        {
            var path = "/" + entity1 + "/" + item + "/";
            path += direction ? "Connection" : "Connecting";
            
            var rawResults = PerformRequest<string>(path);

            //parse response
            var response = new ApigeeResponse(rawResults, true);
            if (response.success)
            {
                try
                {
                    List<Entity2Type> results = new List<Entity2Type>();
                    foreach (var entity in (JToken)response.ResponseData)
                    {
                        //filer: take only specific Verb
                        var connection = entity["metadata"]["connection"];
                        if (connection != null && connection.ToString() == verb)
                            results.Add(TParser(entity));
                    }
                    // put users List as response data
                    response.ResponseData = results;
                }
                catch (Exception)
                {
                    response.success = false;
                    response.Error = new ApigeeResponseError("Error parsing connections: " + path);
                }
            }
            return response;
        }


        public ApigeeResponse POSTConnections<Entity2Type>(string entity1, string item, string verb, string entity2, string item2)
        {
            var path = "/" + entity1 + "/" + item + "/" + verb + "/" + entity2 + "/" + item2;

            var rawResults = PerformRequest<string>(path);

            //parse response
            var response = new ApigeeResponse(rawResults, true);
            if (response.success)
            {
                try
                {
                    List<Entity2Type> results = new List<Entity2Type>();
                    foreach (var entity in (JToken)response.ResponseData)
                    {
                        //filer: take only specific Verb
                        var connection = entity["metadata"]["connection"];
                        if (connection != null && connection.ToString() == verb)
                            results.Add(TParser(entity));
                    }
                    // put users List as response data
                    response.ResponseData = results;
                }
                catch (Exception)
                {
                    response.success = false;
                    response.Error = new ApigeeResponseError("Error parsing connections: " + path);
                }
            }
            return response;
        }

        public string CreateGroup(ApigeeGroup newGroup)
        {
            var rawResults = PerformRequest<string>("/groups", HttpTools.RequestTypes.Post, newGroup);
            var entitiesResult = GetEntitiesFromJson(rawResults);
            if (entitiesResult != null)
            {
                return entitiesResult[0]["uuid"].ToString();
            }
            else
            {
                throw new InvalidOperationException("Failed to creat a group");
            }
        }

        public ApigeeResponse CreateAppUser(ApigeeUser accountModel)
        {
            var rawResults = PerformRequest<string>("/users", HttpTools.RequestTypes.Post, accountModel);
            var response = new ApigeeResponse(rawResults, true);
            if (response.success)
            {
                response.ResponseData = ((JToken)response.ResponseData)[0];
            }
            return response;
        }

        public string UpdateAccount(ApigeeUser accountModel)
        {
            var rawResults = PerformRequest<string>("/users/" + accountModel.Username, HttpTools.RequestTypes.Put, accountModel);

            return "";
        }

        #endregion

        #region Private Token Management

        private ApigeeResponse GetToken(string username, string password)
        {
            var reqString = string.Format("/token/?grant_type=password&username={0}&password={1}", username, password);
            var rawResults = PerformRequest<string>(reqString);
            // paser response and let ApigeeResponse get you your wanted result
            try
            {
                var jsonResult = JObject.Parse(rawResults);
                return new ApigeeResponse(jsonResult, "access_token");
            }
            catch (Exception)
            {
                return new ApigeeResponse() {Error = new ApigeeResponseError("Connection to server failed."), success = false};
            }
        }
        private ApigeeResponse LookUpToken(string token)
        {
            var reqString = "/users/me/?access_token=" + token;
            var rawResults = PerformRequest<string>(reqString);
            var entitiesResult = GetEntitiesFromJson(rawResults);

            return new ApigeeResponse(entitiesResult[0],"username");
        }

        #endregion


        #region Authentication

        public ApigeeResponse ManualAuthentication(string token)
        {
            //TODO Verify Token....

            this.access_token = token;
            this.isAuthenticated = true;
            return new ApigeeResponse { success = true, ResponseData = token };
        }
        public ApigeeResponse AuthenticateUser(string user, string password)
        {
            var tokenResponse = GetToken(user, password);

            if (!tokenResponse.success)
                return tokenResponse;

            //else - auth success
            this.access_token = tokenResponse.ToString();
            this.isAuthenticated = true;
            return tokenResponse;
        }
        
        #endregion
    }
}
