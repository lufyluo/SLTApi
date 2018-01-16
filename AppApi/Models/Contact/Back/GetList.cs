using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppApi.Models.Contact.Back
{
    public class GetList
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
        public Nullable<System.DateTime> Birthday { get; set; }
        public string Bak { get; set; }
        public string QQ { get; set; }
        public string Msn { get; set; }
        public string WeChat { get; set; }
        public string SKYPE { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Interest { get; set; }
        public Nullable<int> MainAdd { get; set; }
        public string UpdaterName { get; set; }
        public Nullable<System.DateTime> UpdateTm { get; set; }
    }
}