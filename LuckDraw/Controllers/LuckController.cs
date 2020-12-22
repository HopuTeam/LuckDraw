using LuckDraw.Handles;
using LuckDraw.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuckDraw.Controllers
{
    public class LuckController : Controller
    {
        private CoreEntities EF { get; }
        public LuckController(CoreEntities _ef)
        {
            EF = _ef;
        }

        public IActionResult Index()
        {
            return View(EF.Lucks.Where(x => x.SignID == HttpContext.Session.GetModel<Sign>("User").ID).ToList());
        }
        
        [HttpPost]
        public IActionResult Add(Luck luck)
        {
            return View();
        }

        [HttpPost]
        public IActionResult Edit()
        {
            EF.SaveChanges();
            return View("Luck/index");
        }

        [HttpPost]
        public IActionResult Del(int id)
        {
            var model = EF.Lucks.FirstOrDefault(x => x.ID == id);
            EF.Lucks.Remove(model);
            int num = EF.SaveChanges();
            return Redirect("Luck/index");
        }
    }
}
