using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuckDraw.Controllers
{
    public class LuckController : Controller
    {
        public Models.CoreEntities EF { get; }

        public LuckController(Models.CoreEntities EF)
        {
            this.EF = EF;
        }
        public IActionResult Index()
        {

            var li = EF.Lucks.ToList();
         
            return View();
        }
      
        public IActionResult Edit()
        {
            var li = EF.Lucks.ToList();
            
            EF.SaveChanges();
            return View("Luck/index");
        }
        public IActionResult Del(int id)
        {
            var model = EF.Lucks.FirstOrDefault(x => x.ID == id);
            EF.Lucks.Remove(model);
            int num = EF.SaveChanges();
            return Redirect("Luck/index");
        }
        
    }
}
