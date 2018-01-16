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
    public class FileCreater:Permissions
    {
        public FileCreater():base("FileId")
        { }
        public FileCreater(string Fileidstr):base(Fileidstr)
        {
        }
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);
            foreach (string item in Pid)
            {
                if (!Tools.Base.IsFileCreater(GP, int.Parse(item)))
                {
                    BP.code = Tools.BackCode.FileNotIsYou;
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