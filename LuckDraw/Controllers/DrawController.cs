using LuckDraw.Handles;
using LuckDraw.Models;
using Microsoft.AspNetCore.Mvc;
using System;
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
                            drawsid=dr.ID,
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
                    DrawId=item.drawsid,
                    DrawName = item.drname,
                    OptionName = item.opname,
                    OptionID = item.opid,
                    LuckCount = item.zongshu,
                    LuckdrawDrawID = item.luckdrawDrawID,
                });
            }
            return View(res);
        }

        public IActionResult SetDraw(int ID)
        {
            int Userid = HttpContext.Session.GetModel<Sign>("User").ID;
            List<Luck> mod = EF.Lucks.Where(a => a.SignID == Userid).ToList();
            ViewData["drwas"] = EF.Draws.Where(b => b.ID == ID).FirstOrDefault();           
            return View(mod);
        }

        /// <summary>
        /// 保存配置奖项
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Save(int id, int[] ids)
        {
            List<Models.LuckDraw> mode = EF.LuckDraws.Where(a => a.DrawID == id).ToList();
            EF.LuckDraws.RemoveRange(mode);
            for (int i = 0; i < ids.Length; i++)
            {
                Models.LuckDraw luckDrawadd = new Models.LuckDraw()
                {
                    DrawID = id,
                    LuckID = Convert.ToInt32(ids[i])
                };
                EF.LuckDraws.Add(luckDrawadd);
            }
            if (EF.SaveChanges() > 0)
            {
                return Content("保存成功");
            }
            return Content("保存失败");
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