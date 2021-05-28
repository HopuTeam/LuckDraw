using LuckDraw.Handles;
using LuckDraw.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckDraw.Controllers
{
    public class LuckController : BaseController
    {
        private CoreEntities EF { get; }
        public LuckController(CoreEntities _ef) : base(_ef)
        {
            EF = _ef;
        }

        public IActionResult Index()
        {
            return View(EF.Lucks.Where(x => x.SignID == HttpContext.Session.GetModel<Sign>("User").ID).ToList());
        }

        [HttpGet]
        public IActionResult Add(int ParentID)
        {
            return View(ParentID);
        }
        [HttpPost]
        public IActionResult Add(Luck luck)
        {
            try
            {
                luck.SignID = HttpContext.Session.GetModel<Sign>("User").ID;
                EF.Lucks.Add(luck);
                EF.SaveChanges();
                return Content("success");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult MulitAdd(int ID, string Text)
        {
            if (Text == null)
                return Content("请填写需要导入的数据");

            var list = Text.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            var userID = HttpContext.Session.GetModel<Sign>("User").ID;
            try
            {
                foreach (var item in list)
                {
                    var luck = new Luck
                    {
                        Name = item,
                        Description = item,
                        ParentID = ID,
                        SignID = userID,
                        Weigh = 1,
                    };
                    EF.Lucks.Add(luck);
                }
                EF.SaveChanges();
                return Content("success");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
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

            try
            {
                mod.Name = luck.Name;
                mod.Description = luck.Description;
                mod.Weigh = luck.Weigh;
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
                var delLuck = EF.Lucks.FirstOrDefault(x => x.ID == ID);
                var lucks = EF.Lucks.Where(x => x.ParentID == delLuck.ID);
                foreach (var luck in lucks)
                {
                    var del = EF.LuckDraws.FirstOrDefault(x => x.LuckID == luck.ID);
                    if (del != null)
                        EF.Remove(del);
                    EF.Remove(luck);
                }

                var luckDraw = EF.LuckDraws.FirstOrDefault(x => x.LuckID == delLuck.ID);
                if (luckDraw != null)
                    EF.Remove(luckDraw);
                EF.Remove(delLuck);

                EF.SaveChanges();
                return Content("success");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public IActionResult ExcelAdd(IFormFile file, int Pid)
        {
            var userID = HttpContext.Session.GetModel<Sign>("User").ID;
            try
            {
                List<Luck> lstLuck = NPOIHelper.InputExcel<Luck>(file);
                foreach (var item in lstLuck)
                {
                    if (string.IsNullOrEmpty(item.Name))
                    {
                        continue;
                    }
                    Luck objluck = new Luck();
                    if (string.IsNullOrEmpty(item.Description))
                    {
                        objluck.Description = item.Name;
                    }
                    else
                    {
                        objluck.Description = item.Description;
                    }

                    if (item.Weigh < 1)
                    {
                        objluck.Weigh = 1;
                    }
                    else if (item.Weigh > 10)
                    {
                        objluck.Weigh = 10;
                    }
                    else
                    {
                        objluck.Weigh = item.Weigh;
                    }

                    objluck.SignID = userID;
                    objluck.ParentID = Pid;
                    objluck.Name = item.Name;
                    EF.Lucks.Add(objluck);
                }
                EF.SaveChanges();
                var rs = new
                {
                    code = 0,
                    msg = "成功",
                    //data = new
                    //{
                    //    src = ""
                    //}
                };
                return Json(rs);
            }
            catch (Exception ex)
            {
                var rs = new
                {
                    code = -1,
                    msg = ex.Message,
                    //data = new
                    //{
                    //    src = ""
                    //}
                };
                return Json(rs);
            }
        }
    }
}