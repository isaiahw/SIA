﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;
using System.Net;
using System.Threading.Tasks;
using Plugin.Vibrate;

using Plugin.Connectivity;
using System.Diagnostics;
using System.Net.Http;
using Acr.UserDialogs;
using Newtonsoft.Json;

namespace SIA
{
    public class DailyJORConsumption : ContentPage
    {
        public ObservableCollection<DPRConsumptionModel> scannedItems = new ObservableCollection<DPRConsumptionModel>()
        {
            
                            
        };
        

        ZXingScannerPage scanPage;
        public Xamarin.Forms.Label DPRIdLbl = new Xamarin.Forms.Label();
        public Xamarin.Forms.Label lblJORNo = new Xamarin.Forms.Label();
        public Xamarin.Forms.Label lblMachineNo = new Xamarin.Forms.Label();
        public Xamarin.Forms.Label lblPostDate = new Xamarin.Forms.Label();
        public Xamarin.Forms.Label lblShift = new Xamarin.Forms.Label();
        public Xamarin.Forms.Label lblBarCodeRefNo = new Xamarin.Forms.Label();
        private Func<Task> loadDPRConsumption;
        public Entry mstId = new Entry { Placeholder = "File Name" };        
        public Button scanBtn = new Button { Text = "Add Scan" };
        public Button delSelectedBtn = new Button { Text = "Delete Selected Items" };
        public Button saveToServerBtn = new Button { Text = "Save to Server" };
        public ListView lstView = new ListView();       
        //private bool _isConnected=false;
        public DPRConsumptionModel DPRConsumption = new DPRConsumptionModel();
        public DailyJOR parentTabbedPage;

        public string itemType = null;
        
        public double totalQtyScanned=0;

