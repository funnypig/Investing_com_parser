using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;
using HtmlAgilityPack;

namespace Investing_parser
{
    class Invest
    {
        public static OutputData OD = new OutputData();
        public static string stateMessage = "";

        static ChromeDriver d;     

        static void getCountries()
        {
            var c = d.FindElementByClassName("countryOption");
            string s = "";
            foreach (var l in c.FindElements(By.TagName("li")))
            {
                string country = l.FindElement(By.ClassName("countryName")).Text.Trim();
                country += ":"+ l.FindElement(By.TagName("input")).GetAttribute("value");

                s += country + "\n";
            }
        }

        static void click(IWebElement elem)
        {
            d.ExecuteScript("arguments[0].click();", elem);
        }

        static void setFilter(Settings settings)
        {
            // set filter and navigate
            {
                string url = "https://sslecal2.forexprostools.com/?columns=exc_flags,exc_currency,exc_importance,exc_actual,exc_forecast,exc_previous&features=datepicker,timezone,filters&timeZone=18&lang=7";

                string ctr = "&countries=";
                foreach (var s in settings.countries)
                {
                    ctr += s + ",";
                }
                ctr = ctr.Remove(ctr.Length - 1, 1);

                string cats = "&category=";
                foreach (var s in settings.categories)
                {
                    cats += s + ",";
                }
                cats = cats.Remove(cats.Length - 1, 1);

                string imp = "&importance=";
                foreach (var s in settings.importance)
                {
                    imp += s.ToString() + ",";
                }
                imp = imp.Remove(imp.Length - 1, 1);

                url += ctr + cats + imp;

                d.Navigate().GoToUrl(url);
            }
        }

        static void setCalendar(int year, int month, int startday = 1, int finday = 31)
        {
            try
            {
                d.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(500);
                // open calendar
                click(d.FindElementById("datePickerIconWrap"));

                // expand to years
                click(d.FindElementByClassName("datepickerMonth"));
                click(d.FindElementByClassName("datepickerMonth"));
                Thread.Sleep(150);

                // select year 
                while (true)
                {
                    var years = d.FindElementByClassName("datepickerMonth").Text.Split('-');
                    int firstYear = int.Parse(years[0].Trim());
                    int lastYear = int.Parse(years[1].Trim());
                    if (year >= firstYear && year <= lastYear) break;

                    if (year < firstYear)
                    {
                        click(d.FindElementByClassName("datepickerViewYears").FindElements(By.TagName("a"))[0]);
                    }else
                    {

                        click(d.FindElementByClassName("datepickerViewYears").FindElements(By.TagName("a"))[2]);
                    }
                }
                foreach(var c in d.FindElementByClassName("datepickerYears").FindElements(By.TagName("a")))
                {
                    if (int.Parse(c.Text.Trim()) == year)
                    {
                        click(c);
                        Thread.Sleep(150);
                        break;
                    }
                }

                // pick month
                var mths = d.FindElementByClassName("datepickerMonths").FindElements(By.TagName("a"));
                var m = mths[month - 1];
                click(m);
                Thread.Sleep(150);

                // pick day
                var days = d.FindElementByClassName("datepickerDays").FindElements(By.TagName("a"));
                int d1 = 0;

                while (days[d1].Text.Trim() != "1") d1++;
                while (days[d1].Text.Trim() != startday.ToString()) d1++;

                click(days[d1]);
                Thread.Sleep(150);
                //click(d.FindElementByClassName("datepickerSelected"));
                //Thread.Sleep(150);

                days = d.FindElementByClassName("datepickerDays").FindElements(By.TagName("a"));
                int dn = days.Count - 1;
                while (true)
                {
                    int D = int.Parse(days[dn].Text.Trim());
                    if (D == finday) break;
                    if (D > 20 && D < finday) break;
                    dn--;
                }

                click(days[dn]);
                Thread.Sleep(150);

                // apply 
                click(d.FindElementById("datePickerApplyButton"));
                Thread.Sleep(1500);
            }
            catch(Exception e)
            {
                Logging.writeLog(e.StackTrace+"\n"+e.Message);
            }
        }

        public static void getData(Settings settings, CancellationToken token)
        {
            d = new ChromeDriver();

            OD.events.Clear();
            Form1.programStatus = "loading site";

            setFilter(settings);

            // wait for loading
            while (true & !token.IsCancellationRequested)
            {
                try
                {
                    d.FindElementById("headerRow");
                    break;
                }
                catch { }
            }

            List<string> eventsHTML = new List<string>();
            int Y = settings.startYear, M = settings.startMonth;
            
            while (true && !token.IsCancellationRequested)
            {
                // check if we have gathered all data
                if (Y > settings.finishYear || (Y == settings.finishYear && M > settings.finishMonth)) break;

                Form1.programStatus = String.Format("собираю данные. Год: {0}, месяц: {1}", Y, M);

                
                if (Y == settings.finishYear && M == settings.finishMonth)
                    setCalendar(Y, M, finday: settings.finishDay);
                else
                    if (Y == settings.startYear && M == settings.startMonth)
                    setCalendar(Y, M, settings.startDay);
                else
                    setCalendar(Y, M);

                // wait for loading
                while (true && !token.IsCancellationRequested)
                {
                    try
                    {
                        eventsHTML.Add(d.FindElementById("ecEventsTable").GetAttribute("outerHTML"));
                        break;
                    }
                    catch { }
                }

                // next month
                if (M == 12)
                {
                    M = 1;
                    Y++;
                }
                else
                {
                    M++;
                }
            }

            if (!token.IsCancellationRequested)
            Form1.programStatus = "Обработка данных (" + eventsHTML.Count.ToString() + " месяцев)";

            d.Close();
            d.Quit();

            if (!token.IsCancellationRequested)
                ParseData.parseSaveData(eventsHTML, token);
            
        }
    }
}
