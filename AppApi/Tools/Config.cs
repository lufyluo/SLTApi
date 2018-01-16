using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace AppApi.Tools
{
    public class Config
    {
        
        public static String AndroidSign = ConfigurationManager.AppSettings["AndroidSign"] ?? string.Empty;
        public static String IOSSign = ConfigurationManager.AppSettings["IosSign"] ?? string.Empty;
    }
}