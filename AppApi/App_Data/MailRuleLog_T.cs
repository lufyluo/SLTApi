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
    
    public partial class MailRuleLog_T
    {
        public int Id { get; set; }
        public Nullable<int> MailBoxId { get; set; }
        public Nullable<int> MailId { get; set; }
        public Nullable<int> RuleId { get; set; }
        public string ParaName { get; set; }
        public string Oper { get; set; }
        public string OperValue { get; set; }
        public Nullable<bool> rv { get; set; }
        public Nullable<System.DateTime> CreateTm { get; set; }
    }
}
