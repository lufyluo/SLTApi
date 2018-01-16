using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppApi.Models.Mail.Back.item
{
    public class Mail
    {
        private String _MailDate;
        private String _SendDate;
        private String _textbody;
        public int Id { get; set; }
        public Nullable<int> MailBoxId { get; set; }
        public String FromName { get; set; }
        public String FromEmail { get; set; }
        public String itFrom { get; set; }
        public String itTo { get; set; }
        public String Subject { get; set; }
        public String MailDate
        {
            get
            {
                return Tools.Base.timestrformat(_MailDate);
            }
            set
            {
                _MailDate = value;
            }
        }
        public DateTime RecDate { get; set; }
        public String SendDate
        {
            get
            {
                return Tools.Base.timestrformat(_SendDate);
            }
            set
            {
                _SendDate = value;
            }
        }
        public String textbody
        {
            get
            {
                try
                {
                    _textbody = _textbody.Replace("\r\n", "").TrimStart(); ;
                    if (_textbody.Length < 50)
                    {
                        if (_textbody.Length >= 1)
                            _textbody = _textbody.Substring(0, _textbody.Length - 1);
                    }
                    else
                        _textbody = _textbody.Substring(0, 50);
                    return _textbody;
                }
                catch (Exception ee)
                {
                    return _textbody;
                }
            }
            set
            {
                _textbody = value;
            }
        }
        public DateTime? CreateTime { get; set; }
        public String MailLabel { get; set; }
        public String cc { get; set; }
        public String bcc { get; set; }
        public String ReplyTo { get; set; }
        public String MailType { get; set; }
        public String Read { get; set; }
        public string priority { get; set; }
        public string RE { get; set; }
        public Nullable<int> MailSize { get; set; }
        public string RevUserId { get; set; }
        public String RevUserName { get; set; }
        public String Bak { get; set; }
        public String IP { get; set; }
        public String Area { get; set; }
        public string star { get; set; }
        public string redflag { get; set; }
        public string BoxBase { get; set; }
        public string Box { get; set; }
        public Nullable<int> RootId { get; set; }
        public Nullable<System.DateTime> TopTime { get; set; }
        public Nullable<int> ReadMode { get; set; }
        public Nullable<short> AccCount { get; set; }
        public Nullable<int> clientid { get; set; }
        //public Nullable<int> Inq { get; set; }
        //public Nullable<int> InqMailIdBase { get; set; }
        //public Nullable<int> InqReLimit { get; set; }
        //public string InqBak { get; set; }
        //public Nullable<int> InqYfp { get; set; }
        //public Nullable<System.DateTime> InqReTime { get; set; }
        //public string InqSaleUserId { get; set; }
        //public string InqSaleUserName { get; set; }
        //public string InqDevUserId { get; set; }
        //public string InqDevUserName { get; set; }
        //public Nullable<int> InqClientId { get; set; }
        //public string InqClientName { get; set; }
        //public Nullable<int> InqLinkManId { get; set; }
        //public string InqLinkMan { get; set; }
        //public string InqLinkManEmail { get; set; }
        //public string InqLinkManMobile { get; set; }
        //public string InqLinkManTel { get; set; }
        //public Nullable<System.DateTime> InqFpTime { get; set; }
        //public string InqFpUserId { get; set; }
        //public string InqFpUserName { get; set; }
        //public string InqProId { get; set; }
        //public string InqProName { get; set; }
        //public string InqWeb { get; set; }
        //public string InqSource { get; set; }
        //public string InqClientClass { get; set; }
        //public string InqCountry { get; set; }
        //public Nullable<int> InqIsOld { get; set; }
    }
}