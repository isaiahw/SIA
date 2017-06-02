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
    public class MachineJORModel: ObservableObject
    {
        private string _machineNo;
        private string _JORNo;
        private ObservableCollection<MachineJORModel> _tmpCollection;

        public MachineJORModel(){}

        public string MachineNo
        {
            get { return _machineNo; }
            set
            {
                _machineNo = value;
                RaisePropertyChanged();
            }                     
        }

        public ObservableCollection<MachineJORModel> TmpCollection
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

        public async Task GetJOR(string machineNo)
        {
            //used for transfer posting to csv
            try
            {
                var Url = new Uri("http://172.11.66.181/xampp/SIAGetJORByMachineNo.php?id=\"" + machineNo + "\"");

                var client = new HttpClient();

                var json = await client.GetAsync(Url);

                json.EnsureSuccessStatusCode();

                string contents = await json.Content.ReadAsStringAsync();
                List<MachineJORModel> tmpModel = await Task.Run(() => JsonConvert.DeserializeObject<List<MachineJORModel>>(contents));
                _tmpCollection = new ObservableCollection<MachineJORModel>();
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
