
using Plugin.Connectivity;
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
        //private bool _isConnected = false;
        //public bool IsConnected
        //{
        //    get { return _isConnected; }
        //    set { _isConnected = value; }
        //}

        public BlankPage()
        {        
            InitializeComponent();
        }
        
        //public async Task UpdateConnection()
        //{
        //    if (await CrossConnectivity.Current.IsRemoteReachable("172.11.66.181"))
        //    {
        //        IsConnected = true;
        //    }
        //    else
        //    {
        //        IsConnected = false;                             
        //    }
        //}

        
    }

    
}
