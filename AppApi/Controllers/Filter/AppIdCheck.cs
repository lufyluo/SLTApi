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
            if (CheckAppidIsEmpty())
            {
                return GetReturn();
            }
            return HandleAppid();
            
        }

        private string HandleAppid()
        {
            bool available = false;
            var appPermission = db.Database.SqlQuery<Application>(
                "select Id,AppId,[Description],[Available] from [dbo].[Application] where Appid='" + GP.appid + "'").FirstOrDefault();
            if (appPermission == null)
            {
                AddNewAppid();
                AddNewAppidWithUser();
            }
            else
            {
                var userPermission = GetUserPermission();
                available = appPermission.Available == 1&& userPermission?.Available ==1;
            }

            return GetReturn(available);
        }

        private AppPermission GetUserPermission()
        {
            var userPermission = db.Database.SqlQuery<AppPermission>(
                    "select AppId,[UserId],[Available] from [dbo].[AppPermission] where Appid='" + GP.appid + "'")
                .FirstOrDefault();
            if (userPermission == null)
            {
                AddNewAppidWithUser();
                return new AppPermission
                {
                    AppId = GP.appid,
                    Available = 1
                };
            }
            return userPermission;
        }

        private  string GetReturn(bool available = true)
        {
            if (LoginStrategy == "-1")
                return Tools.BackCode.Success;
            return available ? Tools.BackCode.Success : Tools.BackCode.AppidForbid;
        }

        private bool CheckAppidIsEmpty()
        {
            return string.IsNullOrEmpty(GP.appid);
        }

        private void AddNewAppid()
        {
            SqlClass.Insert In = new SqlClass.Insert("Application");
            In.Revise("AppId", GP.appid);
            In.Revise("Available", 0);
            db.Database.SqlQuery<Decimal>(In.ToString()).FirstOrDefault();
        }
        private void AddNewAppidWithUser()
        {
            SqlClass.Insert In = new SqlClass.Insert("AppPermission");
            In.Revise("AppId", GP.appid);
            In.Revise("UserId", GP.UserId);
            In.Revise("Available", 1);
            db.Database.SqlQuery<Decimal>(In.ToString()).FirstOrDefault();
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