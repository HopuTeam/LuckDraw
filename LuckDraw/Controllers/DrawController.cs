using LuckDraw.Handles;
using LuckDraw.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

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
            var list = (from dr in EF.Draws
                        where dr.SignID == HttpContext.Session.GetModel<Sign>("User").ID
                        join op in EF.Options on dr.OptionID equals op.ID
                        select new
                        {
                            drname = dr.Name,
                            opname = op.Name,
                            opid = op.ID,
                            zongshu = (from lcda in EF.LuckDraws
                                       where lcda.DrawID == dr.ID
                                       select lcda.DrawID).Count(),
                            luckdrawDrawID = dr.ID,
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
                    LuckdrawDrawID = item.luckdrawDrawID,
                });
            }
            return View(res);
        }

        public IActionResult SetDraw()
        {
            int Userid = HttpContext.Session.GetModel<Sign>("User").ID;
            List<Luck> mod = EF.Lucks.Where(a => a.SignID == Userid).ToList();
            return View(mod);
        }

        /// <summary>
        /// 保存配置奖项
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Save(List<Models.LuckDraw> view)
        {
            List<Models.LuckDraw> draws = EF.LuckDraws.ToList();
            List<Models.LuckDraw> list = new List<Models.LuckDraw>();
            list = view;
            for (int i = 0; i < view.Count; i++)//循环去除重复项
            {
                foreach (var item in draws)
                {
                    if (view[i].LuckID == item.LuckID && view[i].DrawID == item.DrawID)
                    {
                        list.Remove(view[i]);
                        break;
                    }
                }
            }
            EF.LuckDraws.AddRange(list);
            if (EF.SaveChanges() > 0)
            {
                return Content("success");
            }
            else
            {
                return Content("添加失败");
            }
        }

        public IActionResult Add(Draw draw)
        {
            draw.SignID = HttpContext.Session.GetModel<Sign>("User").ID;
            EF.Draws.Add(draw);

            if (EF.SaveChanges() > 0)
                return Content("success");
            else
                return Content("添加失败");
        }

        [HttpPost]
        public IActionResult Delete(int ID)
        {
            var mod = EF.Draws.FirstOrDefault(x => x.ID == ID);
            var info = EF.LuckDraws.Where(x => x.DrawID == ID);
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