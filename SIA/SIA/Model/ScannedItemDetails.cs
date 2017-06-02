using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIA
{
    public class ScannedItemDetails
    {
        public int _entryNo { get; set; }
        public string _lotNo { get; set; }
        public string _postDate { get; set; }
        public string _description { get; set; }
        public string _loc { get; set; }
        public double _remainingQty { get; set; }
        public string _mfgDate { get; set; }
    }

}
