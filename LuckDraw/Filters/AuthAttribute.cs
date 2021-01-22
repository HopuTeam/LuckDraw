using LuckDraw.Handles;
using LuckDraw.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LuckDraw.Filters
{
    public class AuthAttribute : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.Session.GetModel<Sign>("User") == null)
            {
                context.Result = new RedirectResult("/Sign/Index"); ;
            }
        }
    }
}