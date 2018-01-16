using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppApi.Models.WorkDt.Back
{
    public class GetWeekRport : BackList
    {
        public IEnumerable<Models.WorkDt.Back.WorkWeekPort> BGL { get; set; }
    }
}