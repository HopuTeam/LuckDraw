using LuckDraw.Handles;
using LuckDraw.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

                foreach (var item in a)
                {
                    elements.Add(new KeyValuePair<string, int>(item.ToString(), 1));

                };
                Random ra = new Random();
                //概率计算
                int allRate = 1;

                foreach (var item in elements)
                {
                    allRate += item.Value;
                }

                string selectedElement = "";
                while (true)
                {

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
                        ViewData["yi"] = selectedElement;

                    }

                }
            }


        /// <summary>
        /// 不可重复
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public IActionResult Two(int ID)
        {

            var list = EF.LuckDraws.Include("Draw").Include("Luck").Where(x=>x.DrawID==2).ToList();
            return View(list);
        }
        public IActionResult NonLucky(int ID = 2)
        {
            var mod = EF.LuckDraws.Where(x => x.DrawID == ID && x.EntryTime == null).ToList();
            var a = mod.Count();
            string[] list = new string[a];
            int i = 0;
            foreach (var item in mod)
            {
                var Dmod = EF.Lucks.FirstOrDefault(x => x.ID == item.LuckID);
                list[i] = Dmod.Name;
                i++;
            }
            int index = new Random().Next(0, i);
            string Name = list[index];
            return Content(Name);
        }



    }
}


