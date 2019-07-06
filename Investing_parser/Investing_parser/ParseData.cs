using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Threading;

namespace Investing_parser
{
    class ParseData
    {
        public static void parseSaveData(List<string> htmls, CancellationToken token)
        {
            Task<OutputData>[] tasks = new Task<OutputData>[htmls.Count];
            for(int i = 0; i<htmls.Count; i++)
            {
                int ind = i;
                tasks[ind] = new Task<OutputData>(() => parseEvents(htmls[ind], token));
            }

            int ready = 0;
            int threads = 0;
            while (ready < htmls.Count && !token.IsCancellationRequested)
            {
                if (threads < 4)
                {
                    tasks[ready++].Start();
                    threads++;
                }else
                {
                    Task.WaitAny(tasks);
                    threads--;
                    Form1.programStatus = String.Format("обрабатываю данные ({0}/{1})", ready, htmls.Count);
                }
            }

            if (token.IsCancellationRequested)
            {
                return;
            }
            Task.WaitAll(tasks);

            OutputData od = new OutputData();
            foreach(var t in tasks)
            {
                od.events.AddRange(t.Result.events);
            }

            ExcelWriter.saveDate(od);
        }

        public static OutputData parseEvents(string html, CancellationToken token)
        {
            OutputData OD = new OutputData();
            try
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                var tbody = doc.DocumentNode.Element("table").Element("tbody");
                var trs = tbody.Elements("tr");

                string date = "";

                foreach (var tr in trs)
                {
                    if (token.IsCancellationRequested) break;

                    try
                    {
                        try
                        {
                            if (tr.Element("td").Attributes["class"].Value == "theDay")
                            {
                                date = tr.Element("td").InnerText.Trim();
                                continue;
                            }
                        }
                        catch { }

                        string time = "", country = "", imp = "", title = "", fact = "",
                            forecast = "", prev = "", add = "";


                        var tds = tr.SelectNodes("td");
                        time = tds[0].InnerText;
                        country = tds[1].Element("span").Attributes["title"].Value;

                        try
                        {
                            imp = tds[2].SelectNodes("i[@class='newSiteIconsSprite grayFullBullishIcon middle']").Count.ToString();
                        }
                        catch
                        {
                            imp = tds[2].InnerText.Trim();
                        }

                        title = tds[3].InnerText.Trim();
                        title = title.Replace("&nbsp;", "").Trim();

                        try
                        {
                            add = tds[3].Element("span").Attributes["title"].Value;
                        }
                        catch { }

                        try
                        {
                            fact = tds[4].InnerText.Trim();
                            fact = fact.Replace("&nbsp;", "");

                            forecast = tds[5].InnerText.Trim();
                            forecast = forecast.Replace("&nbsp;", "");

                            prev = tds[6].InnerText.Trim();
                            prev = prev.Replace("&nbsp;", "");
                        }
                        catch { }

                        OD.addEvent(date, time, country, imp, title, "", fact, forecast, prev, add);
                    }
                    catch (Exception e)
                    {
                        //Logging.writeLog(e.Message);
                        //Logging.writeLog(e.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.writeLog(ex.Message);
                Logging.writeLog(ex.StackTrace);
            }

            return OD;
        }
    }
}
