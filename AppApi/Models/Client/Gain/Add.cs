using AppApi.App_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppApi.Models.Client.Gain
{
    public class Add : GainParameter
    {
        public Nullable<int> BranchId { get; set; }
        public Nullable<int> DeptId { get; set; }
        public string No { get; set; }
        public string SName { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Tel { get; set; }
        public string Fax { get; set; }
        public string Web { get; set; }
        public string Bak { get; set; }
        public string custom1 { get; set; }
        public string custom2 { get; set; }
        public string custom3 { get; set; }
        public string custom4 { get; set; }
        public string custom5 { get; set; }
        public string custom6 { get; set; }
        public string custom7 { get; set; }
        public string custom8 { get; set; }
        public string custom9 { get; set; }
        public string custom10 { get; set; }
        public string Forwarder { get; set; }
        public string Credit { get; set; }
        public string ClientClass { get; set; }
        public string ClientLevel { get; set; }
        public string ClientState { get; set; }
        public string ClientSource { get; set; }
        public string ClientType { get; set; }
        public string ClientAttr { get; set; }
        public Nullable<int> ClientLock { get; set; }
        public Nullable<int> Aud { get; set; }
        public string Sale { get; set; }
        public Nullable<System.DateTime> SDate { get; set; }
        public Nullable<System.DateTime> RDate { get; set; }
        public Nullable<System.DateTime> KfTime { get; set; }
        public string DevUserId { get; set; }
        public string DevUserName { get; set; }
        public Nullable<System.DateTime> JoinTime { get; set; }
        public Nullable<System.DateTime> TopTime { get; set; }
        public Nullable<int> Pub { get; set; }
        public Nullable<System.DateTime> PubTime { get; set; }
        public string PubMsg { get; set; }
        public string PubClass { get; set; }
        public Nullable<int> IsDel { get; set; }
        public Nullable<System.DateTime> DelTime { get; set; }
        public string DelUserId { get; set; }
        public string DelUserName { get; set; }
        public string DelReason { get; set; }
        public Nullable<int> CommCount { get; set; }
        public Nullable<System.DateTime> CommTime { get; set; }
        public string Star { get; set; }
        public string Creater { get; set; }
        public string CreaterName { get; set; }
        public Nullable<System.DateTime> CreateTm { get; set; }
        public string Updater { get; set; }
        public string UpdaterName { get; set; }
        public Nullable<System.DateTime> UpdateTm { get; set; }
        public Nullable<System.DateTime> LastFollowTm { get; set; }
        public Nullable<int> Yj { get; set; }
        public Nullable<int> Pf { get; set; }
        public Nullable<int> EDM { get; set; }
        public Nullable<System.DateTime> SelTime { get; set; }
        public string LinkManNameStr { get; set; }
        public string LinkManEmailStr { get; set; }
        public string LinkManTelStr { get; set; }
        public Client_T ToClient()
        {
            Client_T C = new Client_T()
            {
                ClientAttr = ClientAttr,
                Name = Name,
                SName = SName,
                Sale = Sale,
                Web = Web,
                Tel = Tel,
                Fax = Fax,
                PostCode = PostCode,
                Star = Star,
                ClientSource = ClientSource,
                ClientState = ClientState,
                ClientClass = ClientClass,
                ClientType = ClientType,
                City = City,
                Country = Country,
                Province = Province,
                Address = Address,
                ClientLevel = ClientLevel,
                Bak = Bak,
                Forwarder = Forwarder,
                Credit = Credit,
                KfTime = KfTime,
                DevUserId = DevUserId,
                DevUserName = DevUserName,
                custom1 = custom1,
                custom2 = custom2,
                custom3 = custom3,
                custom4 = custom4,
                custom5 = custom5,
                custom6 = custom6,
                custom7 = custom7,
                custom8 = custom8,
                custom9 = custom9,
                custom10 = custom10,
                ClientLock = ClientLock,
                Aud = Aud,
                CreateTm = DateTime.Now,
                Creater = UserId,
                CreaterName = Tools.Base.GetUserName(UserId)
        };
            return C;
           
        }
    }
}