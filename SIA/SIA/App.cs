using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Plugin.Connectivity;
using Microsoft.Identity.Client;
using Acr.UserDialogs;
using System.Threading.Tasks;
using System.Net.Http;
using Plugin.Vibrate;


namespace SIA
{
    public class App : Application
    {
        public static PublicClientApplication ClientApplication { get; set; }
        public static string[] Scopes = { "User.Read" };
        public static LoginResult Credential { get; set; }
        private static bool _isConnected = false;

        public static bool IsConnected
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
            try
            {


                var Url = new Uri("http://172.11.66.181/xampp/siaVerifyPwd.php?id=" + Credential.LoginText + "&pwd=" + Credential.Password);

                var client = new HttpClient();

                var json = await client.GetAsync(Url);

                json.EnsureSuccessStatusCode();

                string contents = await json.Content.ReadAsStringAsync();
                if (contents != "\r\nFALSE")
                {                    
                    CrossVibrate.Current.Vibration();
                    UserDialogs.Instance.ShowSuccess("Welcome " + Credential.LoginText, 1000);
                    Credential = new LoginResult(true, Credential.LoginText, Credential.Password);                         
                }
                else
                {                    
                    CrossVibrate.Current.Vibration(2000);
                    UserDialogs.Instance.ShowError("Wrong User ID or Password!", 2000);
                }
            }
            catch (System.Exception ex) { var x = ex.ToString(); x = null; }
            
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
