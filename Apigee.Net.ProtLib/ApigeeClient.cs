﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apigee.Net.Models.ApiResponse;
using Apigee.Net.Networking;
using Apigee.Net.Models;


namespace Apigee.Net
{
    public class ApigeeClient
    {
        /// <summary>
        /// Create a new Apigee Client
        /// </summary>
        /// <param name="userGridUrl">The Base URL To the UserGrid</param>
        public ApigeeClient(string userGridUrl)
        {
            this.UserGridUrl = userGridUrl;
        }

        public string UserGridUrl { get; set; }

        #region Core Worker Methods

        /// <summary>
        /// Combines The UserGridUrl abd a provided path - checking to emsure proper http formatting
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

            return sbResult.ToString();
        }

        private object GetEntitiesFromJson(string rawJson)
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
            return ProtLib.IProtLibHTTPTools.PerformJsonRequest<retrunT>(requestPath, method, data);
        }

        

        #endregion

        #region Account Management

        public List<ApigeeUser> GetUsers()
        {
            var rawResults = PerformRequest<string>("/users");
            var users = GetEntitiesFromJson(rawResults);
            
            List<ApigeeUser> results = new List<ApigeeUser>();
            foreach (var usr in users)
            {
                results.Add(new ApigeeUser { 
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

            return results;
        }


        public List<ApigeeGroup> GetGroups()
        {
            var rawResults = PerformRequest<string>("/groups");
            var groups = GetEntitiesFromJson(rawResults);
            
            var results = new List<ApigeeGroup>();
            foreach (var usr in groups)
            {
                results.Add(new ApigeeGroup { 
                    Uuid = (usr["uuid"] ?? "").ToString(),
                    Created = (usr["created"] ?? "").ToString(),
                    Path = (usr["path"] ?? "").ToString(),
                    Title = (usr["title"] ?? "").ToString(),
                });
            }

            return results;
        }

        public List<ApigeeRole> GetRoles()
        {
            var rawResults = PerformRequest<string>("/roles");
            var roles = GetEntitiesFromJson(rawResults);

            var results = new List<ApigeeRole>();
            foreach (var usr in roles)
            {
                results.Add(new ApigeeRole
                {
                    Uuid = (usr["uuid"] ?? "").ToString(),
                    Created = (usr["created"] ?? "").ToString(),
                    RoleName = (usr["roleName"] ?? "").ToString(),
                    Title = (usr["title"] ?? "").ToString(),
                });
            }

            return results;
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

        
        public string CreateAppUser(ApigeeUser accountModel)
        {
            var rawResults = PerformRequest<string>("/users", HttpTools.RequestTypes.Post, accountModel);
            var entitiesResult = GetEntitiesFromJson(rawResults);
            if (entitiesResult != null)
            {
                return entitiesResult[0]["uuid"].ToString();
            }
            else
            {
                return UpdateAccount(accountModel);
            }
        }

        public string UpdateAccount(ApigeeUser accountModel)
        {
            var rawResults = PerformRequest<string>("/users/" + accountModel.Username, HttpTools.RequestTypes.Put, accountModel);

            return "";
        }

        #endregion

        #region Token Management

        public string GetToken(string username, string password)
        {
            var reqString = string.Format("/token/?grant_type=password&username={0}&password={1}", username, password);
            var rawResults = PerformRequest<string>(reqString);
            var results = JObject.Parse(rawResults);

            return results["access_token"].ToString();
        }

        public string LookUpToken(string token)
        {
            var reqString = "/users/me/?access_token=" + token;
            var rawResults = PerformRequest<string>(reqString);
            var entitiesResult = GetEntitiesFromJson(rawResults);

            return entitiesResult[0]["username"].ToString();
        }

        #endregion

    }
}
