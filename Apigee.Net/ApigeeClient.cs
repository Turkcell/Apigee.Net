using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apigee.Net.Models.ApiResponse;
using Apigee.Net.Networking;
using Apigee.Net.Models;
using Newtonsoft.Json.Linq;

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
            return HttpTools.PerformJsonRequest<retrunT>(requestPath, method, data);
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

        /// <summary>
        /// uses the Role Uuid in request
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public List<ApigeeUser> GetRoleUsers(ApigeeRole role)
        {
            return GetRoleUsers(role.Uuid);
        }
        /// <summary>
        /// uses the RoleName provided in the  request
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public List<ApigeeUser> GetRoleUsers(string roleName)
        {
            var rawResults = PerformRequest<string>("/roles/"+roleName+"/users");
            var users = GetEntitiesFromJson(rawResults);

            List<ApigeeUser> results = new List<ApigeeUser>();
            foreach (var usr in users)
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

        
        public string CreateAppUser(ApigeeUser newAppUser)
        {
            var rawResults = PerformRequest<string>("/users", HttpTools.RequestTypes.Post, newAppUser);
            var entitiesResult = GetEntitiesFromJson(rawResults);
            if (entitiesResult != null)
            {
                var newUserUuid = entitiesResult[0]["uuid"].ToString();
                AddRole2User(newUserUuid, "appuser");   //Add AppUser role
                return newUserUuid;
            }
            else
            {
                return UpdateAccount(newAppUser);
            }
        }

        public string AddRole2User(ApigeeUser user, string newRole)
        {
            return AddRole2User(user.Uuid, newRole);
        }

        public string AddRole2User(String userUuid, string newRole)
        {
            var path = "/users/" + userUuid + "/roles/" + newRole;
            var rawResults = PerformRequest<string>(path, HttpTools.RequestTypes.Post, "" );
            var entitiesResult = GetEntitiesFromJson(rawResults);
            if (entitiesResult != null)
            {

                return entitiesResult[0]["uuid"].ToString();
            }
            else
            {
                throw new InvalidOperationException("Failed to add role ["+newRole+"] to user ("+userUuid+")");
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



