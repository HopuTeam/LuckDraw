using LuckDraw.Handles;
using LuckDraw.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuckDraw.Controllers
{
    public class HomeController : Controller
    {
        private CoreEntities EF { get; }
        public HomeController(CoreEntities _ef)
        {
            EF = _ef;
        }

        public IActionResult Index()
        {
            var mod = (from s in EF.Signs
                       join l in EF.Lucks on s.ID equals l.SignID
                       join d in EF.Draws on s.ID equals d.SignID
                       where s.Account == HttpContext.Session.GetModel<Sign>("User").Account
                       select new { s, l, d }).FirstOrDefault();
            return View(mod);
        }
    }
}
