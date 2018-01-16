using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppApi.Tools
{
    public class Sign
    {
        //检查签名
        public static bool CheckSign(string ran, string md5Sign)
        {
            if (System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(Config.AndroidSign + ran, "MD5") == md5Sign)
            {
                return true;
            }
            else if (System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(Config.IOSSign + ran, "MD5") == md5Sign)
                return true;
            return false;
        }

    }
}