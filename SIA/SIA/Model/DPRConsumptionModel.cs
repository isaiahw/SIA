using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Net.Http;
using Newtonsoft.Json;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using Plugin.Connectivity;

namespace SIA
{
    public class DPRConsumptionModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _barCode;
        private string _barCodeRef;
        private double _quantity;
        private string _entryRef;
        protected string _lotNumber;
        private string _description;
        private string _status;
        private string _loc;
        private double _remainingQty;
        private bool _isDeleteScan;
        private string _itemType;
        private string _displayInfo;
        private string _dprID;
        private int _id;
        //private ActivityIndicator _activityIndicator;
        private ObservableCollection<DPRConsumptionModel> _tmpCollection;


        private string _dataHost = "172.11.66.181";

        public string DataHost
        {
            get { return _dataHost = "172.11.66.181"; }
            set { _dataHost = value; }
        }

        //constructor
        public DPRConsumptionModel()
        {
            _lotNumber = "N/A";
            _description = "N/A";
            _loc = "N/A";
            _remainingQty = 0;
            _displayInfo = _lotNumber + " " + _description + " " + _remainingQty;
        }

        public async Task GetDPRConsumption(string DPRID)
        {
            try
            {
                var Url = new Uri("http://172.11.66.181/xampp/SIAGetDPRConsumption.php?dprid=" + DPRID);

                var client = new HttpClient();

                var json = await client.GetAsync(Url);

                json.EnsureSuccessStatusCode();

                string contents = await json.Content.ReadAsStringAsync();
                
                if (contents.Trim() != "]") //not EMPTY RECORDS.
                {
                    List<DPRConsumptionModel> tmpModel = await Task.Run(() => JsonConvert.DeserializeObject<List<DPRConsumptionModel>>(contents));
                    _tmpCollection = new ObservableCollection<DPRConsumptionModel>();
                    foreach (var i in tmpModel)
                    {
                        _tmpCollection.Add(i);
                    }
                }
                      
            }
            catch (System.Exception e) { var x = e.ToString(); x = null; }
        }

        public async Task GetData(string table, string inBarcode)
        {
            //_activityIndicator.IsRunning = true;
            
            this.barCode = inBarcode;
            this.barCodeRef = inBarcode.Substring(0, 3);
            this.entryRef = inBarcode.Substring(inBarcode.Length - 9, 7);
            this.isDeleteScan = false;
            this.itemType = table;

            try
            {
                this.quantity = Convert.ToDouble(inBarcode.Substring(3, 9));

            }
            catch (FormatException) { this.quantity = 0; }
            catch (OverflowException) { this.quantity = 0; }

            JSONModel itemDetails = null;

            if (!await CrossConnectivity.Current.IsRemoteReachable(DataHost))
            {
                status = "Not Connected";
            } else
            {
                itemDetails = await GetDetails(barCodeRef, entryRef);
            }
                
            
            if (itemDetails == null)
            {
                //await DisplayAlert("Huh?!", "Unable to retrieve the data", "OK");    
                _lotNumber = "N/A";
                _description = "N/A";                
                _loc = "N/A";
                _remainingQty = 0;
                _displayInfo = _barCode;
            }
            else
            {
                lotNumber = itemDetails.lotNo;
                description = itemDetails.description;
                status = itemDetails.status;
                loc = itemDetails.loc;
                remainingQty = itemDetails.remainingqty;
                displayInfo = _barCode;
            }
            //_activityIndicator.IsRunning = false;
            //_activityIndicator.IsVisible = false;
        }

        public string status
        {
            get { return _status; }
            set
            {
                OnPropertyChanged();
                _status = value;
            }
        }

        public string displayInfo
        {
            get { return _displayInfo; }
            set
            {
                OnPropertyChanged();
                _displayInfo = value;
            }
        }

        public bool isDeleteScan
        {
            get { return _isDeleteScan; }
            set
            {
                OnPropertyChanged("isNotDeleteScan");
                _isDeleteScan = value;
            }
        }

        public bool isNotDeleteScan
        {
            get { return !_isDeleteScan; }
        }

        public string barCode
        {
            get { return _barCode; }
            set
            {
                OnPropertyChanged();
                _barCode = value;
            }
        }

        public string itemType
        {
            get { return _itemType; }
            set
            {
                OnPropertyChanged();
                _itemType = value;
            }
        }

        public string barCodeRef
        {
            get { return _barCodeRef; }
            set
            {
                OnPropertyChanged();
                _barCodeRef = value;
            }
        }

        public double quantity
        {
            get { return _quantity; }
            set
            {
                OnPropertyChanged();
                _quantity = value;
            }
        }

        public string loc
        {
            get { return _loc; }
            set
            {
                OnPropertyChanged();
                _loc = value;
            }
        }

        public double remainingQty
        {
            get { return _remainingQty; }
            set
            {
                OnPropertyChanged();
                _remainingQty = value;
            }
        }

