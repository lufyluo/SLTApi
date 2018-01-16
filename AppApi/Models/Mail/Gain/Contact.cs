using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppApi.Models.Mail.Gain
{
    public class Contact:GainList
    {
        public int ContactId { get; set; }
        public Contact(){

            _OrderBy = "Maildate";
        }
    }
}