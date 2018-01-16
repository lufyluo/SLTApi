using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppApi.Models.Contact.Gain
{
    public class GetList:GainParameter
    {
        public int PageIndex { get; set; }
        public int PageMax { get; set; }
        public int ClientId { get; set; }
        public String Where { get; set; }
        public String Key { get; set; }
    }
}