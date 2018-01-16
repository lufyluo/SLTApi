using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppApi.Models.Client.Back
{
    public class Getdetailbf
    {
        public Nullable<int> Pf { get; set; }
        public Nullable<int> pf1 { get; set; }
        public Nullable<int> pf2 { get; set; }
        public Nullable<int> pf3 { get; set; }
        public Nullable<int> pf4 { get; set; }
        public Nullable<int> pf5 { get; set; }
        public string pftext1 { get; set; }
        public string pftext2 { get; set; }
        public string pftext3 { get; set; }
        public string pftext4 { get; set; }
        public string pftext5 { get; set; }
        public int pftop1{ get; set; }
        public int pftop2 { get; set; }
        public int pftop3 { get; set; }
        public int pftop4 { get; set; }
        public int pftop5 { get; set; }
    }
    public class Getdetaisjz
    {
        public string icon { get; set; }
        public string actionMenoNo { get; set; }
        public Nullable<int> actionKeyId { get; set; }
        public string title { get; set; }
        public string html { get; set; }
        public Nullable<DateTime> createtm { get; set; }
        public string url { get; set; }
        public string creatername { get; set; }
    }
    public class Getdetailbj
    {
        public Nullable<int> totalCount { get; set; }
        public Nullable<Decimal> totalMoney { get; set; }
    }
    public class Getdetaildd
    {
        public Nullable<int> totalCount { get; set; }
        public Nullable<Decimal> totalMoney { get; set; }
    }
    public class Getdetaillxr
    {
        public int Id { get; set; }
        public Nullable<int> ClientId { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public string Email { get; set; }
        public string Email1 { get; set; }
        public string Email2 { get; set; }
        public string Mobile { get; set; }
        public string Tel { get; set; }
        public string Dept { get; set; }
        public string Job { get; set; }
        public Nullable<DateTime> Birthday { get; set; }
        public string HomeAddress { get; set; }
        public string HomeTel { get; set; }
        public string Bak { get; set; }
        public string QQ { get; set; }
        public string Msn { get; set; }
        public string WeChat { get; set; }
        public string SKYPE { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Interest { get; set; }
        public int MainAdd { get; set; }
        public string Share { get; set; }
        public string ShareUID { get; set; }
        public string Role { get; set; }
        public string Manner { get; set; }
        public string Creater { get; set; }
        public string CreaterName { get; set; }
        public Nullable<DateTime> CreateTm { get; set; }
        public string Updater { get; set; }
        public string UpdaterName { get; set; }
        public Nullable<DateTime> UpdateTm { get; set; }
    }
}