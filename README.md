Apigee.Net 
==========

This is a Portable Library of C# Client for the Apigee Usergrid : https://apigee.com/usergrid/

This Portable Library is in early Alpha - only works with basic functionalities.
Results May Vary. 

What's a Portable Library? 
=========
Portable library is a .NET binary lib (dll) compatible with few different platforms.

This Apigee.Net ProtLib is compatible with .NET4, .NET 4.5, Win8 Apps & Windows Phone 7.5 and 8.
For this to work, Visual Studio combines the beggist set of APIs - which is common to all platforms.

Atleast for now, some features aren't joint, for example to the Networking libraries.
This makes it difficult to write this kind of Client, since it's based on HTTP requests.

The solution is simple. Interfaces. heard of it?
This Portable Library defiens an IHttpTools interface. this interface supplies us with different
Http request Methods. 
In order to use this Portable Library, you must supply an IHttpTools Implementation, on the constructor.
This way we write the Portble Library logic, in terms of handling Apigee calls and models, and
each platform, supplies its best way to send the Http requests.

Currently we have IHttpTools Implementations for .Net 4.5 & Windows Phone 7.
the implementations are inside the relevant Sample App.

Try It!

Usage is as follows :


            ApigeeClient apiClient; 
            apiClient = new ApigeeClient("http://api.usergrid.com/xxx/sandbox/", new IHttpToolsImplementation()

            //Get a collection of all users 
            var allUsers = apiClient.GetUsers();

            string un = "apigee_" + Guid.NewGuid();

            //Create a new Account
            apiClient.CreateAccount(new UserModel
            {
                Username = un,
                Password = "abc123",
                Email = un + "@zaxyinc.com"
            });

            //Update an Existing Account
            apiClient.UpdateAccount(new UserModel
            {
                Username = un,
                Password = "abc123456",
                Email = un + "@zaxyinc.com"
            });

            //Login User - Get Token 
            var token = apiClient.GetToken(un, "abc123456");

            //Lookup a user by token ID
            var username = apiClient.LookUpToken(token);
            
            and more & more methods every commit.
