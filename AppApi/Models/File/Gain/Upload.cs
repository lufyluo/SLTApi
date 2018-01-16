using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppApi.Models.File.Gain
{
    public class Upload:GainParameter
    {
        public int Position { get; set; }
        public string Data { get; set; }
        public int FileId { get; set; }
    }
}