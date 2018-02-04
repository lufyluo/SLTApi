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
    
    public partial class Receivables_T
    {
        public int Id { get; set; }
        public string No { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Currency { get; set; }
        public string Bank { get; set; }
        public string BankAccount { get; set; }
        public Nullable<decimal> BankFee { get; set; }
        public Nullable<decimal> Received { get; set; }
        public Nullable<decimal> UVR { get; set; }
        public Nullable<decimal> AmountRMB { get; set; }
        public Nullable<decimal> ReceivedRMB { get; set; }
        public string Receiver { get; set; }
        public Nullable<System.DateTime> ReceiveTm { get; set; }
        public string Bak { get; set; }
        public Nullable<bool> IsConstraint { get; set; }
        public string SellNo { get; set; }
        public Nullable<System.DateTime> ComDate { get; set; }
        public Nullable<int> ClientId { get; set; }
        public string ClientName { get; set; }
        public string ExSaleUserId { get; set; }
        public string ExSaleUserName { get; set; }
        public string SellCurrency { get; set; }
        public Nullable<decimal> SellUVR { get; set; }
        public Nullable<decimal> TotalMoney { get; set; }
        public Nullable<System.DateTime> DepositDate { get; set; }
        public Nullable<decimal> Deposit { get; set; }
        public Nullable<System.DateTime> BalanceDate { get; set; }
        public Nullable<decimal> Balance { get; set; }
        public string SpUserId { get; set; }
        public Nullable<int> Submit { get; set; }
        public string SpMsg { get; set; }
        public string SpUserName { get; set; }
        public string Creater { get; set; }
        public string CreaterName { get; set; }
        public Nullable<System.DateTime> CreateTm { get; set; }
        public Nullable<System.DateTime> SpTime { get; set; }
        public Nullable<int> SpResult { get; set; }
    }
}