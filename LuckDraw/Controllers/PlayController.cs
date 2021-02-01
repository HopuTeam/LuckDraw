using LuckDraw.Handles;
using LuckDraw.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckDraw.Controllers
{
    public class PlayController : BaseController
    {
        private CoreEntities EF { get; }
        public PlayController(CoreEntities _ef) : base(_ef)
        {
            EF = _ef;
        }

        public IActionResult Repeat(int ID)
        {
            var DrawName = (from luckdraw in EF.LuckDraws
                            where luckdraw.DrawID == ID
                            join dr in EF.Draws on luckdraw.DrawID equals dr.ID
                            select dr.Name).FirstOrDefault();

            PlayViweModel view = new PlayViweModel
            {
                Drawname = DrawName,
                Drawid = ID
            };
            return View(view);
        }
        /// <summary>
        /// 可重复抽奖的方法
        /// </summary>
        /// <returns></returns>
        public IActionResult One(int Drawid, int Second = 1)
        {
            int Userid = HttpContext.Session.GetModel<Sign>("User").ID;
            var model = (from luckdraw in EF.LuckDraws
                         where luckdraw.DrawID == Drawid
                         join luck1 in EF.Lucks on luckdraw.LuckID equals luck1.ID
                         where luck1.SignID == Userid
                         select new
                         {
                             luckdraw = luckdraw.ID,
                             id = luck1.ID,
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
            //抽到的名字
            string selectedElement = string.Empty;

            //抽多少次
            for (int n = 0; n < Second; n++)
            {
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
                Luck luck = EF.Lucks.Where(a => a.ID == luckid).FirstOrDefault();
                selectedElement = luck.Name;
                //添加抽中的次数
                Models.LuckDraw draw = EF.LuckDraws.Where(c => c.LuckID == luck.ID && c.DrawID == Drawid).FirstOrDefault();
                draw.Number += 1;
                EF.SaveChanges();
            }
            if (Second == 1)
            {
                return Content(selectedElement);
            }
            else
            {
                return Content($"已抽取：{Second}次");
            }

        }


        /// <summary>
        /// 刷新抽中的次数
        /// </summary>
        /// <returns></returns>
        public IActionResult GetOptions(int Drawid)
        {
            var a = (from luckdrawdb in EF.LuckDraws
                     where luckdrawdb.DrawID == Drawid
                     join luck in EF.Lucks on luckdrawdb.LuckID equals luck.ID
                     where luck.SignID == HttpContext.Session.GetModel<Sign>("User").ID
                     select new
                     {
                         name = luck.Name,
                         cishu = luckdrawdb.Number
                     }

            ).ToList();
            return Json(a);
        }

        public IActionResult Empty(int Drawid)
        {
            int t = 0;
            var a = (from luckdrawdb in EF.LuckDraws
                     where luckdrawdb.DrawID == Drawid
                     join luck in EF.Lucks on luckdrawdb.LuckID equals luck.ID
                     where luck.SignID == HttpContext.Session.GetModel<Sign>("User").ID
                     select new
                     {
                         id = luckdrawdb.ID
                     }
                      ).ToList();
            if (a.Count() < 0)
            {
                return Content("没有数据");
            }
            foreach (var item in a)
            {
                var lkda = EF.LuckDraws.FirstOrDefault(c => c.ID == item.id);
                lkda.Number = 0;
                EF.SaveChanges();
                t++;
            }
            if (t > 0)
            {
                return Content("次数已清空");
            }
            else
            {
                return Content("次数清空失败");
            }
        }
        /// <summary>
        /// 不可重复
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        /// 
        public IActionResult Two(int ID)
        {
            return View(EF.Draws.FirstOrDefault(x => x.ID == Convert.ToInt32(ID)));
        }

        public IActionResult Updata(int ID)//ID前端传Draws的id 直接调用
        {
            var list = (from LuckDraw in EF.LuckDraws
                        join Luck in EF.Lucks on LuckDraw.LuckID equals Luck.ID
                        join Draw in EF.Draws on LuckDraw.DrawID equals Draw.ID
                        where LuckDraw.DrawID == ID
                        select new
                        {
                            ID = LuckDraw.ID,
                            Name = Luck.Name,
                            Time = LuckDraw.EntryTime.ToString().Substring(0, 19),
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

            // 定义权重数组
            int arrayCount = 0;
            foreach (var item in mod)
                arrayCount += EF.Lucks.FirstOrDefault(x => x.ID == item.LuckID).Weigh;
            int[] list = new int[arrayCount];

            int z = 0;
            foreach (var item in mod)
            {
                var Dmod = EF.Lucks.FirstOrDefault(x => x.ID == item.LuckID);
                for (int i = 0; i < Dmod.Weigh; i++)
                {
                    list[z] = Dmod.ID;
                    z++;
                }
            }
            int index = new Random().Next(0, z);
            int luckid = list[index];//幸运观众的id
                                     //抽取到的幸运观众
            string Name = (from luname in EF.Lucks
                           where luname.ID == luckid
                           select luname.Name
                           ).FirstOrDefault();//幸运观众的名字
            var EideTime = EF.LuckDraws.FirstOrDefault(x => x.LuckID == luckid && x.DrawID == ID);//给幸运观众加抽中时间
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