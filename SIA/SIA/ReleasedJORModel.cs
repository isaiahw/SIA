using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SIA
{
    public class ReleasedJORModel : ObservableObject
    {
        
        private string _JORNo;
        private string _JORDescription;
        private string _JORBarcodeRefNo;
        private string _JORWorkCenter;
        private string _JORFinishedQty;
        private string _JORStartDate;        
        private ObservableCollection<ReleasedJORModel> _tmpCollection;

        public ReleasedJORModel(){}

        public string JORBarCodeRefNo
        {
            get { return _JORBarcodeRefNo; }
            set
            {
                _JORBarcodeRefNo = value;
                RaisePropertyChanged();
            }

        }

        public string JORDescription
        {
            get { return _JORDescription; }
            set
            {
                _JORDescription = value;
                RaisePropertyChanged();
            }                     
        }

        public ObservableCollection<ReleasedJORModel> TmpCollection
        {
            get { return _tmpCollection; }
            set
            {
                _tmpCollection = value;
                RaisePropertyChanged();
            }
        } 

        public string JORNo
        {
            get { return _JORNo; }
            set
            {
                _JORNo = value;
                RaisePropertyChanged();
            }
        }

        

        public string JORFinishedQty
        {
            get
            {
                return _JORFinishedQty;
            }

            set
            {
                _JORFinishedQty = value;
            }
        }

        public string JORStartDate
        {
            get
            {
                return _JORStartDate;
            }

            set
            {
                _JORStartDate = value;
            }
        }

        public string JORWorkCenter
        {
            get
            {
                return _JORWorkCenter;
            }

            set
            {
                _JORWorkCenter = value;
            }
        }

        public async Task GetReleasedJOR()
        {
            //used for transfer posting to csv
            

            try
            {
                var Url = new Uri("http://172.11.66.181/xampp/SIAGetReleasedJOR.php");

                var client = new HttpClient();

                var json = await client.GetAsync(Url);

                json.EnsureSuccessStatusCode();

                string contents = await json.Content.ReadAsStringAsync();
                if (contents != "/r/nEmpty")
                {
                    List<ReleasedJORModel> tmpModel = await Task.Run(() => JsonConvert.DeserializeObject<List<ReleasedJORModel>>(contents));
                    _tmpCollection = new ObservableCollection<ReleasedJORModel>();
                    foreach (var i in tmpModel)
                    {
                        _tmpCollection.Add(i);
                    }
                    //return tmpCollection;
                }

            }
            catch (System.Exception e) { var x = e.ToString(); x = null; }
        }

    }
}
