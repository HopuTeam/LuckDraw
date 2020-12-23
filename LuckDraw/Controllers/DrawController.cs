using LuckDraw.Handles;
using LuckDraw.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuckDraw.Controllers
{
    public class DrawController : Controller
    {
        private CoreEntities EF { get; }
        public DrawController(CoreEntities _ef)
        {
            EF = _ef;
        }

        public IActionResult Index()
        {
            var sid = HttpContext.Session.GetModel<Sign>("User").ID;
            var list = (from dr in EF.Draws
                        where dr.SignID == sid
                        join op in EF.Options on dr.OptionID equals op.ID
                        select new
                        {
                            drname = dr.Name,
                            opname = op.Name,
                            opid = op.ID,
                            zongshu = (from lcda in EF.LuckDraws
                                       where lcda.DrawID == dr.ID
                                       select lcda.DrawID).Count(),
                            luckdrawDrawID = (from lcda in EF.LuckDraws
                                              where lcda.DrawID == dr.ID
                                              select lcda.DrawID).FirstOrDefault()
                        }).ToList();

            var res = new List<DrawViewModle>();
            foreach (var item in list)
            {
                res.Add(new DrawViewModle
                {
                    DrawName = item.drname,
                    OptionName = item.opname,
                    OptionID = item.opid,
                    LuckCount = item.zongshu,
                    LuckdrawDrawID = item.luckdrawDrawID
                });
            }
            return View(res);
        }

        public IActionResult Add(string LuckType, string Name)
        {
            
            return View();
        }

        [HttpPost]
        public IActionResult Delete(int ID)
        {
            var mod = EF.Draws.FirstOrDefault(x => x.ID == ID);
            var info = EF.LuckDraws.Where(x => x.DrawID == mod.ID);
            if (info.ToList().Count() > 0)
                foreach (var item in info)
                    EF.Remove(item);

            EF.Remove(mod);
            if (EF.SaveChanges() > 0)
                return Content("success");
            else
                return Content("删除失败");
        }
    }
}
