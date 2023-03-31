using Microsoft.AspNetCore.Mvc.Filters;

namespace SmartEco.Web.Helpers.Filters
{
    public class WriteToSession : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            string actionName = context.HttpContext.Request.RouteValues["action"].ToString();
            string controllerName = context.HttpContext.Request.RouteValues["controller"].ToString();
            context.HttpContext.Session.SetString("controller", controllerName);
            context.HttpContext.Session.SetString("action", actionName);
        }
    }
}
