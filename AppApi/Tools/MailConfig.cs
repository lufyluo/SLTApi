using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace AppApi.Tools
{
    public class MailConfig
    {
        public static readonly string Folder = "Email";
        public static readonly string LargeEmailHTmlDirectory =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Email");
    }
}