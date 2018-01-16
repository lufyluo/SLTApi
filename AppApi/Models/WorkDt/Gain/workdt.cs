using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppApi.Models.WorkDt.Gain
{
    public class workdt : GainParameter
    {
        public string act { get; set; }
        public string date { get; set; }
        public string fh { get; set; }
    }
}