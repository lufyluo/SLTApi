using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using AppApi.App_Data;
using AppApi.Filter;

namespace AppApi.Controllers.Filter
{
    public class AppIdCheck : Permissions
    {
        private static TCRMEntities db = new TCRMEntities();

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            String code = "";
            if (AppPermissionCheck())
            {
                code = Tools.BackCode.NoPower;
                BP.back = Tools.BackCode.CodeStr[code];
                BP.code = code;
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.OK, BP);
                return;
            }
        }
        private bool AppPermissionCheck()
        {
            if (string.IsNullOrEmpty(GP.appid))
                return true;
            var permission = db.Database.SqlQuery<Application>(
                "select Id,AppId,[Description],[Available] from [dbo].[Application] where Appid='" + GP.appid + "' AND Available =1").FirstOrDefault();
            return permission == null;
        }
    }
}