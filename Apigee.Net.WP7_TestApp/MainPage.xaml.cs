using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Apigee.Net.PortLib;
using Apigee.Net.Networking;
using System.Threading;
using System.IO;
using System.ComponentModel;
using Apigee.Net.Models;

namespace Apigee.Net.WP7_TestApp
{
    public partial class MainPage : PhoneApplicationPage
    {
         
        public static string APIGEE_SERVER_PATH = "http://api.usergrid.com/zaxyinc/sandbox";
        public Apigee.Net.PortLib.ApigeeClient apigeeServer;
        private ApigeeWP7Implementation wp7Impl;
        BackgroundWorker HttpWorker;

        string apigeeUrl;
        
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            
            HttpWorker = new BackgroundWorker();
            HttpWorker.DoWork += HttpWorker_DoWork;
            HttpWorker.RunWorkerCompleted += HttpWorker_RunWorkerCompleted;
            wp7Impl = new ApigeeWP7Implementation();
            

        }

        private bool verifyURL()
        {
            //just a dummy
            if (tbApigeeUrl.Text.StartsWith("http://api.usergrid.com/"))
            {
                apigeeUrl = tbApigeeUrl.Text;
                return true;
            }
            MessageBox.Show("Please enter correct Apigee Application URL");
            return false;
        }


        private void btnTest_Click_Get(object sender, RoutedEventArgs e)
        {
            if (verifyURL())
                HttpWorker.RunWorkerAsync(HttpTools.RequestTypes.Get);
        }

        private void btnTest_Click_Post(object sender, RoutedEventArgs e)
        {
            if (verifyURL())
                HttpWorker.RunWorkerAsync(HttpTools.RequestTypes.Post);
        }
        
        private void HttpWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string response = "";
            apigeeServer = new ApigeeClient(apigeeUrl, new ImplementationStruct() { iHttpTools = new ApigeeWP7Implementation() });
            switch((HttpTools.RequestTypes)e.Argument)
            {
                
                case HttpTools.RequestTypes.Get:
                    response = "Getting User List" + Environment.NewLine;
                    response = apigeeServer.GetUsers().RawResponse; // wp7Impl.PerformGet(apigeeUrl + "/users");
                    break;
                case HttpTools.RequestTypes.Post:
                    var user = new ApigeeUser() { Username = "test" + DateTime.Now.Millisecond, Firstname = "Apgiee.Net", Lastname = "WP7_TestApp", Email =  "test"+DateTime.Now.Millisecond+"@zaxyinc.com", Password = "123456" };
                    response = "Creating account for user: " + user.Username + "(" + user.Email + ")" + Environment.NewLine;
                    response += apigeeServer.CreateAppUser(user).RawResponse; //wp7Impl.PerformJsonRequest<string>(apigeeUrl + "/users", HttpTools.RequestTypes.Post, user);
                    break;
            }
            e.Result = new KeyValuePair<HttpTools.RequestTypes,string>((HttpTools.RequestTypes)e.Argument,response);
        }
        
        private void HttpWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var result = (KeyValuePair<HttpTools.RequestTypes, string>)e.Result;
            switch(result.Key)
            {
                case HttpTools.RequestTypes.Get:
                    txtDebugGet.Text = result.Value;
                    break;
                case HttpTools.RequestTypes.Post:
                    txtDebugPost.Text = result.Value;
                    break;
            }
        }

    }
}