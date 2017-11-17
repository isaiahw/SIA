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
    public class DailyJORScrap : ContentPage
    {        
        public Xamarin.Forms.Label Lbl1 = new Xamarin.Forms.Label { Text = "JOR No:", TextColor = Color.Aqua };
        public Xamarin.Forms.Label Lbl2 = new Xamarin.Forms.Label { Text = "DPR Date:", TextColor = Color.Aqua };
        public Xamarin.Forms.Label Lbl3 = new Xamarin.Forms.Label { Text = "DPR Shift:", TextColor = Color.Aqua };
        public Xamarin.Forms.Label Lbl4 = new Xamarin.Forms.Label { Text = "DPR Machine:", TextColor = Color.Aqua };
        public Xamarin.Forms.Label lbl5 = new Xamarin.Forms.Label { Text = "Scrap Qty:", TextColor = Color.Aqua, VerticalOptions = LayoutOptions.CenterAndExpand };        
        public Xamarin.Forms.Label lbl7 = new Xamarin.Forms.Label { Text = "Reason:", TextColor = Color.Aqua};

        public ObservableCollection<DPRScrapModel> DPRScraps = new ObservableCollection<DPRScrapModel>();
        public DPRScrapModel DPRScrap = new DPRScrapModel();

        public Xamarin.Forms.Label lblTotalHr = new Xamarin.Forms.Label();
        public Xamarin.Forms.Label lblJORNo = new Xamarin.Forms.Label();
        public Xamarin.Forms.Label lblMachineNo = new Xamarin.Forms.Label();
        public Xamarin.Forms.Label lblPostDate = new Xamarin.Forms.Label();
        public Xamarin.Forms.Label lblShift = new Xamarin.Forms.Label();
        public Xamarin.Forms.Label DPRidLbl = new Xamarin.Forms.Label();
        public Entry scrQty = new Entry();
        private ObservableCollection<ReasonModel> ScrReasons = new ObservableCollection<ReasonModel>();
        public Button delSelectedBtn = new Button { Text = "Delete Selected Items" };
        public Button SaveScrap = new Button
        {
            Text = "Submit",
            HorizontalOptions = LayoutOptions.CenterAndExpand
        };
        private bool authorized = false;
        public BindablePicker ScrapReason = new BindablePicker
        {
            Title = "Select Scrap reason"            
        };
        public ListView SCRLstView = new ListView();
        private Func<Task> loadDPRScrap;
        private Func<Task> promptAuthorization;
        

        public DailyJORScrap()
        {            
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
            stackEntry.Children.Add(scrQty);                       
            var stackEntry2 = new StackLayout { Orientation = StackOrientation.Horizontal };
            stackEntry2.Children.Add(ScrapReason);
            stackEntry2.Children.Add(SaveScrap);

            stackDPRInfo.Children.Add(stack0);
            stackDPRInfo.Children.Add(stack1);
            delSelectedBtn.Clicked += OnDelButtonClicked;
            SCRLstView.ItemSelected += listSelection;

            SCRLstView.ItemTemplate = new DataTemplate(() =>
            {

                //begin trying custom cell
                var parentLayout = new StackLayout();
                var delSwitch = new Xamarin.Forms.Switch();
                delSwitch.HorizontalOptions = LayoutOptions.End;
                delSwitch.SetBinding(Xamarin.Forms.Switch.IsToggledProperty, "IsDelete");

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

                var downfromLbl = new Xamarin.Forms.Label { Text = "Down From", TextColor = Color.Aqua };
                var downtoLbl = new Xamarin.Forms.Label { Text = "Scrap Qty", TextColor = Color.Aqua };
                var reasonIdLbl = new Xamarin.Forms.Label { Text = "Reason", TextColor = Color.Aqua };
                var ScrapIDLbl = new Xamarin.Forms.Label { Text = "" };

                var ScrapID = new Xamarin.Forms.Label();
                ScrapID.SetBinding(Xamarin.Forms.Label.TextProperty, "DailyScrapID");
                ScrapID.FontSize = 13;
                //DTID.FontAttributes = FontAttributes.Bold;
                ScrapID.HorizontalOptions = LayoutOptions.FillAndExpand;
                ScrapID.Margin = new Thickness(0);
                ScrapID.VerticalOptions = LayoutOptions.Center;                

                var scrapQtyLbl = new Xamarin.Forms.Label();
                scrapQtyLbl.SetBinding(Xamarin.Forms.Label.TextProperty, "ScrapQty");
                scrapQtyLbl.FontSize = 13;
                //scrapQtyLbl.FontAttributes = FontAttributes.Bold;
                scrapQtyLbl.HorizontalOptions = LayoutOptions.FillAndExpand;
                scrapQtyLbl.Margin = new Thickness(0);
                scrapQtyLbl.VerticalOptions = LayoutOptions.Center;

                var reasonid = new Xamarin.Forms.Label();
                reasonid.SetBinding(Xamarin.Forms.Label.TextProperty, "ReasonDesc");
                reasonid.FontSize = 13;
                //reasonid.FontAttributes = FontAttributes.Bold;
                reasonid.HorizontalOptions = LayoutOptions.FillAndExpand;
                reasonid.Margin = new Thickness(0);
                reasonid.VerticalOptions = LayoutOptions.Center;

                var reasonDesc = new Xamarin.Forms.Label();
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
                gridLayout.Children.Add(ScrapIDLbl, 0, 0);                
                gridLayout.Children.Add(scrapQtyLbl, 2, 1);
                gridLayout.Children.Add(reasonid, 3, 1);
                gridLayout.Children.Add(ScrapID, 0, 1);



                parentLayout.Orientation = StackOrientation.Horizontal;
                parentLayout.Children.Add(gridLayout);
                parentLayout.Children.Add(delSwitch);


                return new ViewCell { View = parentLayout };
                //end trying custom cell
            });

            loadDPRScrap += async delegate
            {
                
                await DPRScrap.GetDPRScrap(DPRidLbl.Text);
                DPRScraps.Clear();
                
                
                foreach (var i in DPRScrap.TmpCollection)
                {
                    DPRScraps.Add(i);
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
                    lbl7,
                    stackEntry2,
                    SCRLstView,
                    delSelectedBtn
                    }
            };

            //scrollview.Content = stack;
            //Content = scrollview;                                
            Content = stack;

            Device.BeginInvokeOnMainThread(async () =>
            {
                var newReasons = new ReasonModel();
                ScrReasons.Clear();
                await newReasons.GetReason("scrap");

                foreach (var i in newReasons.TmpCollection)
                {
                    ScrReasons.Add(i);
                }
            });

            ScrapReason.ItemsSource = ScrReasons;
            ScrapReason.DisplayMemberPath = "Description";

            ScrapReason.SelectedIndexChanged += (sender, args) =>
            {
                if (ScrapReason.SelectedIndex != -1)
                {
                    var selectedItem = ScrapReason.SelectedItem as ReasonModel;
                    //JORNo.Text = selectedItem.ID;

                }
            };

            //SaveScrap.Clicked += async (sender, e) =>
            //{
            //    try
            //    {
            //        var selectedItem = ScrapReason.SelectedItem as ReasonModel;
                    
            //        //todo 09May2017
            //        var Url = new Uri("http://172.11.66.181/xampp/siaSaveDPRScrap.php?dprid=" + DPRidLbl.Text + "&downfrom=" + UnixDateTimeFormat(dateFrom.Date.Date.ToString("yyyy-MMM-dd"),timeFrom.Time.ToString()) + "&scrapQtyLbl=" + UnixDateTimeFormat(dateTo.Date.Date.ToString("yyyy-MMM-dd"),timeTo.Time.ToString()) + "&reasonid="+selectedItem.ID); 

            //        var client = new HttpClient();

            //        var json = await client.GetAsync(Url);

            //        json.EnsureSuccessStatusCode();

            //        string contents = await json.Content.ReadAsStringAsync();

            //        await loadDPRScrap();

            //    }
            //    catch (System.Exception ex) { var x = ex.ToString(); x = null; }
            //    SCRLstView.ItemsSource = DPRScraps;
            //};
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
                var itemsToRemove = new ObservableCollection<DPRScrapModel>();

                foreach (DPRScrapModel i in DPRScraps)
                {
                    if (i.IsDelete)
                    {
                        itemsToRemove.Add(i);
                    }

                }

                foreach (DPRScrapModel i in itemsToRemove)
                {
                    DPRScraps.Remove(i);
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        try
                        {
                            //todo 09May2017
                            var Url = new Uri("http://172.11.66.181/xampp/siaDelDailyDPRScrap.php?id=" + i.DailyScrapID);
                            var client = new HttpClient();
                            var json = await client.GetAsync(Url);
                            json.EnsureSuccessStatusCode();
                            string contents = await json.Content.ReadAsStringAsync();

                        }
                        catch (System.Exception ex) { var x = ex.ToString(); x = null; }
                    });
                }
                
            }
            await loadDPRScrap();
            SCRLstView.ItemsSource = DPRScraps;
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
                //dateFrom.Date = parentTabbedPage.DPRs.DPRdate;
                //dateTo.Date = parentTabbedPage.DPRs.DPRdate;
                scrQty.Text = "0";
                lblShift.Text = parentTabbedPage.DPRs.DPRshift.ToString();
            }
            else
            {
                lblJORNo.Text = "JOR Number not selected";
            }

            //load DPRdowntime where DPRid = DPRidLbl.Text
            
            loadDPRScrap();

            SCRLstView.ItemsSource = DPRScraps;
        }
    }
}
