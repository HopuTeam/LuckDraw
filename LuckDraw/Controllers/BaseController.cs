using LuckDraw.Handles;
using LuckDraw.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LuckDraw.Controllers
{
    public class BaseController : Controller
    {
        public BaseController(CoreEntities _ef)
        {
            EF = _ef;
        }
        private CoreEntities EF { get; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Path == "/User/Logout")
            {
                // Add address to whitelist.
            }
            else if (context.HttpContext.Session.GetModel<Sign>("User") == null)
            {
                context.Result = new RedirectResult("/Sign/Index");
            }
        }
    }
}