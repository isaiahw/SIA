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

using Acr.UserDialogs;

namespace SIA
{
    public class ConsumptionPage : ContentPage
    {
        public ObservableCollection<ScannedItem> scannedItems = new ObservableCollection<ScannedItem>()
        {            
                            
        };
        public ObservableCollection<MachineJORModel> machineJORS = new ObservableCollection<MachineJORModel>();
        ZXingScannerPage scanPage;
        public Entry JORNo = new Entry { Placeholder = "JOR Number"};
        public Button jorBtn = new Button
        {
            Text = "Get JOR",
            HorizontalOptions = LayoutOptions.End
            
        };
        public Button scanBtn = new Button { Text = "Add Consumption" };
        public Button delSelectedBtn = new Button { Text = "Delete Selected Items" };
        public Button saveToServerBtn = new Button { Text = "Save to Server" };
        public ListView lstView = new ListView();
        public MachineJORModel selectedJOR;
        public BindablePicker jorPicker = new BindablePicker
        {
            Title = "Select JOR Number",
            HorizontalOptions = LayoutOptions.Start
        };

        
        public string itemType = null;
        
        public double totalQtyScanned=0;

        public ConsumptionPage(string JN)
        {
            //this.Title = "RM Consumption Page";
            //var parentTabbedPage = this.Parent as DailyJOR;
            JORNo.Text = JN; //parentTabbedPage.JORNumber;


            jorPicker.ItemsSource = machineJORS;
            jorPicker.DisplayMemberPath = "JORNo";

            jorPicker.SelectedIndexChanged += (sender, args) =>
            {
                if (jorPicker.SelectedIndex != -1)
                {
                    var selectedItem = jorPicker.SelectedItem as MachineJORModel;
                    JORNo.Text = selectedItem.JORNo;
                }                
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
                            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
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
                                   
                  var lotNumber = new Label();
                  lotNumber.SetBinding(Label.TextProperty, "lotNumber");                  
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

                  gridLayout.Children.Add(description, 0, 0);
                  Grid.SetColumnSpan(description, 3);
                  gridLayout.Children.Add(loc, 2, 1);
                  gridLayout.Children.Add(lotNumber, 0, 1);                  
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
                Children = {
                    JORNo,
                    jorPicker,
                    jorBtn, 
                    scanBtn,
                    lstView,
                    delSelectedBtn,
                    saveToServerBtn
                }
                
            };
            
            jorBtn.Clicked += async delegate
            {
                var newItem = new MachineJORModel();
                var scanResult = "";

                scanPage = new ZXingScannerPage();
                await Navigation.PushAsync(scanPage);

                for (int i=0; i<machineJORS.Count;i++)
                {
                    machineJORS.RemoveAt(i);
                }

                scanPage.OnScanResult += (result) =>
                {
                    scanPage.IsScanning = false;
                    machineJORS.Clear();
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await Navigation.PopAsync();
                        scanResult = result.Text;
                        
                        
                        if (scanResult.Length>15)
                        {
                            CrossVibrate.Current.Vibration(2000);                            
                            UserDialogs.Instance.ShowError("Wrong barcode! Please scan machine number.", 2000);                           
                            scanResult = "";
                        }
                        else
                        {
                            await newItem.GetJOR(scanResult);
                            foreach (var i in newItem.TmpCollection)
                            {
                                machineJORS.Add(i);
                            }
                            UserDialogs.Instance.ShowError("Please Select JOR Number for machine " + scanResult, 2000);
                        }                                              
                    });
                    
                };
                
            };

            saveToServerBtn.Clicked += async delegate
            {
                if (scannedItems.Count() > 0)
                {                    
                    //http://sia35-conf/xampp/siapostconsumption.php?barcode=25 250      PC 3611420  &fname=transfer
                    //loop the collection and call above one by one
                    var successFlag = true;
                    
                    selectedJOR = jorPicker.SelectedItem as MachineJORModel;
                    if (selectedJOR.JORNo != null)
                    {
                        JORNo.Text = selectedJOR.JORNo;
                    }else
                    {
                        JORNo.Text = "";
                    }
                    
                    var filename = JORNo.Text + DateTime.Now.ToString("ddMMMyyyy-HH-mm");
                    foreach (ScannedItem i in scannedItems)
                    {
                        var postResult = await i.PostConsumption(i.barCode, filename);
                        
                        if (postResult.Substring(postResult.Length-7,7) != "Success")
                        {
                            CrossVibrate.Current.Vibration(2000);                           
                            await DisplayAlert("Alert", "Save to server failed! Please try again.", "OK");
                            successFlag = false;
                            break;
                        }
                    }
                    //end loop
                    if (successFlag==true)
                    {
                        CrossVibrate.Current.Vibration();
                        UserDialogs.Instance.ShowSuccess("Successfully saved to " + filename, 1000);

                        machineJORS.Clear();
                        scannedItems.Clear();                       
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
                if (jorPicker.SelectedItem == null)
                { 
                    CrossVibrate.Current.Vibration(2000);
                    await DisplayAlert("Alert", "Please select JOR", "OK");
                }
                else
                {                    
                    var duplicate = false;
                    var newItem = new ScannedItem();
                    var scanResult = "";
                    scanPage = new ZXingScannerPage();

                    scanPage.OnScanResult += (result) =>
                    {
                            //scanPage.IsScanning = false;

                        Device.BeginInvokeOnMainThread(async () =>
                        {
                                //await Navigation.PopAsync();
                                scanResult = result.Text;

                            if (scanResult.Length < 23)
                            {
                                UserDialogs.Instance.ShowError("Wrong Barcode!", 2000);
                                scanPage.IsScanning = false;
                                return;
                            }

                            await newItem.GetData(itemType, scanResult);

                            //check duplicate
                            foreach (ScannedItem i in scannedItems)
                            {
                                if (i.entryRef == newItem.entryRef)
                                {
                                    CrossVibrate.Current.Vibration(2000);
                                    await DisplayAlert("Alert", "Item " + newItem.lotNumber + " is already scanned", "OK");
                                    duplicate = true;

                                    break;
                                }
                            }
                            if (!duplicate)
                            {
                                duplicate = false;
                                scannedItems.Add(newItem);
                                totalQtyScanned = totalQtyScanned + newItem.quantity;
                                    //Get lstView and update the header with scannedItems count and total quantity 
                                    lstView.Header = scannedItems.Count().ToString() + " lot scanned. Total Qty = " + totalQtyScanned.ToString();
                                CrossVibrate.Current.Vibration();
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
                }
                 
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

        


    }
}
