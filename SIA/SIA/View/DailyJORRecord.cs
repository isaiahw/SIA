using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using ZXing.Net.Mobile.Forms;
using Plugin.Vibrate;

using Xamarin.Forms;
using System.Threading.Tasks;
using Plugin.Connectivity;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Acr.UserDialogs;

namespace SIA
{
    public class DailyJORRecord : ContentPage
    {
        ZXingScannerPage scanPage;
        


        public Entry JORNo = new Entry { Placeholder = "JOR Number", HorizontalOptions=LayoutOptions.FillAndExpand };
        public Button JORBtn = new Button
        {
            Text = "Get JOR",
            HorizontalOptions = LayoutOptions.Start

        };
        public Button MachineBtn = new Button
        {
            Text = "Get Mch",
            HorizontalOptions = LayoutOptions.Start

        };
        public Button UserBtn = new Button
        {
            Text = "Get User",
            HorizontalOptions = LayoutOptions.Start

        };
        public Button LoadJORBtn = new Button
        {
            Text = "Find DPR",
            HorizontalOptions = LayoutOptions.CenterAndExpand
        };

        public Button SaveDPRBtn = new Button
        {
            Text = "Save DPR",
            HorizontalOptions = LayoutOptions.CenterAndExpand,
            IsEnabled=false
        };

        public Button ValidatePasswordBtn = new Button
        {
            Text = "Go",
            HorizontalOptions = LayoutOptions.CenterAndExpand,
            IsVisible = false
        };

        public Entry QtyHandOver = new Entry { Placeholder = "Quantity Handover", Keyboard=Keyboard.Numeric };
        public Entry QtyTakeOver = new Entry { Placeholder = "Quantity Takeover", Keyboard = Keyboard.Numeric };
        public Entry MachineNo = new Entry { Placeholder = "Machine Number", HorizontalOptions = LayoutOptions.FillAndExpand };        
        public Picker Shift = new Picker { Title = "Shift", HorizontalOptions = LayoutOptions.FillAndExpand };
        public Xamarin.Forms.Label JORDescLbl = new Xamarin.Forms.Label { TextColor = Color.Aqua };
        public Xamarin.Forms.Label DPRidLbl = new Xamarin.Forms.Label();
        public Xamarin.Forms.Label DateLbl = new Xamarin.Forms.Label { TextColor = Color.Aqua, VerticalOptions = LayoutOptions.CenterAndExpand };
        public Xamarin.Forms.Label ShiftLbl = new Xamarin.Forms.Label { TextColor = Color.Aqua, VerticalOptions = LayoutOptions.CenterAndExpand };
        public Xamarin.Forms.Label OperatorNameLbl = new Xamarin.Forms.Label { TextColor = Color.Aqua };
        public Xamarin.Forms.Label OperatorGroupLbl = new Xamarin.Forms.Label { TextColor = Color.Aqua };
        public DatePicker Date = new DatePicker { HorizontalOptions = LayoutOptions.FillAndExpand };
        public Xamarin.Forms.Label QtyScrapLbl = new Xamarin.Forms.Label { TextColor = Color.Aqua };
        public Xamarin.Forms.Label QtyHOLbl = new Xamarin.Forms.Label { TextColor = Color.Aqua };
        public Xamarin.Forms.Label QtyTOLbl = new Xamarin.Forms.Label { TextColor = Color.Aqua };

        public Entry Operator = new Entry { Placeholder = "Operator ID", HorizontalOptions = LayoutOptions.FillAndExpand };
        public Entry Password = new Entry { Placeholder = "", IsPassword = true, HorizontalOptions = LayoutOptions.FillAndExpand };
        public Entry QtyScrap = new Entry { Placeholder = "In-Process Scrap", Keyboard = Keyboard.Numeric };
        private Boolean isConnected = false;
        private Func<Task> checkConnection;
        private string lastInsertID;
        public DailyJOR parentTabbedPage;

        //private ObservableCollection<DPRModel> _tmpCollection;

