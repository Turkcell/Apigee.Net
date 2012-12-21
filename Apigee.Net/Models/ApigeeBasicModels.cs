namespace Apigee.Net.Models
{
    public class ApigeeUser
    {
        public string Uuid { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Title { get; set; }
        public string HomePage { get; set; }
        public string Email { get; set; }
        public string Bday { get; set; }
        public string Picture { get; set; }
        public string Tel { get; set; }
        public string Url { get; set; }
    }

    public class ApigeeGroup
    {
        public string Uuid { get; set; }
        public string Created { get; set; }
        public string Path { get; set; }
        public string Title { get; set; }
    }

    public class ApigeeRole
    {
        public string Uuid { get; set; }
        public string Created { get; set; }
        public string RoleName { get; set; }
        public string Title { get; set; }
    }

}
