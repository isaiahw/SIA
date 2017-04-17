using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIA
{
    public interface IGetNavData
    {
        string getQuerySingleRow(string sql);
        List<string> getQueryMultipleRows(string sql);
        string getQueryJSON(string sql);
    }
}
