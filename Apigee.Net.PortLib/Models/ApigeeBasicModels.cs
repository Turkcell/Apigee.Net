
namespace Apigee.Net.Models
{
    public class ApigeeUser
    {
        public string Uuid { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string HomePage { get; set; }
        public string Email { get; set; }
        public string Bday { get; set; }
        public string Picture { get; set; }
        public string Tel { get; set; }
        public string Url { get; set; }

        public static ApigeeUser Parse(Newtonsoft.Json.Linq.JToken usr)
        {
            return new ApigeeUser()
            {
                Uuid = (usr["uuid"] ?? "").ToString(),
                Username = (usr["username"] ?? "").ToString(),
                Password = (usr["password"] ?? "").ToString(),
                Name = (usr["name"] ?? "").ToString(),
                Title = (usr["title"] ?? "").ToString(),
                Email = (usr["Email"] ?? "").ToString(),
                Tel = (usr["tel"] ?? "").ToString(),
                HomePage = (usr["homepage"] ?? "").ToString(),
                Bday = (usr["bday"] ?? "").ToString(),
                Picture = (usr["picture"] ?? "").ToString(),
                Url = (usr["url"] ?? "").ToString()
            };
        }
    }

    public class ApigeeGroup
    {
        public string Uuid { get; set; }
        public string Created { get; set; }
        public string Path { get; set; }
        public string Title { get; set; }
        public string Description { get; set; } 

        public static ApigeeGroup Parse(Newtonsoft.Json.Linq.JToken group)
        {
            return new ApigeeGroup
            {
                Uuid = (group["uuid"] ?? "").ToString(),
                Created = (group["created"] ?? "").ToString(),
                Path = (group["path"] ?? "").ToString(),
                Title = (group["title"] ?? "").ToString(),
                Description = (group["description"] ?? "").ToString()
            };
        }
    }

    public class ApigeeRole
    {
        public string Uuid { get; set; }
        public string Created { get; set; }
        public string RoleName { get; set; }
        public string Title { get; set; }

        internal static ApigeeRole Parse(Newtonsoft.Json.Linq.JToken role)
        {
            return new ApigeeRole
            {
                Uuid = (role["uuid"] ?? "").ToString(),
                Created = (role["created"] ?? "").ToString(),
                RoleName = (role["roleName"] ?? "").ToString(),
                Title = (role["title"] ?? "").ToString(),
            };
        }

    }

    public class Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
