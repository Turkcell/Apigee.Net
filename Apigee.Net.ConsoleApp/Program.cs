using Apigee.Net;
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

        static ApigeeClient aClient = new ApigeeClient("http://api.usergrid.com/zaxyinc/sandbox/", new ImplementationStruct() { iHttpTools = new ApigeeNET45() } );
        
        static void Main(string[] args)
        {
            Console.WriteLine("Connecting....");
   
            var results = aClient.GetUsers();
            Console.WriteLine("App Users found: "+results.Count);
            foreach(Apigee.Net.Models.ApigeeUser user in results)
            {
                Console.WriteLine(user.Username + ": " + user.Lastname+", "+user.Firstname+"; UUID: "+user.Uuid );
            }

            Console.WriteLine("\n***\n");
        }
    }
}

/*
            var resultsG = aClient.GetGroups();
            Console.WriteLine("Groups found: " + resultsG.Count);
            foreach (Apigee.Net.Models.ApigeeGroup group in resultsG)
            {
                Console.WriteLine(group.Title + ": " + group.Path + "; UUID: " + group.Uuid);
            }

            Console.WriteLine("\n***\n");

            var resultsR = aClient.GetRoles();
            Console.WriteLine("Roles found: " + resultsR.Count);
            foreach (Apigee.Net.Models.ApigeeRole role in resultsR)
            {
                Console.WriteLine(role.Title + ": " + role.RoleName + "; UUID: " + role.Uuid);
            }
            
            Console.WriteLine("\n***\n");

            while (Console.ReadKey().KeyChar != 'q')
            {
                Console.WriteLine("Choose an option:");
                Console.WriteLine("1. Add User Account");
                Console.WriteLine("2. Add Community Group");

                switch (Convert.ToInt16(Console.ReadLine()))
                {
                    case 1:
                        AddNewUser(aClient);
                        break;
                    case 2:
                        AddNewGroup(aClient);
                        break;
                    default:
                        Console.WriteLine("Invalid choice. try again");
                        Console.Clear();
                        break;
                }
            }
            
        }

        private static void AddNewGroup(ApigeeClient client)
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
                client.CreateGroup(newGroup);
                Console.WriteLine("Success! Group Created..");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error! Creation failed.");
                Console.WriteLine("Reason: " + e.Message + " ::" + e.GetType().ToString());
            }
        
        }

        private static void AddNewUser(ApigeeClient client)
        {
            var newUser = new Apigee.Net.Models.ApigeeUser();
            Console.WriteLine("Creating a New User: Infomation");
            Console.WriteLine("User Name");
            newUser.Username =  Console.ReadLine();
            Console.WriteLine("First Name");
            newUser.Firstname = Console.ReadLine();
            Console.WriteLine("Last Name");
            newUser.Lastname = Console.ReadLine();
            Console.WriteLine("Email");
            newUser.Email = Console.ReadLine();
            Console.WriteLine("Password");
            newUser.Password = Console.ReadLine();

            Console.WriteLine("Creating Account...");
            try
            {
                client.CreateAppUser(newUser);
                Console.WriteLine("Success! Account Created..");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error! Creation failed.");
                Console.WriteLine("Reason: " + e.Message + " ::" + e.GetType().ToString());
            }
        }
    }
}



*/