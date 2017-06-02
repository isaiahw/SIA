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
    public class DPRScrapModel : ObservableObject
    {
        private int _DPRid;
        private int _dailyScrapID;
        private int _dailyScrapQty;        
        private int _reasonId;
        private string _reasonDesc;
        private bool _isDelete;
        private ObservableCollection<DPRScrapModel> _tmpCollection;

        public DPRScrapModel(){}

        public int DPRid
        {
            get { return _DPRid; }
            set
            {
                _DPRid = value;
                RaisePropertyChanged();
            }                     
        }

        public ObservableCollection<DPRScrapModel> TmpCollection
        {
            get { return _tmpCollection; }
            set
            {
                _tmpCollection = value;
                RaisePropertyChanged();
            }
        }

        public int DailyScrapQty
        {
            get
            {
                return _dailyScrapQty;
            }

            set
            {
                _dailyScrapQty = value;
                RaisePropertyChanged();
            }
        }
      
        public int ReasonId
        {
            get
            {
                return _reasonId;
            }

            set
            {
                _reasonId = value;
                RaisePropertyChanged();
            }
        }

        public string ReasonDesc
        {
            get
            {
                return _reasonDesc;
            }

            set
            {
                _reasonDesc = value;
                RaisePropertyChanged();
            }
        }

        public bool IsDelete
        {
            get
            {
                return _isDelete;
            }

            set
            {
                _isDelete = value;
                RaisePropertyChanged();
            }
        }

        public int DailyScrapID
        {
            get
            {
                return _dailyScrapID;
            }

            set
            {
                _dailyScrapID = value;
                RaisePropertyChanged();
            }
        }

        public async Task GetDPRScrap(string DPRId)
        {
            //reasonType = "Downtime" or "Scrap"
            try
            {

                //todo 09May2017
                var Url = new Uri("http://172.11.66.181/xampp/SIAGetDPRScrap.php?id=" + DPRId);

                var client = new HttpClient();

                var json = await client.GetAsync(Url);

                json.EnsureSuccessStatusCode();
                
                string contents = await json.Content.ReadAsStringAsync();
                if (contents.Trim() != "]")
                {
                    List<DPRScrapModel> tmpModel = await Task.Run(() => JsonConvert.DeserializeObject<List<DPRScrapModel>>(contents));
                    _tmpCollection = new ObservableCollection<DPRScrapModel>();
                    foreach (var i in tmpModel)
                    {
                        _tmpCollection.Add(i);
                    }
                }
                else
                {
                    _tmpCollection.Clear();
                }
                
                //return tmpCollection;

            }
            catch (System.Exception e) { var x = e.ToString(); x = null; }
        }

    }
}
