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
            var User = HttpContext.Session.GetModel<Sign>("User");
            var mod = (from sign in EF.Signs
                       where sign.ID == User.ID
                       select new
                       {
                           username = sign.Account,
                           luck = (from luck in EF.Lucks
                                   where luck.SignID == sign.ID && luck.ParentID >= 0
                                   select luck
                                ).Count(),
                           draw = (from draw in EF.Draws
                                   where draw.SignID == sign.ID
                                   select draw
                           ).Count()
                       }).FirstOrDefault();

            return View();
        }
        [HttpPost]
        public IActionResult UserView()
        {
   
            return View();
        }

    }
}