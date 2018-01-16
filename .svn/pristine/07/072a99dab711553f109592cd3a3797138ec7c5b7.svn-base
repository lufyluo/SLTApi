using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppApi.Models.Mail.Back.item
{
    public class MailAll
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

        public Nullable<System.Guid> guid { get; set; }

        public string UIDL { get; set; }
        public Nullable<int> Inside { get; set; }


        public string HtmlBody { get; set; }

        public string Notification { get; set; }
        public string notify { get; set; }

        public string UID { get; set; }
        public Nullable<int> ischeck { get; set; }
        public string checkmsg { get; set; }
        public string SPUserId { get; set; }
        public string SPUserName { get; set; }
        public Nullable<System.DateTime> SPTime { get; set; }
        public Nullable<int> SUID { get; set; }

        public string BaseDb { get; set; }
        public Nullable<long> BaseId { get; set; }

        public string REALFROM { get; set; }
        public string ReturnPath { get; set; }

        public string ConType { get; set; }
        public Nullable<int> Type { get; set; }
        public Nullable<int> RFMailId { get; set; }
        public Nullable<int> LocalTrack { get; set; }
        public Nullable<int> RemoteTrack { get; set; }
        public Nullable<int> IsReceipt { get; set; }
        public Nullable<int> ModId { get; set; }

        public Nullable<System.DateTime> RemindTime { get; set; }
        public Nullable<System.DateTime> PfDate { get; set; }
        public Nullable<int> IsDeal { get; set; }
        public Nullable<int> clientid { get; set; }
        //public Nullable<Int64> FileId { get; set; }
        //public string FileName { get; set; }
        //public Nullable<int> FileSize { get; set; }

    }
}