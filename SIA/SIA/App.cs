using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Plugin.Connectivity;
using Microsoft.Identity.Client;

namespace SIA
{
    public class App : Application
    {
        public static PublicClientApplication ClientApplication { get; set; }
        public static string[] Scopes = { "User.Read" };


        public App()
        {
            // The root page of your application

            ClientApplication = new PublicClientApplication("59593ad4-7f4a-4c77-81a3-0439159d7da5");
            //var content = new Login();
            //MainPage = new NavigationPage(content);
            MainPage = new MainPage();
            
        }



        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