        public DailyJORConsumption()
        {
            this.Title = "Consumption Page";

            Task.Run(async () =>
            {
                if (await CrossConnectivity.Current.IsRemoteReachable("172.11.66.181"))
                {
                    App.IsConnected = true;
                }
                else
                {
                    App.IsConnected = false;
                }
            });

            loadDPRConsumption += async delegate
            {
                //load dproutput into scanneditems
                //var parentTabbedPage = this.Parent as DailyJOR;
                //DPRIdLbl.Text = parentTabbedPage.DPRs.DPRid.ToString();
                scannedItems.Clear();
                await DPRConsumption.GetDPRConsumption(DPRIdLbl.Text);
                

                foreach (var i in DPRConsumption.TmpCollection)
                {
                    await i.GetData(null, i.barCode);
                    scannedItems.Add(i);
                };
                lstView.ItemsSource = scannedItems;
            };

            lstView.ItemsSource = scannedItems;
            lstView.ItemSelected += listSelection;            

            lstView.ItemTemplate = new DataTemplate(() =>
            {

                //begin trying custom cell
                var parentLayout = new StackLayout();
                var delSwitch = new Xamarin.Forms.Switch();
                delSwitch.HorizontalOptions = LayoutOptions.End;
                delSwitch.SetBinding(Xamarin.Forms.Switch.IsToggledProperty, "isDeleteScan");

                var gridLayout = new Grid
                {
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions=LayoutOptions.StartAndExpand,
                    RowSpacing=-3,
                    RowDefinitions =
                    {
                        new RowDefinition { Height = GridLength.Auto },
                        new RowDefinition { Height = GridLength.Auto }
                    },

                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(110) },
                        new ColumnDefinition { Width = new GridLength(130) },
                        new ColumnDefinition { Width = new GridLength(1,GridUnitType.Auto) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                    }
                };

                  

                var description = new Xamarin.Forms.Label();
                description.SetBinding(Xamarin.Forms.Label.TextProperty, "description");                  
                description.FontSize = 13;
                description.FontAttributes = FontAttributes.Bold;                                    
                description.HorizontalOptions = LayoutOptions.FillAndExpand;
                description.Margin = new Thickness(0);
                description.VerticalOptions = LayoutOptions.Center;
                //description.BackgroundColor = Color.Pink;

                var loc = new Xamarin.Forms.Label();
                loc.SetBinding(Xamarin.Forms.Label.TextProperty, "loc");
                loc.FontSize = 10;
                loc.Margin = new Thickness(0);
                loc.VerticalOptions = LayoutOptions.Center;
                loc.HorizontalOptions = LayoutOptions.End;
                loc.HorizontalTextAlignment = TextAlignment.End;

                var lotNumber = new Xamarin.Forms.Label();
                lotNumber.SetBinding(Xamarin.Forms.Label.TextProperty, "lotNumber");
                lotNumber.HorizontalOptions = LayoutOptions.FillAndExpand;
                lotNumber.FontSize = 10;
                lotNumber.Margin = new Thickness(0);
                lotNumber.VerticalOptions = LayoutOptions.Center;
                //lotNumber.BackgroundColor = Color.Aqua;

                var status = new Xamarin.Forms.Label();
                status.SetBinding(Xamarin.Forms.Label.TextProperty, "Id");
                status.FontSize = 13;
                status.FontAttributes = FontAttributes.Bold;
                status.HorizontalOptions = LayoutOptions.FillAndExpand;
                status.Margin = new Thickness(0);
                status.VerticalOptions = LayoutOptions.Center;
                status.HorizontalTextAlignment = TextAlignment.End;                  
                //status.BackgroundColor = Color.Aqua;

                var remainingQty = new Xamarin.Forms.Label();
                remainingQty.SetBinding(Xamarin.Forms.Label.TextProperty, "remainingQty");
                remainingQty.HorizontalOptions = LayoutOptions.FillAndExpand;
                remainingQty.HorizontalTextAlignment = TextAlignment.End;
                remainingQty.FontSize = 10;
                remainingQty.Margin = new Thickness(0);
                remainingQty.VerticalOptions = LayoutOptions.Center;
                //remainingQty.BackgroundColor = Color.Aqua;

                var barCode = new Xamarin.Forms.Label();
                barCode.SetBinding(Xamarin.Forms.Label.TextProperty, "displayInfo");
                barCode.HorizontalOptions = LayoutOptions.FillAndExpand;
                barCode.HorizontalTextAlignment = TextAlignment.Start;
                barCode.FontSize = 10;
                barCode.Margin = new Thickness(0);
                barCode.VerticalOptions = LayoutOptions.Center;

                gridLayout.Children.Add(description, 0, 0);
                Grid.SetColumnSpan(description, 3);
                gridLayout.Children.Add(barCode, 0, 1); 
                gridLayout.Children.Add(loc, 2, 1);
                gridLayout.Children.Add(lotNumber, 1, 1);                  
                gridLayout.Children.Add(status, 3, 0);
                gridLayout.Children.Add(remainingQty, 3, 1);


                parentLayout.Orientation = StackOrientation.Horizontal;
                parentLayout.Children.Add(gridLayout);
                parentLayout.Children.Add(delSwitch);


                return new ViewCell { View = parentLayout };
                //end trying custom cell
            });

            

            Content = new StackLayout
            {
                //Padding = new Thickness(0, 20, 0, 0),
                Children = {
                    DPRIdLbl,
                    lblJORNo,
                    lblMachineNo,
                    lblPostDate,
                    lblShift,
                    lblBarCodeRefNo,
                    mstId,                    
                    scanBtn,
                    lstView,
                    delSelectedBtn,
                    saveToServerBtn
                }
                
            };            

            mstId.Text = DateTime.Now.ToString("ddMMMyyyy-HH-mm");

            
            CrossConnectivity.Current.ConnectivityChanged += async (sender, args) =>
            {
                await UpdateConnection();
            };
            
