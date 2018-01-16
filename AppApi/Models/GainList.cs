using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppApi.Models
{
    public class GainList:GainParameter
    {
        protected String _OrderBy = "";
        public int PageIndex { get; set; }
        public int PageMax { get; set; }
        public String Where { get; set; }
        public String OrderBy { get { return _OrderBy; } set { _OrderBy = value; } }
    }
}