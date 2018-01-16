using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppApi.Models.WorkDt.Gain
{
    public class Add : GainParameter
    {
        public string menuno { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public int is_repeat { get; set; }
        public int clientid { get; set; }
        public string note { get; set; }
        public int Urgency_level { get; set; }
        public int Reminder { get; set; }

        public string Creater { get; set; }
        public string CreaterName { get; set; }
        public DateTime CreateTm { get; set; }
    }
}