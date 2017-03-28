using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Acr.UserDialogs;
using Plugin.Vibrate;

namespace SIA
{
    public class DailyJOR:TabbedPage
    {
        private string _JORNumber="";
        private string _JORDescription;
        private string _MachineNo;
        private DateTime _PostDate;
        private string _Shift;
        private int _DPRid;
        private string _UserName;
        public LoginResult credential;

        private Boolean _Editable=false;

        private readonly ContentPage _dailyJORRecord;
        private readonly ContentPage _dailyJOROutput;
        private readonly ContentPage _dailyJORConsumption;
        private readonly ContentPage _dailyJORDowntime;

        private ReleasedJORModel _ReleasedJORs = new ReleasedJORModel();
        private DPRModel _DPRs = new DPRModel();
        private Func<Task> loadReleasedJOR;

        public DailyJOR()
        {            
            _dailyJORRecord = new DailyJORRecord() { Title = "JOR Record" };
            _dailyJOROutput = new DailyJOROutput() { Title = "JOR Output" };
            _dailyJORConsumption = new DailyJORConsumption() { Title = "JOR Consumption" };
            _dailyJORDowntime = new DailyJORDowntime() { Title = "JOR Downtime" };
            

            loadReleasedJOR += async delegate
            {
                if (await CrossConnectivity.Current.IsRemoteReachable("172.11.66.181"))
                {
                    await _ReleasedJORs.GetReleasedJOR();
                }
                else
                {
                    CrossVibrate.Current.Vibration(2000);
                    UserDialogs.Instance.ShowError("Device not connected to SIA WI - FI! Please connect and try again", 2000);                    
                }
                
            };

            loadReleasedJOR();
            Children.Add(_dailyJORRecord);
            Children.Add(_dailyJORDowntime);
            Children.Add(_dailyJOROutput);
            Children.Add(_dailyJORConsumption);


            
        }

        public string JORNumber
        {
            get { return _JORNumber; }
            set
            {
                _JORNumber = value;                
            }
        }

        public Boolean Editable
        {
            get { return _Editable; }
            set
            {
                _Editable = value;
            }
        }

        public int DPRid
        {
            get { return _DPRid; }
            set
            {
                _DPRid = value;
            }
        }

        public string Shift
        {
            get { return _Shift; }
            set
            {
                _Shift = value;
            }
        }
        public string MachineNo
        {
            get { return _MachineNo; }
            set
            {
                _MachineNo = value;
            }
        }

        public DateTime PostDate
        {
            get { return _PostDate; }
            set
            {
                _PostDate = value;
            }
        }
        public string JORDescription
        {
            get { return _JORDescription; }
            set
            {
                _JORDescription = value;                
            }
        }

        public ReleasedJORModel ReleasedJORs
        {
            get { return _ReleasedJORs; }            
        }

        public DPRModel DPRs
        {
            get
            {
                return _DPRs;
            }

            set
            {
                _DPRs = value;
            }
        }

        public string UserName
        {
            get
            {
                return _UserName;
            }

            set
            {
                _UserName = value;
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            







            await loadReleasedJOR();
        }

    }
}