            saveToServerBtn.Clicked += async delegate
            {
                if (scannedItems.Count() > 0)
                {                                        
                    
                    if(App.IsConnected)
                    {
                        //mstId.Text = DateTime.Now.ToString("ddMMMyyyy-HH-mm") + " " + mstId.Text + " " + scannedItems.Count() + "Lot ";
                        //http://sia35-conf/xampp/siaposttransfer.php?barcode=25 250      PC 3611420  &fname=transfer
                        //loop the collection and call above one by one
                        var successFlag = true;
                        var x = scannedItems.Count;
                        var y = 1;
                        var postResult = String.Empty;

                        foreach (DPRConsumptionModel i in scannedItems)
                        {
                            if (y == x)
                            {
                                postResult = await i.PostConsumptionCSV(i.barCode,mstId.Text.ToString().Trim(),i.DprID,lblJORNo.Text.ToString().Trim(),i.Id, 1);
                            }
                            else
                            {
                                postResult = await i.PostConsumptionCSV(i.barCode, mstId.Text.ToString().Trim(), i.DprID, lblJORNo.Text.ToString().Trim(), i.Id);
                            }
                            

                            if (postResult.Substring(postResult.Length - 7, 7) != "Success")
                            {
                                SoundPlayer.PlaySound(65.4, 500);
                                CrossVibrate.Current.Vibration(2000);
                                await DisplayAlert("Alert", "Save to server failed! Please try again.", "OK");
                                successFlag = false;
                                
                                break;
                            }
                            y++;
                        }
                        //end loop
                        if (successFlag == true)
                        {
                            CrossVibrate.Current.Vibration();
                            UserDialogs.Instance.ShowSuccess("Successfully saved to " + mstId.Text, 1000);
                            SoundPlayer.PlaySound(523.25);
                            scannedItems.Clear();
                            await loadDPRConsumption();
                            mstId.Text = "";
                            lstView.Header = "";
                        }
                    }
                    else
                    {
                        CrossVibrate.Current.Vibration(2000);
                        UserDialogs.Instance.ShowError("Device not connected to SIA WI-FI! Unable to save to server", 2000);
                    }
                        
                    
                }
                else
                {
                    CrossVibrate.Current.Vibration(2000);
                    await DisplayAlert("Alert", "Empty List !", "OK");
                }

            };


