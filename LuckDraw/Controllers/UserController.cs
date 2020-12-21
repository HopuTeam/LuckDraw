using LuckDraw.Handles;
using LuckDraw.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuckDraw.Controllers
{
    public class UserController : Controller
    {
        private CoreEntities EF { get; }
        public UserController(CoreEntities _ef)
        {
            EF = _ef;
        }

        public IActionResult Index()
        {
            return View(EF.Signs.FirstOrDefault(x => x.ID == HttpContext.Session.GetModel<Sign>("User").ID));
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.SetModel("User", null);
            return Content("success");
        }

        [HttpGet]
        public IActionResult Auth()
        {
            return View(EF.Signs.FirstOrDefault(x => x.ID == HttpContext.Session.GetModel<Sign>("User").ID));
        }
        [HttpPost]
        public IActionResult Auth(Sign sign)
        {
            return Content("success");
        }
    }
}
