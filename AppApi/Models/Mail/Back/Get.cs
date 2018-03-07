using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppApi.Models.Mail
{
    namespace Back
    {
        public class Get
        {
            private string _textbody;
            private string _htmlbody;
            private string _cc;
            private string _bcc;
            private string _replyto;
            private string _mailtype;
            private String _MailDate;
            private String _SendDate;
            public string Url { get; set; }
            public int id
            {
                get; set;
            }
            public String textbody
            {
                get
                {
                    try
                    {
                        _textbody = _textbody.Replace("\r\n", "").Replace(" ", "");
                        if (_textbody.Length < 50)
                        {
                            if (_textbody.Length >= 1)
                                _textbody = _textbody.Substring(0, _textbody.Length - 1);
                        }
                        else
                            _textbody = _textbody.Substring(0, 128);
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
            public string htmlbody
            {
                get; set;
            }//{ get { return _htmlbody; } set { _htmlbody = Tools.Base.Escape(value); } }
            public string cc
            {
                get; set;
            }//{ get { return _cc; } set { _cc = Tools.Base.Escape(value); } }
            public string bcc
            {
                get; set;
            } //{ get { return _bcc; } set { _bcc = Tools.Base.Escape(value); } }
            public string replyto
            {
                get; set;
            }//{ get { return _replyto; } set { _replyto = Tools.Base.Escape(value); } }
            public string mailtype
            {
                get; set;
            } //{ get { return _mailtype; } set { _mailtype = Tools.Base.Escape(value); } }
            public List<Mail.Back.item.File> file
            {
                get; set;
            } //{ get; set; }

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
            public DateTime? CreateTime { get; set; }
            public String MailLabel { get; set; }
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
            public int MailBoxId { get; set; }
        }
    }
}