            scanBtn.Clicked += async delegate
            {

                var duplicate = false;
                
                var scanResult = "";
                
                scanPage = new ZXingScannerPage();

                scanPage.IsScanning = true;
                scanPage.OnScanResult += (result) =>
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        var newItem = new DPRConsumptionModel();
                        newItem.DprID = DPRIdLbl.Text;
                        scanResult = result.Text;
                        scanPage.IsScanning = false;
                        await Navigation.PopAsync();
                        if (result.Text.Length < 23)
                        {
                            UserDialogs.Instance.ShowError("Wrong Barcode! (" + result.Text + ")", 2000);
                            SoundPlayer.PlaySound(65.4, 500);
                            return;
                        }

                        
                        var tabbedPage = this.Parent as DailyJOR;                        

                        if (App.IsConnected == false)
                        {
                            CrossVibrate.Current.Vibration(2000);
                            UserDialogs.Instance.ShowError("Device not connected to SIA WI-FI! Unable to get details for scanned item (" + result.Text + ")", 2000);
                        }

                        await newItem.GetData(itemType, result.Text);

                        //check duplicate
                        foreach (DPRConsumptionModel i in scannedItems)
                        {
                            //from same collection
                            if (i.entryRef == newItem.entryRef)
                            {
                                SoundPlayer.PlaySound(65.4, 500);
                                CrossVibrate.Current.Vibration(2000);
                                UserDialogs.Instance.ShowError("Item " + newItem.lotNumber + " is already scanned", 2000);
                                scanPage.IsScanning = false;
                                duplicate = true;
                                break;
                            }
                        }
                        //need to check for posted output on DB(qryrmlaststatus & dailyConsumptionLot) as well
                        //from consumption scanned but not posted (dailyConsumptionLot). maybe no need to check as 1 RM lot can be consumed multiple times.
                        //try
                        //{
                        //    var Url = new Uri("http://172.11.66.181/xampp/SIACheckDailyConsumption.php?lotNumber=" + newItem.lotNumber);

                        //    var client = new HttpClient();

                        //    var json = await client.GetAsync(Url);

                        //    json.EnsureSuccessStatusCode();

                        //    string contents = await json.Content.ReadAsStringAsync();
                        //    if (contents.Trim().ToUpper() == "FOUND")
                        //    {
                        //        SoundPlayer.PlaySound(65.4, 500);
                        //        CrossVibrate.Current.Vibration(2000);
                        //        UserDialogs.Instance.ShowError("Item " + newItem.lotNumber + " is already scanned", 2000);
                        //        scanPage.IsScanning = false;
                        //        duplicate = true;
                        //    }

                        //}
                        //catch (System.Exception e) { var x = e.ToString(); x = null; }
                        //from consumption already posted(qryrmlaststatus). maybe no need to check as 1 RM lot can be consumed multiple times.

                        //try
                        //{
                        //    var Url = new Uri("http://172.11.66.181/xampp/SIACheckItemLastStatus.php?entryRef=" + newItem.entryRef+"&barcoderef="+newItem.barCodeRef);

                        //    var client = new HttpClient();

                        //    var json = await client.GetAsync(Url);

                        //    json.EnsureSuccessStatusCode();

                        //    string contents = await json.Content.ReadAsStringAsync();
                        //    if (contents.Trim().ToUpper() == "FOUND")
                        //    {
                        //        SoundPlayer.PlaySound(65.4, 500);
                        //        CrossVibrate.Current.Vibration(2000);
                        //        UserDialogs.Instance.ShowError("Item " + newItem.lotNumber + " is already posted", 2000);
                        //        UserDialogs.Instance.Alert("Successfully saved to " + mstId.Text, "Success");
                        //        scanPage.IsScanning = false;
                        //        duplicate = true;
                        //    }

                        //}

                        //catch (System.Exception e) { var x = e.ToString(); x = null; }

                        if (!duplicate)
                        {
                            duplicate = false;


                            //save to db
                            try
                            {
                                string test ="http://172.11.66.181/xampp/SIAsaveConsumption.php?dprid=" + newItem.DprID + "&barcode=" + newItem.barCode + "&lotnumber=" + newItem.lotNumber;
                                var Url = new Uri(test);

                                var client = new HttpClient();

                                var json = await client.GetAsync(Url);
                                
                                json.EnsureSuccessStatusCode();

                                string contents = await json.Content.ReadAsStringAsync();
                                if (contents.Trim().ToUpper() != "SUCCESS")
                                {
                                    SoundPlayer.PlaySound(65.4, 500);
                                    CrossVibrate.Current.Vibration(2000);                                    
                                    UserDialogs.Instance.ShowError("Item " + newItem.lotNumber + " was not saved successfully", 2000);                                    
                                    scanPage.IsScanning = false;

                                }
                                else
                                {
                                    scannedItems.Add(newItem);
                                    //mstId.Text = newItem.description;
                                    totalQtyScanned = totalQtyScanned + newItem.quantity;
                                    //Get lstView and update the header with scannedItems count and total quantity 
                                    lstView.Header = scannedItems.Count().ToString() + " lot scanned. Total Qty = " + totalQtyScanned.ToString();
                                    UserDialogs.Instance.ShowSuccess("Successfully scanned " + newItem.lotNumber,1000);
                                    SoundPlayer.PlaySound(523.25);
                                    CrossVibrate.Current.Vibration();
                                }

                            }

                            catch (System.Exception e) { var x = e.ToString(); x = null; }
                        }

                    });

                };
                await Navigation.PushAsync(scanPage);
                //await loadDPROutput();
                //lstView.ItemsSource = scannedItems;

            };

            delSelectedBtn.Clicked += OnDelButtonClicked;
        }

        void listSelection(object sender, SelectedItemChangedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }

        async void OnDelButtonClicked(object sender, EventArgs e)
        {
            //get ScannedItem where isdeletescan is true and delete from ScannedItems

            var itemsToRemove = new ObservableCollection<DPRConsumptionModel>();
            totalQtyScanned = 0;

            foreach (DPRConsumptionModel i in scannedItems)
            {
                if (i.isDeleteScan)
                {
                    itemsToRemove.Add(i);
                }
                totalQtyScanned = totalQtyScanned + i.quantity;
            }

            foreach (DPRConsumptionModel i in itemsToRemove)
            {
                scannedItems.Remove(i);
                totalQtyScanned = totalQtyScanned - i.quantity;
                
                try
                {
                    var Url = new Uri("http://172.11.66.181/xampp/SIADelDailyDPRConsumption.php?id=" + i.Id);

                    var client = new HttpClient();

                    var json = await client.GetAsync(Url);

                    json.EnsureSuccessStatusCode();

                    string contents = await json.Content.ReadAsStringAsync();                    
                }

                catch (System.Exception ex) { var x = ex.ToString(); x = null; }
            }
            //Get lstView and update the header with scannedItems count and total quantity 
            lstView.Header = scannedItems.Count().ToString() + " lot scanned. Total Qty = " + totalQtyScanned.ToString();
            await loadDPRConsumption();
            lstView.ItemsSource = scannedItems;

        }

        public async Task UpdateConnection()
        {
            if (await CrossConnectivity.Current.IsRemoteReachable("172.11.66.181"))
            {
                App.IsConnected = true;                
            }
            else
            {
                App.IsConnected = false;
                CrossVibrate.Current.Vibration(2000);
                UserDialogs.Instance.ShowError("Device not connected to SIA WI - FI!", 2000);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            parentTabbedPage = this.Parent as DailyJOR;

            if (parentTabbedPage.JORNumber != "")
            {
                DPRIdLbl.Text = parentTabbedPage.DPRs.DPRid.ToString();                
                lblJORNo.Text = parentTabbedPage.JORNumber;
                lblBarCodeRefNo.Text = parentTabbedPage.DPRs.DPRBarCodeRefno;
                //parentTabbedPage.ReleasedJORs.TmpCollection();
                lblMachineNo.Text = parentTabbedPage.MachineNo;
                lblPostDate.Text = parentTabbedPage.PostDate.ToString("yyyy-MMM-dd");
                lblShift.Text = parentTabbedPage.Shift;
                mstId.Text = "JOR"+parentTabbedPage.JORNumber.Substring(parentTabbedPage.JORNumber.Length-5,5) + "-" + parentTabbedPage.MachineNo + "-" + parentTabbedPage.PostDate.ToString("ddMMMyyyy") + "-" + parentTabbedPage.Shift;
                
                //getdprid
                Task.Run(async () =>
                {
                    try
                    {
                        var Url = new Uri("http://172.11.66.181/xampp/siaGetDPRid.php?jor=" + lblJORNo.Text + "&date=" + lblPostDate.Text + "&shift=" + lblShift.Text + "&machine=" + lblMachineNo.Text);

                        var client = new HttpClient();

                        var json = await client.GetAsync(Url);

                        json.EnsureSuccessStatusCode();

                        string contents = await json.Content.ReadAsStringAsync();
                        if (contents != "\r\nEmpty")
                        {
                            List<DPRModel> tmpModel = await Task.Run(() => JsonConvert.DeserializeObject<List<DPRModel>>(contents));

                            foreach (var i in tmpModel)
                            {
                                DPRIdLbl.Text = i.DPRid.ToString();
                            }

                        }
                        else
                        {
                            lblJORNo.Text = "JOR Number not selected";
                        }
                    }
                    catch (System.Exception ex) { var x = ex.ToString(); x = null; }
                    await loadDPRConsumption();
                });
                
                //getdprid

                
                lstView.ItemsSource = scannedItems;

            }
            else
            {
                lblJORNo.Text = "JOR Number not selected";
            }

        }


    }
}
