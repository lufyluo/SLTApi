using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppApi.Models.Mail
{
    namespace Gain {
    public class Get:GainParameter
    {
            public int MailId { get; set; }
            public int clientid { get; set; }
        }
   }
}