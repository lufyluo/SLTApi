using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppApi.Models.Mail.Back
{
    public class GetMb
    {
        public Nullable<int> id { get; set; }
        public int type { get; set; }
        public string Name { get; set; }
        public string Html { get; set; }
        public string UseBox { get; set; }
        public Nullable<int> Sys { get; set; }
    }
}