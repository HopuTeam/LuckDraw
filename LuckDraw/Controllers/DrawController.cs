using LuckDraw.Handles;
using LuckDraw.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckDraw.Controllers
{
    public class DrawController : BaseController
    {
        private CoreEntities EF { get; }
        public DrawController(CoreEntities _ef) : base(_ef)
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
                            drawsid = dr.ID,
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
                    DrawId = item.drawsid,
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
            var optionid = (from d in EF.Draws
                            where d.ID == ID
                            select d.SignID).FirstOrDefault();
            if (optionid != Userid)//确认当前抽奖项目属于当前登录的用户
            {
                return Redirect("/Draw/index");
            }
            var mod = EF.Lucks.Where(a => a.SignID == Userid).ToList();
            ViewData["drwas"] = EF.Draws.Where(b => b.ID == ID).FirstOrDefault();
            return View(mod);
        }

        /// <summary>
        /// 保存配置奖项
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Save(int id, int[] ids)
        {
            try
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
                EF.SaveChanges();
                return Content("保存成功");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Add(Draw draw)
        {
            try
            {
                draw.SignID = HttpContext.Session.GetModel<Sign>("User").ID;
                EF.Draws.Add(draw);
                EF.SaveChanges();
                return Content("success");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Delete(int ID)
        {
            try
            {
                var Draw = EF.Draws.FirstOrDefault(x => x.ID == ID);
                var luckDraws = EF.LuckDraws.Where(x => x.DrawID == ID);
                foreach (var luckDraw in luckDraws)
                    EF.Remove(luckDraw);
                EF.Remove(Draw);

                EF.SaveChanges();
                return Content("success");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
    }
}