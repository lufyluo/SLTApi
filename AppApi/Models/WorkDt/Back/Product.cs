using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AppApi.Tools;
using AppApi.Filter;
using AppApi.App_Data;

namespace AppApi.Models.WorkDt.Back
{
    public class Product : BackList
    {
        public IEnumerable<Product_T> Pro { get; set; }
    }
}