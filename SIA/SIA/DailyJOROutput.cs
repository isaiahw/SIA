using System;
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

namespace SIA
{
    public class DailyJOROutput : ContentPage
    {
        public ObservableCollection<DPROutputModel> scannedItems = new ObservableCollection<DPROutputModel>()
        {
            
                            
        };
        

        ZXingScannerPage scanPage;
        public Label DPRIdLbl = new Label();
        public Label lblJORNo = new Label();
        public Label lblMachineNo = new Label();
        public Label lblPostDate = new Label();
        public Label lblShift = new Label();
        public Label lblBarCodeRefNo = new Label();
        private Func<Task> loadDPROutput;
        public Entry mstId = new Entry { Placeholder = "File Name" };        
        public Button scanBtn = new Button { Text = "Add Scan" };
        public Button delSelectedBtn = new Button { Text = "Delete Selected Items" };
        public Button saveToServerBtn = new Button { Text = "Save to Server", IsEnabled = false };
        public ListView lstView = new ListView();       
        private bool _isConnected=false;
        public DPROutputModel DPROutput = new DPROutputModel();
        public DailyJOR parentTabbedPage;        

        public string itemType = null;
        
        public double totalQtyScanned=0;

        public DailyJOROutput()
        {
            this.Title = "Output Page";

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

            loadDPROutput += async delegate
            {
                //load dproutput into scanneditems
                scannedItems.Clear();
                await DPROutput.GetDPROutput(DPRIdLbl.Text);
                

                foreach (var i in DPROutput.TmpCollection)
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
                var delSwitch = new Switch();
                delSwitch.HorizontalOptions = LayoutOptions.End;
                delSwitch.SetBinding(Switch.IsToggledProperty, "isDeleteScan");

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

                  

                var description = new Label();
                description.SetBinding(Label.TextProperty, "description");                  
                description.FontSize = 13;
                description.FontAttributes = FontAttributes.Bold;                                    
                description.HorizontalOptions = LayoutOptions.FillAndExpand;
                description.Margin = new Thickness(0);
                description.VerticalOptions = LayoutOptions.Center;
                //description.BackgroundColor = Color.Pink;

                var loc = new Label();
                loc.SetBinding(Label.TextProperty, "loc");
                loc.FontSize = 10;
                loc.Margin = new Thickness(0);
                loc.VerticalOptions = LayoutOptions.Center;
                loc.HorizontalOptions = LayoutOptions.End;
                loc.HorizontalTextAlignment = TextAlignment.End;

                var lotNumber = new Label();
                lotNumber.SetBinding(Label.TextProperty, "lotNumber");
                lotNumber.HorizontalOptions = LayoutOptions.FillAndExpand;
                lotNumber.FontSize = 10;
                lotNumber.Margin = new Thickness(0);
                lotNumber.VerticalOptions = LayoutOptions.Center;
                //lotNumber.BackgroundColor = Color.Aqua;

                var status = new Label();
                status.SetBinding(Label.TextProperty, "Id");
                status.FontSize = 13;
                status.FontAttributes = FontAttributes.Bold;
                status.HorizontalOptions = LayoutOptions.FillAndExpand;
                status.Margin = new Thickness(0);
                status.VerticalOptions = LayoutOptions.Center;
                status.HorizontalTextAlignment = TextAlignment.End;                  
                //status.BackgroundColor = Color.Aqua;

                var remainingQty = new Label();
                remainingQty.SetBinding(Label.TextProperty, "remainingQty");
                remainingQty.HorizontalOptions = LayoutOptions.FillAndExpand;
                remainingQty.HorizontalTextAlignment = TextAlignment.End;
                remainingQty.FontSize = 10;
                remainingQty.Margin = new Thickness(0);
                remainingQty.VerticalOptions = LayoutOptions.Center;
                //remainingQty.BackgroundColor = Color.Aqua;

                var barCode = new Label();
                barCode.SetBinding(Label.TextProperty, "displayInfo");
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

                        foreach (DPROutputModel i in scannedItems)
                        {
                            if (y == x)
                            {
                                postResult = await i.PostOutputCSV(i.barCode,mstId.Text.ToString().Trim(),i.DprID,lblJORNo.Text.ToString().Trim(),i.Id, 1);
                            }
                            else
                            {
                                postResult = await i.PostOutputCSV(i.barCode, mstId.Text.ToString().Trim(), i.DprID, lblJORNo.Text.ToString().Trim(), i.Id);
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
                            UserDialogs.Instance.Alert("Successfully saved to " + mstId.Text, "Success");
                            SoundPlayer.PlaySound(523.25);
                            scannedItems.Clear();
                            await loadDPROutput();
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
                    UserDialogs.Instance.ShowError("Empty List !", 2000);
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
                        

                        var newItem = new DPROutputModel();
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

                        
                        //check if label item is the same as jor item
                        if (result.Text.Substring(0, 3).Trim() != tabbedPage.DPRs.DPRBarCodeRefno.ToString().Trim())
                        {
                            UserDialogs.Instance.ShowError("Wrong Barcode! (" + result.Text + ") does not belong to " + lblJORNo.Text, 2000);
                            SoundPlayer.PlaySound(65.4, 500);
                            return;
                        }

                        if (App.IsConnected == false)
                        {
                            CrossVibrate.Current.Vibration(2000);
                            UserDialogs.Instance.ShowError("Device not connected to SIA WI-FI! Unable to get details for scanned item (" + result.Text + ")", 2000);
                            return;
                        }


                        await newItem.GetData(itemType, result.Text);

                        //check duplicate
                        foreach (DPROutputModel i in scannedItems)
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
                        //need to check for posted output on DB(NAV Item Ledger Entry & dailyOutputLot) as well
                        //from output scanned but not posted (dailyOutputLot)
                        try
                        {
                            var Url = new Uri("http://172.11.66.181/xampp/SIACheckDailyOutput.php?lotNumber=" + newItem.lotNumber);

                            var client = new HttpClient();

                            var json = await client.GetAsync(Url);

                            json.EnsureSuccessStatusCode();

                            string contents = await json.Content.ReadAsStringAsync();
                            if (contents.Trim().ToUpper() == "FOUND")
                            {
                                SoundPlayer.PlaySound(65.4, 500);
                                CrossVibrate.Current.Vibration(2000);
                                UserDialogs.Instance.ShowError("Item " + newItem.lotNumber + " is already scanned", 2000);
                                scanPage.IsScanning = false;
                                duplicate = true;
                            }

                        }
                        catch (System.Exception e) { var x = e.ToString(); x = null; }
                        //from output already posted(NAV Item Ledger Entry)
                        try
                        {
                            var Url = new Uri("http://172.11.66.181/xampp/SIACheckItemLastStatus.php?entryRef=" + newItem.entryRef + "&barcoderef=" + newItem.barCodeRef);

                            var client = new HttpClient();

                            var json = await client.GetAsync(Url);

                            json.EnsureSuccessStatusCode();

                            string contents = await json.Content.ReadAsStringAsync();
                            if (contents.Trim().ToUpper() == "FOUND")
                            {
                                SoundPlayer.PlaySound(65.4, 500);
                                CrossVibrate.Current.Vibration(2000);
                                UserDialogs.Instance.ShowError("Item " + newItem.lotNumber + " is already posted in NAV", 2000);
                                scanPage.IsScanning = false;
                                duplicate = true;
                            }

                        }

                        catch (System.Exception e) { var x = e.ToString(); x = null; }

                        if (!duplicate)
                        {
                            

                            //save to db
                            try
                            {
                                string test ="http://172.11.66.181/xampp/SIAsaveoutput.php?dprid=" + newItem.DprID + "&barcode=" + newItem.barCode + "&lotnumber=" + newItem.lotNumber;
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
                                    UserDialogs.Instance.Alert("Successfully scanned " + newItem.lotNumber, "Success");
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

            var itemsToRemove = new ObservableCollection<DPROutputModel>();
            totalQtyScanned = 0;

            foreach (DPROutputModel i in scannedItems)
            {
                if (i.isDeleteScan)
                {
                    itemsToRemove.Add(i);
                }
                totalQtyScanned = totalQtyScanned + i.quantity;
            }

            foreach (DPROutputModel i in itemsToRemove)
            {
                scannedItems.Remove(i);
                totalQtyScanned = totalQtyScanned - i.quantity;
                //also delete from DB ??
                //...http://172.11.66.181/xampp/SIADelDailyDPROutput.php?id=4
                try
                {
                    var Url = new Uri("http://172.11.66.181/xampp/SIADelDailyDPROutput.php?id=" + i.Id);

                    var client = new HttpClient();

                    var json = await client.GetAsync(Url);

                    json.EnsureSuccessStatusCode();

                    string contents = await json.Content.ReadAsStringAsync();                    
                }

                catch (System.Exception ex) { var x = ex.ToString(); x = null; }
            }
            //Get lstView and update the header with scannedItems count and total quantity 
            lstView.Header = scannedItems.Count().ToString() + " lot scanned. Total Qty = " + totalQtyScanned.ToString();
            await loadDPROutput();
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
                UserDialogs.Instance.ShowError("Device not connected to SIA WI-FI!", 2000);
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
                lblPostDate.Text = parentTabbedPage.PostDate.ToString();
                lblShift.Text = parentTabbedPage.Shift;
                mstId.Text = "JOR"+parentTabbedPage.JORNumber.Substring(parentTabbedPage.JORNumber.Length-5,5) + "-" + parentTabbedPage.MachineNo + "-" + parentTabbedPage.PostDate.ToString("ddMMMyyyy") + "-" + parentTabbedPage.Shift;
                loadDPROutput();
                lstView.ItemsSource = scannedItems;
            }
            else
            {
                lblJORNo.Text = "JOR Number not selected";
            }

        }


    }
}
