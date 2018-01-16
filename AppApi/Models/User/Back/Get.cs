using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppApi.Models.User.Back
{
    public class Get
    {
        public int id { get; set; }
        public string name { get; set; }
        public string sex { get; set; }
        public string phone { get; set; }
        public string state { get; set; }
        public int type { get; set; }
        public string department { get; set; }
        public string branch { get; set; }
        public IEnumerable<item.MailLabel> maillabel { get; set; }
    }
}