        public string entryRef
        {
            get { return _entryRef; }
            set
            {
                OnPropertyChanged();
                _entryRef = value;
            }
        }

        public string lotNumber
        {
            get { return _lotNumber; }
            set
            {
                OnPropertyChanged();
                _lotNumber = value;
            }
        }

        public string description
        {
            get { return _description; }
            set
            {
                OnPropertyChanged();
                _description = value;
            }
        }

        public string DprID
        {
            get
            {
                return _dprID;
            }

            set
            {
                OnPropertyChanged();
                _dprID = value;
            }
        }

        public int Id
        {
            get
            {
                return _id;
            }

            set
            {
                OnPropertyChanged();
                _id = value;
            }
        }

        public ObservableCollection<DPRConsumptionModel> TmpCollection
        {
            get
            {
                return _tmpCollection;
            }

            set
            {
                OnPropertyChanged();
                _tmpCollection = value;
            }
        }

        public async Task<JSONModel> GetDetails(string barcodeRef, string entryRef)
        {
            try
            {
                var Url = "http://172.11.66.181/xampp/siagetfglotno.php?table="+ barcodeRef + "&id="+entryRef;
                var client = new HttpClient();

                var json = await client.GetAsync(Url);
                json.EnsureSuccessStatusCode();

                string contents = await json.Content.ReadAsStringAsync();

                
                JSONModel itemDetails =  await Task.Run(()=> JsonConvert.DeserializeObject<JSONModel>(contents));
                return itemDetails;
            }
            catch (System.Exception e) { var x = e.ToString(); x = null; return null; }
        }
        

        public async Task<string> PostDetails(string inBarcode, string fName, int last = 0)
        {
            //used for transfer posting to csv
            try
            {
                var Url = "";
                if (last == 0)
                {
                    Url = "http://172.11.66.181/xampp/siaposttransfer.php?barcode=" + inBarcode + "&fname=" + fName;
                }
                else
                {
                    Url = "http://172.11.66.181/xampp/siaposttransfer.php?barcode=" + inBarcode + "&fname=" + fName + "&last=1";
                }

                
                var client = new HttpClient();

                var json = await client.GetAsync(Url);

                json.EnsureSuccessStatusCode();
                return await json.Content.ReadAsStringAsync();               
            }
            catch (System.Exception e) { var x = e.ToString(); x = null; return null; }
        }

        

        public async Task<string> PostConsumption(string barcode, string fname, string lotNo, string DPRID, string JORNO, int last = 0)
        {
            //used for Consumption posting to sqlite
            //postResult = await i.PostConsumption(i.barCode, mstId.Text, i.lotNumber, lblDPRid.Text.Trim(),1);
            try
            {
                var Url = "";
                if (last == 0)
                {
                    Url = "http://172.11.66.181/xampp/siasaveConsumption.php?barcode=" + barcode + "&fname=" + fname + "&lotno=" + lotNo + "&DPRID=" + DPRID;
                }
                else
                {
                    Url = "http://172.11.66.181/xampp/siasaveConsumption.php?barcode=" + barcode + "&fname=" + fname + "&lotno=" + lotNo + "&DPRID=" + DPRID + "&last=1";                    
                }


                var client = new HttpClient();

                var json = await client.GetAsync(Url);

                json.EnsureSuccessStatusCode();
                return await json.Content.ReadAsStringAsync();
            }
            catch (System.Exception e) { var x = e.ToString(); x = null; return null; }
        }

        public async Task<string> PostConsumptionCSV(string barcode, string fname, string DPRID, string JORNO, int outputID, int last = 0)
        {
            //used for consumption posting to CSV            
            //http://172.11.66.181/xampp/siasaveConsumptionCSV.php?barcode=" + barcode + "&fname=" + fname + "&outputID=" + outputID.ToString().Trim() + "&DPRID=" + DPRID + "&jorno=" + JORNO + "&last=1"
            //http://172.11.66.181/xampp/siasaveConsumptionCSV.php?barcode=" + barcode + "&fname=" + fname + "&outputID=" + outputID.ToString().Trim() + "&DPRID=" + DPRID + "&jorno=" + JORNO
            try
            {
                var Url = "";
                if (last == 0)
                {
                    Url = "http://172.11.66.181/xampp/siasaveConsumptionCSV.php?barcode=" + barcode + "&fname=" + fname + "&outputID=" + outputID.ToString().Trim() + "&DPRID=" + DPRID + "&jorno="+ JORNO;
                }
                else
                {
                    Url = "http://172.11.66.181/xampp/siasaveConsumptionCSV.php?barcode=" + barcode + "&fname=" + fname + "&outputID=" + outputID.ToString().Trim() + "&DPRID=" + DPRID + "&jorno=" + JORNO + "&last=1";
                }


                var client = new HttpClient();

                var json = await client.GetAsync(Url);

                json.EnsureSuccessStatusCode();
                return await json.Content.ReadAsStringAsync();
            }
            catch (System.Exception e) { var x = e.ToString(); x = null; return null; }
        }
        
        
        
        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if(handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
