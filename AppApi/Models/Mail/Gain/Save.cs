using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppApi.Models.Mail.Gain
{
    public class Save:GainParameter
    {
        public int Id { get; set; }
        public Nullable<int> MailBoxId { get; set; }
        public Nullable<int> ReadMode { get; set; }
        public Nullable<int> Inside { get; set; }
        public string FromName { get; set; }
        public string FromEmail { get; set; }
        public string Subject { get; set; }
        public string SendDate { get; set; }
        public string MailType { get; set; }
        public string itFrom { get; set; }
        public string itTo { get; set; }
        public string cc { get; set; }
        public string bcc { get; set; }
        public string TextBody { get; set; }
        public string HtmlBody { get; set; }
        public string ReplyTo { get; set; }
        public string Read { get; set; }
        public string ReadUID { get; set; }
        public string BoxBase { get; set; }
        public string Box { get; set; }
        public string priority { get; set; }
        public string star { get; set; }
        public string redflag { get; set; }
        public string UID { get; set; }
        public Nullable<int> SUID { get; set; }
        public string MailDate { get; set; }
        public string RevUserId { get; set; }
        public string RevUserName { get; set; }
        public string MailLabel { get; set; }
        public Nullable<int> Type { get; set; }
        public Nullable<int> RFMailId { get; set; }
        public Nullable<int> LocalTrack { get; set; }
        public Nullable<int> RemoteTrack { get; set; }
        public Nullable<int> IsReceipt { get; set; }
        public Nullable<int> ModId { get; set; }
        public String Fileid { get; set; }
    }
}