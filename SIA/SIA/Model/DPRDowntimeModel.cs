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
    public class DPRDowntimeModel : ObservableObject
    {
        private int _DPRid;
        private int _dailyDTID;
        private string _downFrom;
        private string _downTo;
        private int _reasonId;
        private string _reasonDesc;
        private bool _isDelete;
        private ObservableCollection<DPRDowntimeModel> _tmpCollection;

        public DPRDowntimeModel(){}

        public int DPRid
        {
            get { return _DPRid; }
            set
            {
                _DPRid = value;
                RaisePropertyChanged();
            }                     
        }

        public ObservableCollection<DPRDowntimeModel> TmpCollection
        {
            get { return _tmpCollection; }
            set
            {
                _tmpCollection = value;
                RaisePropertyChanged();
            }
        }

        public string DownFrom
        {
            get
            {
                return _downFrom;
            }

            set
            {
                _downFrom = value;
                RaisePropertyChanged();
            }
        }

        public string DownTo
        {
            get
            {
                return _downTo;
            }

            set
            {
                _downTo = value;
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

        public int DailyDTID
        {
            get
            {
                return _dailyDTID;
            }

            set
            {
                _dailyDTID = value;
                RaisePropertyChanged();
            }
        }

        public async Task GetDPRDowntime(string DPRId)
        {
            //reasonType = "Downtime" or "Scrap"
            try
            {
                var Url = new Uri("http://172.11.66.181/xampp/SIAGetDPRDowntime.php?id=" + DPRId);

                var client = new HttpClient();

                var json = await client.GetAsync(Url);

                json.EnsureSuccessStatusCode();
                
                string contents = await json.Content.ReadAsStringAsync();
                if (contents.Trim() != "]")
                {
                    List<DPRDowntimeModel> tmpModel = await Task.Run(() => JsonConvert.DeserializeObject<List<DPRDowntimeModel>>(contents));
                    _tmpCollection = new ObservableCollection<DPRDowntimeModel>();
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
