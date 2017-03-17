
using Plugin.Connectivity;
using Plugin.Toasts;
using Plugin.Vibrate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace SIA
{
    public partial class BlankPage : ContentPage
    {
        private bool _isConnected = false;
        public bool IsConnected
        {
            get { return _isConnected; }
            set { _isConnected = value; }
        }

        public BlankPage()
        {        
            InitializeComponent();
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

        private async void ShowToast(ToastNotificationType type, string text, int second)
        {
            var notificator = DependencyService.Get<IToastNotificator>();
            bool tapped = await notificator.Notify(type, type.ToString().ToLower(), text, TimeSpan.FromSeconds(second));
        }
    }

    
}
