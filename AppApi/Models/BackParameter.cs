using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppApi.Models
{
    public class BackParameter
    {
        private string _code = "0000";
        public object back { get; set; }
        public String timestamp { get { return Tools.Base.GetTimeStamp(); }set { } }
        public String code { get { return _code; } set { _code = value; } }
    }
}