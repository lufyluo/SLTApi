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
    
    public partial class ClientCare_T
    {
        public int Id { get; set; }
        public Nullable<int> ClientId { get; set; }
        public Nullable<System.DateTime> CareDate { get; set; }
        public string Reason { get; set; }
        public Nullable<int> CalendarId { get; set; }
        public string Creater { get; set; }
        public string CreaterName { get; set; }
        public Nullable<System.DateTime> CreateTm { get; set; }
        public string Updater { get; set; }
        public string UpdaterName { get; set; }
        public Nullable<System.DateTime> UpdateTm { get; set; }
    }
}
