using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppApi.Models.User.Gain
{
    public class Pic : GainParameter
    {
        public string pictype { get; set; }
        public string picdata { get; set; }
    }
}