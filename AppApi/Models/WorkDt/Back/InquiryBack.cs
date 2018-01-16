using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AppApi.App_Data;

namespace AppApi.Models.WorkDt.Back
{
    public class InquiryBack : BackList
    {
        public IEnumerable<Inquiry_T> Inq { get; set; }
    }
}