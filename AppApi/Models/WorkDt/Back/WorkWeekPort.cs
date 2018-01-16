using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppApi.Models.WorkDt.Back
{
    public class WorkWeekPort
    {
        private DateTime _time;
        public Nullable<int> id { get; set; }
        public Nullable<int> Type { get; set; }
        public Nullable<int> BranchId { get; set; }
        public Nullable<int> DeptId { get; set; }
        public Nullable<DateTime> RepStartDate { get; set; }
        public Nullable<DateTime> RepEndDate { get; set; }
        public string Html { get; set; }
        public string Creater { get; set; }
        public string CreaterName { get; set; }
        public DateTime CreateTm { get { return _time; } set { _time = value; } }
        public string CreatTmN { get { return _time.ToString(); } set { } }
        public string Updater { get; set; }
        public string UpdaterName { get; set; }
        public Nullable<DateTime> UpdateTm { get; set; }
        public Nullable<int> CommCount { get; set; }
        public string CommReadUID { get; set; }
    }
}