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
           // var sid = HttpContext.Session.GetModel<Sign>("User").ID;
            //var list = from draw in EF.Draws
                       //where draw.SignID == sid
                       //join luck in EF.Lucks on draw.SignID equals luck.SignID
                      // join option in EF.Options on draw.OptionID equals option.ID
                       //select new
                      // {
                           //name = draw.Name,
                         //  mode = option.Name,
                          
                     //}
            return View();
        }


        public IActionResult Add(string LuckType, string Name)
        {
            var sid = HttpContext.Session.GetModel<Sign>("User").ID;
            int luktype = Convert.ToInt32(LuckType);
            int cishu = 0;
            Draw draw = new Draw
            {
                SignID = sid,
                Name = Name,
                OptionID = luktype
            };
            EF.Draws.Add(draw);
            EF.SaveChanges();

            var luck = (from lu in EF.Lucks
                        where lu.SignID == sid
                        select new
                        {
                            lu.ID
                        }).ToList();

            foreach (var item in luck)
            {
                Models.LuckDraw luckDraw = new Models.LuckDraw
                {
                    DrawID = draw.ID,
                    LuckID = item.ID
                };
                EF.LuckDraws.Add(luckDraw);
                EF.SaveChanges();
                cishu++;
            }

            return Json(cishu);
        }




        // 启用codefirst迁移    enable-migrations
        // 添加一个迁移文件      add-migration [name]     -- add-migration one
        // 更新数据库           update-database








    }
}
