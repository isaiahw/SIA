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
    public class DPRModel : ObservableObject
    {
        private int _DPRid;
        private DateTime _DPRtimeStamp;
        private char _DPRshift;
        private string _DPRuserName;
        private string _DPRmachine;
        private bool _DPRmarked;
        private string _DPRnotes;
        private string _DPRjor;
        private DateTime _DPRdate;
        private int _DPRhandoverQty;
        private int _DPRtakeoverQty;
        private int _DPRipScrapQty;
        private int _DPRsScrapQty;
        private int _DPRfullCtnQty;
        private int _DPRrunningCavity;
        private string _DPRBarCodeRefno;
        private ObservableCollection<DPRModel> _tmpCollection;

        public int DPRid
        {
            get
            {
                return _DPRid;
            }

            set
            {
                _DPRid = value;
                RaisePropertyChanged();
            }
        }

        public DateTime DPRtimeStamp
        {
            get
            {
                return _DPRtimeStamp;
            }

            set
            {
                _DPRtimeStamp = value;
                RaisePropertyChanged();
            }
        }

        public char DPRshift
        {
            get
            {
                return _DPRshift;
            }

            set
            {
                _DPRshift = value;
                RaisePropertyChanged();
            }
        }

        public string DPRuserName
        {
            get
            {
                return _DPRuserName;
            }

            set
            {
                _DPRuserName = value;
                RaisePropertyChanged();
            }
        }

        public string DPRmachine
        {
            get
            {
                return _DPRmachine;
            }

            set
            {
                _DPRmachine = value;
                RaisePropertyChanged();
            }
        }

        public bool DPRmarked
        {
            get
            {
                return _DPRmarked;
            }

            set
            {
                _DPRmarked = value;
                RaisePropertyChanged();
            }
        }

        public string DPRnotes
        {
            get
            {
                return _DPRnotes;
            }

            set
            {
                _DPRnotes = value;
                RaisePropertyChanged();
            }
        }

        public string DPRjor
        {
            get
            {
                return _DPRjor;
            }

            set
            {
                _DPRjor = value;
                RaisePropertyChanged();
            }
        }

        public DateTime DPRdate
        {
            get
            {
                return _DPRdate;
            }

            set
            {
                _DPRdate = value;
                RaisePropertyChanged();
            }
        }

        public int DPRhandoverQty
        {
            get
            {
                return _DPRhandoverQty;
            }

            set
            {
                _DPRhandoverQty = value;
                RaisePropertyChanged();
            }
        }

        public int DPRtakeoverQty
        {
            get
            {
                return _DPRtakeoverQty;
            }

            set
            {
                _DPRtakeoverQty = value;
                RaisePropertyChanged();
            }
        }

        public int DPRipScrapQty
        {
            get
            {
                return _DPRipScrapQty;
            }

            set
            {
                _DPRipScrapQty = value;
                RaisePropertyChanged();
            }
        }

        public int DPRsScrapQty
        {
            get
            {
                return _DPRsScrapQty;
            }

            set
            {
                _DPRsScrapQty = value;
                RaisePropertyChanged();
            }
        }

        public int DPRfullCtnQty
        {
            get
            {
                return _DPRfullCtnQty;
            }

            set
            {
                _DPRfullCtnQty = value;
                RaisePropertyChanged();
            }
        }

        public int DPRrunningCavity
        {
            get
            {
                return _DPRrunningCavity;
            }

            set
            {
                _DPRrunningCavity = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<DPRModel> TmpCollection
        {
            get
            {
                return _tmpCollection;
            }

            set
            {
                _tmpCollection = value;
            }
        }

        public string DPRBarCodeRefno
        {
            get
            {
                return _DPRBarCodeRefno;
            }

            set
            {
                _DPRBarCodeRefno = value;
                RaisePropertyChanged();
            }
        }

        public async Task GetDPR(int id)
        {
            //used for transfer posting to csv


            try
            {
                var Url = new Uri("http://sia35-conf/xampp/siaLoadDPR.php?id="+id.ToString());

                var client = new HttpClient();

                var json = await client.GetAsync(Url);

                json.EnsureSuccessStatusCode();

                string contents = await json.Content.ReadAsStringAsync();
                List<DPRModel> tmpModel = await Task.Run(() => JsonConvert.DeserializeObject<List<DPRModel>>(contents));
                _tmpCollection = new ObservableCollection<DPRModel>();
                foreach (var i in tmpModel)
                {
                   _tmpCollection.Add(i);
                }                

            }
            catch (System.Exception e) { var x = e.ToString(); x = null; }
        }

        
    }
}
