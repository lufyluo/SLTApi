using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using AppApi.Filter;
using AppApi.Tools;

namespace AppApi.Controllers.Filter
{
    public class LargeMailFilter : Permissions
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            HttpContext context = System.Web.HttpContext.Current;
            var mailId = context.Request.Form["MailId"];
            if (!string.IsNullOrEmpty(mailId))
            {
                if (System.IO.File.Exists(Path.Combine(MailConfig.LargeEmailHTmlDirectory, mailId + ".html")))
                {
                    BP.back = new Models.Mail.Back.Get { Url = MailConfig.Folder+"/" + mailId + ".html" };
                    BP.code = Tools.BackCode.Success;
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.OK, BP);
                    return;
                }
                
            }
        }
    }
}