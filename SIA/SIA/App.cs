using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Plugin.Connectivity;
using Microsoft.Identity.Client;
using Acr.UserDialogs;
using System.Threading.Tasks;

namespace SIA
{
    public class App : Application
    {
        public static PublicClientApplication ClientApplication { get; set; }
        public static string[] Scopes = { "User.Read" };
        public static LoginResult Credential { get; set; }
        private bool _isConnected = false;

        public bool IsConnected
        {
            get { return _isConnected; }
            set { _isConnected = value; }
        }

        public App()
        {
            // The root page of your application

            ClientApplication = new PublicClientApplication("59593ad4-7f4a-4c77-81a3-0439159d7da5");
            //var content = new Login();
            
            MainPage = new MainPage();

        }

        public async Task UpdateConnection()
        {
            if (await CrossConnectivity.Current.IsRemoteReachable("172.11.66.181"))
            {
                IsConnected = true;
            }
            else
            {
                IsConnected = false;
            }
        }

        public async void CustomLogin()
        {
            Credential = await UserDialogs.Instance.LoginAsync(new LoginConfig
            {
                Message = "to SIA",
                OkText = "OK",
                CancelText = "Cancel",
                LoginPlaceholder = "ID",
                PasswordPlaceholder = "Password"
            }); ;

            //need to check database for correct credential..
            //if credential false, then call Custom Login Again.
            //Need to change DailyJORRecord.cs on the user and password thingy.. 24Mar2017



            UserDialogs.Instance.ShowSuccess(Credential.LoginText + " " + Credential.Password+" "+Credential.Ok, 5000);
            if (Credential.Ok == false) CustomLogin();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            //CustomLogin();
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
