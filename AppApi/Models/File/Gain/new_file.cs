using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppApi.Models.File.Gain
{
    public class new_file
    {
    }
    public class download : GainParameter
    {
        public string fileid { get; set; }
        public string fileclass { get; set; }
    }

    public class filereturn
    {
        public string uri { get; set; }
    }
}