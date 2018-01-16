using AppApi.App_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace AppApi.Controllers
{
    public class BaseControllers: ApiController
    {
        protected TCRMEntities db = new TCRMEntities();
        protected TCRMFILEEntities dbf = new TCRMFILEEntities();
        protected TCRMMAILEntities dbm = new TCRMMAILEntities();
        protected Models.BackParameter BP = new Models.BackParameter();
        protected TCRMDOCEntities dbd = new TCRMDOCEntities();
        public static  System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        public static string luj = System.AppDomain.CurrentDomain.BaseDirectory + @"file\"; //获取文件路径
    }
}