        public DailyJORRecord()
        {
            
            checkConnection += async delegate
            {
                if (await CrossConnectivity.Current.IsRemoteReachable("172.11.66.181"))
                {
                    isConnected = true;
                }
                else
                {
                    CrossVibrate.Current.Vibration(2000);
                    UserDialogs.Instance.ShowError("Device not connected to SIA WI - FI!", 2000);
                    isConnected = false;
                }

            }; 

            checkConnection();
            JORNo.TextChanged += JORNo_TextChanged;
            Operator.TextChanged += Operator_TextChanged;           
            
            MachineNo.TextChanged += MachineNo_TextChanged;
            
            Shift.Items.Add("A");
            Shift.Items.Add("B");
            Shift.Items.Add("C");
            Shift.SelectedIndex=0;
            Date.Format= "yyyy-MMM-dd";
            //Date.MinimumDate = DateTime.Now;
            var scrollview = new ScrollView();
            

            var stackDateShift = new StackLayout { Orientation = StackOrientation.Horizontal };
            stackDateShift.Children.Add(DateLbl);
            stackDateShift.Children.Add(Date);
            stackDateShift.Children.Add(ShiftLbl);
            stackDateShift.Children.Add(Shift);
            
            var stackJOR = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
            };
            stackJOR.Children.Add(JORBtn);
            stackJOR.Children.Add(JORNo);
            

            var stackMachine = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
            };
            stackMachine.Children.Add(MachineBtn);
            stackMachine.Children.Add(MachineNo);
            

