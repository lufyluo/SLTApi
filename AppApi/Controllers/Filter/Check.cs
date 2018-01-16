using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using AppApi.Models;
using AppApi.App_Data;
using AppApi.Controllers;

namespace AppApi.Filter
{
    public class Check :Permissions
    {
        private static TCRMEntities db = new TCRMEntities();
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            String code = "";
            BaseControllers.stopwatch =new System.Diagnostics.Stopwatch();
            BaseControllers.stopwatch.Start(); //  开始监视代码
            base.OnActionExecuting(actionContext);
            if (GP.parentids != GP.UserId && GP.parentids != null && GP.parentids != "")
            {
                if ( Tools.Base.CheckSub(GP.parentids, GP.UserId))
                {
                    GP.UserId = GP.parentids;
                }
                else
                {
                    code = Tools.BackCode.SubErr;
                    BP.back = Tools.BackCode.CodeStr[code];
                    BP.code = code;
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.OK, BP);
                    return;
                }
            }
             code = Tools.Base.Check(GP);
            if (code != "0000")
            {
                BP.back = Tools.BackCode.CodeStr[code];
                BP.code = code;
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.OK, BP);
                return;
            }
            else
            {
                if (GP.appid != null&&GP.appid != "")
                {
                    IEnumerable<string> sameuserid = db.Database.SqlQuery<string>("Select userid from User_T where appid='" + GP.appid + "' and userid <>'" + GP.UserId + "'");
                    foreach (string userid in sameuserid)
                    {
                        db.Database.ExecuteSqlCommand("update User_T set appid='' where userid='" + userid + "'");
                    }
                    string BiMuser = db.Database.SqlQuery<string>("Select appid from User_T where userid='" + GP.UserId + "'").FirstOrDefault();
                    if (BiMuser == "")
                        db.Database.ExecuteSqlCommand("update User_T set appid='" + GP.appid + "' where userid='" + GP.UserId + "'");
                    else
                    {
                        if (BiMuser != GP.appid)
                            db.Database.ExecuteSqlCommand("update User_T set appid='" + GP.appid + "' where userid='" + GP.UserId + "'");
                    }
                }
                
            }
            return;
        }
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);
        }

    }
}