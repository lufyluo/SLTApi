using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace AppApi.Tools
{
    public class MailConfig
    {
        public static readonly string Folder = "Email";
        public static readonly string LargeEmailHTmlDirectory =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Folder);
        public static readonly int TheSizeTransEmailContentToHtml = int.Parse(ConfigurationManager.AppSettings["TheSizeTransEmailContentToHtml"] ?? "350");
    }
}