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
using Acr.UserDialogs;

namespace SIA
{
    public class ShipmentPage : ContentPage
    {
        public ObservableCollection<ScannedItem> scannedItems = new ObservableCollection<ScannedItem>()
        {
            //new ScannedItem("340200      PC 3713529  ","FG")

        };


        ZXingScannerPage scanPage;
        public Entry mstId = new Entry { Placeholder = "File Name" };
        public Button scanBtn = new Button { Text = "Add Scan" };
        public Button delSelectedBtn = new Button { Text = "Delete Selected Items" };
        public Button saveToServerBtn = new Button { Text = "Save to Server" };
        public ListView lstView = new ListView();
        private bool _isConnected = false;
        public bool IsConnected
        {
            get { return _isConnected; }
            set { _isConnected = value; }
        }

        public string itemType = null;

        public double totalQtyScanned = 0;

        public ShipmentPage()
        {
            this.Title = "Shipment Page";

            Task.Run(async () =>
            {
                if (await CrossConnectivity.Current.IsRemoteReachable("172.11.66.181"))
                {
                    IsConnected = true;
                }
                else
                {
                    IsConnected = false;
                }
            });

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
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    RowSpacing = -3,
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
                status.SetBinding(Label.TextProperty, "status");
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
                Padding = new Thickness(0, 60, 0, 0),
                Children = {
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

                    if (IsConnected)
                    {
                        mstId.Text = DateTime.Now.ToString("ddMMMyyyy-HH-mm") + " " + mstId.Text + " " + scannedItems.Count() + "Lot ";
                        //http://sia35-conf/xampp/siapostshipment.php?barcode=25 250      PC 3611420  &fname=transfer
                        //loop the collection and call above one by one
                        var successFlag = true;
                        var x = scannedItems.Count;
                        var y = (int)1;
                        var postResult = String.Empty;

                        foreach (ScannedItem i in scannedItems)
                        {
                            if (y == x)
                            {
                                postResult = await i.PostDetailsShipment(i.barCode, mstId.Text,1);
                            }
                            else
                            {
                                postResult = await i.PostDetailsShipment(i.barCode, mstId.Text);
                            }
                            

                            if (postResult.Substring(postResult.Length - 7, 7) != "Success")
                            {
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
                            UserDialogs.Instance.ShowSuccess("Successfully saved to " + mstId.Text, 500);
                            
                            scannedItems.Clear();
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
                //if (itemType == null)
                //{
                //    CrossVibrate.Current.Vibration(2000);

                //    await DisplayAlert("Alert", "Please select RM or FG", "OK");
                //}
                //else
                //{
                //uncomment below for release
                var duplicate = false;
                var newItem = new ScannedItem();
                var scanResult = "";
                scanPage = new ZXingScannerPage();

                scanPage.OnScanResult += (result) =>
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {

                        scanResult = result.Text;
                        scanPage.IsScanning = false;
                        await Navigation.PopAsync();
                        if (scanResult.Length < 23)
                        {
                            UserDialogs.Instance.ShowError("Wrong Barcode! (" + scanResult + ")", 2000);
                            return;
                        }




                        if (IsConnected == false)
                        {
                            CrossVibrate.Current.Vibration(2000);
                            UserDialogs.Instance.ShowError("Device not connected to SIA WI-FI! Unable to get details for scanned item (" + scanResult + ")", 2000);
                        }

                        await newItem.GetData(itemType, scanResult);

                        //check duplicate
                        foreach (ScannedItem i in scannedItems)
                        {
                            if (i.entryRef == newItem.entryRef)
                            {
                                CrossVibrate.Current.Vibration(2000);
                                UserDialogs.Instance.ShowError("Item " + newItem.lotNumber + " is already scanned", 2000);
                                scanPage.IsScanning = false;
                                duplicate = true;
                                break;
                            }

                        }
                        if (!duplicate)
                        {
                            duplicate = false;
                            scannedItems.Add(newItem);
                            mstId.Text = newItem.description;
                            totalQtyScanned = totalQtyScanned + newItem.quantity;
                            //Get lstView and update the header with scannedItems count and total quantity 
                            lstView.Header = scannedItems.Count().ToString() + " lot scanned. Total Qty = " + totalQtyScanned.ToString();
                            CrossVibrate.Current.Vibration();
                            UserDialogs.Instance.ShowSuccess("Successfully scanned " + newItem.lotNumber, 500);
                        }

                    });

                };
                //uncomment above for release

                //for debuggin purposes. to be commented on release
                //var duplicate = false;
                //var newItem = new ScannedItem();
                //var scanResult = "340200      PC 3713529  ";
                //await newItem.GetData(itemType, scanResult);
                //scannedItems.Add(newItem);
                //mstId.Text = newItem.description;
                //for debuggin purposes. to be commented on release

                //uncomment below for release
                await Navigation.PushAsync(scanPage);
                //uncomment above for release
                //}
            };

            delSelectedBtn.Clicked += OnDelButtonClicked;
        }

        void listSelection(object sender, SelectedItemChangedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }

        void OnDelButtonClicked(object sender, EventArgs e)
        {
            //get ScannedItem where isdeletescan is true and delete from ScannedItems

            var itemsToRemove = new ObservableCollection<ScannedItem>();
            totalQtyScanned = 0;

            foreach (ScannedItem i in scannedItems)
            {
                if (i.isDeleteScan)
                {
                    itemsToRemove.Add(i);
                }
                totalQtyScanned = totalQtyScanned + i.quantity;
            }

            foreach (ScannedItem i in itemsToRemove)
            {
                scannedItems.Remove(i);
                totalQtyScanned = totalQtyScanned - i.quantity;
            }
            //Get lstView and update the header with scannedItems count and total quantity 
            lstView.Header = scannedItems.Count().ToString() + " lot scanned. Total Qty = " + totalQtyScanned.ToString();
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
                CrossVibrate.Current.Vibration(2000);
                UserDialogs.Instance.ShowError("Device not connected to SIA WI-FI!", 2000);
            }
        }


    }
}
