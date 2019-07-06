using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investing_parser
{
    class Logging
    {
        public static object LogLocker = new object();
        public static void writeLog(string msg)
        {
            lock(LogLocker)
            File.AppendAllText("log.txt", "\n"+DateTime.Now.ToLongTimeString() + "\n" + msg +"\n");
        }
    }
}
