using LuckDraw.Handles;
using LuckDraw.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

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

        public IActionResult Setoptions()
        {
            int Userid= HttpContext.Session.GetModel<Sign>("User").ID;
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
            for (int i = 0; i < view.Count(); i++)//循环去除重复项
            {
                foreach (var item in draws)
                {
                    if (view[i].LuckID==item.LuckID && view[i].DrawID==item.DrawID)
                    {
                        list.Remove(view[i]);
                        break;
                    }
                }
            }         
            EF.LuckDraws.AddRange(list);
            if (EF.SaveChanges()>0)
            {
                return Content("success");
            }
            else
            {
                return Content("添加失败");
            }
        }

        [HttpGet]
        public IActionResult Add(int ParentID)
        {
            return View(ParentID);
        }
        [HttpPost]
        public IActionResult Add(Luck luck)
        {
            luck.SignID = HttpContext.Session.GetModel<Sign>("User").ID;
            EF.Lucks.Add(luck);
            if (EF.SaveChanges() > 0)
                return Content("success");
            else
                return Content("添加失败");
        }

        [HttpGet]
        public IActionResult Edit(int ID)
        {
            return View(EF.Lucks.FirstOrDefault(x => x.ID == ID));
        }
        [HttpPost]
        public IActionResult Edit(Luck luck)
        {
            var mod = EF.Lucks.FirstOrDefault(x => x.ID == luck.ID && x.SignID == HttpContext.Session.GetModel<Sign>("User").ID);
            if (mod == null)
                return Content("数据请求异常");
            mod.Name = luck.Name;
            mod.Description = luck.Description;
            mod.Weigh = luck.Weigh;

            if (EF.SaveChanges() > 0)
                return Content("success");
            else
                return Content("没有进行任何更改");
        }

        [HttpPost]
        public IActionResult Delete(int ID)
        {
            var mod = EF.Lucks.FirstOrDefault(x => x.ID == ID);
            var info = EF.Lucks.Where(x => x.ParentID == mod.ID);
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