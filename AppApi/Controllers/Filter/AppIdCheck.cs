using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using AppApi.App_Data;
using AppApi.Filter;
using AppApi.Log;
using AppApi.Models;
using AppApi.Tools;

namespace AppApi.Controllers.Filter
{
    public class AppIdCheck : Permissions
    {
        private static TCRMEntities db = new TCRMEntities();
        private static readonly string LoginStrategy = ConfigurationManager.AppSettings["LoginStrategy"] ??"-1";
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (LoginStrategy == "-1")//允许全部
            {
                return;
            }
            base.OnActionExecuting(actionContext);
            
            String code = AppPermissionCheck();
            LoginStateLog(GP,code);
            if (code != Tools.BackCode.Success)
            {
                BP.back = Tools.BackCode.CodeStr[code];
                BP.code = code;
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.OK, BP);
                return;
            }
        }
        private string AppPermissionCheck()
        {
            if (string.IsNullOrEmpty(GP.appid))
                return Tools.BackCode.Success;
            var permission = db.Database.SqlQuery<Application>(
                "select Id,AppId,[Description],[Available] from [dbo].[Application] where Appid='" + GP.appid + "'").FirstOrDefault();
            if (permission == null)
            {
                SqlClass.Insert In = new SqlClass.Insert("Application");
                In.Revise("AppId", GP.appid);
                In.Revise("Available", 0);
                db.Database.SqlQuery<Decimal>(In.ToString()).FirstOrDefault();
                return LoginStrategy == "-1"?Tools.BackCode.Success: Tools.BackCode.AppidForbid;
            }

            return permission?.Available == 0 ? Tools.BackCode.AppidForbid : Tools.BackCode.Success;
        }

        private void LoginStateLog(GainParameter gp,string state)
        {
            var logMsg = $"登陆账号：{gp.UserId}，appid：{gp.appid},login result code：{state}";
            LoginLog(logMsg);
        }
        private void LoginLog(string msg)
        {
            try
            {
                Logger.Log.Info(msg);
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
           
            
        }
    }
}