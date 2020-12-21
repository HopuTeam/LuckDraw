using LuckDraw.Handles;
using LuckDraw.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            //if (context.HttpContext.Session.GetModel<Sign>("User") != null)
            //{
            //    context.Result = new RedirectResult("/Home/Index");
            //}
        }
    }
}
