//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace AppApi.App_Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class WorkReport_T
    {
        public int Id { get; set; }
        public Nullable<int> Type { get; set; }
        public Nullable<int> BranchId { get; set; }
        public Nullable<int> DeptId { get; set; }
        public Nullable<System.DateTime> RepStartDate { get; set; }
        public Nullable<System.DateTime> RepEndDate { get; set; }
        public string Html { get; set; }
        public string Creater { get; set; }
        public string CreaterName { get; set; }
        public Nullable<System.DateTime> CreateTm { get; set; }
        public string Updater { get; set; }
        public string UpdaterName { get; set; }
        public Nullable<System.DateTime> UpdateTm { get; set; }
        public Nullable<int> CommCount { get; set; }
        public string CommReadUID { get; set; }
    }
}