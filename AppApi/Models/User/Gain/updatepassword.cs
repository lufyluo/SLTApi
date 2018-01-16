using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppApi.Models.User.Gain
{
    public class updatepassword : GainParameter
    {
        public string newpassword { get; set; }
    }
}