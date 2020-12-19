using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LuckDraw.Models;
using LuckDraw.Handles;

namespace LuckDraw.Filters
{
    public class AuthAttribute : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.Session.GetModel<Sign>("User") == null)
            {
                RedirectResult result = new RedirectResult("/Home/Sign");
                context.Result = result;
            }
        }
    }
}
