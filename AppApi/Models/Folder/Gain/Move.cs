using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppApi.Models.Folder.Gain
{
    public class Move:GainParameter
    {
        public int FolderId { get; set; }
        public String ParentId { get; set; }
    }
}