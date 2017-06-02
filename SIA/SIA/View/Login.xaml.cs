using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace SIA
{
    public partial class Login : ContentPage
    {
        public Login()
        {
            InitializeComponent();
            
        }

        public async void CustomLogin()
        {

            

            App.Credential = await UserDialogs.Instance.LoginAsync(new LoginConfig
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



            UserDialogs.Instance.ShowSuccess(App.Credential.LoginText + " " + App.Credential.Password + " " + App.Credential.Ok, 5000);
            if (App.Credential.Ok == false) CustomLogout();
        }

        public void CustomLogout()
        {
            if (App.Credential != null)
            {
                UserDialogs.Instance.ShowSuccess("Bye "+App.Credential.LoginText + " Please visit again!", 2000);
                App.Credential = null;
            };
        }

        protected override void OnAppearing()
        {
            if (App.Credential != null)
            {
                CustomLogout();
            }
            else
            {
                CustomLogin();
            }
            
        }
    }
}
