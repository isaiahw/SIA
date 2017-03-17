using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIA
{
    public class JSONModel
    {
        
        public string description { get; set; }
        public string lotNo { get; set; }
        public string status { get; set; }
        public string loc { get; set; }
        public double remainingqty { get; set; }
        public string itemID { get; set; }
        public string DPRID { get; set; }
        public string DPROutputID { get; set; }

        public JSONModel() { }
    }
}
