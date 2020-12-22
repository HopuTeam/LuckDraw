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
        public IActionResult Repeat(int LuckdrawDrawID)
        {
            var DrawName = (from luckdraw in EF.LuckDraws
                            where luckdraw.DrawID == LuckdrawDrawID
                            join dr in EF.Draws on luckdraw.DrawID equals dr.ID
                            select dr.Name).FirstOrDefault();

            PlayViweModel view = new PlayViweModel
            {
                Drawname = DrawName,
                Drawid = LuckdrawDrawID
            };
            return View(view);

        }

        /// <summary>
        /// 可重复抽奖的方法
        /// </summary>
        /// <returns></returns>
        public IActionResult One(int Drawid)
        {
            var User = HttpContext.Session.GetModel<Sign>("User");



            var model = (from luckdraw in EF.LuckDraws
                         where luckdraw.DrawID == Drawid
                         join luck1 in EF.Lucks on luckdraw.LuckID equals luck1.ID
                         where luck1.SignID == User.ID
                         select new
                         {
                             id =luck1.ID,
                             name = luck1.Name,
                             Weigh = luck1.Weigh
                         }).ToList();


            List<KeyValuePair<int, int>> elements = new List<KeyValuePair<int, int>>();
            //将查到的数据的id和权重填到elements中
            foreach (var item in model)
            {
                elements.Add(new KeyValuePair<int, int>(item.id, item.Weigh));
            }


            Random ra = new Random();
            //计算权限，最终结果加一
            int allRate = 1;

            foreach (var item in elements)
            {
                allRate += item.Value;
            }
            //记录抽到的id
            int luckid = 0;
           
            //在规定范围产生一个随机数
            int diceRoll = ra.Next(1, allRate);
            int cumulative = 0;
            //循环查到的数组的条目数，如果随机数生成的是2，那抽到的人肯定是数组里面的第二个
            for (int i = 0; i < elements.Count; i++)
            {
                //循环一遍就加一遍权重值，就是例如第一个人的权重是1，当前cumulative=1，第二个人权重是3，当前权重=1+3=4
                cumulative += elements[i].Value;

                //如果随机数小于等于当前权重，就是抽到这个人。例如本次随机数是3，第一次 (diceRoll<= cumulative)  => (3<=1),不成立进入下一次  。第二次cumulative=4，随机数3<=4，那抽到的就是数组中的第二个
                if (diceRoll <= cumulative)
                {
                    luckid = elements[i].Key;

                    break;
                }
            }
            
                    
            Luck luck = EF.Lucks.Where(a => a.ID==luckid).FirstOrDefault();
            //抽到的名字
            string selectedElement = luck.Name;
            //添加抽中的次数
            Models.LuckDraw draw = EF.LuckDraws.Where(c => c.LuckID == luck.ID).FirstOrDefault();
            if (draw == null)
            {
                Models.LuckDraw list = new Models.LuckDraw
                {
                    LuckID = luck.ID,
                    DrawID = 1,
                    Number = 1
                };
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
        public IActionResult GetOptions(int Drawid)
        {
            var c = HttpContext.Session.GetModel<Sign>("User");
            var a = (from luckdrawdb in EF.LuckDraws
                     where luckdrawdb.DrawID == Drawid
                     join luck in EF.Lucks on luckdrawdb.LuckID equals luck.ID
                     where luck.SignID == c.ID
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
        /// 
        public IActionResult Two(int id=2)
        {
            var mod = EF.Draws.FirstOrDefault();
            return View(mod);
        }

        public IActionResult Updata(int ID)//ID前端传Draws的id 直接调用
        {
            var list = (from LuckDraw in EF.LuckDraws
                        join Luck in EF.Lucks on LuckDraw.LuckID equals Luck.ID
                        join Draw in EF.Draws on LuckDraw.DrawID equals Draw.ID
                        where LuckDraw.DrawID == Draw.ID
                        select new
                        {
                            ID = LuckDraw.ID,
                            Name = Luck.Name,
                            Time = LuckDraw.EntryTime,
                        }).ToList();
            //foreach (var item in list)
            //{
            //    if (item.Time == null)
            //    {
            //        return Json();
            //    }
            //    else
            //    {

            //    }
            //}
            return Json(list);
        }
        public IActionResult NonLucky(int ID)
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
                          where x.Name == Name
                          select y.ID).FirstOrDefault();
            var EideTime = EF.LuckDraws.FirstOrDefault(x => x.ID == luckID);
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
            else
            {
                mod.EntryTime = null;
            }
            EF.SaveChanges();
            return Content("success");
        }
    }
}