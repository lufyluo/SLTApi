using AppApi.App_Data;
using AppApi.Filter;
using AppApi.Models.Result;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Security.Cryptography;
using System.IO;
using System.Text;


namespace AppApi.Tools
{
    public class newpic
    {
        public static string luj = System.AppDomain.CurrentDomain.BaseDirectory + @"picimage\"; //获取文件路径
        public static void createwebpic(string pictype,string userid, byte[] picdata,string picname)
        {
            if (!Directory.Exists(luj))
            {
                Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory + @"picimage");
            }
            if (System.IO.File.Exists(luj + picname + ""))
            {
                return;
            }
            using (MemoryStream ms = new MemoryStream(picdata))
            {
                System.Drawing.Bitmap bmp2 = new System.Drawing.Bitmap(ms);

                if (pictype == "jpg")
                    bmp2.Save(luj + picname , System.Drawing.Imaging.ImageFormat.Jpeg);
                if (pictype == "bmp")
                    bmp2.Save(luj + picname, System.Drawing.Imaging.ImageFormat.Bmp);
                if (pictype == "gif")
                    bmp2.Save(luj + picname , System.Drawing.Imaging.ImageFormat.Gif);
                if (pictype == "png")
                    bmp2.Save(luj + picname , System.Drawing.Imaging.ImageFormat.Png);
            }
            
        }
       
    }
}