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
    public class ReasonModel: ObservableObject
    {
        private int _id;
        private string _Description;
        private ObservableCollection<ReasonModel> _tmpCollection;

        public ReasonModel(){}

        public int ID
        {
            get { return _id; }
            set
            {
                _id = value;
                RaisePropertyChanged();
            }                     
        }

        public ObservableCollection<ReasonModel> TmpCollection
        {
            get { return _tmpCollection; }
            set
            {
                _tmpCollection = value;
                RaisePropertyChanged();
            }
        } 

        public string Description
        {
            get { return _Description; }
            set
            {
                _Description = value;
                RaisePropertyChanged();
            }
        }

        public async Task GetReason(string reasonType)
        {
            //reasonType = "Downtime" or "Scrap"
            try
            {
                var Url = new Uri("http://172.11.66.181/xampp/SIAGetReasons.php?type=" + reasonType);

                var client = new HttpClient();

                var json = await client.GetAsync(Url);

                json.EnsureSuccessStatusCode();

                string contents = await json.Content.ReadAsStringAsync();
                List<ReasonModel> tmpModel = await Task.Run(() => JsonConvert.DeserializeObject<List<ReasonModel>>(contents));
                _tmpCollection = new ObservableCollection<ReasonModel>();
                foreach (var i in tmpModel)
                {
                    _tmpCollection.Add(i);
                }
                //return tmpCollection;

            }
            catch (System.Exception e) { var x = e.ToString(); x = null; }
        }

    }
}
