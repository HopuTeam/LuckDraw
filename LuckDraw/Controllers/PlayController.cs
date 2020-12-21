using LuckDraw.Handles;
using LuckDraw.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace LuckDraw.Controllers
{
    public class PlayController : BaseController
    {
        private CoreEntities EF { get; }
        public PlayController(CoreEntities _ef) : base(_ef)
        {
            EF = _ef;
        }

        public IActionResult Index()
        {
           
                return View();
            
        }
        public IActionResult Repeat()
        {

            return View();

        }

        /// <summary>
        /// 可重复抽奖的方法
        /// </summary>
        /// <returns></returns>
        public IActionResult One()
            {
                var c = HttpContext.Session.GetModel<Luck>("User");

                var a = (from lu in EF.Lucks
                         select new
                         {
                             name = lu.Name,
                             Weigh = lu.Weigh
                         }).ToList();
                //string[] a = new string[5] { "1", "2", "3","4","5"};
                List<KeyValuePair<string, int>> elements = new List<KeyValuePair<string, int>>();
                foreach (var item in a)
                {
                    elements.Add(new KeyValuePair<string, int>(item.name, item.Weigh));
                }

             
                Random ra = new Random();
                //概率计算
                int allRate = 1;

                foreach (var item in elements)
                {
                    allRate += item.Value;
                }

                string selectedElement = "";
               
                    for (int n = 0; n < 1; n++)
                    {
                        //在规定范围产生一个随机数
                        int diceRoll = ra.Next(1, allRate);

                        int cumulative = 0;

                        for (int i = 0; i < elements.Count; i++)
                        {
                            cumulative += elements[i].Value;

                            if (diceRoll <= cumulative)
                            {
                                selectedElement = elements[i].Key;

                                break;
                            }
                        }
                        //抽到的名字
                        //ViewData["yi"] = selectedElement;
                }
            Luck luck = EF.Lucks.Where(a => a.Name == selectedElement).FirstOrDefault();
            Models.LuckDraw draw = EF.LuckDraws.Where(c => c.LuckID == luck.ID).FirstOrDefault();
            if (draw==null)
            {
                Models.LuckDraw list = new Models.LuckDraw();
                list.LuckID = luck.ID;
                list.DrawID = 2;
                list.Number = 1;
                EF.LuckDraws.Add(list);
                EF.SaveChanges();
            }
            else
            {
                draw.Number += 1;
                EF.SaveChanges();
            }


            return Content(selectedElement);
        }


        /// <summary>
        /// 刷新抽中的次数
        /// </summary>
        /// <returns></returns>
        public IActionResult GetOptions()
        {
            var a = (from luckdrawdb in EF.LuckDraws
                     join luck in EF.Lucks on luckdrawdb.LuckID equals luck.ID
                     select new
                     {
                         name = luck.Name,
                         cishu = luckdrawdb.Number
                     }

            ).ToList();

            
            return Json(a);
        }







        /// <summary>
        /// 不可重复
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public IActionResult Two(int ID=1)//ID前端传Draws的id 直接调用
        {


            var list = EF.LuckDraws.Include("Draw").Include("Luck").Where(x=>x.DrawID==ID).ToList();
            var title = EF.Draws.FirstOrDefault(x => x.ID == ID);
            ViewData["tit"] = title;
            return View(list);
        }
        public IActionResult NonLucky(int ID = 1)//ID前端传Draws的id
        {
            

            var mod = EF.LuckDraws.Where(x => x.DrawID == ID && x.EntryTime == null).ToList();
            var a = mod.Count();
            if (a == 0)
                return Content("没了");
            string[] list = new string[a];
            int i = 0;
            foreach (var item in mod)
            {
                var Dmod = EF.Lucks.FirstOrDefault(x => x.ID == item.LuckID);
                list[i] = Dmod.Name;
                i++;
            }
            int index = new Random().Next(0, i);
            string Name = list[index];//幸运观众的名字
            var luckID = (from x in EF.Lucks
                          join y in EF.LuckDraws on x.ID equals y.LuckID
                          where x.Name==Name
                          select y.ID).FirstOrDefault();
            var EideTime = EF.LuckDraws.FirstOrDefault(x=>x.ID==luckID);
            EideTime.EntryTime = DateTime.Now;
            EF.SaveChanges();
            return Content($"抽奖成功,恭喜 { Name } 同学");
        }
        
        //移除
        public IActionResult TwoEide(int id)
        {
            var mod = EF.LuckDraws.FirstOrDefault(x => x.ID == id);
            if (mod.EntryTime == null)
            {
                mod.EntryTime = DateTime.Now;
            }
            else {
                mod.EntryTime = null;
            }
                 EF.SaveChanges();
            return Content("success");
        }
    }
}


