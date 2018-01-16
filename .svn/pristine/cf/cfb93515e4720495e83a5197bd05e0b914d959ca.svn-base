using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace AppApi.Filter
{
    public class Mail:Permissions
    {
        public Mail():base("MailId"){}
        public Mail(string mailidstr):base(mailidstr)
        {
        }
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);
            foreach (string item in Pid)
            {
                if (!Tools.Base.IsUseMail(GP,int.Parse(item)))
                {
                    BP.code = Tools.BackCode.NotReadMail;
                    BP.back = Tools.BackCode.CodeStr[BP.code];
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.OK, BP);
                    return;
                }
            }
        }
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);
        }
    }
}