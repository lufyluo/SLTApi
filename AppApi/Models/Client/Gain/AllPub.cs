using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppApi.Models.Client.Gain
{
    public class AllPub:GainList
    {
        public AllPub()
        {
            _OrderBy = "CreateTm";
        }
    }
}