            var stackUser = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
            };
            stackUser.Children.Add(UserBtn);
            stackUser.Children.Add(Operator);
            
            stackUser.Children.Add(Password);            
            stackUser.Children.Add(ValidatePasswordBtn);
            

            var stackUserDesc = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
            };
            stackUserDesc.Children.Add(OperatorNameLbl);
            stackUserDesc.Children.Add(OperatorGroupLbl);

            Operator.IsVisible = false;
            UserBtn.IsVisible = false;
            Password.IsVisible = false;
            JORDescLbl.Text = "JOR Description";
            DateLbl.Text = "Date: ";
            ShiftLbl.Text = "Shift: ";
            DPRidLbl.Text = "]";
            QtyHOLbl.Text = "Handover Quantity:";
            QtyTOLbl.Text = "Takeover Quantity:";
            QtyScrapLbl.Text = "In-Process Scrap Quantity:";

            var stack = new StackLayout
            {
                Children = {
                    DPRidLbl,
                    stackDateShift,
                    stackJOR,
                    JORDescLbl,                        
                    stackMachine,
                    LoadJORBtn,
                    stackUser,
                    stackUserDesc,
                    QtyTOLbl,
                    QtyTakeOver,
                    QtyHOLbl,
                    QtyHandOver,
                    QtyScrapLbl,
                    QtyScrap,
                    SaveDPRBtn

                    }

            };

            scrollview.Content = stack;
            Content = scrollview;

            LoadJORBtn.Clicked += async (sender, e) => {
                var tabbedPage = this.Parent as DailyJOR;

                
                tabbedPage.JORNumber = JORNo.Text;
                tabbedPage.MachineNo = MachineNo.Text;
                tabbedPage.PostDate = Date.Date;
                tabbedPage.Shift = Shift.Items[Shift.SelectedIndex];


                //check database, if have record, load record from database, set editable = false.                
                try
                {
                    var Url = new Uri("http://172.11.66.181/xampp/siaGetDPRid.php?jor="+JORNo.Text+"&date="+ Date.Date.ToString("yyyy-MMM-dd") +"&shift="+ Shift.Items[Shift.SelectedIndex]+"&machine="+ MachineNo.Text);

                    var client = new HttpClient();

                    var json = await client.GetAsync(Url);

                    json.EnsureSuccessStatusCode();

                    string contents = await json.Content.ReadAsStringAsync();
                    if (contents != "\r\nEmpty")
                    {
                        List<DPRModel> tmpModel = await Task.Run(() => JsonConvert.DeserializeObject<List<DPRModel>>(contents));

                        foreach (var i in tmpModel)
                        {
                            DPRidLbl.Text = i.DPRid.ToString();
                        }
                        //load DPR
                        Url = new Uri("http://172.11.66.181/xampp/siaLoadDPR.php?id=" + DPRidLbl.Text);

                        client = new HttpClient();

                        json = await client.GetAsync(Url);

                        json.EnsureSuccessStatusCode();
                        contents = "";
                        contents = await json.Content.ReadAsStringAsync();
                        List<DPRModel> tmpDPRModels = await Task.Run(() => JsonConvert.DeserializeObject<List<DPRModel>>(contents));
                        //_tmpCollection = new ObservableCollection<DPRModel>();
                        foreach (var i in tmpDPRModels)
                        {
                            tabbedPage.DPRs = i;
                        }
                        DPRidLbl.Text = tabbedPage.DPRs.DPRid.ToString();
                        Operator.Text = tabbedPage.DPRs.DPRuserName;
                        QtyTakeOver.Text = tabbedPage.DPRs.DPRtakeoverQty.ToString();
                        QtyHandOver.Text = tabbedPage.DPRs.DPRhandoverQty.ToString();
                        QtyScrap.Text = tabbedPage.DPRs.DPRipScrapQty.ToString();

                        //get DPR barcoderefno
                        Url = new Uri("http://172.11.66.181/xampp/siaGetBarcodeRefno.php?jor=" + tabbedPage.DPRs.DPRjor.Trim());

                        client = new HttpClient();

                        json = await client.GetAsync(Url);

                        json.EnsureSuccessStatusCode();
                        contents = "";
                        contents = await json.Content.ReadAsStringAsync();
                        tabbedPage.DPRs.DPRBarCodeRefno = contents.Trim();


                        SaveDPRBtn.IsEnabled = false;
                    }
                    else
                    {
                        //add new DPR
                        DPRidLbl.Text = "]";
                        Operator.Text = "";
                        Password.Text = "";
                        Password.Placeholder = "";
                        QtyTakeOver.Text = "";
                        QtyHandOver.Text = "";
                        QtyScrap.Text = "";
                        SaveDPRBtn.IsEnabled = true;                                                
                    }
                    Operator.IsVisible = true;
                    UserBtn.IsVisible = true;
                }
                catch (System.Exception ex) { var x = ex.ToString(); x = null; }
                
            };

            ValidatePasswordBtn.Clicked += async (sender, e)=>{
                try
                {
                    

                    var Url = new Uri("http://172.11.66.181/xampp/siaVerifyPwd.php?id=" + Operator.Text + "&pwd=" + Password.Text);

                    var client = new HttpClient();

                    var json = await client.GetAsync(Url);

                    json.EnsureSuccessStatusCode();

                    string contents = await json.Content.ReadAsStringAsync();
                    if (contents != "\r\nFALSE")
                    {
                        SaveDPRBtn.IsEnabled = true;
                        CrossVibrate.Current.Vibration();
                        UserDialogs.Instance.ShowSuccess("Success", 1000);
                        var credentialLocal = new LoginResult(true, Operator.Text, Password.Text);
                        var tabbedPage = this.Parent as DailyJOR;
                        tabbedPage.credential = credentialLocal;
                    }
                    else
                    {
                        SaveDPRBtn.IsEnabled = false;
                        CrossVibrate.Current.Vibration(2000);
                        UserDialogs.Instance.ShowError("Wrong User ID or Password!", 2000);
                    }
                }
                catch (System.Exception ex) { var x = ex.ToString(); x = null; }
            };

            SaveDPRBtn.Clicked += async (sender, e) => {
                var tabbedPage = this.Parent as DailyJOR;

                tabbedPage.JORNumber = JORNo.Text;
                tabbedPage.MachineNo = MachineNo.Text;
                tabbedPage.PostDate = Date.Date;
                tabbedPage.Shift = Shift.Items[Shift.SelectedIndex];                

                try
                {
                    var Url = new Uri("http://172.11.66.181/xampp/siaSaveDPR.php?jor=" + JORNo.Text + "&date=" + Date.Date.ToString("yyyy-MMM-dd") + "&shift=" + Shift.Items[Shift.SelectedIndex] + "&machine=" + MachineNo.Text + "&user="+Operator.Text+"&hoqty="+QtyHandOver.Text+"&toqty="+QtyTakeOver.Text+"&ipscrqty="+QtyScrap.Text+"&dprid="+ DPRidLbl.Text);  

                    var client = new HttpClient();

                    var json = await client.GetAsync(Url);

                    json.EnsureSuccessStatusCode();

                    string contents = await json.Content.ReadAsStringAsync();
                    lastInsertID = contents;
                    try
                    {
                        Url = new Uri("http://172.11.66.181/xampp/siaGetDPRid.php?jor=" + JORNo.Text + "&date=" + Date.Date.ToString("yyyy-MMM-dd") + "&shift=" + Shift.Items[Shift.SelectedIndex] + "&machine=" + MachineNo.Text);

                        client = new HttpClient();

                        json = await client.GetAsync(Url);

                        json.EnsureSuccessStatusCode();

                        contents = "";
                        contents = await json.Content.ReadAsStringAsync();
                        if (contents != "Empty")
                        {
                            List<DPRModel> tmpModel = await Task.Run(() => JsonConvert.DeserializeObject<List<DPRModel>>(contents));

                            foreach (var i in tmpModel)
                            {
                                DPRidLbl.Text = i.DPRid.ToString();
                            }
                            //load DPR
                            Url = new Uri("http://172.11.66.181/xampp/siaLoadDPR.php?id=" + DPRidLbl.Text);

                            client = new HttpClient();

                            json = await client.GetAsync(Url);

                            json.EnsureSuccessStatusCode();
                            contents = "";
                            contents = await json.Content.ReadAsStringAsync();
                            List<DPRModel> tmpDPRModels = await Task.Run(() => JsonConvert.DeserializeObject<List<DPRModel>>(contents));
                            //_tmpCollection = new ObservableCollection<DPRModel>();
                                           
                            foreach (var i in tmpDPRModels)
                            {
                                tabbedPage.DPRs = i;
                            }

                            Operator.Text = tabbedPage.DPRs.DPRuserName;
                            QtyTakeOver.Text = tabbedPage.DPRs.DPRtakeoverQty.ToString();
                            QtyHandOver.Text = tabbedPage.DPRs.DPRhandoverQty.ToString();
                            QtyScrap.Text = tabbedPage.DPRs.DPRipScrapQty.ToString();
                            Password.Text = "";
                            SaveDPRBtn.IsEnabled = false;                            
                        }
                        else { DPRidLbl.Text = "]"; }
                    }
                    catch (System.Exception ex) { var x = ex.ToString(); x = null; }                    
                }
                catch (System.Exception ex) { var x = ex.ToString(); x = null; }

            };


            JORBtn.Clicked += async delegate
            {
                var scanResult = "";

                scanPage = new ZXingScannerPage();
                await Navigation.PushAsync(scanPage);

                scanPage.OnScanResult += (result) =>
                {
                    scanPage.IsScanning = false;
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await Navigation.PopAsync();

                        scanResult = result.Text;
                        //await DisplayAlert("Alert", scanResult, "OK");
                        if (scanResult.Length != 12)
                        {
                            CrossVibrate.Current.Vibration(2000);
                            UserDialogs.Instance.ShowError("Wrong barcode! Please scan JOR number.", 2000);
                            scanResult = "";
                            JORNo.Text = "";
                        }
                        else
                        {
                            JORNo.Text = scanResult;
                        }
                    });

                };

            };


            MachineBtn.Clicked += async delegate
            {
                var scanResult = "";

                scanPage = new ZXingScannerPage();
                await Navigation.PushAsync(scanPage);

                scanPage.OnScanResult += (result) =>
                {
                    scanPage.IsScanning = false;
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await Navigation.PopAsync();

                        scanResult = result.Text;
                        if (scanResult.Length > 14)
                        {
                            CrossVibrate.Current.Vibration(2000);
                            UserDialogs.Instance.ShowError("Wrong barcode! Please scan Machine number.", 2000);
                            scanResult = "";
                            MachineNo.Text = "";
                        }
                        else
                        {
                            MachineNo.Text = scanResult;
                        }
                    });

                };

            };

            UserBtn.Clicked += async delegate
            {
                var scanResult = "";

                scanPage = new ZXingScannerPage();
                await Navigation.PushAsync(scanPage);

                scanPage.OnScanResult += (result) =>
                {
                    scanPage.IsScanning = false;
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await Navigation.PopAsync();

                        scanResult = result.Text;
                        if (scanResult.Length > 6)
                        {
                            CrossVibrate.Current.Vibration(2000);
                            UserDialogs.Instance.ShowError("Wrong barcode! Please scan User number.", 2000);
                            scanResult = "";
                            JORNo.Text = "";
                        }
                        else
                        {
                            Operator.Text = scanResult;
                        }
                    });

                };

            };
        }

        private async void Operator_TextChanged(object sender, TextChangedEventArgs e)
        {
            Operator.Text = ((Entry)sender).Text.ToUpper();

            //check database for user group(only if ((Entry)sender).Text.Length = ???). if user group = supervisor then set SaveDPRBtn.IsEnabled = true;.
            
            if (((Entry)sender).Text.Length >= 5 && isConnected)
            {
                
                try
                {
                    var Url = new Uri("http://172.11.66.181/xampp/siaGetUserName.php?id=" + Operator.Text);

                    var client = new HttpClient();

                    var json = await client.GetAsync(Url);

                    json.EnsureSuccessStatusCode();

                    string contents = await json.Content.ReadAsStringAsync();
                    if (contents != "\r\nNot Found")
                    {
                        OperatorNameLbl.Text = contents.ToUpper().Trim();
                        Url = new Uri("http://172.11.66.181/xampp/siaGetUserGroup.php?id=" + Operator.Text);
                        client = new HttpClient();
                        json = await client.GetAsync(Url);
                        json.EnsureSuccessStatusCode();
                        contents = await json.Content.ReadAsStringAsync();
                        if (contents != "\r\nNot Found")
                        {
                            OperatorGroupLbl.Text = contents.ToUpper().Trim();
                            if (OperatorGroupLbl.Text=="ADMIN" || OperatorGroupLbl.Text=="SUPERVISOR")
                            {
                                Password.IsVisible = true;
                                ValidatePasswordBtn.IsVisible = true;
                            }
                            else
                            {
                                Password.IsVisible = false;
                                ValidatePasswordBtn.IsVisible = false;
                            }
                        }
                        else { OperatorGroupLbl.Text = ""; }
                    }                        
                    else
                    {
                        OperatorNameLbl.Text = "NOT FOUND!";
                        Password.IsVisible = false;
                        ValidatePasswordBtn.IsVisible = false;
                        OperatorGroupLbl.Text = "";
                    }
                        
                }
                catch (System.Exception ex) { var x = ex.ToString(); x = null; }                
            }
            else
            {
                OperatorNameLbl.Text = "";
                OperatorGroupLbl.Text = "";
                Password.IsVisible = false;
                ValidatePasswordBtn.IsVisible = false;
            }


        }

        
        private void MachineNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            MachineNo.Text = ((Entry)sender).Text.ToUpper();
        }

        private void JORNo_TextChanged(object sender, TextChangedEventArgs e)
        {

            var found = false;

            JORNo.Text = ((Entry)sender).Text.ToUpper();
            if (((Entry)sender).Text.Length == 12 && isConnected)
            {
                var tabbedPage = this.Parent as DailyJOR;
                
                foreach (ReleasedJORModel i in tabbedPage.ReleasedJORs.TmpCollection)
                {
                    if (i.JORNo == JORNo.Text)
                    {
                        found = true;
                        JORDescLbl.Text = i.JORDescription;
                        //tabbedPage.DPRs.DPRBarCodeRefno = i.JORBarCodeRefNo.Trim();                        
                        break;
                    }
                }
                if (found == false)
                {
                    CrossVibrate.Current.Vibration(2000);
                    UserDialogs.Instance.ShowError(JORNo.Text + " is not found", 2000);
                    JORNo.Text = "";
                    JORDescLbl.Text = "";
                }
            }

        }


       

        

    }
    
}
