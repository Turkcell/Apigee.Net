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

namespace Apigee.Net.WP7_TestApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        
        public static string APIGEE_SERVER_PATH = "http://api.usergrid.com/zaxyinc/sandbox";
        public Apigee.Net.PortLib.ApigeeClient apigeeServer;
        private ApigeeWP7Implementation wp7Impl;
        BackgroundWorker HttpWorker;

        public string result;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            
            HttpWorker = new BackgroundWorker();
            HttpWorker.DoWork += HttpWorker_DoWork;
            HttpWorker.RunWorkerCompleted += HttpWorker_RunWorkerCompleted;
            wp7Impl = new ApigeeWP7Implementation();
            
            
            //apigeeServer = new ApigeeClient(APIGEE_SERVER_PATH,new ImplementationStruct() { iHttpTools = new ApigeeWP7Implementation()});

        }

        private void HttpWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            result = wp7Impl.PerformGet(APIGEE_SERVER_PATH + "/users"); 
        }

        private void HttpWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            txtDebug.Text += result;            
        }

        private void btnTest_Click_1(object sender, RoutedEventArgs e)
        {
            HttpWorker.RunWorkerAsync();
        }


    }
}