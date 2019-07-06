using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investing_parser
{
    public class OutputData
    {
        public class Event
        {
            public string Date, Time, Country, Importance,
                Title, Url, Fact, Predict, Before, Additional;

            public Event(string Date, string Time, string Country, string Importance,
                string Title, string Url, string Fact, string Predict = "",
                string Before = "", string Additional = "")
            {
                this.Date = Date;
                this.Time = Time;
                this.Country = Country;
                this.Importance = Importance;
                this.Title = Title;
                this.Url = Url;
                this.Fact = Fact;
                this.Predict = Predict;
                this.Before = Before;
                this.Additional = Additional;
            }
        }

        public void addEvent(string Date, string Time, string Country, string Importance,
                string Title, string Url, string Fact, string Predict = "",
                string Before = "", string Additional = "")
        {
            events.Add(new Event(Date, Time, Country, Importance,
                Title, Url, Fact, Predict, Before, Additional));
        }

        public List<Event> events = new List<Event>();
    }
}
