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
    
    public partial class ErrLog_T
    {
        public int Id { get; set; }
        public Nullable<int> MailBoxId { get; set; }
        public string Type { get; set; }
        public string Subject { get; set; }
        public string UIDL { get; set; }
        public string Msg { get; set; }
        public string Creater { get; set; }
        public Nullable<System.DateTime> CreateTm { get; set; }
    }
}