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
        public ApigeeClient(string userGridUrl, ImplementationStruct currentImple)
        {
            this.UserGridUrl = userGridUrl;
            this.IhttpTools = currentImple.iHttpTools;
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
        /// Performs a Get agianst the UserGridUrl + provided path
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
                        results.Add(new ApigeeUser
                        {
                            Uuid = (usr["uuid"] ?? "").ToString(),
                            Username = (usr["username"] ?? "").ToString(),
                            Password = (usr["password"] ?? "").ToString(),
                            Lastname = (usr["lastname"] ?? "").ToString(),
                            Firstname = (usr["firstname"] ?? "").ToString(),
                            Title = (usr["title"] ?? "").ToString(),
                            Email = (usr["Email"] ?? "").ToString(),
                            Tel = (usr["tel"] ?? "").ToString(),
                            HomePage = (usr["homepage"] ?? "").ToString(),
                            Bday = (usr["bday"] ?? "").ToString(),
                            Picture = (usr["picture"] ?? "").ToString(),
                            Url = (usr["url"] ?? "").ToString()
                        });
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


        public ApigeeResponse GetGroups()
        {
            var rawResults = PerformRequest<string>("/groups");
            var response = new ApigeeResponse(rawResults, true);
            if (response.success)
            {
                try
                {
                    var results = new List<ApigeeGroup>();
                    foreach (var usr in (JToken)response.ResponseData)
                    {
                        results.Add(new ApigeeGroup
                        {
                            Uuid = (usr["uuid"] ?? "").ToString(),
                            Created = (usr["created"] ?? "").ToString(),
                            Path = (usr["path"] ?? "").ToString(),
                            Title = (usr["title"] ?? "").ToString(),
                        });
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
                    foreach (var usr in (JToken)response.ResponseData)
                    {
                        results.Add(new ApigeeRole
                        {
                            Uuid = (usr["uuid"] ?? "").ToString(),
                            Created = (usr["created"] ?? "").ToString(),
                            RoleName = (usr["roleName"] ?? "").ToString(),
                            Title = (usr["title"] ?? "").ToString(),
                        });
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
            return new ApigeeResponse(JObject.Parse(rawResults), "access_token");
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
