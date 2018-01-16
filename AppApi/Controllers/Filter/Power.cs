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
    public class Power:Permissions
    {
        private string power;
        public Power(string power)
        {
            this.power = power;
        }
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);
            if (!Tools.Base.HasPower(GP,power))
            {
                BP.code = Tools.BackCode.NoPower;
                BP.back = Tools.BackCode.CodeStr[BP.code];
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.OK, BP);
                return;
            }
            return;
        }
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);
        }
    }
}