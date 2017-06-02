using Acr.UserDialogs;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SIA
{
    public class DailyJORDowntime : ContentPage
    {        
        public Label Lbl1 = new Label { Text = "JOR No:", TextColor = Color.Aqua };
        public Label Lbl2 = new Label { Text = "DPR Date:", TextColor = Color.Aqua };
        public Label Lbl3 = new Label { Text = "DPR Shift:", TextColor = Color.Aqua };
        public Label Lbl4 = new Label { Text = "DPR Machine:", TextColor = Color.Aqua };
        public Label lbl5 = new Label { Text = "From:", TextColor = Color.Aqua, VerticalOptions = LayoutOptions.CenterAndExpand };
        public Label lbl6 = new Label { Text = "To:", TextColor = Color.Aqua, VerticalOptions = LayoutOptions.CenterAndExpand };
        public Label lbl7 = new Label { Text = "Reason:", TextColor = Color.Aqua};

        public ObservableCollection<DPRDowntimeModel> DPRDowntimes = new ObservableCollection<DPRDowntimeModel>();
        public DPRDowntimeModel DPRDowntime = new DPRDowntimeModel();

        public Label lblTotalHr = new Label();
        public Label lblJORNo = new Label();
        public Label lblMachineNo = new Label();
        public Label lblPostDate = new Label();
        public Label lblShift = new Label();
        public Label DPRidLbl = new Label();
        public DatePicker dateFrom = new DatePicker();
        public DatePicker dateTo = new DatePicker();
        public TimePicker timeFrom = new TimePicker();
        public TimePicker timeTo = new TimePicker();
        private ObservableCollection<ReasonModel> DTReasons = new ObservableCollection<ReasonModel>();
        public Button delSelectedBtn = new Button { Text = "Delete Selected Items" };
        public Button SaveDowntime = new Button
        {
            Text = "Submit",
            HorizontalOptions = LayoutOptions.CenterAndExpand
        };
        private bool authorized = false;
        public BindablePicker DowntimeReason = new BindablePicker
        {
            Title = "Select Downtime reason"            
        };
        public ListView DTLstView = new ListView();
        private Func<Task> loadDPRDowntime;
        private Func<Task> promptAuthorization;
        

        public DailyJORDowntime()
        {
            dateFrom.Format = "yyyy-MMM-dd";
            dateTo.Format = "yyyy-MMM-dd";
            timeFrom.Format = "HH:mm";
            timeTo.Format = "HH:mm";
            var scrollview = new ScrollView();
            var stackDPRInfo = new StackLayout { };
            var stack0 = new StackLayout { Orientation = StackOrientation.Horizontal };
            stack0.Children.Add(Lbl1);
            stack0.Children.Add(lblJORNo);
            stack0.Children.Add(Lbl4);
            stack0.Children.Add(lblMachineNo);
            var stack1 = new StackLayout { Orientation = StackOrientation.Horizontal };
            stack1.Children.Add(Lbl2);
            stack1.Children.Add(lblPostDate);
            stack1.Children.Add(Lbl3);
            stack1.Children.Add(lblShift);
            var stackEntry = new StackLayout { Orientation = StackOrientation.Horizontal };
            stackEntry.Children.Add(lbl5);
            stackEntry.Children.Add(dateFrom);
            stackEntry.Children.Add(timeFrom);
            var stackEntry1 = new StackLayout { Orientation = StackOrientation.Horizontal };
            stackEntry1.Children.Add(lbl6);
            stackEntry1.Children.Add(dateTo);
            stackEntry1.Children.Add(timeTo);
            var stackEntry2 = new StackLayout { Orientation = StackOrientation.Horizontal };
            stackEntry2.Children.Add(DowntimeReason);
            stackEntry2.Children.Add(SaveDowntime);

            stackDPRInfo.Children.Add(stack0);
            stackDPRInfo.Children.Add(stack1);
            delSelectedBtn.Clicked += OnDelButtonClicked;
            DTLstView.ItemSelected += listSelection;

            DTLstView.ItemTemplate = new DataTemplate(() =>
            {

                //begin trying custom cell
                var parentLayout = new StackLayout();
                var delSwitch = new Switch();
                delSwitch.HorizontalOptions = LayoutOptions.End;
                delSwitch.SetBinding(Switch.IsToggledProperty, "IsDelete");

                var gridLayout = new Grid
                {
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    RowSpacing = -3,
                    RowDefinitions =
                    {
                        new RowDefinition { Height = GridLength.Auto },
                        new RowDefinition { Height = GridLength.Auto }
                    },

                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(30) },
                        new ColumnDefinition { Width = new GridLength(1,GridUnitType.Auto) },
                        new ColumnDefinition { Width = new GridLength(1,GridUnitType.Auto) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                    }
                };

                var downfromLbl = new Label { Text = "Down From", TextColor = Color.Aqua };
                var downtoLbl = new Label { Text = "Down To", TextColor = Color.Aqua };
                var reasonIdLbl = new Label { Text = "Reason", TextColor = Color.Aqua };
                var DTIDLbl = new Label { Text = "" };

                var DTID = new Label();
                DTID.SetBinding(Label.TextProperty, "DailyDTID");
                DTID.FontSize = 13;
                //DTID.FontAttributes = FontAttributes.Bold;
                DTID.HorizontalOptions = LayoutOptions.FillAndExpand;
                DTID.Margin = new Thickness(0);
                DTID.VerticalOptions = LayoutOptions.Center;

                var downfrom = new Label();
                downfrom.SetBinding(Label.TextProperty, "DownFrom");
                downfrom.FontSize = 13;
                //downfrom.FontAttributes = FontAttributes.Bold;
                downfrom.HorizontalOptions = LayoutOptions.FillAndExpand;
                downfrom.Margin = new Thickness(0);
                downfrom.VerticalOptions = LayoutOptions.Center;
                //description.BackgroundColor = Color.Pink;

                var downto = new Label();
                downto.SetBinding(Label.TextProperty, "DownTo");
                downto.FontSize = 13;
                //downto.FontAttributes = FontAttributes.Bold;
                downto.HorizontalOptions = LayoutOptions.FillAndExpand;
                downto.Margin = new Thickness(0);
                downto.VerticalOptions = LayoutOptions.Center;

                var reasonid = new Label();
                reasonid.SetBinding(Label.TextProperty, "ReasonDesc");
                reasonid.FontSize = 13;
                //reasonid.FontAttributes = FontAttributes.Bold;
                reasonid.HorizontalOptions = LayoutOptions.FillAndExpand;
                reasonid.Margin = new Thickness(0);
                reasonid.VerticalOptions = LayoutOptions.Center;

                var reasonDesc = new Label();
                //status.SetBinding(Label.TextProperty, "status");
                //status.FontSize = 13;
                //status.FontAttributes = FontAttributes.Bold;
                //status.HorizontalOptions = LayoutOptions.FillAndExpand;
                //status.Margin = new Thickness(0);
                //status.VerticalOptions = LayoutOptions.Center;
                //status.HorizontalTextAlignment = TextAlignment.End;
                ////status.BackgroundColor = Color.Aqua;

                gridLayout.Children.Add(downfromLbl, 1, 0);
                gridLayout.Children.Add(downtoLbl, 2, 0);
                gridLayout.Children.Add(reasonIdLbl, 3, 0);
                gridLayout.Children.Add(DTIDLbl, 0, 0);
                gridLayout.Children.Add(downfrom, 1, 1);
                gridLayout.Children.Add(downto, 2, 1);
                gridLayout.Children.Add(reasonid, 3, 1);
                gridLayout.Children.Add(DTID, 0, 1);



                parentLayout.Orientation = StackOrientation.Horizontal;
                parentLayout.Children.Add(gridLayout);
                parentLayout.Children.Add(delSwitch);


                return new ViewCell { View = parentLayout };
                //end trying custom cell
            });

            loadDPRDowntime += async delegate
            {
                
                await DPRDowntime.GetDPRDowntime(DPRidLbl.Text);
                DPRDowntimes.Clear();
                
                
                foreach (var i in DPRDowntime.TmpCollection)
                {
                    DPRDowntimes.Add(i);
                }                

            };

            promptAuthorization += async delegate
            {
                if (await CrossConnectivity.Current.IsRemoteReachable("172.11.66.181"))
                {
                    LoginResult x = await UserDialogs.Instance.LoginAsync("Authorization");
                    //check editDeleteAuth.php
                    try
                    {
                        var Url = new Uri("http://172.11.66.181/xampp/siaGetUserGroup.php?id=" + x.Value.UserName);
                        var client = new HttpClient();
                        var json = await client.GetAsync(Url);
                        json.EnsureSuccessStatusCode();
                        string contents = await json.Content.ReadAsStringAsync();
                        contents = contents.ToUpper().Trim();

                        if (contents == "ADMIN" || contents == "SUPERVISOR")
                        {
                            //verify password
                            Url = new Uri("http://172.11.66.181/xampp/siaVerifyPwd.php?id=" + x.Value.UserName + "&pwd=" + x.Value.Password);

                            client = new HttpClient();

                            json = await client.GetAsync(Url);

                            json.EnsureSuccessStatusCode();

                            contents = await json.Content.ReadAsStringAsync();
                            if (contents != "\r\nFALSE")
                                authorized = true;
                            else
                            {
                                authorized = false;
                                UserDialogs.Instance.ShowError("User " + x.Value.UserName + " is not authorized to Delete/Edit",3500);
                            }
                        }
                        else
                        {
                            authorized = false;
                            UserDialogs.Instance.ShowError("User " + x.Value.UserName + " is not authorized to Delete/Edit",3500);
                        }
                    }
                    catch (System.Exception ex) { var zx = ex.ToString(); zx = null; }
                }
                else
                {
                    UserDialogs.Instance.ShowError("Not Connected to SIA Wi-Fi");
                    authorized = false;
                }
            };

            var stack = new StackLayout
            {
                Children = {
                    DPRidLbl,
                    stackDPRInfo,
                    stackEntry,                    
                    stackEntry1,
                    lbl7,
                    stackEntry2,
                    DTLstView,
                    delSelectedBtn
                    }
            };

            //scrollview.Content = stack;
            //Content = scrollview;                                
            Content = stack;

            Device.BeginInvokeOnMainThread(async () =>
            {
                var newReasons = new ReasonModel();
                DTReasons.Clear();
                await newReasons.GetReason("downtime");

                foreach (var i in newReasons.TmpCollection)
                {
                    DTReasons.Add(i);
                }
            });

            DowntimeReason.ItemsSource = DTReasons;
            DowntimeReason.DisplayMemberPath = "Description";

            DowntimeReason.SelectedIndexChanged += (sender, args) =>
            {
                if (DowntimeReason.SelectedIndex != -1)
                {
                    var selectedItem = DowntimeReason.SelectedItem as ReasonModel;
                    //JORNo.Text = selectedItem.ID;

                }
            };

            SaveDowntime.Clicked += async (sender, e) =>
            {
                try
                {
                    var selectedItem = DowntimeReason.SelectedItem as ReasonModel;
                    
                    
                    var Url = new Uri("http://172.11.66.181/xampp/siaSaveDPRDowntime.php?dprid=" + DPRidLbl.Text + "&downfrom=" + UnixDateTimeFormat(dateFrom.Date.Date.ToString("yyyy-MMM-dd"),timeFrom.Time.ToString()) + "&downto=" + UnixDateTimeFormat(dateTo.Date.Date.ToString("yyyy-MMM-dd"),timeTo.Time.ToString()) + "&reasonid="+selectedItem.ID); 

                    var client = new HttpClient();

                    var json = await client.GetAsync(Url);

                    json.EnsureSuccessStatusCode();

                    string contents = await json.Content.ReadAsStringAsync();

                    await loadDPRDowntime();

                }
                catch (System.Exception ex) { var x = ex.ToString(); x = null; }
                DTLstView.ItemsSource = DPRDowntimes;
            };
        }

        void listSelection(object sender, SelectedItemChangedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }

        async void OnDelButtonClicked(object sender, EventArgs e)
        {


            //prompt username password and check authorization
            await promptAuthorization();



            if (authorized)
            {
                var itemsToRemove = new ObservableCollection<DPRDowntimeModel>();

                foreach (DPRDowntimeModel i in DPRDowntimes)
                {
                    if (i.IsDelete)
                    {
                        itemsToRemove.Add(i);
                    }

                }

                foreach (DPRDowntimeModel i in itemsToRemove)
                {
                    DPRDowntimes.Remove(i);
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        try
                        {
                            
                            var Url = new Uri("http://172.11.66.181/xampp/siaDelDailyDPRDT.php?id=" + i.DailyDTID);
                            var client = new HttpClient();
                            var json = await client.GetAsync(Url);
                            json.EnsureSuccessStatusCode();
                            string contents = await json.Content.ReadAsStringAsync();

                        }
                        catch (System.Exception ex) { var x = ex.ToString(); x = null; }
                    });
                }
                
            }
            await loadDPRDowntime();
            DTLstView.ItemsSource = DPRDowntimes;
        }

        private string UnixDateTimeFormat(string oriDate, string oriTime)
        {
            DateTime datetimeFrom = Convert.ToDateTime(oriDate.Trim() + " " + oriTime.Trim());
            return datetimeFrom.ToString("yyyy-MMM-dd HH:mm");            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var parentTabbedPage = this.Parent as DailyJOR;

            if (parentTabbedPage.JORNumber != "")
            {
                DPRidLbl.Text = parentTabbedPage.DPRs.DPRid.ToString();
                lblJORNo.Text = parentTabbedPage.DPRs.DPRjor;
                lblMachineNo.Text = parentTabbedPage.DPRs.DPRmachine;
                lblPostDate.Text = parentTabbedPage.DPRs.DPRdate.ToString("yyyy-MMM-dd");
                dateFrom.Date = parentTabbedPage.DPRs.DPRdate;
                dateTo.Date = parentTabbedPage.DPRs.DPRdate;
                lblShift.Text = parentTabbedPage.DPRs.DPRshift.ToString();
            }
            else
            {
                lblJORNo.Text = "JOR Number not selected";
            }

            //load DPRdowntime where DPRid = DPRidLbl.Text
            
            loadDPRDowntime();

            DTLstView.ItemsSource = DPRDowntimes;
        }
    }
}
