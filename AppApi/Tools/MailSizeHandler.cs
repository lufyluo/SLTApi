using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace AppApi.Tools
{
    public class MailSizeHandler
    {
        public  bool  MailHtmlFileCheck(int mailId)
        {
            return System.IO.File.Exists(Path.Combine(MailConfig.LargeEmailHTmlDirectory, mailId + ".html"));
        }

        public string ContentSizeCheck(string content,string fileName)
        {
            var contentByte = Encoding.UTF8.GetByteCount(content);
            if (contentByte >= MailConfig.TheSizeTransEmailContentToHtml*1024)
            {
                new FileWriter().Write(MailConfig.LargeEmailHTmlDirectory,content,fileName);
                return MailConfig.Folder + "/" + fileName;
            }
            return "";
        }
    }
}