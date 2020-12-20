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
            var sign = HttpContext.Session.GetModel<Sign>("User");
            return View(EF.Signs.FirstOrDefault(x => x.ID == sign.ID));
        }
    }
}
