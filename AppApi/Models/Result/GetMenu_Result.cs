using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppApi.Models.Result
{
    public class GetMenu_Result
    {
        public string MenuNo { get; set; }
        public string MenuName { get; set; }
        public string MenuNameEn { get; set; }
        public Nullable<decimal> MenuOrder { get; set; }
        public Nullable<bool> MenuSub { get; set; }
        public string MenuUrl { get; set; }
        public string ParentNo { get; set; }
        public Nullable<bool> OpenWin { get; set; }
        public Nullable<int> WinWidth { get; set; }
        public Nullable<int> WinHeight { get; set; }
        public string Icon { get; set; }
        public string IconUrl { get; set; }
        public Nullable<bool> IsLinkPower { get; set; }
        public Nullable<int> Num { get; set; }
    }
}