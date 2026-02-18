using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using MinistryInvestment.Mvc.Controllers;
using System;
using System.Diagnostics;
namespace MinistryInvestment.Mvc.ActionFilters
{
    public class ActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var methodName = filterContext.RouteData.Values["action"];
            // infinite loop on keep session alive 
            if (methodName.ToString() != "GetLastSessionTime" && methodName.ToString() != "KeepSessionAlive" && methodName.ToString() != "CheckTimer")
            {
                var controller = filterContext.Controller as BaseController;
                controller.KeepSessionAlive();
            }
        }
    }
}
