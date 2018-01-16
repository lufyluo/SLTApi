using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppApi.Models.WorkDt.Back
{
    public class workdt
    {
        private DateTime _time;
        public int Id { get; set; }
        public string MenuNo { get; set; }
        public Nullable<int> KeyId { get; set; }
        public string TableName { get; set; }
        public string ShareUID { get; set; }
        public string TitleHtml { get; set; }
        public string Bak { get; set; }
        public Nullable<int> CommCount { get; set; }
        public string Creater { get; set; }
        public string CreaterName { get; set; }
        public DateTime CreateTm { get { return _time; } set { _time = value; } }
        public string CreatTmN { get { return _time.ToString(); } set { } }
        public string ShareUID1 { get; set; }
        public IEnumerable<Models.WorkDt.Back.Comm> commlist { get; set; }
    }
}