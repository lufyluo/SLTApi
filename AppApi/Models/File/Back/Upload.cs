using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppApi.Models.File.Back
{
    public class Upload
    {
        public long fileid { get; set; }
        public int length { get; set; }
        public string msg { get; set; }
    }
}