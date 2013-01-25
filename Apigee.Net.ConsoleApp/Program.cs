using Apigee.Net;
using Apigee.Net.Models;
using Apigee.Net.PortLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apigee.Net.ConsoleApp
{
    class Program
    {

        static ApigeeClient aClient = new ApigeeClient("http://api.usergrid.com/zaxyinc/imhere/", new ImplementationStruct() { iHttpTools = new ApigeeNET45() } );
        
        static void Main(string[] args)
        {
            Console.WriteLine("Connected.... Press any key");
            while (Console.ReadKey().KeyChar != 'q')
            {
                Console.WriteLine("Choose an option:");
                Console.WriteLine("1. Add User Account");
                Console.WriteLine("2. Add Community Group");
                Console.WriteLine("3. Login");
                Console.WriteLine("4. Show All Groups");
                Console.WriteLine("5. Show All Users");

                switch (Convert.ToInt16(Console.ReadLine()))
                {
                    case 1:
                        AddNewUser();
                        break;
                    case 2:
                        AddNewGroup();
                        break;
                    case 3:
                        LoginUser();
                        break;
                    case 4:
                        GetGroups();
                        break;
                    case 5:
                        GetUsers();
                        break;
                    default:
                        Console.WriteLine("Invalid choice. try again");
                        Console.Clear();
                        break;
                }
            }
            
        }

        private static void LoginUser()
        {
            string usern, pass;
            Console.WriteLine("UserName:");
            usern = Console.ReadLine();
            Console.WriteLine("Password:");
            pass = Console.ReadLine();

            var response = aClient.AuthenticateUser(usern, pass);
            Console.WriteLine("success? = " + response.success );

            if (response.success)
            {
                Console.WriteLine("token? = " + response.ToString());

                Console.WriteLine("Hello User!   retrieving Groups: ");
                GetGroups();
            }
            else Console.WriteLine("Login Failed: " + response.Error.Message);
        }

        private static void GetGroups()
        {
            var response = aClient.GetGroups();
            if (response.success)
            {
                var resultsG = (List<ApigeeGroup>)response.ResponseData;
                Console.WriteLine("Groups found: " + resultsG.Count);
                foreach (Apigee.Net.Models.ApigeeGroup group in resultsG)
                {
                    Console.WriteLine(group.Title + ": " + group.Path + "; UUID: " + group.Uuid);
                }
            }
            else Console.WriteLine("Error getting Groups: " + response.Error.Message);
        }

        private static void GetUsers()
        {
            var response = aClient.GetUsers();
            if (response.success)
            {
                var resultsG = (List<ApigeeUser>)response.ResponseData;
                Console.WriteLine("Users found: " + resultsG.Count);
                foreach (ApigeeUser user in resultsG)
                {
                    Console.WriteLine(user.Username + ": " + user.Email + "; UUID: " + user.Uuid);
                }
            }
            else Console.WriteLine("Error getting Users: " + response.Error.Message);
        }


        private static void AddNewUser()
        {
            var newUser = new Apigee.Net.Models.ApigeeUser();
            Console.WriteLine("Creating a New User: Infomation");
            Console.WriteLine("User Name");
            newUser.Username = Console.ReadLine();
            Console.WriteLine("First Name");
            newUser.Firstname = Console.ReadLine();
            Console.WriteLine("Last Name");
            newUser.Lastname = Console.ReadLine();
            Console.WriteLine("Email");
            newUser.Email = Console.ReadLine();
            Console.WriteLine("Password");
            newUser.Password = Console.ReadLine();

            Console.WriteLine("Creating Account...");

            //API call
            var res = aClient.CreateAppUser(newUser);
            if (res.success)
                Console.WriteLine("Success! Account Created..");
            else
            {
                Console.WriteLine("Error! Creation failed.");
                Console.WriteLine("Reason: " + res.Error.Message);
            }            
        }


        private static void AddNewGroup()
        {
         var newGroup = new Apigee.Net.Models.ApigeeGroup();
            Console.WriteLine("Creating a New Charity Orginization: (group)");
            Console.WriteLine("Tite");
            newGroup.Title =  Console.ReadLine();
            Console.WriteLine("Path");
            newGroup.Path = Console.ReadLine();

            Console.WriteLine("Creating Group...");
            try
            {
                aClient.CreateGroup(newGroup);
                Console.WriteLine("Success! Group Created..");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error! Creation failed.");
                Console.WriteLine("Reason: " + e.Message + " ::" + e.GetType().ToString());
            }
        
        }

    }
}

