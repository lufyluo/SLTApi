using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppApi.Models.WorkDt.Back
{
    public class Dt 
    {
        private Nullable<DateTime> _stime;
        private DateTime _etime;
        public Nullable<int> id { get; set; }
        public string menuno { get; set; }
        public Nullable<DateTime> start_date { get { return _stime; } set { _stime = value; } }
        public string start_dateN { get { return _stime.ToString(); } set { } }
        public DateTime end_date { get { return _etime; } set { _etime = value; } }
        public string end_dateN { get { return _etime.ToString(); } set { } }
        public int cctime { get { return (DateTime.Now-_etime).Days; } set { } }
        public Nullable<int> is_repeat { get; set; }
        public Nullable<int> clientid { get; set; }
        public string note { get; set; }
        public Nullable<int> status { get; set; }
        public Nullable<int> Urgency_level { get; set; }
        public string UrlTitle { get; set; }
        public string Url { get; set; }
        public string Creater { get; set; }
        public string CreaterName { get; set; }
        public Nullable<DateTime> CreateTm { get; set; }
        public Nullable<int> IsEnd { get; set; }
        public Nullable<int> Reminder { get; set; }
        public Nullable<int> Reminder_State { get; set; }
        public Nullable<DateTime> StartTimeEx { get; set; }
    }
}