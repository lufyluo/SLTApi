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
    
    public partial class Warning_T
    {
        public int Id { get; set; }
        public string WarningNo { get; set; }
        public string Descript { get; set; }
        public Nullable<int> Type { get; set; }
        public string Clerk_id { get; set; }
        public Nullable<int> Is_Email { get; set; }
        public Nullable<int> Is_SMS { get; set; }
        public Nullable<int> Times { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<System.DateTime> CreateTm { get; set; }
        public Nullable<int> State { get; set; }
        public Nullable<int> Is_Sql { get; set; }
        public Nullable<int> Is_repeat { get; set; }
        public Nullable<int> Time_frame { get; set; }
        public string MenuNo { get; set; }
    }
}
