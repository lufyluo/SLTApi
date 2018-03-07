using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace AppApi.Tools
{
    public class FileWriter
    {
        public void Write(string path, string content,string fileName)
        {
            path = Path.Combine(path, fileName);
            System.IO.File.WriteAllText(path, content, Encoding.UTF8);
        }
    }
}