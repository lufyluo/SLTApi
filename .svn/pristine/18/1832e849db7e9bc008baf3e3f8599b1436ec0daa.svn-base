
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace AppApi.Filter
{
    public class Permissions : ActionFilterAttribute
    {
        protected Models.BackParameter BP = new Models.BackParameter();
        protected List<string> Pid = new List<string>();
        protected Models.GainParameter GP = new Models.GainParameter();
        string Paraname = "";
        public Permissions() { }
        public Permissions(string Paraname)
        {
            this.Paraname = Paraname;
        }
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);
            HttpContext context = System.Web.HttpContext.Current;

            GP.UserId = context.Request.Form["userid"];
            GP.Password = context.Request.Form["password"];
            GP.Ran = context.Request.Form["ran"];
            GP.Sign = context.Request.Form["sign"];
            GP.appid = context.Request.Form["appid"];
            GP.parentids = context.Request.Form["parentids"];
            try { Pid = context.Request.Form[Paraname].Split(',').ToList(); } catch { }
            
        }
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);
        }
    }
}