using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using System.Net;  
namespace AppApi.Tools
{
    public class Log
    {
        public static string newapppathios = @"http://www.traderen.net/app/ios.txt";
        public static string newapppathandroid = @"http://www.traderen.net/app/android.txt";
        /**/
        /// <summary>
        /// 写入日志文件
        /// </summary>
        /// <param name="input"></param>
        public static void WriteLogFile(string logname,string input)
        {
            /**/
            ///指定日志文件的目录
            ///System.AppDomain.CurrentDomain.BaseDirectory+@"picimage\"
            string fname = System.AppDomain.CurrentDomain.BaseDirectory  +@"pushlog\"+ logname + ".txt";
            /**/
            ///定义文件信息对象
            if (!Directory.Exists(System.AppDomain.CurrentDomain.BaseDirectory+logname+@"\"))
            {
                Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory + logname+@"\");
            }
            FileInfo finfo = new FileInfo(fname);

            if (!finfo.Exists)
            {
                FileStream fs;
                fs = File.Create(fname);
                fs.Close();
                finfo = new FileInfo(fname);
            }

            /**/
            ///判断文件是否存在以及是否大于2K
            if (finfo.Length > 1024 * 1024 * 10)
            {
                /**/
                ///文件超过10MB则重命名
                File.Move(Directory.GetCurrentDirectory() + "\\" + logname + ".txt", Directory.GetCurrentDirectory() + DateTime.Now.TimeOfDay + "\\" + logname + ".txt");
                /**/
                ///删除该文件
                //finfo.Delete();
            }
            //finfo.AppendText();
            /**/
            ///创建只写文件流

            using (FileStream fs = finfo.OpenWrite())
            {
                /**/
                ///根据上面创建的文件流创建写数据流
                StreamWriter w = new StreamWriter(fs);

                /**/
                ///设置写数据流的起始位置为文件流的末尾
                w.BaseStream.Seek(0, SeekOrigin.End);

                /**/
                ///写入“Log Entry : ”
                w.Write("\n\rLog Entry : ");

                /**/
                ///写入当前系统时间并换行
                w.Write("{0} {1} \n\r", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString());

                /**/
                ///写入日志内容并换行
                w.Write(input + "\n\r");

                /**/
                ///写入------------------------------------“并换行
                w.Write("-------------------------------------------------------------------------\n\r");

                /**/
                ///清空缓冲区内容，并把缓冲区内容写入基础流
                w.Flush();

                /**/
                ///关闭写数据流
                w.Close();
            }

        }

        public static string getbeh(int app)
        {
            if (app == 1)
                return GetContentFromUrl(newapppathios);
            else
                return GetContentFromUrl(newapppathandroid);
        }

        /// <summary>
        /// 文件读取
        /// </summary>
        /// <param name="path"></param>
        public static string Read(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.Default);
            String line;
            string all = "";
            while ((line = sr.ReadLine()) != null)
            {
                if (all == "")
                    all = line.ToString();
                else
                    all += "_" + line.ToString();
            }
            return all; 
        }
        /// <summary>
        /// uri获取
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetContentFromUrl(string url)
        {
            try
            {
                WebClient client = new WebClient();
                client.Credentials = CredentialCache.DefaultCredentials;//获取或设置请求凭据  
                Byte[] pageData = client.DownloadData(url); //下载数据  
                string pageHtml = System.Text.Encoding.UTF8.GetString(pageData);
                return pageHtml;
            }
            catch (WebException ex)
            {
                return ex.Message;
            }
        }  

    }
}