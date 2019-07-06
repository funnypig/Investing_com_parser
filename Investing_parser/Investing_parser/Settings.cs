using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investing_parser
{
    public class Settings
    {
        public int startDay = 1, startMonth = 6, startYear = 2018;
        public int finishDay = 30, finishMonth = 6, finishYear = 2019;
        public List<int> importance = new List<int>() { 1,2,3 };
        public List<string> countries = new List<string>();
        public List<string> categories = new List<string>();
    }
}
