using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Core;
using System.IO;

namespace Investing_parser
{
    class ExcelWriter
    {
        public static void saveDate(OutputData od)
        {
            Form1.programStatus = "сохраняю данные в эксель (" + od.events.Count + " событий)";

            Excel.Application app;
            Excel.Workbooks wbs;
            Excel.Workbook wb;
            object misval = System.Reflection.Missing.Value;

            app = new Excel.Application();
            wb = app.Workbooks.Add();
            Excel.Worksheet wsheet = (Excel.Worksheet)wb.Sheets.Add();

            for (int i = 1; i<=od.events.Count; i++)
            {
                wsheet.Cells[i, 1] = od.events[i - 1].Date;
                wsheet.Cells[i, 2] = od.events[i - 1].Time;
                wsheet.Cells[i, 3] = od.events[i - 1].Country;
                wsheet.Cells[i, 4] = od.events[i - 1].Importance;
                wsheet.Cells[i, 5] = od.events[i - 1].Title;
                wsheet.Cells[i, 6] = od.events[i - 1].Additional;
                wsheet.Cells[i, 7] = od.events[i - 1].Fact;
                wsheet.Cells[i, 8] = od.events[i - 1].Predict;
                wsheet.Cells[i, 9] = od.events[i - 1].Before;
            }

            string dir = Directory.GetCurrentDirectory() + "/Results/";
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }


            app.ActiveWorkbook.SaveAs(dir+"investParser_"+DateTime.Now.ToLongTimeString().Replace(":", "-") + ".xls", 
                Excel.XlFileFormat.xlWorkbookNormal);

            wb.Close();
            app.Quit();

            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(wsheet);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(wb);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(app);
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Form1.programStatus = od.events.Count.ToString() + " событий записано. ищите файл в папке с программой";
        }
    }
}
