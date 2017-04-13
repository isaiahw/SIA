using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Forms;
using Acr.UserDialogs;
using System.Data.SqlClient;

namespace SIA.Droid
{
    [Activity(Label = "SIA", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            UserDialogs.Init(this);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            
            LoadApplication(new App());
        }
    }
}

